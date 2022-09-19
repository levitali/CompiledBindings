#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

namespace CompiledBindings;

public class XamlDomParser
{
	private static readonly XName NameAttr = XNamespace.None + "Name";

	public readonly XName DataTemplate;
	public readonly XName Style;
	public readonly XName VisualStatesGroups;

	public readonly XName xBind;
	public readonly XName xDefaultBindMode;
	public readonly XName xSet;
	public readonly XName xNull;
	public readonly XName xType;
	public readonly TypeInfo? DependencyObjectType;
	public readonly TypeInfo? DependencyPropertyType;

	private int _localVarIndex;
	private readonly Func<string, IEnumerable<string>> _getClrNsFromXmlNs;

	public XamlDomParser(
		XNamespace defaultNamespace,
		XNamespace xNamespace,
		Func<string, IEnumerable<string>> getClrNsFromXmlNs,
		TypeInfo converterType,
		TypeInfo bindingType,
		TypeInfo? dependencyObjectType,
		TypeInfo? dependencyPropertyType)
	{
		DefaultNamespace = defaultNamespace;
		_getClrNsFromXmlNs = getClrNsFromXmlNs;
		ConverterType = converterType;
		BindingType = bindingType;
		DependencyObjectType = dependencyObjectType;
		DependencyPropertyType = dependencyPropertyType;

		Style = DefaultNamespace + "Style";
		DataTemplate = DefaultNamespace + "DataTemplate";
		VisualStatesGroups = DefaultNamespace + "VisualStateManager.VisualStateGroups";

		this.xNamespace = xNamespace;
		xClass = xNamespace + "Class";
		xName = xNamespace + "Name";
		xBind = xNamespace + "Bind";
		xDefaultBindMode = xNamespace + "DefaultBindMode";
		xSet = xNamespace + "Set";
		xNull = xNamespace + "Null";
		xType = xNamespace + "Type";
		xDataType = xNamespace + "DataType";
	}

	public XNamespace DefaultNamespace { get; }
	public XNamespace xNamespace { get; }
	public XName xClass { get; }
	public XName xName { get; }
	public XName xDataType { get; }
	public TypeInfo ConverterType { get; }
	public TypeInfo BindingType { get; }
	public IList<XamlNamespace>? KnownNamespaces { get; set; }
	public string CurrentFile { get; set; }
	public string CurrentLineFile { get; set; }
	public TypeInfo TargetType { get; set; }
	public TypeInfo DataType { get; set; }

	public TypeInfo GetRootType(XElement root)
	{
		var xClassAttr = root.Attribute(xClass);
		try
		{
			//xClassAttr.Remove();
			return new TypeInfo(TypeInfo.GetTypeThrow(xClassAttr.Value), false);

		}
		catch (Exception ex)
		{
			throw new GeneratorException(ex.Message, CurrentFile, xClassAttr, xClassAttr.Value.Length);
		}
	}

	public TypeInfo FindType(XElement xelement)
	{
		return FindType(xelement.Name, xelement);
	}

	public XName? GetTypeNameFromAttribute(string value, XAttribute attr)
	{
		if (value.StartsWith("{"))
		{
			var xamlNode = XamlParser.ParseMarkupExtension(CurrentLineFile, value, attr, KnownNamespaces);
			if (xamlNode.Name == xNull)
			{
				return null;
			}
			else if (xamlNode.Name == xType)
			{
				value = xamlNode.Value!;
			}
			else
			{
				throw new GeneratorException($"Unexpected markup extension {xamlNode.Name}", CurrentFile, attr);
			}
		}
		if (value == null)
		{
			throw new GeneratorException($"Missing type.", CurrentFile, attr);
		}
		return XamlParser.GetTypeName(value, attr.Parent, KnownNamespaces);
	}

	public TypeInfo? FindTypeFromAttribute(XAttribute attr)
	{
		return FindType(attr.Value, attr);
	}

	public TypeInfo? FindType(string value, XAttribute attr)
	{
		var typeName = GetTypeNameFromAttribute(value, attr);
		if (typeName == null)
		{
			return null;
		}
		return FindType(typeName, attr);
	}

	public IEnumerable<XamlNamespace> GetNamespaces(XamlNode xamlNode)
	{
		var namespaces = XamlNamespace.GetClrNamespaces(xamlNode.Element);
		if (KnownNamespaces != null)
		{
			namespaces = namespaces.Union(KnownNamespaces, n => n.Prefix);
		}
		return namespaces;
	}

