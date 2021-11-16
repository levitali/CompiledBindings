#nullable enable

namespace CompiledBindings;

public static class TypeInfoUtils
{
	private static readonly HashSet<string> _keywords = new HashSet<string>
	{
		"abstract", "event", "new", "struct","as", "explicit", "null", "switch",
		"base", "extern", "object", "this","bool", "false", "operator", "throw",
		"break", "finally", "out", "true","byte", "fixed", "override", "try",
		"case", "float", "params", "typeof","catch", "for", "private", "uint",
		"char", "foreach", "protected", "ulong","checked", "goto", "public", "unchecked",
		"class", "if", "readonly", "unsafe","const", "implicit", "ref", "ushort",
		"continue", "in", "return", "using","decimal", "int", "sbyte", "virtual",
		"default", "interface", "sealed", "volatile","delegate", "internal", "short", "void",
		"do", "is", "sizeof", "while","double", "lock", "stackalloc",
		"else", "long", "static",  "enum", "namespace", "string"
	};
	private static List<AssemblyDefinition>? _assemblies;
	private static Dictionary<string, TypeDefinition>? _allTypes;
	private static readonly Dictionary<TypeReference, TypeDefinition> _resolveCache = new Dictionary<TypeReference, TypeDefinition>();
	private static readonly Dictionary<TypeReference, TypeReference> _baseTypeCache = new Dictionary<TypeReference, TypeReference>();

	public static IList<AssemblyDefinition>? Assemblies => _assemblies;

	public static Dictionary<string, TypeDefinition> AllTypes => _allTypes ??= TypeInfoUtils.EnumerateAllTypes().Distinct(t => t.FullName.ToUpper()).ToDictionary(t => t.FullName, StringComparer.OrdinalIgnoreCase);

	public static AssemblyDefinition LoadLocalAssembly(string file)
	{
		var prm = new ReaderParameters(ReadingMode.Deferred)
		{
			AssemblyResolver = new AssemblyResolver(),
			ReadSymbols = true
		};
		var assembly = AssemblyDefinition.ReadAssembly(file, prm);
		if (_assemblies == null)
		{
			_assemblies = new List<AssemblyDefinition>();
		}

		_assemblies.Add(assembly);
		_allTypes = null;

		return assembly;
	}

	public static void LoadReferences(IEnumerable<string> assemblies)
	{
		var prm = new ReaderParameters(ReadingMode.Deferred)
		{
			AssemblyResolver = new AssemblyResolver(),
		};
		_assemblies = assemblies.Select(f => AssemblyDefinition.ReadAssembly(f, prm)).ToList();
		_allTypes = null;
	}

	public static void Cleanup()
	{
		if (_assemblies != null)
		{
			_assemblies.ForEach(a => a.Dispose());
			_assemblies = null;
		}
		_resolveCache.Clear();
		_baseTypeCache.Clear();
		_allTypes = null;
		TypeInfo.Cleanup();
	}

	public static bool TryGetType(string fullName, out TypeDefinition type)
	{
		return AllTypes.TryGetValue(fullName, out type);
	}

	public static IEnumerable<TypeDefinition> EnumerateAllTypes()
	{
		return _assemblies.SelectMany(a => a.MainModule.GetAllTypes());
	}

	public static TypeDefinition? ResolveEx(this TypeReference type)
	{
		if (type == null)
		{
			return null;
		}
		if (type is TypeDefinition td)
		{
			return td;
		}

		if (_resolveCache.TryGetValue(type, out var typeDefinition))
		{
			return typeDefinition;
		}

		if (type.IsArray)
		{
			return null;
		}

		typeDefinition = type.Resolve();
		if (typeDefinition == null)
		{
			if (type.IsGenericInstance)
			{
				return null;
			}

			if (TryGetType(type.FullName, out typeDefinition))
			{
				_resolveCache[type] = typeDefinition;
			}
		}

		return typeDefinition;
	}

	public static bool IsAssignableFrom(this TypeInfo baseType, TypeInfo type)
	{
		return baseType.Type.IsAssignableFrom(type.Type);
	}

	public static bool IsAssignableFrom(this TypeReference baseType, TypeReference type)
	{
		if (baseType.FullName == "System.Object")
		{
			return true;
		}

		string baseTypeFullName = baseType.FullName;
		return (baseType.ResolveEx()?.IsInterface == true ? type.GetAllInterfaces() : EnumerableExtensions.SelectSequence(type, t => t.ResolveEx()?.BaseType, true))
			.Any(t => t.FullName == baseTypeFullName);
	}

	public static IList<TypeReference> GetGenericArguments(this TypeReference type)
	{
		return type is IGenericInstance gi ? (IList<TypeReference>)gi.GenericArguments : Array.Empty<TypeReference>();
	}

