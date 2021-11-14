﻿using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

#nullable enable

namespace CompiledBindings;

public class SimpleXamlDomParser : XamlDomParser
{
	public static readonly XNamespace mcNamespace = "http://schemas.openxmlformats.org/markup-compatibility/2006";
	public static readonly XNamespace mNamespace = "http://compiledbindings.com/";
	public static readonly XNamespace mxNamespace = "http://compiledbindings.com/x";
	public static readonly XName mcIgnorable = mcNamespace + "Ignorable";
	public static readonly XName mxDataType = mxNamespace + "DataType";

	public static readonly XName NameAttr = XNamespace.None + "Name"; // WPF Name attribute
	public static readonly XName DataTypeAttr = XNamespace.None + "DataType"; // WPF DataType attribute

	public readonly XName DataTemplate;

	public TypeInfo? DependencyObjectType;

	public HashSet<string>? UsedNames { get; private set; }

	public SimpleXamlDomParser(XNamespace xmlns, XNamespace xNs, Func<string, IEnumerable<string>> getClrNsFromXmlNs) : base(xmlns, xNs)
	{
		DataTemplate = DefaultNamespace + "DataTemplate";

		GetClrNsFromXmlNs = getClrNsFromXmlNs;
	}

	public SimpleXamlDom Parse(string file, XDocument xamlDoc)
	{
		File = file;

		UsedNames = new HashSet<string>(xamlDoc.Descendants().Select(e => e.Attribute(xName)).Where(a => a != null).Select(a => a.Value).Distinct());

		TargetType = DataType = GetRootType(xamlDoc.Root);
		var result = new SimpleXamlDom(xamlDoc.Root)
		{
			TargetType = TargetType
		};

		var visualStatesGroupsName = DefaultNamespace + "VisualStateManager.VisualStateGroups";

		ProcessRoot(result, xamlDoc.Root, null);

		result.HasDestructor = result.TargetType.Type.ResolveEx()!.Methods.Any(m => m.Name == "Finalize" && m.IsVirtual && m.IsFamily && m.IsHideBySig);

		return result;

		void ProcessRoot(SimpleXamlDom rootResult, XElement xroot, TypeInfo? dataType)
		{
			var rootBindingScope = new BindingScope() { DataType = dataType };
			rootResult.BindingScopes.Add(rootBindingScope);
			rootResult.XamlObjects = new List<XamlObject>();

			var savedDataType = DataType;
			if (dataType != null)
			{
				DataType = dataType;
			}

			var obj = ProcessElement(xroot, rootBindingScope, null);
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

			rootResult.StaticUpdate = ExpressionUtils.CreateUpdateMethod(rootResult.XamlObjects);

			DataType = savedDataType;

			XamlObject? ProcessElement(XElement xelement, BindingScope currentBindingScope, TypeInfo? elementType)
			{
				var savedCurrentBindingScope = currentBindingScope;
				var savedDataType = DataType;

				var attrs = xelement.Attributes().Where(a => IsMemExtension(a)).ToList();

				var dataTypeAttr = xelement.Attribute(xDataType) ?? xelement.Attribute(mxDataType) ?? xelement.Attribute(DataTypeAttr);
				if (dataTypeAttr != null &&
					!xelement.DescendantsAndSelf().SelectMany(e => e.Attributes()).Any(a =>
						IsMemExtension(a) && a.Value.StartsWith("{x:Bind ")))
				{
					dataTypeAttr = null;
				}

				string? viewName = null;
				bool nameExplicitlySet = false;
				if (xelement != xroot && (attrs.Count > 0 || (dataTypeAttr != null && xelement.Name != DataTemplate)))
				{
					viewName = xelement.Attribute(xName)?.Value ?? xelement.Attribute(NameAttr)?.Value;
					if (viewName == null && xelement != xamlDoc.Root)
					{
						viewName = XamlDomParser.GenerateName(xelement, UsedNames);
					}
					else
					{
						nameExplicitlySet = true;
					}
				}

				// Process x:DataType attribute
				TypeInfo? dataType;
				if (dataTypeAttr != null)
				{
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

				if (attrs.Count > 0 || (dataTypeAttr != null && xelement.Name != DataTemplate && xelement != xamlDoc.Root))
				{
					var type = xelement == xamlDoc.Root ? result.TargetType : FindType(xelement);
					obj = new XamlObject(new XamlNode(file, xelement, xelement.Name), type)
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
								var xamlNode = XamlParser.ParseAttribute(file, attr, KnownNamespaces);
								var prop = GetObjectProperty(obj, xamlNode);
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
											throw new ParseException("IsItemsSource is already set in some other x:Bind of this element.");
										}
										var eType = bind.Expression!.Type.Type.GetItemType();
										if (eType == null)
										{
											throw new ParseException("Element type cannot be inferred. Set IsItemsSource to false or remove, and set DataType for child DataTemplates manually.");
										}
										elementType = new TypeInfo(eType);
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
								var lineInfo = new LineInfo() { File = file };
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
					if (child.Name == DataTemplate)
					{
						if (child.Descendants().SelectMany(e => e.Attributes()).Any(a => IsMemExtension(a)))
						{
							if (elementType == null && child.Attribute(xDataType) == null && child.Attribute(mxDataType) == null && child.Attribute(DataTypeAttr) == null &&
								EnumerableExtensions.SelectTree(child, c => c.Attribute(xDataType) == null && child.Attribute(mxDataType) == null && child.Attribute(DataTypeAttr) == null ? c.Elements() : XElement.EmptySequence)
									.SelectMany(e => e.Attributes())
									.Any(a => IsMemExtension(a) && a.Value.StartsWith("{x:Bind ")))
							{
								throw new GeneratorException("DataType must be set for a DataTemplate.", file, child);
							}

							var dataTemplate = new SimpleXamlDom(child);
							int index = result.DataTemplates.Count;
							ProcessRoot(dataTemplate, child, elementType);
							if (dataTemplate.BindingScopes.Count > 0 || !dataTemplate.StaticUpdate!.IsEmpty)
							{
								result.DataTemplates.Insert(index, dataTemplate);
							}
						}
					}
					else if (child.Name != visualStatesGroupsName)
					{
						ProcessElement(child, currentBindingScope, elementType);
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
		return a.Name.Namespace == mNamespace || a.Value.StartsWith("{x:Set ");
	}
}

public class SimpleXamlDom
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

	public UpdateMethod StaticUpdate;

	public List<SimpleXamlDom> DataTemplates = new List<SimpleXamlDom>();

	public bool GenerateInitializeMethod =>
		BindingScopes.Count > 0 ||
		StaticUpdate.SetExpressions.Count > 0 ||
		StaticUpdate.SetProperties?.Count > 0;

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
				IMemberDefinition dp = prop.Object.Type.Type.GetAllFields().FirstOrDefault(f =>
					f.Name == dpName &&
					f.IsStatic &&
					f.FieldType.FullName == dependencyPropertyType);
				if (dp == null)
				{
					dp = prop.Object.Type.Type.GetAllProperties().FirstOrDefault(p =>
						p.Name == dpName &&
						p.IsStatic() &&
						p.PropertyType.FullName == dependencyPropertyType);
				}
				if (dp != null)
				{
					prop.Value.BindValue.IsDPChangeEvent = true;
				}
			}
		}
	}
}

public class BindingScope
{
	public string? ViewName;
	public TypeInfo? DataType;
	public List<Bind> Bindings = new List<Bind>();
	public BindingsData? BindingsData;
}