	public XamlObjectProperty GetObjectProperty(XamlDomBase xamlDom, XamlObject obj, XamlNode xamlNode, bool throwIfBindWithoutDataType)
	{
		return GetObjectProperty(xamlDom, obj, xamlNode.Name.LocalName, xamlNode, throwIfBindWithoutDataType);
	}

	public static string GenerateName(XElement element, HashSet<string> usedNames)
	{
		string id;
		string className = element.Name.LocalName.Split('.').Last();
		for (int i = 1; ; i++)
		{
			string name = className + i;
			name = char.ToLower(name[0]) + name.Substring(1);
			if (usedNames.Contains(name))
			{
				continue;
			}
			id = name;
			usedNames.Add(name);
			break;
		}
		return id;
	}

	private TypeInfo FindType(XName className, XObject xobject)
	{
		var clrNs = XamlNamespace.GetClrNamespace(className.NamespaceName);
		if (clrNs != null)
		{
			return TypeInfo.GetTypeThrow(clrNs + "." + className.LocalName);
		}
		foreach (string clrNr2 in _getClrNsFromXmlNs(className.NamespaceName))
		{
			string clrTypeName = clrNr2;
			if (!clrTypeName.EndsWith("/"))
			{
				clrTypeName += '.';
			}

			clrTypeName += className.LocalName;
			var type = TypeInfo.GetType(clrTypeName);
			if (type != null)
			{
				return type;
			}
		}
		throw new GeneratorException($"The type {className} was not found.", CurrentFile, xobject);
	}

	private XamlObjectProperty GetObjectProperty(XamlDomBase xamlDom, XamlObject obj, string memberName, XamlNode xamlNode, bool throwIfBindWithoutDataType)
	{
		try
		{
			var objProp = new XamlObjectProperty
			{
				Object = obj,
				XamlNode = xamlNode,
			};

			XName? attachedClassName = null;
			string? attachedPropertyName = null;

			int index = memberName.IndexOf('.');
			if (index != -1)
			{
				string typeName = memberName.Substring(0, index);
				attachedClassName = (xamlNode.Name.Namespace is var n && n == XNamespace.None ? DefaultNamespace : n) + typeName;
				attachedPropertyName = memberName.Substring(index + 1);
			}

			if (attachedClassName != null)
			{
				var attachPropertyOwnerType = FindType(attachedClassName, xamlNode.Element);

				string setMethodName = "Set" + attachedPropertyName;
				var setPropertyMethod = attachPropertyOwnerType.Methods.FirstOrDefault(m => m.Definition.Name == setMethodName);
				if (setPropertyMethod == null)
				{
					throw new GeneratorException($"Invalid attached property {attachPropertyOwnerType.Reference.FullName}.{attachedPropertyName}. Missing method {setMethodName}.", CurrentFile, xamlNode);
				}

				var parameters = setPropertyMethod.Parameters;
				if (parameters.Count != 2)
				{
					throw new GeneratorException($"Invalid set method for attached property {attachPropertyOwnerType.Reference.FullName}.{attachedPropertyName}. The {setMethodName} method must have two parameters.", CurrentFile, xamlNode);
				}

				if (!parameters[0].ParameterType.IsAssignableFrom(obj.Type))
				{
					throw new GeneratorException($"The attached property {attachPropertyOwnerType.Reference.FullName}.{attachedPropertyName} cannot be used for objects of type {obj.Type.Reference.FullName}.", CurrentFile, xamlNode);
				}

				objProp.MemberName = attachedPropertyName!;
				objProp.TargetMethod = setPropertyMethod;
				objProp.IsAttached = true;
			}
			else
			{
				objProp.MemberName = memberName;
				var typeInfo = obj.Type;
				var pi = typeInfo.Properties.FirstOrDefault(p => p.Definition.Name == memberName);
				if (pi != null)
				{
					objProp.TargetProperty = pi;
				}
				else
				{
					var @event = obj.Type.Events.FirstOrDefault(e => e.Definition.Name == memberName);
					if (@event != null)
					{
						objProp.TargetEvent = @event;
					}
					else
					{
						var method = typeInfo.Methods.FirstOrDefault(e => e.Definition.Name == memberName);
						if (method == null)
						{
							foreach (var ns in GetNamespaces(xamlNode))
							{
								var clrNs = ns.ClrNamespace!;
								method = TypeInfo.FindExtensionMethods(clrNs, memberName, typeInfo)
										.FirstOrDefault(m => m.Parameters.Count == 2 && m.Parameters[0].ParameterType.IsAssignableFrom(obj.Type));
								if (method != null)
								{
									xamlDom.IncludeNamespaces.Add(clrNs);
									break;
								}
							}
							if (method == null)
							{
								throw new GeneratorException($"No target member {memberName} found in type {obj.Type.Reference.FullName}.", CurrentFile, xamlNode);
							}
						}
						else if (method.Parameters.Count != 1)
						{
							throw new GeneratorException($"Cannot bind to method {obj.Type.Reference.FullName}.{memberName}. To use a method as target, the method must have one parameter.", CurrentFile, xamlNode);
						}

						objProp.TargetMethod = method;
					}
				}
			}

			objProp.Value = GetObjectValue(xamlDom, obj, objProp, xamlNode, throwIfBindWithoutDataType);
			return objProp;
		}
		catch (ParseException ex)
		{
			throw new GeneratorException(ex.Message, CurrentFile, xamlNode, ex.Position, ex.Length);
		}
		catch (Exception ex) when (ex is not GeneratorException)
		{
			throw new GeneratorException(ex.Message, CurrentFile, xamlNode);
		}
	}

