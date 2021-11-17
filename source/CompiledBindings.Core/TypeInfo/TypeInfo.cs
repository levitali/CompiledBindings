#nullable enable

namespace CompiledBindings;

public class TypeInfo
{
	private static readonly Dictionary<string, TypeInfo> _typeCache = new Dictionary<string, TypeInfo>(StringComparer.OrdinalIgnoreCase);

	private TypeInfo? _baseType;
	private IList<PropertyInfo>? _properties;
	private IList<FieldInfo>? _fields;
	private IList<MethodInfo>? _methods;
	private IList<EventInfo>? _events;
	private readonly bool? _isNullable;
	private readonly byte? _nullableContext;
	private readonly byte[]? _nullabileFlags;

	public TypeInfo(TypeInfo typeInfo, bool isNullable)
	{
		Type = typeInfo.Type;
		_baseType = typeInfo._baseType;
		_properties = typeInfo._properties;
		_fields = typeInfo._fields;
		_methods = typeInfo._methods;
		_events = typeInfo._events;
		_nullabileFlags = typeInfo._nullabileFlags;
		_nullableContext = typeInfo._nullableContext;

		_isNullable = isNullable;
	}

	private TypeInfo(TypeReference type)
	{
		Type = type;
		_nullableContext = GetNullableContext(type);
		_nullabileFlags = GetNullableFlags(type);
		var b = _nullabileFlags?[0] ?? _nullableContext;
		_isNullable = b == null ? null : b == 2;
	}

	private TypeInfo(TypeReference type, bool? isNullable, byte[]? nullabileFlags, byte? nullableContext)
	{
		Type = type;
		_nullableContext = nullableContext;
		_nullabileFlags = nullabileFlags;

		if (isNullable != null)
		{
			_isNullable = isNullable;
		}
		else
		{
			var b = _nullabileFlags?[0] ?? _nullableContext;
			_isNullable = b == null ? null : b == 2;
		}
	}

	public static Dictionary<string, HashSet<string>> NotNullableProperties { get; } = new Dictionary<string, HashSet<string>>();

	public TypeReference Type { get; }

	public bool IsNullable => _isNullable != false && Type.IsNullable();

	public IList<PropertyInfo> Properties
	{
		get
		{
			if (_properties == null)
			{
				HashSet<string> notNullabelProperties = new();

				var type = Type;
				if (Type.IsArray)
				{
					type = GetTypeThrow(typeof(Array)).Type;
				}
				else
				{
					var type2 = type;
					do
					{
						if (NotNullableProperties.TryGetValue(type2.FullName, out var properties))
						{
							notNullabelProperties.UnionWith(properties);
						}
						type2 = type2.GetBaseType();
					}
					while (type2 != null);
				}

				_properties = type.GetAllProperties().Select(p =>
				{
					var knownNotNull = notNullabelProperties?.Contains(p.Name) == true;
					return new PropertyInfo(p, GetTypeSumElement(p.PropertyType, p.DeclaringType, knownNotNull ? false : null, p.CustomAttributes));
				}).ToList();
			}
			return _properties;
		}
	}

	public IList<FieldInfo> Fields
	{
		get
		{
			if (_fields == null)
			{
				var type = Type;
				if (Type.IsArray)
				{
					type = GetTypeThrow(typeof(Array)).Type;
				}
				_fields = type.GetAllFields().Select(f => new FieldInfo(f, GetTypeSumElement(f.FieldType, f.DeclaringType, null, f.CustomAttributes))).ToList();
			}
			return _fields;
		}
	}

	public IList<MethodInfo> Methods => _methods ??=
		Type.GetAllMethods()
			.Select(m => new MethodInfo(
				m,
				m.Parameters.Select(p => new ParameterInfo(p, GetTypeSumElement(p.ParameterType, m.DeclaringType, null, p.CustomAttributes, m.CustomAttributes))).ToList(),
				GetTypeSumElement(m.ReturnType, m.DeclaringType, null, m.MethodReturnType.CustomAttributes, m.CustomAttributes)))
			.ToList();

	public IList<EventInfo> Events => _events ??=
		TypeInfoUtils.GetAllEvents(Type)
		.Select(e => new EventInfo(e, GetTypeSumElement(e.EventType, e.DeclaringType, null, e.CustomAttributes)))
		.ToList();

	public TypeInfo? BaseType => _baseType ??= Type.ResolveEx()?.BaseType is var bt && bt != null ? new TypeInfo(bt) : null; //TODO nullablility in base type from this one

