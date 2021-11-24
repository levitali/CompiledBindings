#nullable enable
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

namespace CompiledBindings;

public class XamlDomParser
{
	private static readonly XName NameAttr = XNamespace.None + "Name";

	private readonly XName xBind;
	private readonly XName xDefaultBindMode;
	private readonly XName xSet;
	private readonly XName xNull;
	private readonly XName xType;

	private string _projectPath;
	private int _localVarIndex;
	private readonly Func<string, IEnumerable<string>> _getClrNsFromXmlNs;

	public XamlDomParser(XNamespace defaultNamespace, XNamespace xNamespace, Func<string, IEnumerable<string>> getClrNsFromXmlNs, TypeInfo converterType)
	{
		DefaultNamespace = defaultNamespace;
		_getClrNsFromXmlNs = getClrNsFromXmlNs;
		ConverterType = converterType;

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

	public IList<XamlNamespace>? KnownNamespaces { get; set; }

	public string CurrentFile { get; set; }
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
			var xamlNode = XamlParser.ParseMarkupExtension(value, attr, CurrentFile, KnownNamespaces);
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

	public XamlObjectProperty GetObjectProperty(XamlObject obj, XamlNode xamlNode)
	{
		return GetObjectProperty(obj, xamlNode.Name.LocalName, xamlNode);
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

	private XamlObjectProperty GetObjectProperty(XamlObject obj, string memberName, XamlNode xamlNode)
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
				attachedClassName = XamlParser.GetTypeName(typeName, xamlNode.Element, KnownNamespaces);
				attachedPropertyName = memberName.Substring(index + 1);
			}

			if (attachedClassName != null)
			{
				var attachPropertyOwnerType = FindType(attachedClassName, xamlNode.Element);

				string setMethodName = "Set" + attachedPropertyName;
				var setPropertyMethod = attachPropertyOwnerType.Methods.FirstOrDefault(m => m.Definition.Name == setMethodName);
				if (setPropertyMethod == null)
				{
					throw new GeneratorException($"Invalid attached property {attachPropertyOwnerType.Type.FullName}.{attachedPropertyName}. Missing method {setMethodName}.", xamlNode);
				}

				var parameters = setPropertyMethod.Parameters;
				if (parameters.Count != 2)
				{
					throw new GeneratorException($"Invalid set method for attached property {attachPropertyOwnerType.Type.FullName}.{attachedPropertyName}. The {setMethodName} method must have two parameters.", xamlNode);
				}

				if (!parameters[0].ParameterType.IsAssignableFrom(obj.Type))
				{
					throw new GeneratorException($"The attached property {attachPropertyOwnerType.Type.FullName}.{attachedPropertyName} cannot be used for objects of type {obj.Type.Type.FullName}.", xamlNode);
				}

				objProp.MemberName = setMethodName;
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
								method = TypeInfo.FindExtensionMethods(clrNs, memberName)
										.FirstOrDefault(m => m.Parameters.Count == 2 && m.Parameters[0].ParameterType.IsAssignableFrom(obj.Type));
								if (method != null)
								{
									if (!objProp.IncludeNamespaces.Contains(clrNs))
									{
										objProp.IncludeNamespaces.Add(clrNs);
									}
									break;
								}
							}
							if (method == null)
							{
								throw new GeneratorException($"No target member {memberName} found in type {obj.Type.Type.FullName}.", xamlNode);
							}
						}
						else if (method.Parameters.Count != 1)
						{
							throw new GeneratorException($"Cannot bind to method {obj.Type.Type.FullName}.{memberName}. To use a method as target, the method must have one parameter.", xamlNode);
						}

