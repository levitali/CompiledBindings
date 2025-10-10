﻿namespace CompiledBindings;

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

	public virtual Expression TypeDefiningExpression => this;

	public virtual bool IsNullable => Type.IsNullable || Enumerate().Any(e => e.IsNullable);

	public bool IsAssignableTo(TypeInfo type)
	{
		if (type.IsAssignableFrom(Type))
		{
			return true;
		}

		if (Type.Reference.IsValueNullable() &&
			Type.Reference.GetGenericArguments()![0].FullName == type.Reference.FullName)
		{
			return true;
		}

		if (TypeDefiningExpression is not ConstantExpression ce)
		{
			return false;
		}

		if (type.IsNullable && ce.Value == null)
		{
			return true;
		}

		switch ((type.Reference.FullName, ce.Value))
		{
			case ("System.Char", string v1) when v1.Length == 1:
			case ("System.Int8", long v2) when v2 >= sbyte.MinValue:
			case ("System.Int8", ulong v22) when v22 <= (ulong)sbyte.MaxValue:
			case ("System.UInt8", ulong v3) when v3 <= byte.MaxValue:
			case ("System.Int16", long v4) when v4 >= short.MinValue:
			case ("System.Int16", ulong v44) when v44 <= (ulong)short.MaxValue:
			case ("System.UInt16", ulong v5) when v5 <= ushort.MaxValue:
			case ("System.Int32", long v6) when v6 >= int.MinValue:
			case ("System.Int32", ulong v66) when v66 <= int.MaxValue:
			case ("System.UInt32", ulong v7) when v7 <= uint.MaxValue:
			case ("System.Decimal", long v8) when v8 >= decimal.MinValue:
			case ("System.Decimal", ulong v9) when v9 <= decimal.MaxValue:
			case ("System.Decimal", double v10) when v10 is >= ((double)decimal.MinValue) and <= ((double)decimal.MaxValue):
			case ("System.Single", long v11) when v11 >= float.MinValue:
			case ("System.Single", ulong v12) when v12 <= float.MaxValue:
			case ("System.Single", double v13) when v13 is >= float.MinValue and <= float.MaxValue:
			case ("System.Double", long v14) when v14 >= double.MinValue:
			case ("System.Double", ulong v15) when v15 <= double.MaxValue:
				return true;
		}

		return false;
	}

	public virtual IEnumerable<Expression> Enumerate()
	{
		return Enumerable.Empty<Expression>();
	}

	public string CSharpCode => _cSharpCode ??= GetCSharpCode();

	public virtual string Key => CSharpCode;

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
		if (current.Key.Equals(Key))
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

	public static Expression StripParenExpression(Expression expression)
	{
		while (expression is ParenExpression pe)
		{
			expression = pe.Expression;
		}
		return expression;
	}

	// Checks the need to add null-check operator ? to the expression when generating C# code.
	public static bool CheckEndExpressionNullable(Expression expression)
	{
		return
			expression is not (TypeExpression or NewExpression or StaticResourceExpression) &&
			(expression.Type.IsNullable ||
			 (expression.IsNullable && StripParenExpression(expression) is CastExpression && !expression.Type.IsNullable));
	}

	public static Expression Convert(Expression expression, TypeInfo targetType)
	{
		var targetType2 = targetType;
		
		if (targetType2.Reference.IsValueNullable())
		{
			targetType2 = targetType2.GetGenericArguments()![0];
		}

		if (!expression.IsAssignableTo(targetType2))
		{
			if (expression is not CompiledBindings.DefaultExpression &&
				!TypeInfo.GetTypeThrow(typeof(System.Threading.Tasks.Task)).IsAssignableFrom(expression.Type) &&
				targetType.Reference.FullName == "System.String" && expression.Type.Reference.FullName != "System.String")
			{
				var method = TypeInfo.GetTypeThrow(typeof(object)).Methods.First(m => m.Definition.Name == "ToString");
				if (expression is UnaryExpression or BinaryExpression or CoalesceExpression)
				{
					expression = new ParenExpression(expression);
				}
				expression = new CallExpression(expression, method, Array.Empty<Expression>());
			}			
			if (expression.Type.Reference.FullName == "System.Object")
			{
				return new CastExpression(expression, targetType);
			}
		}
		else if (expression.IsNullable && !targetType.IsNullable)
		{
			Expression defaultExpr = targetType.Reference.FullName == "System.String"
				? new ConstantExpression("")
				: Expression.DefaultExpression;
			return new CoalesceExpression(expression, defaultExpr);
		}
		return expression;
	}
}

public class ConstantExpression : Expression
{
	public ConstantExpression(object? value) : base(TypeInfo.GetTypeThrow(value?.GetType() ?? typeof(object)))
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
			_ => System.Convert.ToString(Value, CultureInfo.InvariantCulture),
		};
	}

	public override bool IsNullable => Value == null;
}

public class DefaultExpression : Expression
{
	public DefaultExpression(TypeInfo? type) : base(type ?? TypeInfo.GetTypeThrow(typeof(object)))
	{
	}

