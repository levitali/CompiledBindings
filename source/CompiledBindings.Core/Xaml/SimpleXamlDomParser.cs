namespace CompiledBindings;

public class SimpleXamlDomParser : XamlDomParser
{
	public static readonly XNamespace mNamespace = "http://compiledbindings.com";
	public static readonly XNamespace mxNamespace = "http://compiledbindings.com/x";
	public static readonly XName mxDataType = mxNamespace + "DataType";

	public static readonly XName NameAttr = XNamespace.None + "Name"; // WPF Name attribute
	public static readonly XName DataTypeAttr = XNamespace.None + "DataType"; // WPF DataType attribute

	public readonly XName HierarchicalDataTemplate; // WPF

	public HashSet<string>? UsedNames { get; private set; }

	public SimpleXamlDomParser(
		XNamespace xmlns,
		XNamespace xNs,
		Func<string, IEnumerable<string>> getClrNsFromXmlNs,
		TypeInfo converterType,
		TypeInfo bindingType,
		TypeInfo? dependencyObjectType = null,
		TypeInfo? dependencyPropertyType = null)
		: base(xmlns, xNs, getClrNsFromXmlNs, converterType, bindingType, dependencyObjectType, dependencyPropertyType)
	{
		HierarchicalDataTemplate = DefaultNamespace + "HierarchicalDataTemplate";
	}

