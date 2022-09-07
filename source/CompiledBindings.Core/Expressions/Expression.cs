namespace CompiledBindings;

public abstract class Expression
{
	private string? _cSharpCode;

	public Expression(TypeInfo type)
	{
		Type = type;
	}

	public static DefaultExpression DefaultExpression { get; } = new DefaultExpression(null);

	public static ConstantExpression NullExpression { get; } = new ConstantExpression(null);

	public TypeInfo Type { get; }

	public virtual bool IsNullable => Type.IsNullable || Enumerate().Any(e => e.IsNullable);

	public virtual IEnumerable<Expression> Enumerate()
	{
		return Enumerable.Empty<Expression>();
	}

	public string CSharpCode => _cSharpCode ??= GetCSharpCode();

	public override string ToString()
	{
		return CSharpCode;
	}

	protected abstract string GetCSharpCode();

	public IEnumerable<Expression> EnumerateTree()
	{
		return EnumerableExtensions.SelectTree(this, e => e.Enumerate(), true).Distinct();
	}

	public Expression CloneReplace(Expression current, Expression replace)
	{
		if (current.CSharpCode.Equals(CSharpCode))
		{
			return replace;
		}

		var result = CloneReplaceCore(current, replace);
		return result;
	}

	protected virtual Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var expression = (Expression)MemberwiseClone();
		expression._cSharpCode = null;
		return expression;
	}

	protected static Expression StripParenExpression(Expression expression)
	{
		while (expression is ParenExpression pe)
		{
			expression = pe.Expression;
		}
		return expression;
	}
}

public class ConstantExpression : Expression
{
	public ConstantExpression(object? value) : base(TypeInfo.GetTypeThrow(value?.GetType().FullName ?? "System.Object"))
	{
		Value = value;
	}

	public object? Value { get; }

	protected override string GetCSharpCode()
	{
		return Value switch
		{
			null => "null",
			string str => $"\"{str}\"",
			bool b => b ? "true" : "false",
			char c => $"'{c}'",
			_ => Convert.ToString(Value, CultureInfo.InvariantCulture),
		};
	}

	public override bool IsNullable => false;
}

public class DefaultExpression : Expression
{
	public DefaultExpression(TypeInfo? type) : base(type ?? TypeInfo.GetTypeThrow("System.Object"))
	{
	}

	protected override string GetCSharpCode()
	{
		string str = "default";
		if (Type.Type.FullName != "System.Object")
		{
			str += "(global::" + Type.Type.GetCSharpFullName() + ")";
		}

		return str;
	}

	public override bool IsNullable => false;
}

public class VariableExpression : Expression
{
	public VariableExpression(TypeInfo type, string name) : base(type)
	{
		Name = name;
	}

	public string Name { get; }

	public override bool IsNullable => Type.IsNullable;

	protected override string GetCSharpCode()
	{
		return Name;
	}
}

public class MemberExpression : Expression, IAccessExpression
{
	public MemberExpression(Expression expression, IMemberInfo member, TypeInfo type) : base(type)
	{
		Expression = expression;
		Member = member;
	}

	public Expression Expression { get; private set; }

	public IMemberInfo Member { get; }

	protected override string GetCSharpCode()
	{
		string res = Expression.CSharpCode;
		if (res != null)
		{
			if (CheckExpressionNullable(Expression))
			{
				res += '?';
			}
			res += '.';
		}
		return res + Member.Definition.Name;
	}

	public override IEnumerable<Expression> Enumerate()
	{
		yield return Expression;
	}

	protected override Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var clone = (MemberExpression)base.CloneReplaceCore(current, replace);
		clone.Expression = Expression.CloneReplace(current, replace);
		return clone;
	}

	internal static bool CheckExpressionNullable(Expression expression)
	{
		return expression is not (TypeExpression or NewExpression) &&
			expression.Type.IsNullable && (expression is not VariableExpression pe || pe.IsNullable);
	}
}

public class UnaryExpression : Expression
{
	public UnaryExpression(Expression expression, string operand) : base(expression.Type)
	{
		Expression = expression;
		Operand = operand;
	}

	public Expression Expression { get; private set; }

	public string Operand { get; }

	protected override string GetCSharpCode()
	{
		return $"{Operand}{Expression}";
	}

	public override IEnumerable<Expression> Enumerate()
	{
		yield return Expression;
	}
	
	protected override Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var clone = (UnaryExpression)base.CloneReplaceCore(current, replace);
		clone.Expression = Expression.CloneReplace(current, replace);
		return clone;
	}
}

public class BinaryExpression : Expression
{
	public BinaryExpression(Expression left, Expression right, string operand) : base(GetExpressionType(left, right, operand))
	{
		Left = left;
		Right = right;
		Operand = operand;
	}