	public static bool IsStatic(this TypeDefinition type)
	{
		return type.IsAbstract && type.IsSealed;
	}

	public static bool IsStatic(this PropertyDefinition property)
	{
		return property.GetMethod?.IsStatic ?? property.SetMethod?.IsStatic ?? false;
	}

	public static bool IsNullable(this TypeReference type)
	{
		return !type.IsValueType || type.IsValueNullable();
	}

	public static bool IsValueNullable(this TypeReference type)
	{
		return type.IsGenericInstance && type.GetElementType().FullName == "System.Nullable`1";
	}

	public static bool IsInterface(this TypeReference type)
	{
		var typeDefinition = type.ResolveEx();
		if (typeDefinition != null)
		{
			return typeDefinition.IsInterface;
		}
		if (type.IsGenericInstance)
		{
			return type.GetElementType().IsInterface();
		}
		return false;
	}

	public static TypeReference? GetBaseType(this TypeReference type)
	{
		if (type == null)
		{
			return null;
		}

		if (_baseTypeCache.TryGetValue(type, out var baseType))
		{
			return baseType;
		}

		var definition = type.ResolveEx();
		if (definition == null && type.IsGenericInstance)
		{
			definition = type.GetElementType().ResolveEx();
		}
		if (definition == null)
		{
			return null;
		}
		baseType = definition.BaseType;
		if (baseType == null)
		{
			return null;
		}
		baseType = ReplaceGenericParameters(type, definition, baseType);
		_baseTypeCache[type] = baseType;
		return baseType;
	}

	public static IEnumerable<TypeReference> GetAllInterfaces(this TypeReference type)
	{
		var definition = type.ResolveEx();
		if (definition == null && type.IsGenericInstance)
		{
			definition = type.GetElementType().ResolveEx();
		}
		if (definition == null)
		{
			return Enumerable.Empty<TypeReference>();
		}
		var result = definition.Interfaces.Select(i => ReplaceGenericParameters(type, definition, i.InterfaceType));
		var baseType = definition.BaseType;
		if (baseType != null)
		{
			result = result.Concat(baseType.GetAllInterfaces().Select(i => ReplaceGenericParameters(type, definition, i)));
		}

		if (definition.IsInterface)
		{
			result = EnumerableExtensions.AsEnumerable(type).Concat(result);
		}

		return result.Distinct(i => i.FullName);
	}

	public static IEnumerable<PropertyDefinition> GetProperties(this TypeReference type)
	{
		var typeDefinition = type.ResolveEx();
		if (typeDefinition == null && type.IsGenericInstance)
		{
			typeDefinition = type.GetElementType().ResolveEx();
		}
		if (typeDefinition == null)
		{
			return Enumerable.Empty<PropertyDefinition>();
		}
		return typeDefinition.Properties;
	}

	public static IEnumerable<PropertyDefinition> GetAllProperties(this TypeReference type)
	{
		if (type.IsInterface())
		{
			return type.GetAllInterfaces().Select(i =>
			{
				var t = i.ResolveEx();
				if (t != null)
				{
					return t;
				}
				if (i.IsGenericInstance)
				{
					return i.GetElementType().ResolveEx();
				}
				return null;
			}).Where(i => i != null).SelectMany(i => i!.Properties);
		}
		return EnumerableExtensions.SelectSequence(type, t => t.GetBaseType(), true).SelectMany(t => t.GetProperties());
	}

	public static IEnumerable<FieldDefinition> GetFields(this TypeReference type)
	{
		var typeDefinition = type.ResolveEx();
		if (typeDefinition == null && type.IsGenericInstance)
		{
			typeDefinition = type.GetElementType().ResolveEx();
		}
		if (typeDefinition == null)
		{
			return Enumerable.Empty<FieldDefinition>();
		}
		return typeDefinition.Fields;
	}

	public static IEnumerable<FieldDefinition> GetAllFields(this TypeReference type)
	{
		if (type.IsInterface())
		{
			return Enumerable.Empty<FieldDefinition>();
		}
		return EnumerableExtensions.SelectSequence(type, t => t.GetBaseType(), true).SelectMany(t => t.GetFields());
	}

	public static IEnumerable<EventDefinition> GetAllEvents(this TypeReference type)
	{
		if (type.IsInterface())
		{
			return type.GetAllInterfaces().Select(i => i.ResolveEx()).Where(i => i != null).SelectMany(i => i!.Events);
		}

		var definition = type.ResolveEx();
		if (definition != null)
		{
			return EnumerableExtensions.SelectSequence(definition, t => t.BaseType.ResolveEx(), true).SelectMany(t => t.Events);
		}

		if (type.IsGenericInstance)
		{
			return type.GetElementType().GetAllEvents();
		}

		return Enumerable.Empty<EventDefinition>();
	}

