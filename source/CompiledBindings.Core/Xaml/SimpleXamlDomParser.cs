namespace CompiledBindings;

public abstract class SimpleXamlDomParser : XamlDomParser
{
	public static readonly XNamespace mNamespace = "http://compiledbindings.com";
	public static readonly XNamespace mxNamespace = "http://compiledbindings.com/x";
	public static readonly XName mxDataType = mxNamespace + "DataType";
	public static readonly XName mxDefaultBindMode = mxNamespace + "DefaultBindMode";

	public static readonly XName NameAttr = XNamespace.None + "Name"; // WPF Name attribute
	public static readonly XName DataTypeAttr = XNamespace.None + "DataType"; // WPF DataType attribute

	public XName HierarchicalDataTemplate { get; } // WPF
	public XName ControlTemplate { get; }

	public HashSet<string>? UsedNames { get; private set; }

	public SimpleXamlDomParser(
		XNamespace xmlns,
		XNamespace xNs,
		TypeInfo converterType,
		TypeInfo bindingType,
		TypeInfo? dependencyObjectType = null,
		TypeInfo? dependencyPropertyType = null)
		: base(xmlns, xNs, converterType, bindingType, dependencyObjectType, dependencyPropertyType)
	{
		HierarchicalDataTemplate = DefaultNamespace + "HierarchicalDataTemplate";
		ControlTemplate = DefaultNamespace + "ControlTemplate";
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

		var generationRoot = processRoot(xdoc.Root, null, TargetType);

		return errors ? null : new SimpleXamlDom
		{
			TargetType = TargetType,
			GeneratedClass = generationRoot,
			IncludeNamespaces = includeNamespaces,
			DataTemplates = dataTemplates,
		};

		GeneratedClass processRoot(XElement xroot, TypeInfo? dataType, TypeInfo? targetType)
		{
			var rootBindingScope = new BindingScope { DataType = dataType };
			var bindingScopes = new List<BindingScope> { rootBindingScope };
			var xamlObjects = new List<XamlObject>();

			var savedDataType = DataType;
			if (dataType != null)
			{
				DataType = dataType;
			}

			var obj = processElement(xroot, rootBindingScope, true, null);
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

			XamlObject? processElement(XElement xelement, BindingScope currentBindingScope, bool isSupportedParent, string? parentDescription)
			{
				var savedDataType = DataType;

				bool isDataTemplateElement = xelement.Name == DataTemplate ||
					                         xelement.Name == HierarchicalDataTemplate ||
											 xelement.Name == ControlTemplate;

				var attrs = xelement.Attributes().Where(a => isMemExtensionWrapper(a) != null).ToList();
				if (attrs.Count > 0)
				{
					if (isDataTemplateElement)
					{
						throw new GeneratorException("x:Bind or x:Set extensions cannot be used to set properties of a DataTemplate/ControlTemplate.", CurrentFile, attrs[0]);
					}
					if (!isSupportedParent)
					{
						throw new GeneratorException($"x:Bind and x:Set extensions are not supported in {parentDescription}.", CurrentFile, attrs[0]);
					}
				}

				var dataTypeAttr = xelement.Attribute(xDataType) ?? xelement.Attribute(mxDataType) ?? xelement.Attribute(DataTypeAttr);
				if (dataTypeAttr != null &&
					!xelement.DescendantsAndSelf().SelectMany(e => e.Attributes()).Any(a =>
						isMemExtensionWrapper(a) == ExtenstionType.Bind))
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
				TypeInfo? elementType = null;

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
							var prop = GetObjectProperty(obj, xamlNode, includeNamespaces, currentBindingScope, bindingScopes, ref elementType, xroot != xdoc.Root && currentBindingScope.DataType == null);
							obj.Properties.Add(prop);
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

				if (elementType != null)
				{
					DataType = elementType;
				}

				foreach (var child in xelement.Elements())
				{
					if (child.Name == DataTemplate ||
						child.Name == HierarchicalDataTemplate ||
						child.Name == ControlTemplate)
					{
						int index = dataTemplates.Count;
						var dataTemplate = processRoot(child, elementType ?? DataType, null);
						dataTemplates.Insert(index, dataTemplate);
					}
					else
					{
						var (isSupported, controlName) = IsElementSupported(child.Name);
						processElement(child, currentBindingScope, isSupported && isSupportedParent, controlName ?? parentDescription);
					}
				}

				DataType = savedDataType;

				return obj;
			}
		}

		ExtenstionType? isMemExtensionWrapper(XAttribute attribute)
		{
			try
			{
				return IsMemExtension(attribute);
			}
			catch (ParseException ex)
			{
				throw new GeneratorException(ex.Message, file, attribute, ex.Position, ex.Length);
			}
			catch (Exception ex)
			{
				throw new GeneratorException(ex.Message, file, attribute);
			}
		}
	}

	public virtual ExtenstionType? IsMemExtension(XAttribute a)
	{
		var xamlNode = XamlParser.ParseAttribute(CurrentLineFile, a, KnownNamespaces);
		if (xamlNode.Children.Count == 1)
		{
			var name = xamlNode.Children[0].Name;
			var res = IsMemExtension(name);
			if (res != null)
			{
				return res;
			}
			if (a.Name.Namespace == mNamespace && name.Namespace == xNamespace)
			{
				return name.LocalName switch
				{
					"Bind" or "BindExtension" => ExtenstionType.Bind,
					"Set" or "SetExtension" => ExtenstionType.Set,
					_ => null
				};
			}
		}
		return null;
	}

	protected virtual ExtenstionType? IsMemExtension(XName name)
	{
		var clrNs = XamlNamespace.GetClrNamespace(name.NamespaceName);
		if (clrNs == "CompiledBindings.Markup")
		{
			return name.LocalName switch
			{
				"Bind" or "BindExtension" => ExtenstionType.Bind,
				"Set" or "SetExtension" => ExtenstionType.Set,
				_ => null
			};
		}
		return null;
	}

	public virtual (bool isSupported, string? controlName) IsElementSupported(XName elementName)
	{
		if (elementName == VisualStateGroups)
		{
			return (false, "VisualStateManager");
		}
		if (elementName == Style)
		{
			return (false, "a Style");
		}
		return (true, null);
	}

	public virtual bool IsDataContextSupported(TypeInfo type) => true;

	protected override XAttribute GetDefaultBindModeAttribute(XElement element)
	{
		return base.GetDefaultBindModeAttribute(element) ?? element.Attribute(mxDefaultBindMode); ;
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

	public IEnumerable<Bind> EnumerateBindings()
	{
		return EnumerateAllProperties().Select(p => p.Value.BindingValue!).Where(b => b != null);
	}

	public IEnumerable<(string name, TypeInfo type)> EnumerateResources()
	{
		return XamlObjects
			.SelectMany(o => o.Properties)
			.Select(p => p.Value.BindValue)
			.Where(b => b != null)
			.SelectMany(b => b!.Resources)
			.Select(b => (b.name, b.type))
			.Union(XamlObjects
				.SelectMany(o => o.Properties)
				.Select(p => p.Value.StaticValue ?? p.Value.BindValue?.SourceExpression)
				.Where(e => e != null)
				.SelectMany(e => e!.EnumerateTree())
				.OfType<StaticResourceExpression>()
				.Select(e => (name: e.Name, type: e.Type)))
			.Distinct(b => b.name);
	}

	public bool GenerateClass => BindingScopes.Count > 0 || !UpdateMethod.IsEmpty;

	public bool GenerateCode => GenerateClass || EnumerateBindings().Any();
}

public class SimpleXamlDom : XamlDomBase
{
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