	public Expression Left { get; private set; }
	public Expression Right { get; private set; }
	public string Operand { get; private set; }

	protected override string GetCSharpCode()
	{
		string left = Left.CSharpCode;
		string right = Right.CSharpCode;
		if (Operand is "&&" or "||")
		{
			if (Left.IsNullable)
			{
				left = $"({left} ?? default)";
			}
			if (Right.IsNullable)
			{
				right = $"({right} ?? default)";
			}
		}
		return $"{left} {Operand} {right}";
	}

	public override IEnumerable<Expression> Enumerate()
	{
		yield return Left;
		yield return Right;
	}

	protected override Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var clone = (BinaryExpression)base.CloneReplaceCore(current, replace);
		clone.Left = Left.CloneReplace(current, replace);
		clone.Right = Right.CloneReplace(current, replace);
		return clone;
	}

	public override bool IsNullable => Type.IsNullable;

	private static TypeInfo GetExpressionType(Expression left, Expression right, string operand)
	{
		switch (operand)
		{
			case ">":
			case ">=":
			case "<":
			case "<=":
			case "==":
			case "!=":
			case "&&":
			case "||":
				return TypeInfo.GetTypeThrow("System.Boolean");
		}
		var type = left.Type;
		if ((type.Type.IsValueType && left.IsNullable) || right.IsNullable)
		{
			if (!type.Type.IsNullable())
			{
				type = TypeInfo.GetTypeThrow("System.Nullable`1").MakeGenericInstanceType(type);
			}
		}
		return type;
	}
}

public class CallExpression : Expression, IAccessExpression
{
	public CallExpression(Expression expression, MethodInfo method, Expression[] args) : base(method.ReturnType)
	{
		Expression = expression;
		Method = method;
		Args = args;
	}

	public Expression Expression { get; private set; }
	public MethodInfo Method { get; private set; }
	public Expression[] Args { get; private set; }

	protected override string GetCSharpCode()
	{
		string expr = Expression.CSharpCode;
		if (MemberExpression.CheckExpressionNullable(Expression) ||
			(Expression.IsNullable && StripParenExpression(Expression) is CastExpression && !Expression.Type.IsNullable))
		{
			expr += '?';
		}

		return $"{expr}.{Method.Definition.Name}({string.Join(", ", Args.Select(a => a.CSharpCode))})";
	}

	public override IEnumerable<Expression> Enumerate()
	{
		yield return Expression;
		foreach (var arg in Args)
		{
			yield return arg;
		}
	}

	public override bool IsNullable => Expression.IsNullable || Type.IsNullable;

	protected override Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var clone = (CallExpression)base.CloneReplaceCore(current, replace);
		clone.Expression = Expression.CloneReplace(current, replace);
		clone.Args = new Expression[Args.Length];
		for (int i = 0; i < Args.Length; i++)
		{
			clone.Args[i] = Args[i].CloneReplace(current, replace);
		}
		return clone;
	}
}

public class InvokeExpression : Expression, IAccessExpression
{
	public InvokeExpression(Expression expression, Expression[] args, TypeInfo resultType) : base(resultType)
	{
		Expression = expression;
		Args = args;
	}

	public Expression Expression { get; private set; }
	public Expression[] Args { get; private set; }

	protected override string GetCSharpCode()
	{
		string res = Expression.CSharpCode;
		if (res != null)
		{
			if (MemberExpression.CheckExpressionNullable(Expression))
			{
				res += "?.Invoke";
			}
		}
		return $"{res}({string.Join(", ", Args.Select(a => a.CSharpCode))})";
	}

	public override IEnumerable<Expression> Enumerate()
	{
		yield return Expression;
		foreach (var arg in Args)
		{
			yield return arg;
		}
	}

	protected override Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var clone = (InvokeExpression)base.CloneReplaceCore(current, replace);
		clone.Expression = Expression.CloneReplace(current, replace);
		clone.Args = new Expression[Args.Length];
		for (int i = 0; i < Args.Length; i++)
		{
			clone.Args[i] = Args[i].CloneReplace(current, replace);
		}
		return clone;
	}
}

public class NewExpression : Expression
{
	public NewExpression(TypeExpression typeExpression, Expression[] args) : base(typeExpression.Type)
	{
		TypeExpression = typeExpression;
		Args = args;
	}

	public TypeExpression TypeExpression { get; private set; }
	public Expression[] Args { get; private set; }

	protected override string GetCSharpCode()
	{
		return $"new {TypeExpression}({string.Join(", ", Args.Select(a => a.CSharpCode))})";
	}