	public static IEnumerable<MethodDefinition> GetAllMethods(this TypeReference type)
	{
		if (type.IsInterface())
		{
			return type.GetAllInterfaces().Select(i => i.ResolveEx()).Where(i => i != null).SelectMany(i => i!.Methods);
		}

		var definition = type.ResolveEx();
		if (definition != null)
		{
			return EnumerableExtensions.SelectSequence(definition, t => t.BaseType.ResolveEx(), true).SelectMany(t => t.Methods);
		}

		if (type.IsGenericInstance)
		{
			return type.GetElementType().GetAllMethods();
		}

		return Enumerable.Empty<MethodDefinition>();
	}

	public static IEnumerable<CustomAttribute> GetAllAttributes(this TypeDefinition type)
	{
		return EnumerableExtensions.SelectSequence(type, t => t.BaseType.ResolveEx(), true).SelectMany(t => t.CustomAttributes);
	}

	public static string GetCSharpFullName(this TypeReference type)
	{
		if (type is IGenericInstance gi)
		{
			return $"{Regex.Replace(type.GetElementType().FullName, @"(.+?)`(\d+)", "$1")}<{string.Join(",", gi.GenericArguments.Select(a => $"global::{GetCSharpFullName(a)}"))}>";
		}
		return type.FullName.Replace('/', '.');
	}

	public static string GetCSharpName(this TypeReference type)
	{
		if (type is IGenericInstance gi)
		{
			return $"{Regex.Replace(type.GetElementType().Name, @"(.+?)`(\d+)", "$1")}<{string.Join(",", gi.GenericArguments.Select(a => GetCSharpName(a)))}>";
		}
		else if (type.HasGenericParameters)
		{
			return $"{Regex.Replace(type.Name, @"(.+?)`(\d+)", "$1")}<{string.Join(",", type.GenericParameters.Select(a => a.Name))}>";
		}
		return type.Name;
	}

	public static IEnumerable<TypeReference> GetEventHandlerParameterTypes(this EventDefinition @event)
	{
		if (@event.InvokeMethod != null)
		{
			return @event.InvokeMethod.Parameters.Select(p => p.ParameterType);
		}

		var eventType = @event.EventType;
		if (eventType is IGenericInstance gi)
		{
			var definition = eventType.GetElementType().ResolveEx();
			if (definition == null)
			{
				return Enumerable.Empty<TypeReference>();
			}

			var invoke = definition.Methods.First(m => m.Name == "Invoke");

			var tGenericArgument = gi.GenericArguments;
			var genericParameters = definition.GenericParameters;
			return invoke.Parameters.Select(param =>
			{
				var parameterType = param.ParameterType;
				if (parameterType.IsGenericParameter)
				{
					int index = genericParameters.IndexOf(p => p.Name == parameterType.Name);
					Debug.Assert(index != -1);
					return tGenericArgument[index];
				}
				else
				{
					return parameterType;
				}
			});
		}
		else
		{
			var definition = eventType.ResolveEx();
			if (definition != null)
			{
				return definition.Methods.First(m => m.Name == "Invoke").Parameters.Select(p => p.ParameterType);
			}

		}
		return Enumerable.Empty<TypeReference>();
	}

	private static TypeReference ReplaceGenericParameters(TypeReference type, TypeDefinition typeDefinition, TypeReference interfaceType)
	{
		if (type is not IGenericInstance tgi || interfaceType is not IGenericInstance igi || igi.GenericArguments.All(a => !a.IsGenericParameter))
		{
			return interfaceType;
		}
		else
		{
			var tGenericArgument = tgi.GenericArguments;
			var genericArguments = igi.GenericArguments.ToList(); //Make copy
			var genericParameters = typeDefinition.GenericParameters;
			for (int i = 0; i < genericArguments.Count; i++)
			{
				var a = genericArguments[i];
				if (a.IsGenericParameter)
				{
					int index = genericParameters.IndexOf(p => p.Name == a.Name);
					if (index != -1)
					{
						genericArguments[i] = tGenericArgument[index];
					}
					else
					{
						Debug.Assert(false);
					}
				}
			}
			var definitionType = interfaceType.GetElementType();
			return definitionType.MakeGenericInstanceType(genericArguments.ToArray());
		}
	}

	public class AssemblyResolver : IAssemblyResolver
	{
		public AssemblyDefinition Resolve(AssemblyNameReference name)
		{
			var fullName = name.FullName;
			var assembly = _assemblies.FirstOrDefault(a => a.Name.FullName == fullName);
			return assembly;
		}

		public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
		{
			return Resolve(name);
		}

		public void Dispose()
		{
		}
	}
}