	public TypeInfo? GetElementType()
	{
		var elementType = Type.GetElementType();
		if (elementType == null)
		{
			return null;
		}
		return new TypeInfo(elementType, GetIsNullableSumElement(1), GetNullableFlags(elementType), GetNullableContext(elementType) ?? _nullableContext);
	}

	public IList<TypeInfo>? GetGenericArguments()
	{
		var genericArguments = Type.GetGenericArguments();
		if (genericArguments == null)
		{
			return null;
		}

		return genericArguments
			.Select((ga, i) => new TypeInfo(ga, GetIsNullableSumElement(i + 1), GetNullableFlags(ga), GetNullableContext(ga) ?? _nullableContext))
			.ToList();
	}

	public TypeInfo? GetItemType()
	{
		if (Type.IsArray)
		{
			return GetElementType();
		}
		else
		{
			var enumerableType = Type.GetAllInterfaces().FirstOrDefault(i => i.GetElementType().FullName == "System.Collections.Generic.IEnumerable`1");
			if (enumerableType != null)
			{
				var itemType = enumerableType.GetGenericArguments()[0];
				return new TypeInfo(itemType, GetIsNullableSumElement(1), GetNullableFlags(itemType), GetNullableContext(itemType) ?? _nullableContext);
			}
		}
		return null;
	}

	public static TypeInfo? GetType(string typeName, bool ignoreCase = false)
	{
		if (_typeCache.TryGetValue(typeName, out var type) && (!ignoreCase || type.Type.FullName == typeName))
		{
			return type;
		}
		if (TypeInfoUtils.TryGetType(typeName, out var typeDefinition) && (!ignoreCase || typeDefinition.FullName == typeName))
		{
			_typeCache[typeName] = type = new TypeInfo(typeDefinition);
		}
		return type;
	}

	public static TypeInfo GetTypeThrow(string typeName, bool ignoreCase = false)
	{
		var type = GetType(typeName, ignoreCase);
		if (type == null)
		{
			throw new Exception($"Type not found: {typeName}");
		}
		return type;
	}

	public static TypeInfo GetTypeThrow(Type type)
	{
		return GetTypeThrow(type.FullName);
	}

	public static void Cleanup()
	{
		_typeCache.Clear();
	}