	public override IEnumerable<Expression> Enumerate()
	{
		foreach (var arg in Args)
		{
			yield return arg;
		}
	}

	protected override Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var clone = (NewExpression)base.CloneReplaceCore(current, replace);
		clone.Args = new Expression[Args.Length];
		for (int i = 0; i < Args.Length; i++)
		{
			clone.Args[i] = Args[i].CloneReplace(current, replace);
		}
		return clone;
	}

	public override bool IsNullable => false;
}

public class TypeExpression : Expression
{
	public TypeExpression(TypeInfo type) : base(type)
	{
	}

	protected override string GetCSharpCode()
	{
		return Type.Type.GetCSharpFullName();
	}

	public override bool IsNullable => false;
}

public class CastExpression : Expression
{
	private readonly bool _checkNull;

	public CastExpression(Expression expression, TypeInfo castType, bool checkNull = true) : base(castType)
	{
		Expression = expression;
		_checkNull = checkNull;
	}

	public Expression Expression { get; private set; }

	protected override string GetCSharpCode()
	{
		string str = $"global::{Type.Type.GetCSharpFullName()}";
		if (_checkNull && !Type.Type.IsNullable() && Expression.IsNullable)
		{
			str += "?";
		}
		return $"(({str}){Expression})";
	}

	public override IEnumerable<Expression> Enumerate()
	{
		yield return Expression;
	}

	protected override Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var clone = (CastExpression)base.CloneReplaceCore(current, replace);
		clone.Expression = Expression.CloneReplace(current, replace);
		return clone;
	}
}

public class ParenExpression : Expression
{
	public ParenExpression(Expression expression) : base(expression.Type)
	{
		Expression = expression;
	}

	public Expression Expression { get; private set; }

	protected override string GetCSharpCode()
	{
		return $"({Expression})";
	}

	public override IEnumerable<Expression> Enumerate()
	{
		yield return Expression;
	}

	protected override Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var clone = (ParenExpression)base.CloneReplaceCore(current, replace);
		clone.Expression = Expression.CloneReplace(current, replace);
		return clone;
	}
}

public class ElementAccessExpression : Expression, IAccessExpression
{
	public ElementAccessExpression(TypeInfo elementType, Expression instance, Expression[] parameters) : base(elementType)
	{
		Expression = instance;
		Parameters = parameters;
	}

	public Expression Expression { get; private set; }
	public Expression[] Parameters { get; private set; }

	protected override string GetCSharpCode()
	{
		var instance = Expression.CSharpCode;
		if (Expression.IsNullable)
		{
			instance += "?";
		}
		return $"{instance}[{string.Join(", ", Parameters.Select(p => p.CSharpCode))}]";
	}

	public override IEnumerable<Expression> Enumerate()
	{
		yield return Expression;
		foreach (var prm in Parameters)
		{
			yield return prm;
		}
	}

	protected override Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var clone = (ElementAccessExpression)base.CloneReplaceCore(current, replace);
		clone.Expression = Expression.CloneReplace(current, replace);
		clone.Parameters = new Expression[Parameters.Length];
		for (int i = 0; i < Parameters.Length; i++)
		{
			clone.Parameters[i] = Parameters[i].CloneReplace(current, replace);
		}
		return clone;
	}
}

public class ConditionalExpression : Expression
{
	public ConditionalExpression(Expression test, Expression ifTrue, Expression ifFalse) : base(ifTrue.Type)
	{
		Test = test;
		IfTrue = ifTrue;
		IfFalse = ifFalse;
	}

	public Expression Test { get; private set; }
	public Expression IfTrue { get; private set; }
	public Expression IfFalse { get; private set; }

	protected override string GetCSharpCode()
	{
		var test = Test.CSharpCode;
		if (Test.IsNullable)
		{
			test += " ?? default";
		}
		return $"({test} ? {IfTrue} : {IfFalse})";
	}

	public override bool IsNullable => IfTrue.IsNullable || IfFalse.IsNullable;

	public override IEnumerable<Expression> Enumerate()
	{
		yield return Test;
		yield return IfTrue;
		yield return IfFalse;
	}

	protected override Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var clone = (ConditionalExpression)base.CloneReplaceCore(current, replace);
		clone.Test = Test.CloneReplace(current, replace);
		clone.IfTrue = IfTrue.CloneReplace(current, replace);
		clone.IfFalse = IfFalse.CloneReplace(current, replace);
		return clone;
	}
}

public class CoalesceExpression : Expression
{
	public CoalesceExpression(Expression left, Expression right) : base(right.Type)
	{
		Debug.Assert(left.IsNullable);
		Left = left;
		Right = right;
	}

	public Expression Left { get; private set; }
	public Expression Right { get; private set; }

