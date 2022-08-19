#nullable enable

namespace CompiledBindings;

public class TypeInfo
{
	private static readonly Dictionary<string, TypeInfo> _typeCache = new(StringComparer.OrdinalIgnoreCase);

	private IList<PropertyInfo>? _properties;
	private IList<FieldInfo>? _fields;
	private IList<MethodInfo>? _methods;
	private IList<MethodInfo>? _constructors;
	private IList<EventInfo>? _events;
	private readonly bool? _canBeNullable;
	private readonly byte? _nullableContext;
	private readonly byte[]? _nullabileFlags;

	public TypeInfo(TypeInfo typeInfo, bool canBeNullable)
	{
		Type = typeInfo.Type;
		_properties = typeInfo._properties;
		_fields = typeInfo._fields;
		_methods = typeInfo._methods;
		_events = typeInfo._events;
		_nullabileFlags = typeInfo._nullabileFlags;
		_nullableContext = typeInfo._nullableContext;

		_canBeNullable = canBeNullable;
	}

	private TypeInfo(TypeReference type)
	{
		Type = type;
		_nullableContext = GetNullableContext(type);
		_nullabileFlags = GetNullableFlags(type);
		var b = _nullabileFlags?[0] ?? _nullableContext;
		_canBeNullable = b is null or 0 ? null : b == 2;
	}

	private TypeInfo(TypeReference type, bool? isNullable, byte[]? nullabileFlags, byte? nullableContext)
	{
		Type = type;
		_nullableContext = nullableContext;
		_nullabileFlags = nullabileFlags;

		if (isNullable != null)
		{
			_canBeNullable = isNullable;
		}
		else
		{
			var b = _nullabileFlags?[0] ?? _nullableContext;
			_canBeNullable = b is null or 0 ? null : b == 2;
		}
	}

	public static Dictionary<string, HashSet<string>> NotNullableProperties { get; } = new Dictionary<string, HashSet<string>>();

	public TypeReference Type { get; }

	public bool IsNullable => Type.IsValueNullable() || (_canBeNullable != false && !Type.IsValueType);

	public IList<PropertyInfo> Properties => _properties ??=
		EnumerateTypeAndSubTypes()
			.SelectMany(t => t.GetProperties())
			.Select(p =>
			{
				var knownNotNull = NotNullableProperties.TryGetValue(p.DeclaringType.FullName, out var props) && props.Contains(p.Name);
				return new PropertyInfo(p, GetTypeSumElement(p.PropertyType, p.DeclaringType, knownNotNull ? false : null, p.CustomAttributes));
			})
			.ToList();

	public IList<FieldInfo> Fields => _fields ??=
		EnumerateTypeAndSubTypes()
			.SelectMany(t => t.GetFields())
			.Select(f => new FieldInfo(f, GetTypeSumElement(f.FieldType, f.DeclaringType, null, f.CustomAttributes)))
			.ToList();

	public IList<MethodInfo> Methods => _methods ??=
		EnumerateTypeAndSubTypes()
			.SelectMany(t => t.GetMethods())
			.Where(m => !m.IsConstructor)
			.Select(m => new MethodInfo(
				m,
				m.Parameters.Select(p => new ParameterInfo(p, GetTypeSumElement(p.ParameterType, m.DeclaringType, null, p.CustomAttributes, m.CustomAttributes))).ToList(),
				GetTypeSumElement(m.ReturnType, m.DeclaringType, null, m.MethodReturnType.CustomAttributes, m.CustomAttributes)))
			.ToList();

	public IList<MethodInfo> Constructors => _constructors ??=
		Type.ResolveEx()!.GetConstructors()
			.Select(m => new MethodInfo(
				m,
				m.Parameters.Select(p => new ParameterInfo(p, GetTypeSumElement(p.ParameterType, m.DeclaringType, null, p.CustomAttributes, m.CustomAttributes))).ToList(),
				GetTypeSumElement(m.ReturnType, m.DeclaringType, null, m.MethodReturnType.CustomAttributes, m.CustomAttributes)))
			.ToList();

	public IList<EventInfo> Events => _events ??=
		EnumerateTypeAndSubTypes()
			.SelectMany(t => t.GetEvents())
			.Select(e => new EventInfo(e, GetTypeSumElement(e.EventType, e.DeclaringType, null, e.CustomAttributes)))
			.ToList();

