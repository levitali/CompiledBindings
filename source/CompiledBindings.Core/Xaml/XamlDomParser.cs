namespace CompiledBindings;

public abstract class XamlDomParser
{
	public static readonly XNamespace mNamespace = "http://compiledbindings.com";
	public static readonly XName NameAttr = XNamespace.None + "Name";

	public readonly XName DataTemplate;
	public readonly XName Style;
	public readonly XName VisualStateGroups;

	public readonly XName xBind;
	public readonly XName xDefaultBindMode;
	public readonly XName xSet;
	public readonly XName xNull;
	public readonly XName xType;
	public readonly TypeInfo? DependencyObjectType;
	public readonly TypeInfo? DependencyPropertyType;

	private int _localVarIndex;

	public XamlDomParser(
		XNamespace defaultNamespace,
		XNamespace xNamespace,
		TypeInfo converterType,
		TypeInfo bindingType,
		TypeInfo? dependencyObjectType,
		TypeInfo? dependencyPropertyType)
	{
		DefaultNamespace = defaultNamespace;
		ConverterType = converterType;
		BindingType = bindingType;
		DependencyObjectType = dependencyObjectType;
		DependencyPropertyType = dependencyPropertyType;

		Style = DefaultNamespace + "Style";
		DataTemplate = DefaultNamespace + "DataTemplate";
		VisualStateGroups = DefaultNamespace + "VisualStateManager.VisualStateGroups";

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
	public string CurrentFile { get; set; } = null!;
	public string CurrentLineFile { get; set; } = null!;
	public TypeInfo TargetType { get; set; } = null!;
	public TypeInfo DataType { get; set; } = null!;

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
		var xamlNode = XamlParser.ParseMarkupExtension(CurrentLineFile, value, attr, KnownNamespaces);
		if (xamlNode != null)
		{
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

	public XamlObjectProperty GetObjectProperty(
		XamlObject obj,
		XamlNode xamlNode,
		HashSet<string> includeNamespaces,
		BindingScope currentBindingScope,
		List<BindingScope> bindingScopes,
		ref TypeInfo? elementType,
		bool throwIfBindWithoutDataType)
	{
		return GetObjectProperty(obj, xamlNode.Name.LocalName, xamlNode, includeNamespaces, currentBindingScope, bindingScopes, ref elementType, throwIfBindWithoutDataType);
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

	protected abstract IEnumerable<string> GetClrNsFromXmlNs(string xmlNs);

	private TypeInfo FindType(XName className, XObject xobject)
	{
		var clrNs = XamlNamespace.GetClrNamespace(className.NamespaceName);
		if (clrNs != null)
		{
			return TypeInfo.GetTypeThrow(clrNs + "." + className.LocalName);
		}
		foreach (string clrNr2 in GetClrNsFromXmlNs(className.NamespaceName))
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

	private XamlObjectProperty GetObjectProperty(
		XamlObject obj,
		string memberName,
		XamlNode xamlNode,
		HashSet<string> includeNamespaces,
		BindingScope currentBindingScope,
		List<BindingScope> bindingScopes,
		ref TypeInfo? elementType,
		bool throwIfBindWithoutDataType)
	{
		try
		{
			PropertyInfo? targetProperty = null;
			EventInfo? targetEvent = null;
			MethodInfo? targetMethod = null;
			bool isAttached = false;
			XName? attachedClassName = null;
			string? attachedPropertyName = null;

			int index = memberName.IndexOf('.');
			if (index != -1)
			{
				string typeName = memberName.Substring(0, index);
				attachedClassName = 
					(xamlNode.Name.Namespace is var n && (n == XNamespace.None || n == mNamespace)
						? DefaultNamespace 
						: n) 
					+ typeName;
				attachedPropertyName = memberName.Substring(index + 1);

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

				memberName = attachedPropertyName!;
				targetMethod = setPropertyMethod;
				isAttached = true;
			}
			else
			{
				var typeInfo = obj.Type;
				var pi = typeInfo.Properties.FirstOrDefault(p => p.Definition.Name == memberName);
				if (pi != null)
				{
					targetProperty = pi;
				}
				else
				{
					var @event = obj.Type.Events.FirstOrDefault(e => e.Definition.Name == memberName);
					if (@event != null)
					{
						targetEvent = @event;
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
									includeNamespaces.Add(clrNs);
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

						targetMethod = method;
					}
				}
			}

			var objProp = new XamlObjectProperty
			{
				Object = obj,
				XamlNode = xamlNode,
				MemberName = memberName,
				TargetProperty = targetProperty,
				TargetMethod = targetMethod,
				TargetEvent = targetEvent,
				IsAttached = isAttached,
			};

			objProp.Value = GetObjectValue(obj, objProp, xamlNode, includeNamespaces, currentBindingScope, bindingScopes, ref elementType, throwIfBindWithoutDataType);
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

	private XamlObjectValue GetObjectValue(
		XamlObject obj,
		XamlObjectProperty objProp,
		XamlNode xamlNode,
		HashSet<string> includeNamespaces,
		BindingScope currentBindingScope,
		List<BindingScope> bindingScopes,
		ref TypeInfo? elementType,
		bool throwIfBindWithoutDataType)
	{
		var value = new XamlObjectValue();

		bool isCompiledBindingsNs =
			xamlNode.Children.Count == 1 &&
			XamlNamespace.GetClrNamespace(xamlNode.Children[0].Name.NamespaceName) == "CompiledBindings.Markup";

		var iNotifyPropChanged = TypeInfo.GetTypeThrow(typeof(INotifyPropertyChanged));

		var propType = objProp.MemberType;
		if (xamlNode.Children.Count == 1 &&
			(xamlNode.Children[0].Name == xSet ||
				isCompiledBindingsNs && xamlNode.Children[0].Name.LocalName is "Set" or "SetExtension"))
		{
			try
			{
				var staticNode = xamlNode.Children[0];
				value.StaticValue = ExpressionParser.Parse(TargetType, "this", staticNode.Value!, propType, false, GetNamespaces(xamlNode).ToList(), out var includeNamespaces2, out var dummy);
				includeNamespaces.UnionWith(includeNamespaces2.Select(ns => ns.ClrNamespace!));
				value.StaticValue = CorrectSourceExpression(value.StaticValue, objProp);
				CorrectMethod(objProp, value.StaticValue.Type);
				obj.GenerateMember = true;
			}
			catch (ParseException ex)
			{
				handleParseException(ex);
			}
		}
		else if (xamlNode.Children.Count == 1 &&
			(xamlNode.Children[0].Name == xBind ||
				(isCompiledBindingsNs && xamlNode.Children[0].Name.LocalName is "Bind" or "BindExtension")))
		{
			try
			{
				bool isPropertyTypeBinding = BindingType.IsAssignableFrom(propType);
				if (isPropertyTypeBinding && !CanSetBindingTypeProperty)
				{
					throw new GeneratorException($"You cannot use x:Bind for this property, because the property type is Binding.", CurrentFile, xamlNode);
				}

				var defaultBindModeAttr =
					EnumerableExtensions.SelectSequence(xamlNode.Element, e => e.Parent, xamlNode.Element is XElement)
						.Cast<XElement>()
						.Select(e => GetDefaultBindModeAttribute(e))
						.FirstOrDefault(a => a != null);
				var defaultBindMode = defaultBindModeAttr != null ? (BindingMode)Enum.Parse(typeof(BindingMode), defaultBindModeAttr.Value) : BindingMode.OneWay;

				var bind = BindingParser.Parse(objProp, DataType, TargetType, "dataRoot", defaultBindMode, this, includeNamespaces, throwIfBindWithoutDataType, ref _localVarIndex);

				if (isPropertyTypeBinding)
				{
					CheckBinding(bind, xamlNode);
					value.BindingValue = bind;
				}
				else
				{
					if (bind.SourceExpression != null)
					{
						if (bind.Converter == null)
						{
							bind.SourceExpression = CorrectSourceExpression(bind.SourceExpression, objProp);
						}
						CorrectMethod(objProp, bind.SourceExpression.Type);
					}

					if (DependencyPropertyType != null)
					{
						if (bind?.Mode is BindingMode.TwoWay or BindingMode.OneWayToSource &&
							bind.UpdateSourceEvents.Count == 0)
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
								bind.DependencyProperty = dp;
							}
						}
					}

					if (bind!.Mode is BindingMode.TwoWay or BindingMode.OneWayToSource &&
						bind.UpdateSourceEvents.Count == 0 &&
						bind.DependencyProperty == null &&
						!iNotifyPropChanged.IsAssignableFrom(bind.Property.Object.Type))
					{
						ResolveTargetChangeEventCore(bind);
						if (bind.UpdateSourceEvents.Count == 0)
						{
							throw new GeneratorException($"Target change event cannot be determined. Set the event explicitly by setting the UpdateSourceEventNames property.", CurrentFile, objProp.XamlNode);
						}
					}

					if (bind.DataTypeSet)
					{
						BindingScope? scope = null;
						if (bind.DataType == null)
						{
							scope = bindingScopes.FirstOrDefault(s => s.DataType == null);
						}
						if (scope != null)
						{
							scope.Bindings.Add(bind);
						}
						else
						{
							scope = new BindingScope
							{
								DataType = bind.DataType,
								ViewName = obj.Name,
							};
							bindingScopes.Add(scope);
						}
					}
					else
					{
						currentBindingScope.Bindings.Add(bind);
					}

					if (bind.IsItemsSource)
					{
						if (elementType != null)
						{
							throw new ParseException(Res.IsItemsSourceAlreadySet);
						}
						elementType = bind.Expression!.Type.GetItemType();
						if (elementType == null)
						{
							var expr = Expression.StripParenExpression(bind.Expression);
							if (expr is CastExpression cast)
							{
								elementType = cast.Expression.Type.GetItemType();
							}
							if (elementType == null)
							{
								throw new ParseException(Res.ElementTypeCannotBeInferred);
							}
						}
					}

					value.BindValue = bind;
					obj.GenerateMember = true;
				}
			}
			catch (ParseException ex)
			{
				handleParseException(ex);
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

		void handleParseException(ParseException ex)
		{
			var offset = objProp.MemberName.Length +
					2 + // ="    Note, if there are spaces in between, they are not considered
					xamlNode.Children[0].ValueOffset +
					ex.Position;
			throw new ParseException(ex.Message, offset, ex.Length);
		}
	}

	protected virtual XAttribute GetDefaultBindModeAttribute(XElement element)
	{
		return element.Attribute(xDefaultBindMode);
	}

	protected virtual bool CanSetBindingTypeProperty => false;

	protected virtual void CheckBinding(Bind bind, XamlNode xamlNode)
	{
		if (bind.Mode is BindingMode.TwoWay or BindingMode.OneWayToSource)
		{
			throw new GeneratorException($"TwoWay or OneWayToSource x:Binds are not supported if the property type is a standard Binding.", CurrentFile, xamlNode);
		}
	}

	private static Expression CorrectSourceExpression(Expression expression, XamlObjectProperty prop)
	{
		if (prop.TargetEvent == null)
		{
			var targetType = prop.MemberType;
			if (expression is not DefaultExpression &&
				!TypeInfo.GetTypeThrow(typeof(System.Threading.Tasks.Task)).IsAssignableFrom(expression.Type) &&
				targetType.Reference.FullName == "System.String" && expression.Type.Reference.FullName != "System.String")
			{
				var method = TypeInfo.GetTypeThrow(typeof(object)).Methods.First(m => m.Definition.Name == "ToString");
				if (expression is UnaryExpression or BinaryExpression or CoalesceExpression)
				{
					expression = new ParenExpression(expression);
				}
				expression = new CallExpression(expression, method, Array.Empty<Expression>());
			}
			if (expression.IsNullable && !targetType.IsNullable)
			{
				Expression defaultExpr;
				if (targetType.Reference.FullName == "System.String")
				{
					defaultExpr = new ConstantExpression("");
				}
				else
				{
					defaultExpr = Expression.DefaultExpression;
				}

				return new CoalesceExpression(expression, defaultExpr);
			}
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

	protected virtual void ResolveTargetChangeEventCore(Bind binding)
	{
	}

	private static class Res
	{
		public const string IsItemsSourceAlreadySet = "IsItemsSource is already set in some other x:Bind of this element.";
		public const string ElementTypeCannotBeInferred = "Element type cannot be inferred. Set IsItemsSource to false or remove, and set DataType for child DataTemplates manually.";
	}
}
