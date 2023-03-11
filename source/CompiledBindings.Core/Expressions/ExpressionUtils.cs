namespace CompiledBindings;

public static class ExpressionUtils
{
	public static List<LocalVariable> GroupExpressions(IReadOnlyList<PropertySetExpression> setExpressions)
	{
		int localVarIndex = 1;
		List<LocalVariable> localVars = new();

		while (true)
		{
			var group = setExpressions
				.SelectMany(p => p.Expression.EnumerateTree()
					.Where(e => e is not (ConstantExpression or VariableExpression or TypeExpression or DefaultExpression or NewExpression) &&
								(e is not MemberExpression me || (me.Member is not (MethodInfo or FieldInfo))))
					.Select(e => (property: p, expr: e)))
				.GroupBy(e => e.expr.Key)
				.Where(g => g.Take(2).Count() > 1)
				.OrderByDescending(g => g.Select(p => p.property).Distinct().Count())
				.ThenBy(g => g.First().expr, ExpressionComparer.Instance)
				.FirstOrDefault();
			if (group == null)
			{
				break;
			}

			var (pr, expression) = group.First();
			var type = GetExpressionType(expression);

			var localVar = new LocalVariable("value" + localVarIndex++, expression, pr.Property.XamlNode);
			var localVarExpr = new VariableExpression(type, localVar.Name);
			foreach (var (property, expr) in group.Distinct(p => p.property))
			{
				property.Expression = property.Expression.CloneReplace(expr, localVarExpr);
			}
			localVars.Add(localVar);
		}

		return localVars;
	}

	public static ExpressionGroup GroupExpressions(IEnumerable<XamlObject> objects)
	{
		return GroupExpressions(objects.SelectMany(o => o.Properties));
	}

	public static ExpressionGroup GroupExpressions(IEnumerable<XamlObjectProperty> properties)
	{
		var setExpressions = properties.Where(p => p.Value.StaticValue != null).Select(p => new PropertySetExpression(p, p.Value.StaticValue!)).ToList();
		var setProperties = properties.Where(p => p.Value.CSharpValue != null).ToList();

		var localVars = GroupExpressions(setExpressions);
		var updateMethod = new ExpressionGroup
		{
			LocalVariables = localVars,
			SetExpressions = setExpressions,
			SetProperties = setProperties,
		};

		return updateMethod;
	}

	public static TypeInfo GetExpressionType(Expression expression)
	{
		var type = expression.Type;
		if (!type.IsNullable && expression.IsNullable)
		{
			if (type.Reference.IsValueType)
			{
				type = TypeInfo.GetTypeThrow(typeof(Nullable<>)).MakeGenericInstanceType(type);
			}
			else
			{
				type = new TypeInfo(type, true);
			}
		}
		return type;
	}

	private class ExpressionComparer : IComparer<Expression>
	{
		public static readonly ExpressionComparer Instance = new();

		public int Compare(Expression x, Expression y)
		{
			var sx = x.Key;
			var sy = y.Key;
			if (sx == sy)
			{
				return 0;
			}

			if (sx.Contains(sy))
			{
				return -1;
			}

			if (sy.Contains(sx))
			{
				return 1;
			}

			return string.Compare(sx, sy);
		}
	}
}

public class PropertySetExpression
{
	public PropertySetExpression(XamlObjectProperty property, Expression expression)
	{
		Property = property;
		Expression = expression;
	}

	public XamlObjectProperty Property { get; }
	public Expression Expression { get; set; }
}

public class LocalVariable
{
	public LocalVariable(string name, Expression expression, XamlNode xamlNode)
	{
		Name = name;
		Expression = expression;
		XamlNode = xamlNode;
	}

	public string Name { get; }
	public Expression Expression { get; }
	public XamlNode XamlNode { get; }
}

public class ExpressionGroup
{
	public required List<LocalVariable> LocalVariables { get; init; }
	public required IReadOnlyList<PropertySetExpression> SetExpressions { get; init; }

	public List<XamlObjectProperty>? SetProperties { get; init; }

	public bool IsEmpty => SetExpressions.Count == 0 && SetProperties?.Count is not > 0;
}
