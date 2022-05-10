#nullable enable

namespace CompiledBindings;

public class SimpleXamlDomParser : XamlDomParser
{
	public static readonly XNamespace mNamespace = "http://compiledbindings.com/";
	public static readonly XNamespace mxNamespace = "http://compiledbindings.com/x";
	public static readonly XName mxDataType = mxNamespace + "DataType";

	public static readonly XName NameAttr = XNamespace.None + "Name"; // WPF Name attribute
	public static readonly XName DataTypeAttr = XNamespace.None + "DataType"; // WPF DataType attribute

	public readonly XName HierarchicalDataTemplate; // WPF
	public readonly TypeInfo? DependencyObjectType;

	public HashSet<string>? UsedNames { get; private set; }

	public SimpleXamlDomParser(
		XNamespace xmlns,
		XNamespace xNs,
		Func<string, IEnumerable<string>> getClrNsFromXmlNs,
		TypeInfo converterType,
		TypeInfo bindingType,
		TypeInfo? dependencyObjectType = null)
		: base(xmlns, xNs, getClrNsFromXmlNs, converterType, bindingType)
	{
		HierarchicalDataTemplate = DefaultNamespace + "HierarchicalDataTemplate";
		DependencyObjectType = dependencyObjectType;
	}

	public SimpleXamlDom Parse(string file, string lineFile, XDocument xdoc)
	{
		CurrentFile = file;
		CurrentLineFile = lineFile;

		UsedNames = new HashSet<string>(xdoc.Descendants().Select(e => e.Attribute(xName)).Where(a => a != null).Select(a => a.Value).Distinct());

		TargetType = DataType = GetRootType(xdoc.Root);
		var result = new SimpleXamlDom(xdoc.Root)
		{
			TargetType = TargetType
		};

		ProcessRoot(result, xdoc.Root, null);

		result.HasDestructor = result.TargetType.Methods.Any(m =>
			m.Definition.DeclaringType == result.TargetType.Type &&
			m.Definition.Name == "Finalize" && m.Definition.IsVirtual && m.Definition.IsFamily && m.Definition.IsHideBySig);

		return result;

		void ProcessRoot(SimpleXamlDom rootResult, XElement xroot, TypeInfo? dataType)
		{
			var rootBindingScope = new BindingScope { DataType = dataType };
			rootResult.BindingScopes.Add(rootBindingScope);
			rootResult.XamlObjects = new List<XamlObject>();

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

			for (int i = rootResult.BindingScopes.Count - 1; i >= 0; i--)
			{
				if (rootResult.BindingScopes[i].Bindings.Count == 0)
				{
					rootResult.BindingScopes.RemoveAt(i);
				}
				else
				{
					var s = rootResult.BindingScopes[i];
					s.BindingsData = BindingParser.CreateBindingsData(s.Bindings, rootResult.TargetType, s.DataType ?? rootResult.TargetType!, DependencyObjectType);
				}
			}

			rootResult.UpdateMethod = ExpressionUtils.CreateUpdateMethod(rootResult.XamlObjects);

			DataType = savedDataType;

			XamlObject? ProcessElement(XElement xelement, BindingScope currentBindingScope, TypeInfo? elementType, bool isSupportedParent, string? parentDescription)
			{
				var savedCurrentBindingScope = currentBindingScope;
				var savedDataType = DataType;

				bool isDataTemplateElement = xelement.Name == DataTemplate || xelement.Name == HierarchicalDataTemplate;

				var attrs = xelement.Attributes().Where(a => IsMemExtension(a)).ToList();
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
						IsMemExtension(a) && a.Value.StartsWith("{x:Bind ")))
				{
					dataTypeAttr = null;
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
					if (currentBindingScope.DataType?.Type.FullName != dataType?.Type.FullName)
					{
						DataType = dataType ?? result.TargetType;
						BindingScope bs;
						if (dataType == null && (bs = result.BindingScopes.FirstOrDefault(s => s.DataType == null)) != null)
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
							rootResult.BindingScopes.Add(currentBindingScope);
						}
					}
				}

				XamlObject? obj = null;