	public TypeInfo? GetElementType()
	{
		var elementType = Type.GetElementType();
		if (elementType == null)
		{
			return null;
		}
		return new TypeInfo(elementType, GetIsNullableSubElement(1), GetNullableFlags(elementType), GetNullableContext(elementType) ?? _nullableContext);
	}

	public IList<TypeInfo>? GetGenericArguments()
	{
		var genericArguments = Type.GetGenericArguments();
		if (genericArguments == null)
		{
			return null;
		}

		return genericArguments
			.Select((ga, i) => new TypeInfo(ga, GetIsNullableSubElement(i + 1), GetNullableFlags(ga), GetNullableContext(ga) ?? _nullableContext))
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
				return new TypeInfo(itemType, GetIsNullableSubElement(1), GetNullableFlags(itemType), GetNullableContext(itemType) ?? _nullableContext);
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

	public static IEnumerable<MethodInfo> FindExtensionMethods(string ns, string name, TypeInfo type)
	{
		foreach (var refTyp in TypeInfoUtils.AllTypes.Values.Where(t => t.Namespace == ns))
		{
			if (refTyp.IsSealed && refTyp.IsAbstract)
			{
				foreach (var method in refTyp.Methods.Where(m =>
					m.Name == name &&
					m.CustomAttributes.Any(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.ExtensionAttribute") &&
					m.Parameters.Count > 0 &&
					m.Parameters[0].ParameterType.IsAssignableFrom(type.Type)))
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
						isNullable ??= GetIsNullableSubElement(i + 1);
						goto Label_Break1;
					}
				}
				type2 = td.BaseType;
			}
			while (type2 != null);
		}
Label_Break1:

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

		if (isNullable == null)
		{
			var b = nullableFlags?[0] ?? nullableContext;
			isNullable = b is null or 0 ? null : b == 2;
		}

		return new TypeInfo(type, isNullable, nullableFlags, nullableContext);
	}

	private IEnumerable<TypeReference> EnumerateTypeAndSubTypes()
	{
		var type = Type;
		if (type.IsArray)
		{
			return EnumerableExtensions.AsEnumerable(GetTypeThrow(typeof(Array)).Type);
		}

		if (type.IsGenericInstance)
		{
			type = type.GetElementType();
		}

		if (type.IsInterface())
		{
			return type.GetAllInterfaces();
		}

		return EnumerableExtensions.SelectSequence(type, t => t.GetBaseType(), true).Where(t => t != null).Select(t => t!);
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
		return (byte?)type.ResolveEx()?
			.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute")?
			.ConstructorArguments[0].Value;
	}

	private bool? GetIsNullableSubElement(int index)
	{
		var b = _nullabileFlags?.Skip(index).FirstOrDefault() ?? _nullableContext;
		return b is null or 0 ? null : b == 2;
	}
}

public class PropertyInfo : IMemberInfo
{
	private bool? _isReadOnly;

	public PropertyInfo(PropertyDefinition definition, TypeInfo propertyType)
	{
		Definition = definition;
		PropertyType = propertyType;
	}

	public PropertyDefinition Definition { get; }
	public TypeInfo PropertyType { get; }

	public bool IsReadOnly => _isReadOnly ??=
		Definition.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == "System.ComponentModel.ReadOnlyAttribute") is var attr && attr != null
		? (bool)attr.ConstructorArguments[0].Value
		: Definition.GetMethod.Body?.Instructions is var instructions &&
			instructions != null &&
			instructions.Count == 3 &&
			instructions[0].OpCode == OpCodes.Ldarg_0 &&
			instructions[1].OpCode == OpCodes.Ldfld &&
			instructions[2].OpCode == OpCodes.Ret &&
			instructions[1].Operand is FieldDefinition fld &&
			fld.IsInitOnly;

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

	public IEnumerable<TypeInfo> GetEventHandlerParameterTypes()
	{
		var invokeMethod = EventType.Methods.FirstOrDefault(m => m.Definition.Name == "Invoke");
		if (invokeMethod == null)
		{
			return Enumerable.Empty<TypeInfo>();
		}
		return invokeMethod.Parameters.Select(p => p.ParameterType);
	}
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

