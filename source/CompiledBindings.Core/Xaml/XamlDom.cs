namespace CompiledBindings;

public class XamlDomBase
{
	public required TypeInfo TargetType { get; init; }
	public required HashSet<string> IncludeNamespaces { get; init; }
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
	public required XamlNode XamlNode { get; init; }
	public required XamlObject Object { get; init; }
	public required bool IsAttached { get; init; }
	public required PropertyInfo? TargetProperty { get; init; }
	public required MethodInfo? TargetMethod { get; set; }
	public required EventInfo? TargetEvent { get; init; }
	public required string MemberName { get; init; }
	public XamlObjectValue Value { get; set; } = null!;

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

	// x:Bind used to set property of the standard Binding type
	public Bind? BindingValue { get; set; }

	public override string ToString()
	{
		if (StaticValue != null)
		{
			return StaticValue.ToString();
		}

		var b = BindValue ?? BindingValue;
		if (b != null)
		{
			var res = (b.SourceExpression ?? b.BindBackExpression!).ToString();
			if (res.StartsWith("dataRoot."))
			{
				res = res.Substring("dataRoot.".Length);
			}
			return res;
		}
		
		return CSharpValue!;
	}
}

public class ViewBindingScope
{
	public string? ViewName { get; set; }
	public required TypeInfo? DataType { get; init; }
	public List<Bind> Bindings { get; } = new();
	public BindingsClass BindingsData { get; set; } = null!;
}