				if (attrs.Count > 0 ||
					(dataTypeAttr != null && !isDataTemplateElement && xelement != xdoc.Root))
				{
					var type = xelement == xdoc.Root ? result.TargetType : FindType(xelement);
					obj = new XamlObject(new XamlNode(CurrentLineFile, xelement, xelement.Name), type)
					{
						Name = viewName,
						NameExplicitlySet = nameExplicitlySet
					};
					rootResult.XamlObjects.Add(obj);

					if (attrs.Count > 0)
					{
						foreach (var attr in attrs)
						{
							try
							{
								var xamlNode = XamlParser.ParseAttribute(CurrentLineFile, attr, KnownNamespaces);
								var prop = GetObjectProperty(result, obj, xamlNode, xroot != xdoc.Root && currentBindingScope.DataType == null);
								obj.Properties.Add(prop);

								var bind = prop.Value.BindValue;
								if (bind != null)
								{
									if (bind.DataTypeSet)
									{
										BindingScope? scope;
										if (bind.DataType == null)
										{
											scope = rootResult.BindingScopes.FirstOrDefault(s => s.DataType == null);
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
										rootResult.BindingScopes.Add(scope);
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
											throw new ParseException(Res.ElementTypeCannotBeInferred);
										}
									}
								}
							}
							catch (GeneratorException ex)
							{
								ex.File = file;
								throw;
							}
							catch (ParseException ex)
							{
								var lineInfo = new LineInfo { File = file };
								lineInfo.ColumnNumber = attr.Name.LocalName.Length + 1 + ex.Position;
								if (attr is IXmlLineInfo li)
								{
									lineInfo.LineNumber = li.LineNumber;
									lineInfo.ColumnNumber += li.LinePosition;
								}
								lineInfo.EndColumnNumber = lineInfo.ColumnNumber + ex.Length;
								throw new GeneratorException(ex.Message, lineInfo);
							}
						}
					}
				}

				foreach (var child in xelement.Elements())
				{
					if (child.Name == DataTemplate || child.Name == HierarchicalDataTemplate)
					{
						var dataTemplate = new SimpleXamlDom(child);
						int index = result.DataTemplates.Count;
						ProcessRoot(dataTemplate, child, elementType);
						if (dataTemplate.BindingScopes.Count > 0 || !dataTemplate.UpdateMethod!.IsEmpty)
						{
							result.DataTemplates.Insert(index, dataTemplate);
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

	public virtual bool IsMemExtension(XAttribute a)
	{
		return a.Name.Namespace == mNamespace ||
			   a.Value.StartsWith("[x:Bind ") ||
			   a.Value.StartsWith("{x:Set ") ||
			   a.Value.StartsWith("[x:Set ");
	}

	public virtual (bool isSupported, string? controlName) IsElementSupported(XName elementName)
	{
		if (elementName == VisualStatesGroups)
		{
			return (false, "VisualStateManager");
		}
		return (true, null);
	}

	private static class Res
	{
		public const string IsItemsSourceAlreadySet = "IsItemsSource is already set in some other x:Bind of this element.";
		public const string ElementTypeCannotBeInferred = "Element type cannot be inferred. Set IsItemsSource to false or remove, and set DataType for child DataTemplates manually.";
	}
}

public class SimpleXamlDom : XamlDomBase
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public SimpleXamlDom(XElement rootElement)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	{
		RootElement = rootElement;
	}

	public XElement RootElement { get; }
	public TypeInfo? TargetType;
	public List<BindingScope> BindingScopes = new List<BindingScope>();
	public List<XamlObject>? XamlObjects;
	public bool HasDestructor;

	public UpdateMethod UpdateMethod;

	public List<SimpleXamlDom> DataTemplates = new List<SimpleXamlDom>();

	public bool GenerateInitializeMethod =>
		BindingScopes.Count > 0 ||
		UpdateMethod.SetExpressions.Count > 0 ||
		UpdateMethod.SetProperties?.Count > 0;

	public bool GenerateCode =>
		GenerateInitializeMethod ||
		DataTemplates.Any(t => t.GenerateCode);

	public IEnumerable<XamlObject> EnumerateAllObjects()
	{
		return EnumerableExtensions.SelectTree(this, p => p.DataTemplates, true).SelectMany(p => p.XamlObjects);
	}

	public IEnumerable<XamlObjectProperty> EnumerateAllProperties()
	{
		return EnumerateAllObjects().SelectMany(o => o.Properties);
	}

	public void SetDependecyPropertyChangedEventHandlers(string dependencyPropertyType)
	{
		foreach (var prop in EnumerateAllProperties())
		{
			if (prop.Value.BindValue?.Mode is BindingMode.TwoWay or BindingMode.OneWayToSource &&
				prop.Value.BindValue.UpdateSourceTrigger != UpdateSourceTrigger.Explicit &&
				prop.Value.BindValue.TargetChangedEvent == null)
			{
				var dpName = prop.TargetProperty!.Definition.Name + "Property";
				IMemberInfo dp = prop.Object.Type.Fields.FirstOrDefault(f =>
					f.Definition.Name == dpName &&
					f.Definition.IsStatic &&
					f.FieldType.Type.FullName == dependencyPropertyType);
				if (dp == null)
				{
					dp = prop.Object.Type.Properties.FirstOrDefault(p =>
						p.Definition.Name == dpName &&
						p.Definition.IsStatic() &&
						p.PropertyType.Type.FullName == dependencyPropertyType);
				}
				if (dp != null)
				{
					prop.Value.BindValue.IsDPChangeEvent = true;
				}
			}
		}
	}

	public void Validate(string file)
	{
		BindingScopes.ForEach(bs => bs.BindingsData?.Validate(file));
	}
}
