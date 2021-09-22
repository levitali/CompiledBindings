using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

#nullable enable

namespace CompiledBindings
{
	public class TypeInfo
	{
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
						type = TypeInfoUtils.GetTypeThrow(typeof(Array));
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
						return new PropertyInfo(p, GetType(p.PropertyType, p.DeclaringType, p.CustomAttributes, knownNotNull));
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
						type = TypeInfoUtils.GetTypeThrow(typeof(Array));
					}
					_fields = type.GetAllFields().Select(f => new FieldInfo(f, GetType(f.FieldType, f.DeclaringType, f.CustomAttributes, false))).ToList();
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
							m.Parameters.Select(p => new ParameterInfo(p, GetType(p.ParameterType, m.DeclaringType, p.CustomAttributes, false))).ToList(),
							GetType(m.ReturnType, m.DeclaringType, m.MethodReturnType.CustomAttributes, false)))
						.ToList();
				}
				return _methods;
			}
		}

		public IList<EventDefinition>? Events => _events ??= TypeInfoUtils.GetAllEvents(Type).ToList();

		public static implicit operator TypeInfo(TypeReference type)
		{
			return new TypeInfo(type);
		}

		public static implicit operator TypeReference(TypeInfo typeInfo)
		{
			return typeInfo.Type;
		}

		public TypeInfo GetType(TypeReference type, TypeDefinition declaringType, IEnumerable<CustomAttribute> attributes, bool knownNotNullable)
		{
			if (type.IsGenericParameter/* && Type.IsGenericInstance*/)
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
						nullability = ((byte?)declaringType.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute")?.ConstructorArguments[0].Value) ?? 0;
					}
					isNullable = nullability != 1;
				}
			}
			return new TypeInfo(type, isNullable);
		}

		public static (string ns, string className) SplitFullName(string fullName)
		{
			var parts = fullName.Split('.');
			return (string.Join(".", parts.Take(parts.Length - 1)), parts.Last());
		}
	}

	public class PropertyInfo
	{
		public PropertyInfo(PropertyDefinition definition, TypeInfo propertyType)
		{
			Definition = definition;
			PropertyType = propertyType;
		}

		public PropertyDefinition Definition { get; }
		public TypeInfo PropertyType { get; }
	}

	public class FieldInfo
	{
		public FieldInfo(FieldDefinition definition, TypeInfo fieldType)
		{
			Definition = definition;
			FieldType = fieldType;
		}

		public FieldDefinition Definition { get; }
		public TypeInfo FieldType { get; }
	}

	public class MethodInfo
	{
		public MethodInfo(TypeInfo declearingType, MethodDefinition definition)
		{
			Definition = definition;
			Parameters = definition.Parameters.Select(p => new ParameterInfo(p, declearingType.GetType(p.ParameterType, definition.DeclaringType, p.CustomAttributes, false))).ToList();
			ReturnType = declearingType.GetType(definition.ReturnType, definition.DeclaringType, definition.MethodReturnType.CustomAttributes, false);
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
}