	public SimpleXamlDom? Parse(string file, string lineFile, XDocument xdoc, Action<int, int, int, string> log)
	{
		bool errors = false;

		CurrentFile = file;
		CurrentLineFile = lineFile;

		UsedNames = new HashSet<string>(xdoc.Descendants().Select(e => e.Attribute(xName)).Where(a => a != null).Select(a => a.Value).Distinct());

		TargetType = DataType = GetRootType(xdoc.Root);

		var includeNamespaces = new HashSet<string>();
		var dataTemplates = new List<GeneratedClass>();

		var generationRoot = ProcessRoot(xdoc.Root, null, TargetType);

		return errors ? null : new SimpleXamlDom
		{
			TargetType = TargetType,
			GeneratedClass = generationRoot,
			IncludeNamespaces = includeNamespaces,
			DataTemplates = dataTemplates,
		};

		GeneratedClass ProcessRoot(XElement xroot, TypeInfo? dataType, TypeInfo? targetType)
		{
			var rootBindingScope = new BindingScope { DataType = dataType };
			var bindingScopes = new List<BindingScope> { rootBindingScope };
			var xamlObjects = new List<XamlObject>();

			var savedDataType = DataType;
			if (dataType != null)
			{
				DataType = dataType;
			}

			var obj = ProcessElement(xroot, rootBindingScope, null, true, null);
			if (obj != null)
			{
				obj.IsRoot = true;
			}

			for (int i = bindingScopes.Count - 1; i >= 0; i--)
			{
				if (bindingScopes[i].Bindings.Count == 0)
				{
					bindingScopes.RemoveAt(i);
				}
				else
				{
					var s = bindingScopes[i];
					s.BindingsData = BindingParser.CreateBindingsData(s.Bindings, targetType, s.DataType ?? targetType!, DependencyObjectType);
				}
			}

			var updateMethod = ExpressionUtils.GroupExpressions(xamlObjects);

			DataType = savedDataType!;

			return new GeneratedClass
			{
				RootElement = xroot,
				UpdateMethod = updateMethod,
				BindingScopes = bindingScopes,
				XamlObjects = xamlObjects,
			};

			XamlObject? ProcessElement(XElement xelement, BindingScope currentBindingScope, TypeInfo? elementType, bool isSupportedParent, string? parentDescription)
			{
				var savedCurrentBindingScope = currentBindingScope;
				var savedDataType = DataType;

				bool isDataTemplateElement = xelement.Name == DataTemplate || xelement.Name == HierarchicalDataTemplate;

				var attrs = xelement.Attributes().Where(a => IsMemExtension(a) != null).ToList();
				if (attrs.Count > 0)
				{
					if (isDataTemplateElement)
					{
						throw new GeneratorException("x:Bind or x:Set extensions cannot be used to set properties of a DataTemplate.", CurrentFile, attrs[0]);
					}
					if (!isSupportedParent)
					{
						throw new GeneratorException($"x:Bind and x:Set extensions are not supported in {parentDescription}.", CurrentFile, attrs[0]);
					}
				}

				var dataTypeAttr = xelement.Attribute(xDataType) ?? xelement.Attribute(mxDataType) ?? xelement.Attribute(DataTypeAttr);
				if (dataTypeAttr != null &&
					!xelement.DescendantsAndSelf().SelectMany(e => e.Attributes()).Any(a =>
						IsMemExtension(a) == ExtenstionType.Bind))
				{
					dataTypeAttr = null;
				}

				if (dataTypeAttr != null && !IsDataContextSupported(TargetType))
				{
					throw new GeneratorException("The root type does not support setting data type. The data source can only be the page or user control itself.", CurrentFile, dataTypeAttr);
				}

				string? viewName = null;
				bool nameExplicitlySet = false;
				if (xelement != xroot &&
					(attrs.Count > 0 ||
					 (dataTypeAttr != null && !isDataTemplateElement)))
				{
					viewName = xelement.Attribute(xName)?.Value ?? xelement.Attribute(NameAttr)?.Value;
					if (viewName == null && xelement != xdoc.Root)
					{
						viewName = XamlDomParser.GenerateName(xelement, UsedNames);
					}
					else
					{
						nameExplicitlySet = true;
					}
				}

				// Process x:DataType attribute
				if (dataTypeAttr != null)
				{
					TypeInfo? dataType;
					try
					{
						var type = FindTypeFromAttribute(dataTypeAttr);
						dataType = type == null ? null : new TypeInfo(type, false);
					}
					catch (Exception ex)
					{
						throw new GeneratorException(ex.Message, file, dataTypeAttr, dataTypeAttr.Value.Length);
					}
					if (currentBindingScope.DataType?.Reference.FullName != dataType?.Reference.FullName)
					{
						DataType = dataType ?? targetType!;
						BindingScope bs;
						if (dataType == null && (bs = bindingScopes.FirstOrDefault(s => s.DataType == null)) != null)
						{
							currentBindingScope = bs;
						}
						else
						{
							currentBindingScope = new BindingScope
							{
								DataType = dataType,
								ViewName = viewName
							};
							bindingScopes.Add(currentBindingScope);
						}
					}
				}

				XamlObject? obj = null;

				if (attrs.Count > 0 ||
					(dataTypeAttr != null && !isDataTemplateElement && xelement != xdoc.Root))
				{
					var type = xelement == xdoc.Root ? targetType! : FindType(xelement);
					obj = new XamlObject(new XamlNode(CurrentLineFile, xelement, xelement.Name), type)
					{
						Name = viewName,
						NameExplicitlySet = nameExplicitlySet
					};
					xamlObjects.Add(obj);

					foreach (var attr in attrs)
					{
						try
						{
							var xamlNode = XamlParser.ParseAttribute(CurrentLineFile, attr, KnownNamespaces);
							var prop = GetObjectProperty(obj, xamlNode, xroot != xdoc.Root && currentBindingScope.DataType == null, includeNamespaces);
							obj.Properties.Add(prop);

							var bind = prop.Value.BindValue;
							if (bind != null)
							{
								if (bind.DataTypeSet)
								{
									BindingScope? scope;
									if (bind.DataType == null)
									{
										scope = bindingScopes.FirstOrDefault(s => s.DataType == null);
										if (scope != null)
										{
											scope.Bindings.Add(bind);
											continue;
										}
									}
									scope = new BindingScope
									{
										DataType = bind.DataType,
										ViewName = viewName
									};
									bindingScopes.Add(scope);
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
							}
						}
						catch (GeneratorException ex)
						{
							log(ex.LineNumber, ex.ColumnNumber, ex.EndColumnNumber, ex.Message);
							errors |= true;
						}
						catch (ParseException ex)
						{
							int lineNumber = 0, columnNumber = attr.Name.LocalName.Length + 1 + ex.Position;
							if (attr is IXmlLineInfo li)
							{
								lineNumber = li.LineNumber;
								columnNumber += li.LinePosition;
							}
							int endColumnNumber = columnNumber + ex.Length;

							log(lineNumber, columnNumber, endColumnNumber, ex.Message);
							errors |= true;
						}
					}
				}

				foreach (var child in xelement.Elements())
				{
					if (child.Name == DataTemplate || child.Name == HierarchicalDataTemplate)
					{
						int index = dataTemplates.Count;
						var dataTemplate = ProcessRoot(child, elementType, null);
						if (dataTemplate.GenerateCode)
						{
							dataTemplates.Insert(index, dataTemplate);
						}
					}
					else
					{
						var (isSupported, controlName) = IsElementSupported(child.Name);
						ProcessElement(child, currentBindingScope, elementType, isSupported && isSupportedParent, controlName ?? parentDescription);
					}
				}

				DataType = savedDataType;
				currentBindingScope = savedCurrentBindingScope;

				return obj;
			}
		}
	}

	public virtual ExtenstionType? IsMemExtension(XAttribute a)
	{
		var ns = a.Name.Namespace.NamespaceName;
		if (ns.EndsWith("/"))
		{
			ns = ns.Substring(0, ns.Length - 1);
		}
		bool isSet = false;
		if (ns == mNamespace.NamespaceName ||
			   a.Value.StartsWith("[x:Bind ") ||
			   (isSet = a.Value.StartsWith("{x:Set ")) ||
			   (isSet = a.Value.StartsWith("[x:Set ")))
		{
			return isSet ? ExtenstionType.Set : ExtenstionType.Bind;
		}
		var xamlNode = XamlParser.ParseAttribute(CurrentFile, a, KnownNamespaces);
		if (xamlNode.Children.Count == 1)
		{
			var c = xamlNode.Children[0].Name;
			var clrNs = XamlNamespace.GetClrNamespace(c.NamespaceName);
			if (clrNs == "CompiledBindings.Markup")
			{
				return c.LocalName switch
				{
					"Bind" or "BindExtension" => ExtenstionType.Bind,
					"Set" or "SetExtension" => ExtenstionType.Set,
					_ => null
				};
			}
		}
		return null;
	}

	public virtual (bool isSupported, string? controlName) IsElementSupported(XName elementName)
	{
		if (elementName == VisualStatesGroups)
		{
			return (false, "VisualStateManager");
		}
		return (true, null);
	}

	public virtual bool IsDataContextSupported(TypeInfo type) => true;

	private static class Res
	{
		public const string IsItemsSourceAlreadySet = "IsItemsSource is already set in some other x:Bind of this element.";
		public const string ElementTypeCannotBeInferred = "Element type cannot be inferred. Set IsItemsSource to false or remove, and set DataType for child DataTemplates manually.";
	}
}

public enum ExtenstionType
{
	Bind,
	Set
}

public class GeneratedClass
{
	public required XElement RootElement { get; init; }
	public required List<BindingScope> BindingScopes { get; init; }
	public required List<XamlObject> XamlObjects { get; init; }
	public required ExpressionGroup UpdateMethod { get; init; }

	public IEnumerable<XamlObjectProperty> EnumerateAllProperties()
	{
		return XamlObjects.SelectMany(o => o.Properties);
	}

	public bool GenerateCode =>
		BindingScopes.Count > 0 ||
		!UpdateMethod.IsEmpty;
}

public class SimpleXamlDom : XamlDomBase
{
	public required TypeInfo TargetType { get; init; }

	public required GeneratedClass GeneratedClass { get; init; }

	public required List<GeneratedClass> DataTemplates { get; init; }

	public bool GenerateCode => GeneratedClass.GenerateCode || DataTemplates.Any(t => t.GenerateCode);

	public IEnumerable<XamlObject> EnumerateAllObjects()
	{
		return GeneratedClass.XamlObjects.Concat(DataTemplates.SelectMany(_ => _.XamlObjects));
	}

	public IEnumerable<XamlObjectProperty> EnumerateAllProperties()
	{
		return GeneratedClass.EnumerateAllProperties().Concat(DataTemplates.SelectMany(_ => _.EnumerateAllProperties()));
	}
}