	protected override string GetCSharpCode()
	{
		string str = "default";
		if (Type.Reference.FullName != "System.Object")
		{
			str += "(global::" + Type.Reference.GetCSharpFullName() + ")";
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

	protected override string GetCSharpCode()
	{
		return Name;
	}

	public VariableExpression ToNotNullable(string? name = null)
	{
		return Type.IsNullable ? new VariableExpression(Type.ToNotNullable(), name ?? Name) : this;
	}
}

public class ValueExpression : Expression
{
	private string _value;

	public ValueExpression(TypeInfo type, string? value = null) : base(type)
	{
		_value = value ?? "value";
	}

	protected override string GetCSharpCode()
	{
		return _value;
	}
}

public class MemberExpression : Expression, INotifiableExpression
{
	public MemberExpression(Expression expression, IMemberInfo member, TypeInfo type, bool? isNotifiable = null) : base(type)
	{
		Expression = expression;
		Member = member;
		IsNotifiable = isNotifiable;
	}

	public Expression Expression { get; private set; }

	public IMemberInfo Member { get; }

	public bool? IsNotifiable { get; }

	protected override string GetCSharpCode()
	{
		string res = Expression.CSharpCode;
		if (res != null)
		{
			if (CheckEndExpressionNullable(Expression))
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

	public override Expression TypeDefiningExpression => Expression.TypeDefiningExpression;

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

	public override Expression TypeDefiningExpression =>
		Type.Reference.FullName == "System.Boolean"
		? base.TypeDefiningExpression
		: Left.TypeDefiningExpression;

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
				return TypeInfo.GetTypeThrow(typeof(bool));
		}
		var type = left.Type;
		if ((type.Reference.IsValueType && left.IsNullable) || right.IsNullable)
		{
			if (!type.Reference.IsNullable())
			{
				type = TypeInfo.GetTypeThrow(typeof(Nullable<>)).MakeGenericInstanceType(type);
			}
		}
		return type;
	}
}

public class CallExpression : Expression, INotifiableExpression
{
	public CallExpression(Expression expression, MethodInfo method, Expression[] args, bool? isNotifiable = null) : base(method.ReturnType)
	{
		Expression = expression;
		Method = method;
		Args = args;
		IsNotifiable = isNotifiable;
	}

	public Expression Expression { get; private set; }
	public MethodInfo Method { get; private set; }
	public Expression[] Args { get; private set; }
	public bool? IsNotifiable { get; }

	protected override string GetCSharpCode()
	{
		string expr = Expression.CSharpCode;
		if (CheckEndExpressionNullable(Expression))
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

	// Do not consinder argument expressions
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

	IMemberInfo? INotifiableExpression.Member => Method;
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
			if (CheckEndExpressionNullable(Expression))
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

	// Do not consinder argument expressions
	public override bool IsNullable => Expression.IsNullable || Type.IsNullable;

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
		return Args;
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
		return $"global::{Type.Reference.GetCSharpFullName()}";
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
		string str = $"global::{Type.Reference.GetCSharpFullName()}";
		if (_checkNull && !Type.Reference.IsNullable() && Expression.IsNullable)
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

	public override Expression TypeDefiningExpression => Expression.TypeDefiningExpression;

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

public class ElementAccessExpression : Expression, INotifiableExpression
{
	public ElementAccessExpression(TypeInfo elementType, Expression instance, Expression[] parameters, PropertyInfo? indexerProperty, bool? isNotifiable) : base(elementType)
	{
		Expression = instance;
		Parameters = parameters;
		IndexerProperty = indexerProperty;
		IsNotifiable = isNotifiable;
	}

	public Expression Expression { get; private set; }
	public Expression[] Parameters { get; private set; }
	public PropertyInfo? IndexerProperty { get; }
	public bool? IsNotifiable { get; }

	// Do not consinder parameter expressions
	public override bool IsNullable => Expression.IsNullable || Type.IsNullable;

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

	IMemberInfo? INotifiableExpression.Member => IndexerProperty;
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

	public override Expression TypeDefiningExpression => IfTrue.TypeDefiningExpression;

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

	public override Expression TypeDefiningExpression => Right.TypeDefiningExpression;

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
		return $"typeof({ThisType})";
	}

	public override bool IsNullable => false;
}

public class FallbackExpression : Expression
{
	private Expression? _notNull;
	private string? _key;

	public FallbackExpression(Expression expression, Expression nullableExpression, Expression fallbackExpression, VariableExpression localVarExpression, Expression? check = null) : base(expression.Type)
	{
		Expression = expression;
		NullableExpression = nullableExpression;
		Fallback = fallbackExpression;
		LocalVarExpression = localVarExpression;
		Check = check ?? new BinaryExpression(localVarExpression, Expression.NullExpression, "!=");
	}

	public Expression Expression { get; private set; }
	public Expression NullableExpression { get; private set; }
	public Expression Fallback { get; private set; }
	public Expression Check { get; private set; }
	public VariableExpression LocalVarExpression { get; private set; }

	public override Expression TypeDefiningExpression => Expression.TypeDefiningExpression;

	public override bool IsNullable => Fallback.IsNullable;

	public static Expression CreateFallbackExpression(Expression expression, Expression fallbackValue, ref int localVarIndex)
	{
		var expr = expression
			.EnumerateTree().OrderByDescending(e => e.CSharpCode.Length).OfType<IAccessExpression>().FirstOrDefault(e => e.Expression.IsNullable);
		if (expr == null)
		{
			return expression;
		}

		var nullableExpr = expr.Expression;

		var localVarName = "v" + localVarIndex++;
		var localVarExpr = new VariableExpression(nullableExpr.Type, localVarName);

		return new FallbackExpression(
			expression,
			nullableExpr,
			fallbackValue,
			localVarExpr);
	}

	public override IEnumerable<Expression> Enumerate()
	{
		yield return Expression;
		yield return NullableExpression;
		yield return Fallback;
	}

	protected override Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var nullableExpr = NullableExpression.CloneReplace(current, replace);
		var fallback = Fallback.CloneReplace(current, replace);

		if (nullableExpr is VariableExpression ve)
		{
			var check = Check.CloneReplace(LocalVarExpression, ve);

			var newVar = ve.ToNotNullable();
			return new ConditionalExpression(
				check,
				Expression.CloneReplace(NullableExpression, newVar),
				fallback);
		}

		var expression = Expression.CloneReplace(current, replace);

		var clone = (FallbackExpression)base.CloneReplaceCore(current, replace);
		clone.Expression = expression;
		clone.NullableExpression = nullableExpr;
		clone.Fallback = fallback;
		clone.Check = Check;
		clone.LocalVarExpression = LocalVarExpression;
		clone._notNull = null;
		clone._key = null;
		return clone;
	}

	protected override string GetCSharpCode()
	{
		return $"{NullableExpression} is var {LocalVarExpression} && {Check} ? {NotNull} : {Fallback}";
	}

	public override string Key => _key ??= $"{NullableExpression} is var v && {Check.CloneReplace(LocalVarExpression, new VariableExpression(LocalVarExpression.Type, "v"))} ? {NotNull.CloneReplace(LocalVarExpression, LocalVarExpression.ToNotNullable("v"))} : {Fallback}";

	private Expression NotNull => _notNull ??= Expression.CloneReplace(NullableExpression, LocalVarExpression.ToNotNullable());
}

public class InterpolatedStringExpression : Expression
{
	public InterpolatedStringExpression(string format, IReadOnlyList<Expression> expressions) : base(TypeInfo.GetTypeThrow(typeof(string)))
	{
		Format = format;
		Expressions = expressions;
	}

	public string Format { get; }
	public IReadOnlyList<Expression> Expressions { get; }

	public override IEnumerable<Expression> Enumerate()
	{
		return Expressions;
	}

	public override bool IsNullable => false;

	protected override Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var expressions = new List<Expression>(Expressions.Count);
		foreach (var expression in Expressions)
		{
			expressions.Add(expression.CloneReplace(current, replace));
		}
		return new InterpolatedStringExpression(Format, expressions);
	}

	protected override string GetCSharpCode()
	{
		return string.Format(Format, Expressions.Cast<object>().ToArray());
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
		return $"({Expression} as {AsType})";
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

public class IsExpression : Expression
{
	public IsExpression(Expression expression, TypeExpression type) : base(TypeInfo.GetTypeThrow(typeof(bool)))
	{
		Expression = expression;
		IsType = type;
	}

	public Expression Expression { get; private set; }

	public TypeExpression IsType { get; }

	protected override string GetCSharpCode()
	{
		return $"{Expression} is {IsType}";
	}

	public override IEnumerable<Expression> Enumerate()
	{
		yield return Expression;
	}

	protected override Expression CloneReplaceCore(Expression current, Expression replace)
	{
		var clone = (IsExpression)base.CloneReplaceCore(current, replace);
		clone.Expression = Expression.CloneReplace(current, replace);
		return clone;
	}

	public override bool IsNullable => false;
}

public class StaticResourceExpression : Expression
{
	public StaticResourceExpression(string name, TypeInfo type) : base(type)
	{
		Name = name;
	}

	public string Name { get; }

	public override bool IsNullable => false;

	protected override string GetCSharpCode()
	{
		return $"_targetRoot.{Name}";
	}
}

public class AssignExpression : Expression
{
	public AssignExpression(Expression left, Expression right) : base(left.Type)
	{
		Left = left;
		Right = right;
	}

	public Expression Left { get; }

	public Expression Right { get; }

	protected override string GetCSharpCode()
	{
		return $"{Left} = {Right}";
	}

	protected override Expression CloneReplaceCore(Expression current, Expression replace)
	{
		return new AssignExpression(Left.CloneReplace(current, replace), Right.CloneReplace(current, replace));
	}

	public override IEnumerable<Expression> Enumerate()
	{
		yield return Left;
		yield return Right;
	}

	public override bool IsNullable => Left.IsNullable;
}

public interface IAccessExpression
{
	Expression Expression { get; }
}

public interface INotifiableExpression : IAccessExpression
{
	IMemberInfo? Member { get; }
	bool? IsNotifiable { get; }
}
