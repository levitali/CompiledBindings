using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Mono.Cecil;

#nullable enable
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

namespace CompiledBindings;

public class XamlDomParser
{
	public XName xClass { get; }
	public XName xName { get; }
	public XName xDataType { get; }
	private XName xBind;
	private XName xDefaultBindMode;
	private XName xSet;
	private XName xNull;
	private XName xType;
	private XName xKey;
	private XName xFieldModifier;

	private static readonly XName NameAttr = XNamespace.None + "Name";

	private int _localVarIndex;

	public XamlDomParser(XNamespace defaultNamespace, XNamespace xNamespace) :
		this(defaultNamespace, xNamespace, defaultNamespace + "VisualStateManager.VisualStateGroups", defaultNamespace + "VisualStateGroup", defaultNamespace + "VisualState")
	{
	}

	public XamlDomParser(XNamespace defaultNamespace, XNamespace xNamespace, XName visualStateGroups, XName visualStateGroup, XName visualState)
	{
		DefaultNamespace = defaultNamespace;
		ResourceDictionary = defaultNamespace + "ResourceDictionary";
		Style = defaultNamespace + "Style";
		StaticResource = defaultNamespace + "StaticResource";
		VisualStateGroups = visualStateGroups;
		VisualStateGroup = visualStateGroup;
		VisualState = visualState;

		this.xNamespace = xNamespace;
		xClass = xNamespace + "Class";
		xName = xNamespace + "Name";
		xBind = xNamespace + "Bind";
		xDefaultBindMode = xNamespace + "DefaultBindMode";
		xSet = xNamespace + "Set";
		xNull = xNamespace + "Null";
		xType = xNamespace + "Type";
		xKey = xNamespace + "Key";
		xFieldModifier = xNamespace + "FieldModifier";
		xDataType = xNamespace + "DataType";
	}

	public XNamespace DefaultNamespace { get; }
	public XNamespace xNamespace { get; }
	public XName VisualStateGroups { get; }
	public XName VisualStateGroup { get; }
	public XName VisualState { get; }

	private XName ResourceDictionary;
	private XName Style;
	private XName StaticResource;

	private static readonly XName StyleName = "Style";

	public string BasePath;
	public string File;
	public TypeInfo TargetType;
	public TypeInfo DataType;
	public TypeInfo ConverterType;
	public IList<XamlNamespace>? KnownNamespaces;
	public HashSet<string> KnownContentTypes;
	public Dictionary<string, string> KnownContentProperties;
	public Dictionary<XName, string> KnownTypeMappings;
	public Dictionary<string, (XName className, string propertyName)> KnownAttachedProperties;
	public Dictionary<string, ICSharpTypeConverter> TypeConverters;

	private Func<string, IEnumerable<string>> _getClrNsFromXmlNs;

