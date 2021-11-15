using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

#nullable enable

namespace CompiledBindings;

public class TypeInfo
{
	private static readonly Dictionary<string, TypeInfo> _typeCache = new Dictionary<string, TypeInfo>(StringComparer.OrdinalIgnoreCase);

	private TypeInfo? _baseType;
	private IList<PropertyInfo>? _properties;
	private IList<FieldInfo>? _fields;
	private IList<MethodInfo>? _methods;
	private IList<EventDefinition>? _events;
	private readonly bool _isNullable;

	public TypeInfo(TypeReference type, bool isNullable = true)
	{
		if (type == null)
		{
			throw new ArgumentNullException(nameof(type));
		}

		Type = type;
		_isNullable = isNullable;
	}

	public static Dictionary<string, HashSet<string>> NotNullableProperties { get; } = new Dictionary<string, HashSet<string>>();

	public TypeReference Type { get; }

	public bool IsNullable => _isNullable && Type.IsNullable();

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
					type = GetTypeThrow(typeof(Array));
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
					return new PropertyInfo(p, GetType(p.PropertyType, p.DeclaringType, knownNotNull, p.CustomAttributes));
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
					type = GetTypeThrow(typeof(Array));
				}
				_fields = type.GetAllFields().Select(f => new FieldInfo(f, GetType(f.FieldType, f.DeclaringType, false, f.CustomAttributes))).ToList();
			}
			return _fields;
		}
	}

	public IList<MethodInfo> Methods
	{
		get
		{
			if (_methods == null)
			{
				_methods = Type.GetAllMethods()
					.Select(m => new MethodInfo(m,
						m.Parameters.Select(p => new ParameterInfo(p, GetType(p.ParameterType, m.DeclaringType, false, p.CustomAttributes, m.CustomAttributes))).ToList(),
						GetType(m.ReturnType, m.DeclaringType, false, m.MethodReturnType.CustomAttributes, m.CustomAttributes)))
					.ToList();
			}
			return _methods;
		}
	}

	public IList<EventDefinition>? Events => _events ??= TypeInfoUtils.GetAllEvents(Type).ToList();

	public TypeInfo? BaseType => _baseType ??= Type.ResolveEx()?.BaseType is var bt && bt != null ? new TypeInfo(bt) : null;


	public static implicit operator TypeInfo(TypeReference type)
	{
		return new TypeInfo(type);
	}

	public static implicit operator TypeReference(TypeInfo typeInfo)
	{
		return typeInfo.Type;
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

	internal TypeInfo GetType(TypeReference type, TypeDefinition declaringType, bool knownNotNullable, params IEnumerable<CustomAttribute>[] attributesHierarchy)
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
						goto Lable_Break1;
					}
				}
				type2 = td.BaseType;
			}
			while (type2 != null);
		}
	Lable_Break1:

		bool isNullable;
		if (knownNotNullable)
		{
			isNullable = false;
		}
		else
		{
			isNullable = true;
			if (!type.IsValueNullable())
			{
				var attributes = attributesHierarchy.First();
				byte? nullability = null;
				var attr = attributes.FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");
				if (attr != null)
				{
					byte[] nullableFlags;
					if (attr.ConstructorArguments[0].Value is CustomAttributeArgument[] arr)
					{
						nullableFlags = arr.Select(b => (byte)b.Value).ToArray();
					}
					else
					{
						nullableFlags = new[] { (byte)attr.ConstructorArguments[0].Value };
					}

					if (nullableFlags.Length > 0)
					{
						nullability = nullableFlags[0];
					}
				}
				if (nullability == null)
				{
					CustomAttribute? attr2 = null;
					foreach (var attributes2 in attributesHierarchy.Concat(
						EnumerableExtensions.SelectSequence(declaringType, t => t.DeclaringType, true).Select(t => t.CustomAttributes)))
					{
						attr2 = attributes2.FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute");
						if (attr2 != null)
						{
							break;
						}
					}

					nullability = ((byte?)attr2?.ConstructorArguments[0].Value) ?? 0;
				}
				isNullable = nullability != 1;
			}
		}
		
		var typeInfo = new TypeInfo(type, isNullable);



		return typeInfo;
	}

	public static (string ns, string className) SplitFullName(string fullName)
	{
		var parts = fullName.Split('.');
		return (string.Join(".", parts.Take(parts.Length - 1)), parts.Last());
	}

	public override string ToString()
	{
		return Type.ToString();
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
		Parameters = definition.Parameters.Select(p => new ParameterInfo(p, declearingType.GetType(p.ParameterType, definition.DeclaringType, false, p.CustomAttributes, definition.CustomAttributes))).ToList();
		ReturnType = declearingType.GetType(definition.ReturnType, definition.DeclaringType, false, definition.MethodReturnType.CustomAttributes, definition.CustomAttributes);
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