	private XamlObjectValue GetObjectValue(XamlDomBase xamlDom, XamlObject obj, XamlObjectProperty objProp, XamlNode xamlNode, bool throwIfBindWithoutDataType)
	{
		var value = new XamlObjectValue();

		var propType = objProp.MemberType;
		if (xamlNode.Children.Count == 1 && xamlNode.Children[0].Name == xSet)
		{
			try
			{
				CheckPropertyTypeNotBinding();
				var staticNode = xamlNode.Children[0];
				value.StaticValue = ExpressionParser.Parse(TargetType, "this", staticNode.Value!, propType, false, GetNamespaces(xamlNode).ToList(), out var includeNamespaces, out var dummy);
				xamlDom.IncludeNamespaces.UnionWith(includeNamespaces.Select(ns => ns.ClrNamespace!));
				value.StaticValue = CorrectSourceExpression(value.StaticValue, objProp);
				CorrectMethod(objProp, value.StaticValue.Type);
				obj.GenerateMember = true;
			}
			catch (ParseException ex)
			{
				HandleParseException(ex);
			}
		}
		else if (xamlNode.Children.Count == 1 && xamlNode.Children[0].Name == xBind)
		{
			try
			{
				CheckPropertyTypeNotBinding();
				var defaultBindModeAttr =
					EnumerableExtensions.SelectSequence(xamlNode.Element, e => e.Parent, xamlNode.Element is XElement)
					.Cast<XElement>()
					.Select(e => e.Attribute(xDefaultBindMode))
					.FirstOrDefault(a => a != null);
				var defaultBindMode = defaultBindModeAttr != null ? (BindingMode)Enum.Parse(typeof(BindingMode), defaultBindModeAttr.Value) : BindingMode.OneWay;
				value.BindValue = BindingParser.Parse(objProp, DataType, TargetType, "dataRoot", defaultBindMode, this, xamlDom.IncludeNamespaces, throwIfBindWithoutDataType, ref _localVarIndex);
				if (value.BindValue.SourceExpression != null)
				{
					if (value.BindValue.Converter == null)
					{
						value.BindValue.SourceExpression = CorrectSourceExpression(value.BindValue.SourceExpression, objProp);
					}
					CorrectMethod(objProp, value.BindValue.SourceExpression.Type);
				}
				obj.GenerateMember = true;

				if (DependencyPropertyType != null)
				{
					if (value.BindValue?.Mode is BindingMode.TwoWay or BindingMode.OneWayToSource &&
						value.BindValue.TargetChangedEvents.Count == 0)
					{
						string dpName;
						TypeDefinition type;
						if (objProp.IsAttached)
						{
							dpName = objProp.TargetMethod!.Definition.Name.Substring("Set".Length);
							type = objProp.TargetMethod.Definition.DeclaringType;
						}
						else
						{
							dpName = objProp.TargetProperty!.Definition.Name;
							type = objProp.TargetProperty.Definition.DeclaringType;
						}

						dpName += "Property";
						var typeInfo = TypeInfo.GetTypeThrow(type.FullName);
						var dependencyPropertyType = DependencyPropertyType.Reference.FullName;

						IMemberInfo dp = typeInfo.Fields.FirstOrDefault(f =>
							f.Definition.Name == dpName &&
							f.Definition.IsStatic &&
							f.FieldType.Reference.FullName == dependencyPropertyType);
						dp ??= typeInfo.Properties.FirstOrDefault(p =>
								p.Definition.Name == dpName &&
								p.Definition.IsStatic() &&
								p.PropertyType.Reference.FullName == dependencyPropertyType);
						if (dp != null)
						{
							value.BindValue.DependencyProperty = dp;
						}
					}
				}
			}
			catch (ParseException ex)
			{
				HandleParseException(ex);
			}
		}
		else
		{
			throw new GeneratorException($"The value cannot be processed.", CurrentFile, xamlNode);
		}

		if (objProp.TargetEvent != null)
		{
			var expr = value.BindValue?.Expression ?? value.StaticValue;
			if (expr != null &&
				(expr is not MemberExpression me || me.Member is not MethodInfo) &&
				(expr is not (CallExpression or InvokeExpression)))
			{
				throw new GeneratorException($"Expression type must be a method.", CurrentFile, xamlNode);
			}
		}

		return value;

		void CheckPropertyTypeNotBinding()
		{
			if (BindingType.IsAssignableFrom(propType))
			{
				throw new GeneratorException($"You cannot use x:Bind for this property, because the property type is Binding.", CurrentFile, xamlNode);
			}
		}

		void HandleParseException(ParseException ex)
		{
			var offset = objProp.MemberName.Length +
					2 + // ="    Note, if there are spaces in between, they are not considered
					xamlNode.Children[0].ValueOffset +
					ex.Position;
			throw new ParseException(ex.Message, offset, ex.Length);
		}
	}