	public static IEnumerable<MethodInfo> FindExtensionMethods(string ns, string name)
	{
		foreach (var refTyp in TypeInfoUtils.AllTypes.Values.Where(t => t.Namespace == ns))
		{
			if (refTyp.IsSealed && refTyp.IsAbstract)
			{
				foreach (var method in refTyp.Methods.Where(m => m.Name == name && m.CustomAttributes.Any(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.ExtensionAttribute")))
				{
					var typeInfo = GetTypeThrow(refTyp.FullName);
					yield return typeInfo.Methods.First(m => m.Definition == method);
				}
			}
		}
	}

	public TypeInfo MakeGenericInstanceType(params TypeInfo[] arguments)
	{
		var typeRef = Type.MakeGenericInstanceType(arguments.Select(a => a.Type).ToArray());
		var typeInfo = new TypeInfo(typeRef, null, _nullabileFlags, _nullableContext);
		return typeInfo;
	}

	public override string ToString()
	{
		return Type.ToString();
	}

	internal TypeInfo GetTypeSumElement(TypeReference type, TypeDefinition declaringType, bool? isNullable, params IEnumerable<CustomAttribute>[] attributesHierarchy)
	{
		if (type.IsGenericParameter)
		{
			var type2 = Type;
			do
			{
				var td = type2.ResolveEx() ?? type2.GetElementType().ResolveEx();
				if (td == null)
				{
					break;
				}

				var gp = td.GenericParameters;
				for (int i = 0; i < gp.Count; i++)
				{
					if (gp[i].Name == type.Name)
					{
						type = type2.GetGenericArguments()[i];
						if (isNullable == null)
						{
							isNullable = GetIsNullableSumElement(i + 1);
						}
						goto Lable_Break1;
					}
				}
				type2 = td.BaseType;
			}
			while (type2 != null);
		}
	Lable_Break1:

		byte[]? nullableFlags = null;
		var attributes = attributesHierarchy.First();
		var attr = attributes.FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");
		if (attr != null)
		{
			if (attr.ConstructorArguments[0].Value is CustomAttributeArgument[] arr)
			{
				nullableFlags = arr.Select(b => (byte)b.Value).ToArray();
			}
			else
			{
				nullableFlags = new[] { (byte)attr.ConstructorArguments[0].Value };
			}
		}

		byte? nullableContext = null;
		foreach (var attributes2 in attributesHierarchy.Concat(
			EnumerableExtensions.SelectSequence(declaringType, t => t.DeclaringType, true).Select(t => t.CustomAttributes)))
		{
			var attr2 = attributes2.FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute");
			if (attr2 != null)
			{
				nullableContext = (byte)attr2.ConstructorArguments[0].Value;
				break;
			}
		}

		nullableContext ??= _nullableContext;

		if (isNullable == null)
		{
			var b = (nullableFlags?[0] ?? nullableContext);
			isNullable = b == null ? null : b == 2;
		}

		return new TypeInfo(type, isNullable, nullableFlags, nullableContext);
	}

	private static byte[]? GetNullableFlags(TypeReference type)
	{
		var attrs = type.ResolveEx()?.CustomAttributes;
		var attr = attrs?.FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");
		if (attr != null)
		{
			if (attr.ConstructorArguments[0].Value is CustomAttributeArgument[] arr)
			{
				return arr.Select(b => (byte)b.Value).ToArray();
			}
			else
			{
				return new[] { (byte)attr.ConstructorArguments[0].Value };
			}
		}
		return null;
	}

	private static byte? GetNullableContext(TypeReference type)
	{
		var attrs = type.ResolveEx()?.CustomAttributes;
		var attr = attrs?.FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute");
		return (byte?)attr?.ConstructorArguments[0].Value;
	}

	private bool? GetIsNullableSumElement(int index)
	{
		var b = _nullabileFlags?.Skip(index).FirstOrDefault() ?? _nullableContext;
		return b == null ? null : b == 2;
	}
}

public class PropertyInfo : IMemberInfo
{
	public PropertyInfo(PropertyDefinition definition, TypeInfo propertyType)
	{
		Definition = definition;
		PropertyType = propertyType;
	}

	public PropertyDefinition Definition { get; }
	public TypeInfo PropertyType { get; }

	IMemberDefinition IMemberInfo.Definition => Definition;
	TypeInfo IMemberInfo.MemberType => PropertyType;
}

public class FieldInfo : IMemberInfo
{
	public FieldInfo(FieldDefinition definition, TypeInfo fieldType)
	{
		Definition = definition;
		FieldType = fieldType;
	}

	public FieldDefinition Definition { get; }
	public TypeInfo FieldType { get; }

	IMemberDefinition IMemberInfo.Definition => Definition;
	TypeInfo IMemberInfo.MemberType => FieldType;
}

public class MethodInfo : IMemberInfo
{
	public MethodInfo(TypeInfo declearingType, MethodDefinition definition)
	{
		Definition = definition;
		Parameters = definition.Parameters.Select(p => new ParameterInfo(p, declearingType.GetTypeSumElement(p.ParameterType, definition.DeclaringType, null, p.CustomAttributes, definition.CustomAttributes))).ToList();
		ReturnType = declearingType.GetTypeSumElement(definition.ReturnType, definition.DeclaringType, null, definition.MethodReturnType.CustomAttributes, definition.CustomAttributes);
	}

	public MethodInfo(MethodDefinition definition, IList<ParameterInfo> parameters, TypeInfo returnType)
	{
		Definition = definition;
		Parameters = parameters;
		ReturnType = returnType;
	}

	public MethodDefinition Definition { get; }
	public IList<ParameterInfo> Parameters { get; }
	public TypeInfo ReturnType { get; }

	IMemberDefinition IMemberInfo.Definition => Definition;
	TypeInfo IMemberInfo.MemberType => ReturnType;
}

public class EventInfo : IMemberInfo
{
	public EventInfo(EventDefinition definition, TypeInfo eventType)
	{
		Definition = definition;
		EventType = eventType;
	}

	public EventDefinition Definition { get; }

	public TypeInfo EventType { get; }

	IMemberDefinition IMemberInfo.Definition => Definition;
	TypeInfo IMemberInfo.MemberType => EventType;
}

public class ParameterInfo
{
	public ParameterInfo(ParameterDefinition definition, TypeInfo parameterType)
	{
		Definition = definition;
		ParameterType = parameterType;
	}

	public ParameterDefinition Definition { get; }
	public TypeInfo ParameterType { get; }
}

public interface IMemberInfo
{
	IMemberDefinition Definition { get; }
	TypeInfo MemberType { get; }
}