						objProp.TargetMethod = method;
					}
				}
			}

			var value = new XamlObjectValue(objProp);

			var propType = objProp.MemberType;
			if (xamlNode.Children.Count == 1 && xamlNode.Children[0].Name == xSet)
			{
				try
				{
					var staticNode = xamlNode.Children[0];
					value.StaticValue = ExpressionParser.Parse(TargetType, "this", staticNode.Value, propType, false, GetNamespaces(xamlNode).ToList(), out var includeNamespaces, out var dummy);
					includeNamespaces.ForEach(ns => objProp.IncludeNamespaces.Add(ns.ClrNamespace!));
					value.StaticValue = CorrectSourceExpression(value.StaticValue, objProp);
					CorrectMethod(objProp, value.StaticValue.Type);
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
					var defaultBindModeAttr =
						EnumerableExtensions.SelectSequence(objProp.XamlNode.Element, e => e.Parent, objProp.XamlNode.Element is XElement)
						.Cast<XElement>()
						.Select(e => e.Attribute(xDefaultBindMode))
						.FirstOrDefault(a => a != null);
					var defaultBindMode = defaultBindModeAttr != null ? (BindingMode)Enum.Parse(typeof(BindingMode), defaultBindModeAttr.Value) : BindingMode.OneWay;
					value.BindValue = BindingParser.Parse(objProp, DataType, TargetType, "dataRoot", defaultBindMode, this, ref _localVarIndex);
					if (value.BindValue.SourceExpression != null)
					{
						if (value.BindValue.Converter == null)
						{
							value.BindValue.SourceExpression = CorrectSourceExpression(value.BindValue.SourceExpression, objProp);
						}
						CorrectMethod(objProp, value.BindValue.SourceExpression.Type);
					}
					obj.GenerateMember = true;
				}
				catch (ParseException ex)
				{
					HandleParseException(ex);
				}
			}
			else
			{
				throw new GeneratorException($"The value cannot be processed.", xamlNode);
			}

			if (objProp.TargetEvent != null)
			{
				var expr = value.BindValue?.Expression ?? value.StaticValue;
				if (expr != null &&
					(expr is not MemberExpression me || me.Member is not MethodInfo) &&
					(expr is not (CallExpression or InvokeExpression)))
				{
					throw new GeneratorException($"Expression type must be a method.", xamlNode);
				}
			}

			objProp.Value = value;

			return objProp;

			void HandleParseException(ParseException ex)
			{
				var offset = value!.Property.MemberName.Length +
						2 + // ="    Note, if there are spaces in between, they are not considered
						xamlNode.Children[0].ValueOffset +
						ex.Position;
				throw new ParseException(ex.Message, offset, ex.Length);
			}
		}
		catch (ParseException ex)
		{
			throw new GeneratorException(ex.Message, xamlNode, ex.Position, ex.Length);
		}
		catch (Exception ex) when (ex is not GeneratorException)
		{
			throw new GeneratorException(ex.Message, xamlNode);
		}
	}

	private static Expression CorrectSourceExpression(Expression expression, XamlObjectProperty prop)
	{
		if (expression is not (ConstantExpression or DefaultExpression) &&
			!TypeInfo.GetTypeThrow(typeof(System.Threading.Tasks.Task)).IsAssignableFrom(expression.Type) &&
			prop.MemberType?.Type.FullName == "System.String" && expression.Type.Type.FullName != "System.String")
		{
			var method = TypeInfo.GetTypeThrow(typeof(object)).Methods.First(m => m.Definition.Name == "ToString");
			if (expression is (UnaryExpression or BinaryExpression or CoalesceExpression))
			{
				expression = new ParenExpression(expression);
			}
			expression = new CallExpression(expression, method, Array.Empty<Expression>());
		}
		if (expression.IsNullable && prop.MemberType?.IsNullable == false)
		{
			Expression defaultExpr;
			if (prop.MemberType.Type.FullName == "System.String")
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
		if (prop.TargetMethod != null)
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
			.Union(namespaces.SelectMany(n => TypeInfo.FindExtensionMethods(n, methodName)
											  .Where(m => m.Parameters.Count == 2 || (m.Parameters.Count > 2 && m.Parameters[2].Definition.IsOptional))))
			.FirstOrDefault(m => m.Parameters[m.Parameters.Count == 1 ? 0 : 1].ParameterType.IsAssignableFrom(targetType));
	}
}