	private static Expression CorrectSourceExpression(Expression expression, XamlObjectProperty prop)
	{
		if (expression is not (ConstantExpression or DefaultExpression) &&
			!TypeInfo.GetTypeThrow(typeof(System.Threading.Tasks.Task)).IsAssignableFrom(expression.Type) &&
			prop.MemberType?.Reference.FullName == "System.String" && expression.Type.Reference.FullName != "System.String")
		{
			var method = TypeInfo.GetTypeThrow(typeof(object)).Methods.First(m => m.Definition.Name == "ToString");
			if (expression is UnaryExpression or BinaryExpression or CoalesceExpression)
			{
				expression = new ParenExpression(expression);
			}
			expression = new CallExpression(expression, method, Array.Empty<Expression>());
		}
		if (expression.IsNullable && prop.MemberType?.IsNullable == false)
		{
			Expression defaultExpr;
			if (prop.MemberType.Reference.FullName == "System.String")
			{
				defaultExpr = new ConstantExpression("");
			}
			else
			{
				defaultExpr = Expression.DefaultExpression;
			}

			return new CoalesceExpression(expression, defaultExpr);
		}
		return expression;
	}

	private void CorrectMethod(XamlObjectProperty prop, TypeInfo type)
	{
		if (prop.TargetMethod != null && !prop.IsAttached)
		{
			// Try to find best suitable method.
			// Note! So far TypeInfoUtils.IsAssignableFrom method does not handle all cases.
			// So it can be, that the method is not found.
			var method = FindBestSuitableTargetMethod(prop.Object.Type, prop.TargetMethod.Definition.Name, type, GetNamespaces(prop.Object.XamlNode).Select(n => n.ClrNamespace!));
			if (method != null)
			{
				prop.TargetMethod = method;
			}
		}
	}

	private static MethodInfo FindBestSuitableTargetMethod(TypeInfo type, string methodName, TypeInfo targetType, IEnumerable<string> namespaces)
	{
		return type.Methods
			.Where(m => m.Definition.Name == methodName && m.Parameters.Count == 1)
			.Concat(namespaces.SelectMany(n => TypeInfo.FindExtensionMethods(n, methodName, type)
											  .Where(m => m.Parameters.Count == 2 || (m.Parameters.Count > 2 && m.Parameters[2].Definition.IsOptional))))
			.FirstOrDefault(m => m.Parameters[m.Parameters.Count == 1 ? 0 : 1].ParameterType.IsAssignableFrom(targetType));
	}
}
