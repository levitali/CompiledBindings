#nullable enable

namespace CompiledBindings;

public class XamlDomBase
{
	public HashSet<string> IncludeNamespaces { get; } = new HashSet<string>();

	public void AddNamespace(string ns)
	{
		if (!IncludeNamespaces.Contains(ns))
		{
			IncludeNamespaces.Add(ns);
		}
	}
}

public class XamlObject
{
	public XamlObject(XamlNode xamlNode, TypeInfo type)
	{
		XamlNode = xamlNode;
		Type = type;
	}

	public XamlNode XamlNode { get; }
	public TypeInfo Type { get; }

	public string? Name { get; set; }
	public bool NameExplicitlySet { get; set; }
	public XamlObject? Parent { get; set; }
	public bool IsRoot { get; set; }

	public List<XamlObjectProperty> Properties { get; } = new List<XamlObjectProperty>();

	public string? CreateCSharpValue { get; set; }

	public bool GenerateMember { get; set; }
	public string? FieldModifier { get; set; }
}

public class XamlObjectProperty
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public XamlNode XamlNode { get; set; }
	public XamlObject Object { get; set; }
	public string MemberName { get; set; }
	public XamlObjectValue Value { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public bool IsAttached;

	public PropertyInfo? TargetProperty { get; set; }
	public MethodInfo? TargetMethod { get; set; }
	public EventInfo? TargetEvent { get; set; }

	public TypeInfo MemberType => TargetMethod?.Parameters.Last().ParameterType ?? TargetProperty?.PropertyType ?? TargetEvent!.EventType;

	public override string ToString()
	{
		if (TargetProperty != null)
		{
			return $"{TargetProperty.Definition.Name} = {Value}";
		}
		if (TargetMethod != null)
		{
			var res = $"{TargetMethod.Definition.Name}({Value})";
			if (IsAttached)
			{
				res = $"{TargetMethod.Definition.DeclaringType.Name}.{res}";
			}
			return res;
		}
		return $"{TargetEvent!.Definition.Name} += {Value}";
	}
}

public class XamlObjectValue
{
	public Expression? StaticValue { get; set; }
	public Bind? BindValue { get; set; }
	public string? CSharpValue { get; set; }

	public override string ToString()
	{
		if (StaticValue != null)
		{
			return StaticValue.ToString();
		}
		if (BindValue != null)
		{
			var res = (BindValue.Expression ?? BindValue.BindBackExpression!).ToString();
			if (res.StartsWith("dataRoot."))
			{
				res = res.Substring("dataRoot.".Length);
			}
			return res;
		}
		return CSharpValue!;
	}
}

public class BindingScope
{
	public string? ViewName;
	public TypeInfo? DataType;
	public List<Bind> Bindings = new();
	public BindingsData? BindingsData;
}