	public Func<string, IEnumerable<string>> GetClrNsFromXmlNs
	{
		get => _getClrNsFromXmlNs;
		set
		{
			value ??= _ => Enumerable.Empty<string>();
			_getClrNsFromXmlNs = value;
		}
	}

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
			throw new GeneratorException(ex.Message, File, xClassAttr, xClassAttr.Value.Length);
		}
	}

	public TypeInfo FindType(XElement xelement)
	{
		return FindType(xelement.Name, xelement);
	}

	public TypeInfo FindType(string value, XObject xobject)
	{
		var typeName = XamlParser.GetTypeName(value, xobject, KnownNamespaces);
		return FindType(typeName, xobject);
	}

	private TypeInfo FindType(XName className, XObject xobject)
	{
		if (KnownTypeMappings != null && KnownTypeMappings.TryGetValue(className, out var typeName))
		{
			return TypeInfo.GetTypeThrow(typeName);
		}
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
		throw new GeneratorException($"The type {className} was not found.", File, xobject);
	}

	public string? FindFullTypeName(XElement xelement)
	{
		return FindFullTypeName(xelement.Name, xelement);
	}

	public string? FindFullTypeName(XName className, XObject xobject)
	{
		if (KnownTypeMappings != null && KnownTypeMappings.TryGetValue(className, out var typeName))
		{
			return typeName;
		}
		var clrNs = XamlNamespace.GetClrNamespace(className.NamespaceName);
		if (clrNs != null)
		{
			return clrNs + "." + className.LocalName;
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
				return type.Type.GetCSharpFullName();
			}
		}
		return null;
	}

	public XName? GetTypeNameFromAttribute(string value, XAttribute attr)
	{
		if (value.StartsWith("{"))
		{
			var xamlNode = XamlParser.ParseMarkupExtension(value, attr, File, KnownNamespaces);
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
				throw new GeneratorException($"Unexpected markup extension {xamlNode.Name}", File, attr);
			}
		}
		if (value == null)
		{
			throw new GeneratorException($"Missing type.", File, attr);
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
		return GetObjectProperty(obj, xamlNode.Name.LocalName, xamlNode, null);
	}

	private XamlObjectProperty GetObjectProperty(XamlObject obj, XamlNode xamlNode, List<ResourceDictionary>? resourceDictionaries)
	{
		return GetObjectProperty(obj, xamlNode.Name.LocalName, xamlNode, resourceDictionaries);
	}

	private XamlObjectProperty GetObjectProperty(XamlObject obj, string memberName, XamlNode xamlNode, List<ResourceDictionary>? resourceDictionaries)
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

			if (KnownAttachedProperties != null && KnownAttachedProperties.TryGetValue(memberName, out var propertyName2) == true)
			{
				(attachedClassName, attachedPropertyName) = propertyName2;
			}
			else
			{
				int index = memberName.IndexOf('.');
				if (index != -1)
				{
					string typeName = memberName.Substring(0, index);
					attachedClassName = XamlParser.GetTypeName(typeName, xamlNode.Element, KnownNamespaces);
					attachedPropertyName = memberName.Substring(index + 1);
				}
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
				//if (!setPropertyMethod.IsStatic)
				//{
				//	throw new GeneratorException($"Invalid attached property {type.FullName}.{attachedPropertyName}. The method {setMethodName} is not static.", xamlNode);
				//}

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
					var @event = obj.Type.Events.FirstOrDefault(e => e.Name == memberName);
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
			if (xamlNode.Children.Count == 1 && xamlNode.Children[0].Name == StaticResource && resourceDictionaries != null)
			{
				var key = xamlNode.Children[0].Value; //TODO!!
				var resource = EnumerableExtensions.SelectTree(resourceDictionaries, rd => rd.MergedDictionaries).SelectMany(rd => rd.Values).FirstOrDefault(s => s.Key == key);
				if (resource == null)
				{
					throw new GeneratorException($"Resource {key} not found.", xamlNode);
				}

				if (resource.Object.Type.Type.IsValueType)
				{
					value.CSharpValue = resource.Object.CreateCSharpValue;
				}
				else
				{
					value.ObjectValue = resource.Object;
					resource.IsUsed = true;
				}
			}
			else if (xamlNode.Children.Count == 1 && xamlNode.Children[0].Name == xSet)
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
					throw new ParseException(ex.Message, ex.Position + "x:Set".Length, ex.Length);
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
					value.BindValue = BindingParser.Parse(objProp, DataType, TargetType, "dataRoot", defaultBindMode, this, ConverterType, ref _localVarIndex);
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
					throw new ParseException(ex.Message, ex.Position + "x:Bind".Length, ex.Length);
				}
			}
			else if (objProp.TargetEvent != null)
			{
				value.CSharpValue = xamlNode.Value;
			}
			else if (xamlNode.Children.Count > 1 ||
				(propType.Type.FullName != "System.String" && TypeInfo.GetTypeThrow(typeof(IEnumerable)).IsAssignableFrom(propType)))
			{
				value.CollectionValue = xamlNode.Children.Select(n => ParseXamlNode(obj, n, resourceDictionaries!, false)).ToList();
			}
			else if (xamlNode.Children.Count == 0 && !string.IsNullOrEmpty(xamlNode.Value))
			{
				value.CSharpValue = GetCSharpValue(xamlNode.Value!, xamlNode, propType);
			}
			else if (xamlNode.Children.Count == 0 && propType.Type.FullName == "System.String")
			{
				value.CSharpValue = "\"\"";
			}
			else
			{
				throw new GeneratorException($"The value cannot be processed.", xamlNode);
			}

			if (objProp.TargetEvent != null)
			{
				var expr = value.BindValue?.Expression ?? value.StaticValue;
				if (expr != null && (expr is not MemberExpression me || me.Member is not MethodInfo))
				{
					throw new GeneratorException($"Expression type must be a method.", xamlNode);
				}
			}

			objProp.Value = value;

			return objProp;
		}
		catch (ParseException ex)
		{
			throw new GeneratorException(ex.Message, xamlNode, ex.Position);
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
				defaultExpr = new ConstantExpression("");
			else
				defaultExpr = Expression.DefaultExpression;
			return new CoalesceExpression(expression, defaultExpr);
		}
		return expression;
	}

	private void CorrectMethod(XamlObjectProperty prop, TypeInfo type)
	{
		if (prop.TargetMethod != null)
		{
			// Try to find best suitable method.
			// Note! So far TypeReference.IsAssignableFrom method does not handle all cases.
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

	public XamlObject ParseXamlNode(XamlObject? parent, XamlNode xamlNode, List<ResourceDictionary> resourceDictionaries, bool rootNode, bool parseChildren = true)
	{
		try
		{
			var type = rootNode ? TargetType : FindType((XElement)xamlNode.Element);

			var xamlObj = new XamlObject(xamlNode, type);
			xamlObj.Parent = parent;

			// Parse name
			var nameProp = xamlNode.Properties.FirstOrDefault(p => p.Name == xName || p.Name == NameAttr);
			if (nameProp != null)
			{
				xamlObj.Name = nameProp.Value;
				xamlObj.NameExplicitlySet = true;
				xamlObj.GenerateMember = true;
			}

			// Parse ResourceDictionary
			var resourcesProp = xamlNode.Properties.FirstOrDefault(p => p.Name.LocalName == "Resources");
			if (resourcesProp != null)
			{
				var resourceDictionary = ParseResourceDictionary(resourcesProp, resourceDictionaries);
				resourceDictionaries.Insert(0, resourceDictionary);
				xamlNode.Properties.Remove(resourcesProp);
			}


			if (/*type.IsValueType || */KnownContentTypes?.Contains(type.Type.FullName) == true)
			{
				if (xamlNode.Value == null)
				{
					throw new GeneratorException("Value cannot be null.", xamlNode);
				}
				xamlObj.CreateCSharpValue = GetCSharpValue(xamlNode.Value, xamlNode, type);
			}
			else
			{
				Style? style = null;
				var styleProp = xamlNode.Properties.FirstOrDefault(p => p.Name == StyleName);
				if (styleProp != null)
				{
					var key = styleProp.Children[0].Value; //TODO!!
					style = EnumerableExtensions.SelectTree(resourceDictionaries, rd => rd.MergedDictionaries).SelectMany(rd => rd.Styles).FirstOrDefault(s => s.Key == key);
					if (style == null)
					{
						throw new GeneratorException($"Style {key} not found.", styleProp);
					}
					xamlNode.Properties.Remove(styleProp);
				}
				else
				{
					style =
						EnumerableExtensions.SelectTree(resourceDictionaries, rd => rd.MergedDictionaries)
						.SelectMany(rd => rd.Styles)
						.FirstOrDefault(s => s.Key == null && s.TargetType.IsAssignableFrom(type));
				}

				var properties = new List<XamlNode>();
				properties.AddRange(xamlNode.Properties);

				if (style != null)
				{
					foreach (var setter in EnumerableExtensions.SelectSequence(style, s => s.BasedOn, true).SelectMany(s => s.Setters))
					{
						if (!properties.Any(p => p.Name == setter.Name))
						{
							properties.Add(setter);
						}
					}
				}

				foreach (var prop in properties)
				{
					if (prop.Name == xFieldModifier)
					{
						xamlObj.FieldModifier = prop.Value;
					}
					else if (prop.Name.Namespace == xNamespace)
					{
						// Ignore here
					}
					else
					{
						xamlObj.Properties.Add(GetObjectProperty(xamlObj, prop, resourceDictionaries));
					}
				}

				if (parseChildren && xamlNode.Children.Count > 0)
				{
					string? contentProperty = null;
					bool found = false;
					if (KnownContentProperties != null)
					{
						var type2 = type;
						do
						{
							found = KnownContentProperties.TryGetValue(type2.Type.FullName, out contentProperty);
							if (found)
							{
								break;
							}

							type2 = type2.BaseType;
						}
						while (type2 != null);
					}
					if (!found)
					{
						throw new GeneratorException("Child element not expected.", xamlNode);
					}

					xamlObj.Properties.Add(GetObjectProperty(xamlObj, contentProperty!, xamlNode, resourceDictionaries));
				}
			}
			return xamlObj;
		}
		catch (ParseException ex)
		{
			throw new GeneratorException(ex.Message, xamlNode, ex.Position);
		}
		catch (Exception ex) when (ex is not GeneratorException)
		{
			throw new GeneratorException(ex.Message, xamlNode);
		}
	}

	private ResourceDictionary ParseResourceDictionary(XamlNode xamlNode, List<ResourceDictionary> resourceDictionaries)
	{
		ResourceDictionary resourceDictionary;
		var source = xamlNode.Properties.FirstOrDefault(p => p.Name.LocalName == "Source");
		if (source != null)
		{
			if (string.IsNullOrWhiteSpace(source.Value))
			{
				throw new GeneratorException($"Empty source.", source);
			}
			var fileName = Path.Combine(BasePath, source.Value!.Replace('/', '\\'));
			if (!System.IO.File.Exists(fileName))
			{
				throw new GeneratorException($"ResourceDictionary not found: {fileName}.", source);
			}

			resourceDictionary = EnumerableExtensions.SelectTree(resourceDictionaries, rd => rd.MergedDictionaries).FirstOrDefault(rd => rd.Source == fileName);
			if (resourceDictionary != null)
			{
				return resourceDictionary;
			}

			resourceDictionary = new ResourceDictionary
			{
				Source = fileName
			};

			var xdoc = XDocument.Load(fileName, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
			if (xdoc.Root.Name != ResourceDictionary)
			{
				throw new GeneratorException("ResourceDictionary expected.", fileName, xdoc.Root);
			}

			var dictionaryXamlNode = XamlParser.ParseElement(fileName, xdoc.Root, KnownNamespaces);

			xamlNode = dictionaryXamlNode;
		}
		else
		{
			resourceDictionary = new ResourceDictionary();
		}

		var mergedDictionaries = xamlNode.Properties.FirstOrDefault(p => p.Name.LocalName == "MergedDictionaries");
		if (mergedDictionaries != null)
		{
			foreach (var child in mergedDictionaries.Children)
			{
				if (child.Name != ResourceDictionary)
				{
					throw new GeneratorException("ResourceDictionary expected.", File, child.Element);
				}
				resourceDictionary.MergedDictionaries.Add(ParseResourceDictionary(child, resourceDictionaries));
			}
		}

		foreach (var resource in xamlNode.Children)
		{
			if (resource.Name == ResourceDictionary)
			{
				var resourceDictionary2 = ParseResourceDictionary(resource, resourceDictionaries);
				resourceDictionary.MergedDictionaries.Add(resourceDictionary2);
			}
			else
			{
				var key = resource.Properties.FirstOrDefault(p => p.Name == xKey);

				if (resource.Name == Style)
				{
					var targetTypeAttr = resource.Properties.FirstOrDefault(p => p.Name.LocalName == "TargetType");
					if (targetTypeAttr?.Value == null)
					{
						throw new GeneratorException("Missing TargetType.", resource);
					}
					var typeName = XamlParser.GetTypeName(targetTypeAttr.Value, targetTypeAttr.Element, KnownNamespaces);
					var targetType = FindType(targetTypeAttr.Value, resource.Element);

					var style = new Style(key?.Value, targetType);

					var basedOnAttr = resource.Properties.FirstOrDefault(p => p.Name.LocalName == "BasedOn");
					if (basedOnAttr != null)
					{
						var key2 = basedOnAttr.Children[0].Value; //TODO!!
						var baseStyle = EnumerableExtensions.SelectTree(resourceDictionary, rd => rd.MergedDictionaries, true).SelectMany(rd => rd.Styles).FirstOrDefault(s => s.Key == key2);
						if (baseStyle == null)
						{
							throw new GeneratorException($"Base style {key2} not found.", basedOnAttr);
						}

						if (baseStyle.TargetType != null && !baseStyle.TargetType.IsAssignableFrom(style.TargetType))
						{
							throw new GeneratorException($"The base style's target type {baseStyle.TargetType.Type.GetCSharpFullName()} is not assinable from {style.TargetType.Type.GetCSharpFullName()}", resource);
						}

						style.BasedOn = baseStyle;
					}

					resourceDictionary.Styles.Add(style);

					foreach (var setter in resource.Children)
					{
						var property = setter.Properties.FirstOrDefault(p => p.Name.LocalName == "Property");
						if (property == null)
						{
							throw new GeneratorException("No 'Property' node.", setter);
						}
						if (string.IsNullOrEmpty(property.Value))
						{
							throw new GeneratorException("Property is not set.", setter);
						}

						var value = setter.Properties.FirstOrDefault(p => p.Name.LocalName == "Value");
						if (value == null)
						{
							throw new GeneratorException("No 'Value' node.", setter);
						}
						var setter2 = new XamlNode(setter.File, value.Element, /*typeName.Namespace + */property.Value);

						if (value.Value != null)
						{
							setter2.Value = value.Value;
						}
						else
						{
							setter2.Children = value.Children;
						}

						style.Setters.Add(setter2);
					}
				}
				else
				{
					var resourceObj = ParseXamlNode(null, resource, resourceDictionaries, false);
					var staticResource = new StaticResource(key?.Value, resourceObj);
					if (resourceObj.Name != null)
					{
						staticResource.IsUsed = true;
					}
					resourceDictionary.Values.Add(staticResource);
				}
			}
		}

		return resourceDictionary;
	}

	public string GetCSharpValue(string value, XamlNode xamlNode, TypeInfo type)
	{
		if (type.Type.FullName == "System.Type")
		{
			var xmlTypeName = XamlParser.GetTypeName(value, xamlNode.Element, KnownNamespaces);
			var clrFullTypeName = FindFullTypeName(xmlTypeName, xamlNode.Element);
			value = $"typeof({clrFullTypeName})";
		}
		else
		{
			if (TypeConverters != null && TypeConverters.TryGetValue(type.Type.FullName, out var converter))
			{
				value = converter.Convert(value);
			}
			else if (type.Type.FullName == "System.String")
			{
				value = '\"' + value + '\"';
			}
			else if (type.Type.FullName == "System.Char")
			{
				value = '\'' + value + '\'';
			}
			else if (type.Type.ResolveEx()?.IsEnum == true)
			{
				value = type.Type.FullName + "." + value;
			}
		}
		return value;
	}
}

public interface ICSharpTypeConverter
{
	string Convert(string value);
}