	protected override string GetCSharpCode()
	{
		return $"{Left} ?? {Right}";
	}

	public override IEnumerable<Expression> Enumerate()
	{
		yield return Left;
		yield return Right;
	}

	protected override Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var left = Left.CloneReplace(current, replace);
		if (!left.IsNullable)
		{
			return left;
		}
		var clone = (CoalesceExpression)base.CloneReplaceCore(current, replace);
		clone.Left = left;
		clone.Right = Right.CloneReplace(current, replace);
		return clone;
	}

	public override bool IsNullable => Type.IsNullable;
}

public class TypeofExpression : Expression
{
	public TypeofExpression(TypeExpression type) : base(TypeInfo.GetTypeThrow(typeof(Type)))
	{
		ThisType = type;
	}

	public TypeExpression ThisType { get; }

	protected override string GetCSharpCode()
	{
		return $"typeof(global::{ThisType})";
	}

	public override bool IsNullable => false;
}

public class FallbackExpression : Expression
{
	private readonly string _localVarName;
	private Expression? _notNull;

	public FallbackExpression(Expression expression, Expression fallbackExpression, string localVarName) : base(expression.Type)
	{
		Expression = expression;
		Fallback = fallbackExpression;
		_localVarName = localVarName;

		Debug.Assert(NullableExpression != null);
	}

	public Expression Expression { get; private set; }
	public Expression Fallback { get; private set; }

	public Expression NullableExpression => (Expression)Expression.EnumerateTree().OrderByDescending(e => e.CSharpCode).OfType<IAccessExpression>().First(e => e.Expression.IsNullable).Expression;

	public override IEnumerable<Expression> Enumerate()
	{
		yield return Expression;
		yield return Fallback;
	}

	protected override Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var expression = Expression.CloneReplace(current, replace);
		var expr = expression
			.EnumerateTree().OrderByDescending(e => e.CSharpCode).OfType<IAccessExpression>().FirstOrDefault(e => e.Expression.IsNullable);
		if (expr == null)
		{
			return expression;
		}

		var fallback = Fallback.CloneReplace(current, replace);

		if (expr.Expression is VariableExpression ve)
		{
			return new ConditionalExpression(
				new BinaryExpression(ve, Expression.NullExpression, "!="),
				NotNull.CloneReplace(
					((IAccessExpression)NotNull).Expression,
					new VariableExpression(new TypeInfo(ve.Type, false), ve.Name)),
				fallback);
		}

		var clone = (FallbackExpression)base.CloneReplaceCore(current, replace);
		clone.Expression = expression;
		clone.Fallback = fallback;
		return clone;
	}

	protected override string GetCSharpCode()
	{
		return $"{NullableExpression} is var {_localVarName} && {_localVarName} != null ? {NotNull} : {Fallback}";
	}

	private Expression NotNull
	{
		get
		{
			if (_notNull == null)
			{
				var expr = NullableExpression;
				_notNull = Expression.CloneReplace(
					expr,
					new VariableExpression(new TypeInfo(Expression.Type, false), _localVarName));
			}
			return _notNull;
		}
	}
}

public class InterpolatedStringExpression : Expression
{
	private readonly string _format;
	private readonly IList<Expression> _expressions;

	public InterpolatedStringExpression(string format, IList<Expression> expressions) : base(TypeInfo.GetTypeThrow(typeof(string)))
	{
		_format = format;
		_expressions = expressions;
	}

	public override IEnumerable<Expression> Enumerate()
	{
		return _expressions;
	}

	public override bool IsNullable => false;

	protected override Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var expressions = new List<Expression>(_expressions.Count);
		foreach (var expression in _expressions)
		{
			expressions.Add(expression.CloneReplace(current, replace));
		}
		return new InterpolatedStringExpression(_format, expressions);
	}

	protected override string GetCSharpCode()
	{
		return string.Format(_format, _expressions.Cast<object>().ToArray());
	}
}

public class AsExpression : Expression
{
	public AsExpression(Expression expression, TypeExpression type) : base(type.Type)
	{
		Expression = expression;
		AsType = type;
	}

	public Expression Expression { get; private set; }

	public TypeExpression AsType { get; }

	protected override string GetCSharpCode()
	{
		return $"({Expression} as global::{AsType})";
	}

	public override IEnumerable<Expression> Enumerate()
	{
		yield return Expression;
	}

	protected override Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var clone = (AsExpression)base.CloneReplaceCore(current, replace);
		clone.Expression = Expression.CloneReplace(current, replace);
		return clone;
	}

	public override bool IsNullable => true;
}

public interface IAccessExpression
{
	Expression Expression { get; }
}
