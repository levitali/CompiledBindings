#nullable enable

namespace CompiledBindings;

public abstract class Expression
{
	private string? _toString;

	public Expression(TypeInfo type)
	{
		Type = type;
	}

	public static DefaultExpression DefaultExpression { get; } = new DefaultExpression(null);

	public static ConstantExpression NullExpression { get; } = new ConstantExpression(null);

	public TypeInfo Type { get; }

	public virtual bool IsNullable => Type.IsNullable || Enumerate().Any(e => e.IsNullable);

	public abstract IEnumerable<Expression> Enumerate();

	public override string ToString()
	{
		if (_toString == null)
		{
			_toString = ToStringCore();
		}
		else
		{
			//Debug.Assert(_toString == ToStringCore());
		}
		return _toString;
	}

	protected abstract string ToStringCore();

	public IEnumerable<Expression> EnumerateTree()
	{
		return EnumerableExtensions.SelectTree(this, e => e.Enumerate(), true).Distinct();
	}

	public Expression CloneReplace(Expression current, Expression replace)
	{
		if (current.ToString() == ToString())
		{
			return replace;
		}

		var result = CloneReplaceCore(current, replace);
		return result;
	}

	protected virtual Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var expression = (Expression)MemberwiseClone();
		expression._toString = null;
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

	protected override string ToStringCore()
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

	public override IEnumerable<Expression> Enumerate()
	{
		return Enumerable.Empty<Expression>();
	}

	public override bool IsNullable => false; //TODO!!
}

public class DefaultExpression : Expression
{
	public DefaultExpression(TypeInfo? type) : base(type ?? TypeInfo.GetTypeThrow("System.Object"))
	{
	}

	protected override string ToStringCore()
	{
		string str = "default";
		if (Type.Type.FullName != "System.Object")
		{
			str += "(global::" + Type.Type.GetCSharpFullName() + ")";
		}

		return str;
	}

	public override IEnumerable<Expression> Enumerate()
	{
		return Enumerable.Empty<Expression>();
	}

	public override bool IsNullable => false; //TODO!!
}

public class ParameterExpression : Expression
{
	public ParameterExpression(TypeInfo type, string name) : base(type)
	{
		Name = name;
	}

	public string Name { get; }

	public override bool IsNullable => Type.IsNullable;

	protected override string ToStringCore()
	{
		return Name;
	}

	public override IEnumerable<Expression> Enumerate()
	{
		return Enumerable.Empty<Expression>();
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

	protected override string ToStringCore()
	{
		string res = Expression.ToString();
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
			expression.Type.IsNullable && (expression is not ParameterExpression pe || pe.IsNullable);
	}

	public Expression CloneReplaceExpression(Expression expression)
	{
		return new MemberExpression(expression, Member, Type);
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

	protected override string ToStringCore()
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

	protected override string ToStringCore()
	{
		string left = Left.ToString();
		string right = Right.ToString();
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
			if (!type.Type.IsValueNullable())
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

	protected override string ToStringCore()
	{
		string expr = Expression.ToString();
		if (MemberExpression.CheckExpressionNullable(Expression) ||
			(Expression.IsNullable && StripParenExpression(Expression) is CastExpression && !Expression.Type.IsNullable))
		{
			expr += '?';
		}

		return $"{expr}.{Method.Definition.Name}({string.Join(", ", Args.Select(a => a.ToString()))})";
	}

	public override IEnumerable<Expression> Enumerate()
	{
		yield return Expression;
		foreach (var arg in Args)
		{
			yield return arg;
		}
	}

	public override bool IsNullable => Type.IsNullable;

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

	public Expression CloneReplaceExpression(Expression expression)
	{
		return new CallExpression(expression, Method, Args);
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

	protected override string ToStringCore()
	{
		string res = Expression.ToString();
		if (res != null)
		{
			if (MemberExpression.CheckExpressionNullable(Expression))
			{
				res += "?.Invoke";
			}
		}
		return $"{res}({string.Join(", ", Args.Select(a => a.ToString()))})";
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

	public Expression CloneReplaceExpression(Expression expression)
	{
		return new InvokeExpression(expression, Args, Type);
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

	protected override string ToStringCore()
	{
		return $"new {TypeExpression}({string.Join(", ", Args.Select(a => a.ToString()))})";
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
	public TypeExpression(TypeInfo type) : base(type) //TODO actually the ExpressionType is TypeInfo.GetType("System.Type")
	{
	}

	protected override string ToStringCore()
	{
		return Type.Type.GetCSharpFullName();
	}

	public override IEnumerable<Expression> Enumerate()
	{
		return Enumerable.Empty<Expression>();
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

	protected override string ToStringCore()
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

	protected override string ToStringCore()
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

	protected override string ToStringCore()
	{
		var instance = Expression.ToString();
		if (Expression.IsNullable)
		{
			instance += "?";
		}
		return $"{instance}[{string.Join(", ", Parameters.Select(p => p.ToString()))}]";
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

	public Expression CloneReplaceExpression(Expression expression)
	{
		return new ElementAccessExpression(Type, expression, Parameters);
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

	protected override string ToStringCore()
	{
		var test = Test.ToString();
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
		Left = left;
		Right = right;
	}

	public Expression Left { get; private set; }
	public Expression Right { get; private set; }

	protected override string ToStringCore()
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

	protected override string ToStringCore()
	{
		return $"typeof(global::{ThisType})";
	}

	public override IEnumerable<Expression> Enumerate()
	{
		return Enumerable.Empty<Expression>();
	}

	public override bool IsNullable => false;
}

public class FallbackExpression : Expression
{
	private readonly string _localVarName;

	public FallbackExpression(Expression expression, Expression fallbackExpression, Expression notNullExpression, string localVarName, TypeInfo type) : base(type)
	{
		Expression = expression;
		Fallback = fallbackExpression;
		NotNull = notNullExpression;
		_localVarName = localVarName;
	}

	public Expression Expression { get; private set; }
	public Expression Fallback { get; private set; }
	public Expression NotNull { get; private set; }

	public override IEnumerable<Expression> Enumerate()
	{
		yield return Expression;
		yield return Fallback;
		yield return NotNull;
	}

	protected override Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var clone = (FallbackExpression)MemberwiseClone();
		clone.Expression = Expression.CloneReplace(current, replace);
		clone.Fallback = Fallback.CloneReplace(current, replace);
		clone.NotNull = NotNull.CloneReplace(current, replace);
		return clone;
	}

	protected override string ToStringCore()
	{
		return $"{Expression} is var {_localVarName} && {_localVarName} != null ? {NotNull} : {Fallback}";
	}
}

public interface IAccessExpression
{
	Expression Expression { get; }

	Expression CloneReplaceExpression(Expression expression);
}
