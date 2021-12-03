﻿#nullable enable

namespace CompiledBindings;

public class ExpressionParser
{
	private static readonly Dictionary<string, Expression> _keywords = new Dictionary<string, Expression>
	{
		{ "true", new ConstantExpression(true) },
		{ "false", new ConstantExpression(false) },
		{ "null", Expression.NullExpression },
	};
	private readonly VariableExpression _root;
	private readonly IList<XamlNamespace> _namespaces;
	private readonly List<XamlNamespace> _includeNamespaces = new List<XamlNamespace>();
	private readonly string _text;
	private int _textPos;
	private readonly int _textLen;
	private char _ch;
	private Token _token;
	private TypeInfo? _expectedType;

	private ExpressionParser(VariableExpression root, string expression, TypeInfo resultType, IList<XamlNamespace> namespaces)
	{
		_root = root;
		_namespaces = namespaces;
		_text = expression;
		_textLen = _text.Length;
		_expectedType = resultType;
		SetTextPos(0);
		NextToken();
	}

	public static Expression Parse(TypeInfo dataType, string member, string? expression, TypeInfo resultType, bool validateEnd, IList<XamlNamespace> namespaces, out IList<XamlNamespace> includeNamespaces, out int textPos)
	{
		if (string.IsNullOrWhiteSpace(expression))
		{
			throw new ParseException(Res.EmptyExpression);
		}

		var parser = new ExpressionParser(new VariableExpression(dataType, member), expression!, resultType, namespaces);

		var res = parser.ParseExpression();
		if (!(parser._token.id == TokenId.End || (!validateEnd && parser._token.id == TokenId.Comma)))
		{
			throw new ParseException(Res.SyntaxError, parser._token.pos);
		}

		includeNamespaces = parser._includeNamespaces;
		textPos = parser._textPos;

		return res;
	}

	// ?:, ?? operator
	private Expression ParseExpression()
	{
		int errorPos = _token.pos;
		var expr = ParseLogicalOr();

		if (_token.id == TokenId.Question)
		{
			NextToken();
			var expr1 = ParseExpression();
			ValidateToken(TokenId.Colon, Res.ColonExpected);
			NextToken();
			var expr2 = ParseExpression();
			expr = new ConditionalExpression(expr, expr1, expr2);
		}
		else if (_token.id == TokenId.DoubleQuestion)
		{
			NextToken();
			var right = ParseExpression();
			expr = new CoalesceExpression(expr, right);
		}
		return expr;
	}

	// ||, or operator
	private Expression ParseLogicalOr()
	{
		var left = ParseLogicalAnd();
		while (_token.id == TokenId.DoubleBar || TokenIdentifierIs("or"))
		{
			var savedExpectedType = _expectedType;
			_expectedType = GetNullableUnderlyingType(left.Type);
			ValidateNotMethodAccess(left);
			//var op = _token;
			NextToken();
			var right = ParseLogicalAnd();
			ValidateNotMethodAccess(right);
			left = new BinaryExpression(left, right, "||");
			_expectedType = savedExpectedType;
		}
		return left;
	}

	// &&, and operator
	private Expression ParseLogicalAnd()
	{
		var left = ParseComparison();
		while (_token.id == TokenId.DoubleAmphersand || TokenIdentifierIs("and"))
		{
			var savedExpectedType = _expectedType;
			_expectedType = GetNullableUnderlyingType(left.Type);
			ValidateNotMethodAccess(left);
			//var op = _token;
			NextToken();
			var right = ParseComparison();
			ValidateNotMethodAccess(right);
			left = new BinaryExpression(left, right, "&&");
			_expectedType = savedExpectedType;
		}
		return left;
	}

	// =, ==, !=, <>, >, >=, <, <= operators
	private Expression ParseComparison()
	{
		var left = ParseAdditive();
		while (_token.id is TokenId.DoubleEqual or TokenId.ExclamationEqual or TokenId.LessGreater
			 or TokenId.GreaterThan or TokenId.GreaterThanEqual or TokenId.LessThan or TokenId.LessThanEqual ||
			ReplaceTokenIdentifier("gt", TokenId.GreaterThan) || ReplaceTokenIdentifier("ge", TokenId.GreaterThanEqual) ||
			ReplaceTokenIdentifier("lt", TokenId.LessThan) || ReplaceTokenIdentifier("le", TokenId.LessThanEqual) ||
			ReplaceTokenIdentifier("eq", TokenId.DoubleEqual) || ReplaceTokenIdentifier("ne", TokenId.ExclamationEqual))
		{
			var savedExpectedType = _expectedType;
			_expectedType = GetNullableUnderlyingType(left.Type);
			ValidateNotMethodAccess(left);
			var op = _token;
			NextToken();
			var right = ParseAdditive();
			ValidateNotMethodAccess(right);
			_expectedType = savedExpectedType;

			switch (op.id)
			{
				case TokenId.DoubleEqual:
					left = new BinaryExpression(left, right, "==");
					break;
				case TokenId.ExclamationEqual:
				case TokenId.LessGreater:
					left = new BinaryExpression(left, right, "!=");
					break;
				case TokenId.GreaterThan:
					left = new BinaryExpression(left, right, ">");
					break;
				case TokenId.GreaterThanEqual:
					left = new BinaryExpression(left, right, ">=");
					break;
				case TokenId.LessThan:
					left = new BinaryExpression(left, right, "<");
					break;
				case TokenId.LessThanEqual:
					left = new BinaryExpression(left, right, "<=");
					break;
			}
		}
		return left;

		bool ReplaceTokenIdentifier(string identiferId, TokenId tokenId)
		{
			if (!TokenIdentifierIs(identiferId))
			{
				return false;
			}

			_token.id = tokenId;
			return true;
		}
	}

	// +, -, & operators
	private Expression ParseAdditive()
	{
		var left = ParseMultiplicative();
		while (_token.id is TokenId.Plus or TokenId.Minus)
		{
			var savedExpectedType = _expectedType;
			_expectedType = GetNullableUnderlyingType(left.Type);
			ValidateNotMethodAccess(left);
			var op = _token;
			NextToken();
			var right = ParseMultiplicative();
			ValidateNotMethodAccess(right);
			_expectedType = savedExpectedType;
			switch (op.id)
			{
				case TokenId.Plus:
					left = new BinaryExpression(left, right, "+");
					break;
				case TokenId.Minus:
					left = new BinaryExpression(left, right, "-");
					break;
			}
		}
		return left;
	}

	// *, /, %, mod operators
	private Expression ParseMultiplicative()
	{
		var left = ParseUnary();
		while (_token.id is TokenId.Asterisk or TokenId.Slash or TokenId.Percent || TokenIdentifierIs("mod"))
		{
			var savedExpectedType = _expectedType;
			_expectedType = GetNullableUnderlyingType(left.Type);
			ValidateNotMethodAccess(left);
			var op = _token;
			NextToken();
			var right = ParseUnary();
			ValidateNotMethodAccess(right);
			_expectedType = savedExpectedType;
			switch (op.id)
			{
				case TokenId.Asterisk:
					left = new BinaryExpression(left, right, "*");
					break;
				case TokenId.Slash:
					left = new BinaryExpression(left, right, "/");
					break;
				case TokenId.Percent:
					left = new BinaryExpression(left, right, "%");
					break;
			}
		}
		return left;
	}

	// -, !, not unary operators
	private Expression ParseUnary()
	{
		if (_token.id is TokenId.Minus or TokenId.Exclamation || TokenIdentifierIs("not"))
		{
			var op = _token;
			NextToken();
			if (op.id == TokenId.Minus && (_token.id == TokenId.IntegerLiteral || _token.id == TokenId.RealLiteral))
			{
				_token.text = "-" + _token.text;
				_token.pos = op.pos;
				return ParsePrimary();
			}
			var expr = ParseUnary();
			ValidateNotMethodAccess(expr);
			if (op.id == TokenId.Minus)
			{
				expr = new UnaryExpression(expr, "-");
			}
			else
			{
				expr = new UnaryExpression(expr, "!");
			}
			return expr;
		}
		return ParsePrimary();
	}

	private Expression ParsePrimary()
	{
		var expr = ParsePrimaryStart();
		while (true)
		{
			if (_token.id == TokenId.Dot)
			{
				ValidateNotMethodAccess(expr);
				NextToken();
				expr = ParseMemberAccess(expr);
			}
			else if (_token.id == TokenId.OpenBracket)
			{
				ValidateNotMethodAccess(expr);
				expr = ParseElementAccess(expr);
			}
			else if (_token.id == TokenId.OpenParen)
			{
				expr = ParseInvoke(expr);
			}
			else
			{
				break;
			}
		}
		return expr;
	}

	private MethodInfo FindMethod(TypeInfo type, string methodName, Expression[] args, int errPos)
	{
		type = GetNullableUnderlyingType(type);

		var methods = type.Methods
			.Where(m => m.Definition.Name == methodName &&
						(m.Parameters.Count >= args.Length ||
						 (m.Parameters.Count > 0 && m.Parameters[m.Parameters.Count - 1].Definition.CustomAttributes.Any(a => a.AttributeType.FullName == "System.ParamArrayAttribute"))))
			.ToList();
		if (methods.Count == 0)
		{
			foreach (var ns in _namespaces)
			{
				methods = TypeInfo.FindExtensionMethods(ns.ClrNamespace!, methodName).ToList();
				if (methods.Count > 0)
				{
					_includeNamespaces.Add(ns);
					break;
				}
			}
		}

		var method = FindMethod(methods, args, errPos);
		if (method == null)
		{
			throw new ParseException($"No applicable method '{methodName}' exists in type '{type.Type.FullName}'", errPos, methodName.Length);
		}
		return method;
	}

	private MethodInfo? FindMethod(IList<MethodInfo> methods, Expression[] args, int errPos)
	{		
		if (methods.Count == 1)
		{
			return methods[0];
			// Note! So far we do not check if parameters are assignable.
			// If not, the generated code will not compile
		}
		else if (methods.Count > 1)
		{
			// If there are many methods, it is need to find the right one.
			foreach (var m in methods)
			{
				var parameters = m.Parameters;
				for (int i = 0; i < args.Length; i++)
				{
					if (parameters.Count <= i ||
						(!parameters[i].ParameterType.IsAssignableFrom(args[i].Type) &&
							(args[i] is not ConstantExpression ce || ce.Value is not string || parameters[i].ParameterType.Type.FullName != "System.Char")))
					{
						goto Label_NextMethod;
					}
				}
				if (m.Parameters.Count > args.Length)
				{
					if (!m.Parameters[args.Length].Definition.IsOptional)
					{
						goto Label_NextMethod;
					}
				}
				return m;

			Label_NextMethod:;
			}
		}
		return null;
	}

	private Expression ParseInvoke(Expression expr)
	{
		int errorPos = _token.pos;

		var args = ParseArgumentList();

		var delegateType = TypeInfo.GetTypeThrow(typeof(MulticastDelegate));
		if (!delegateType.IsAssignableFrom(expr.Type))
		{
			throw new ParseException($"The type '{expr.Type.Type.Name}' is not a MulticastDelegate.", errorPos);
		}
		var method = expr.Type.Methods.Single(m => m.Definition.Name == "Invoke");
		if (method.Parameters.Count < args.Length)
		{
			throw new ParseException($"Delegate '{expr.Type.Type.Name}' does not take {args.Length} arguments.", errorPos);
		}

		CorrectCharParameters(method, args, errorPos);
		CorrectNotNullableParameters(method, args);

		return new InvokeExpression(expr, args, method.ReturnType);
	}

	private static void CorrectNotNullableParameters(MethodInfo method, Expression[] args)
	{
		var parameters = method.Parameters;
		for (int i = 0; i < args.Length; i++)
		{
			if (i >= parameters.Count)
			{
				ParameterInfo pi;
				if (i == 0 || !(pi = parameters[parameters.Count - 1]).Definition.CustomAttributes.Any(a => a.AttributeType.FullName == "System.ParamArrayAttribute"))
				{
					throw new InvalidProgramException();
				}
				if (!pi.ParameterType.Type.IsArray)
				{
					throw new InvalidProgramException();
				}

				bool isNullable = pi.ParameterType.GetElementType()!.IsNullable;
				for (; i < args.Length; i++)
				{
					if (isNullable && args[i].IsNullable)
					{
						args[i] = new CoalesceExpression(args[i], Expression.DefaultExpression);
					}
				}
				break;
			}
			else if (!parameters[i].ParameterType.IsNullable && args[i].IsNullable)
			{
				args[i] = new CoalesceExpression(args[i], Expression.DefaultExpression);
			}
		}
	}

	private static void CorrectCharParameters(MethodInfo method, Expression[] args, int errorPos)
	{
		bool? isParamsChar = null;
		for (int i = 0; i < args.Length; i++)
		{
			var arg = args[i];
			if (arg is ConstantExpression ce && ce.Value is string)
			{
				if (isParamsChar == null && i == method.Parameters.Count)
				{
					throw new InvalidProgramException();
				}

				if (i == method.Parameters.Count - 1)
				{
					if (method.Parameters[i].Definition.CustomAttributes.Any(a => a.AttributeType.FullName == "System.ParamArrayAttribute"))
					{
						if (!method.Parameters[i].ParameterType.Type.IsArray)
						{
							throw new InvalidProgramException();
						}

						isParamsChar = method.Parameters[i].ParameterType.Type.GetElementType().FullName == "System.Char";
						if (isParamsChar == false)
						{
							return;
						}
					}
				}
				if (isParamsChar == true || method.Parameters[i].ParameterType.Type.FullName == "System.Char")
				{
					string value = (string)((ConstantExpression)arg).Value!;
					if (value.Length != 1)
					{
						throw new ParseException($"Cannot convert {i + 1} argument '{value}' to char.", errorPos);
					}

					args[i] = new ConstantExpression(value[0]);
				}
			}
		}
	}

	private Expression ParsePrimaryStart()
	{
		switch (_token.id)
		{
			case TokenId.Identifier:
				return ParseIdentifier();
			case TokenId.StringLiteral:
				return ParseStringLiteral();
			case TokenId.IntegerLiteral:
				return ParseIntegerLiteral();
			case TokenId.RealLiteral:
				return ParseRealLiteral();
			case TokenId.OpenParen:
				return ParseParenExpression();
			//case TokenId.Dot:// Dot at the beginning of expression means root; used actually only when the whole expression is a dot
			//	NextToken();
			//	return _root;
			default:
				throw new ParseException(Res.ExpressionExpected, _token.pos);
		}
	}

	private Expression ParseNewExpression()
	{
		NextToken();

		int errorPos = _token.pos;
		string prefix = GetIdentifier();

		NextToken();
		ValidateToken(TokenId.Colon, Res.ColonExpected);

		var typeExpr = ParseTypeExpression(prefix, errorPos);
		var args = ParseArgumentList();

		var method = FindMethod(typeExpr.Type.Constructors, args, errorPos);
		if (method == null)
		{
			throw new ParseException($"No applicable constructor exists in type '{typeExpr.Type.Type.FullName}'", errorPos);
		}

		CorrectCharParameters(method, args, errorPos);
		CorrectNotNullableParameters(method, args);

		return new NewExpression(typeExpr, args);
	}

	private Expression ParseTypeofExpression()
	{
		NextToken();
		int errorPos = _token.pos;
		var args = ParseArgumentList();
		if (args.Length != 1 || args[0] is not TypeExpression typeExpression)
		{
			throw new ParseException(Res.InvalidType, errorPos);
		}
		return new TypeofExpression(typeExpression);
	}

	private Expression ParseStringLiteral()
	{
		ValidateToken(TokenId.StringLiteral);
		char quote = _token.text[0];
		string s = _token.text.Substring(1, _token.text.Length - 2);
		int start = 0;
		while (true)
		{
			int i = s.IndexOf(quote, start);
			if (i < 0)
			{
				break;
			}

			s = s.Remove(i, 1);
			start = i + 1;
		}
		NextToken();
		return CreateLiteral(s, s);
	}

	private Expression ParseIntegerLiteral()
	{
		ValidateToken(TokenId.IntegerLiteral);
		string text = _token.text;
		if (text[0] != '-')
		{
			if (!ulong.TryParse(text, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out ulong value))
			{
				throw new ParseException($"Invalid integer literal '{text}'", _token.pos);
			}

			NextToken();
			if (value <= int.MaxValue)
			{
				return CreateLiteral((int)value, text);
			}

			if (value <= uint.MaxValue)
			{
				return CreateLiteral((uint)value, text);
			}

			if (value <= long.MaxValue)
			{
				return CreateLiteral((long)value, text);
			}

			return CreateLiteral(value, text);
		}
		else
		{
			if (!long.TryParse(text, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out long value))
			{
				throw new ParseException($"Invalid integer literal '{text}'", _token.pos);
			}

			NextToken();
			if (value >= int.MinValue && value <= int.MaxValue)
			{
				return CreateLiteral((int)value, text);
			}

			return CreateLiteral(value, text);
		}
	}

	private Expression ParseRealLiteral()
	{
		ValidateToken(TokenId.RealLiteral);
		string text = _token.text;
		object? value = null;
		char last = text[text.Length - 1];
		if (last == 'F' || last == 'f')
		{
			if (float.TryParse(text.Substring(0, text.Length - 1), NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out float f))
			{
				value = f;
			}
		}
		else if (last == 'M' || last == 'm')
		{
			if (decimal.TryParse(text.Substring(0, text.Length - 1), NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out decimal d))
			{
				value = d;
			}
		}
		else
		{
			if (double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out double d))
			{
				value = d;
			}
		}
		if (value == null)
		{
			throw new ParseException($"Invalid real literal '{text}'", _token.pos);
		}

		NextToken();
		return CreateLiteral(value, text);
	}

	private Expression CreateLiteral(object value, string text)
	{
		var expr = new ConstantExpression(value);
		return expr;
	}

	private Expression ParseParenExpression()
	{
		ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
		NextToken();
		var e = ParseExpression();
		ValidateToken(TokenId.CloseParen, Res.CloseParenOrOperatorExpected);
		NextToken();
		if (e is TypeExpression te)
		{
			var expr = ParsePrimary();
			return new CastExpression(expr, te.Type);
		}
		return new ParenExpression(e);
	}

	private Expression ParseIdentifier()
	{
		ValidateToken(TokenId.Identifier);

		if (_token.text == "this")
		{
			NextToken();
			return _root;
		}

		if (_keywords.TryGetValue(_token.text, out var value))
		{
			NextToken();
			return value;
		}

		if (_token.text == "new")
		{
			return ParseNewExpression();
		}

		if (_token.text == "typeof")
		{
			return ParseTypeofExpression();
		}

		return ParseMemberAccess(_root);
	}

	private Expression ParseMemberAccess(Expression instance)
	{
		int errorPos = _token.pos;
		string id = GetIdentifier();
		NextToken();

		var type = instance.Type;
		if (type.Type.IsValueNullable())
		{
			type = type.GetGenericArguments()![0];
		}

		IMemberInfo member;
		TypeInfo memberType;
		var prop = type.Properties.FirstOrDefault(p => p.Definition.Name == id);
		if (prop != null)
		{
			member = prop;
			memberType = prop.PropertyType;
		}
		else
		{
			var field = type.Fields.FirstOrDefault(f => f.Definition.Name == id);
			if (field != null)
			{
				member = field;
				memberType = field.FieldType;
			}
			else
			{
				if (_token.id == TokenId.OpenParen)
				{
					// Try to find the first member method with the name in order to take argument types.
					// Afterwards the more suitable member or extension method will be found.
					var methodInfo = type.Methods.FirstOrDefault(m => m.Definition.Name == id);
					var argumentTypes = methodInfo?.Parameters.Select(p => p.ParameterType).ToList();

					var args = ParseArgumentList(argumentTypes);

					var method = FindMethod(type, id, args, errorPos);
					CorrectCharParameters(method, args, errorPos);
					CorrectNotNullableParameters(method, args);
					return new CallExpression(instance, method, args);
				}

				var method2 = type.Methods.FirstOrDefault(m => m.Definition.Name == id);
				if (method2 != null)
				{
					member = method2;
					memberType = new TypeInfo(TypeInfo.GetTypeThrow(typeof(Delegate)), false);
				}
				else
				{
					if (instance == _root)
					{
						if (_expectedType != null)
						{
							var staticFieldOrProp =
								_expectedType.Fields.Where(f => f.Definition.IsStatic).Cast<IMemberInfo>()
								.Concat(
									_expectedType.Properties.Where(p => p.Definition.IsStatic()))
								.FirstOrDefault(m => m.Definition.Name == id);
							if (staticFieldOrProp != null)
							{
								return new MemberExpression(new TypeExpression(_expectedType), staticFieldOrProp, staticFieldOrProp.MemberType);
							}
						}
					}

					if (instance == _root && _expectedType?.Type.Name == id)
					{
						return new TypeExpression(_expectedType);
					}

					if (type.Type.FullName.StartsWith("System.ValueTuple"))
					{
						var attrs = instance switch
						{
							MemberExpression me => me.Member.Definition.CustomAttributes,
							CallExpression ce => ce.Method.Definition.MethodReturnType.CustomAttributes,
							_ => null
						};

						var attr = attrs?.FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.TupleElementNamesAttribute");
						if (attr != null)
						{
							var names = ((CustomAttributeArgument[])attr.ConstructorArguments[0].Value).Select(a => a.Value).ToList();
							var index = names.IndexOf(id);
							if (index != -1)
							{
								var name = "Item" + (index + 1);
								var field2 = type.Fields.FirstOrDefault(f => f.Definition.Name == name);
								if (field2 != null)
								{
									member = field2;
									memberType = field2.FieldType;
									goto Label_CreateMemberExpression;
								}
							}
						}
					}

					if (_token.id == TokenId.Colon && instance == _root)
					{
						return ParseTypeExpression(id, errorPos);
					}

					throw new ParseException($"No property, field or method '{id}' exists in type '{type.Type.FullName}'", errorPos, id.Length);
				}
			}
		}
	Label_CreateMemberExpression:
		return new MemberExpression(instance, member, memberType);
	}

	private TypeExpression ParseTypeExpression(string prefix, int errorPos)
	{
		NextToken();
		string typeName = GetIdentifier();
		NextToken();

		var ns = _namespaces.FirstOrDefault(n => n.Prefix == prefix);
		if (ns == null)
		{
			throw new ParseException($"Namespace '{prefix}' is not defined.", errorPos);
		}
		if (ns.ClrNamespace == null)
		{
			throw new ParseException($"Namespace '{ns.Namespace.NamespaceName}' is not CLR-Namespace.", errorPos);
		}
		typeName = ns.ClrNamespace + '.' + typeName;
		var expressionType = TypeInfo.GetType(typeName);
		if (expressionType == null)
		{
			throw new ParseException($"Type not found: {typeName}", errorPos);
		}
		return new TypeExpression(expressionType);
	}

	private Expression[] ParseArgumentList(IList<TypeInfo>? argumentTypes = null)
	{
		ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
		NextToken();
		var args = _token.id != TokenId.CloseParen ? ParseArguments(argumentTypes) : new Expression[0];
		ValidateToken(TokenId.CloseParen, Res.CloseParenOrCommaExpected);
		NextToken();
		return args;
	}

	private Expression[] ParseArguments(IList<TypeInfo>? argumentTypes = null)
	{
		var savedExpectedType = _expectedType;
		var argList = new List<Expression>();
		for (int i = 0; ; i++)
		{
			_expectedType = i < argumentTypes?.Count ? argumentTypes[i] : null;
			argList.Add(ParseExpression());
			if (_token.id != TokenId.Comma)
			{
				break;
			}

			NextToken();
		}
		_expectedType = savedExpectedType;
		return argList.ToArray();
	}

	private Expression ParseElementAccess(Expression expr)
	{
		int errorPos = _token.pos;

		TypeInfo expressionType;
		IList<TypeInfo> argumentTypes;
		if (expr.Type.Type.IsArray)
		{
			expressionType = expr.Type.GetElementType()!;
			argumentTypes = new[] { TypeInfo.GetTypeThrow(typeof(int)) };
		}
		else
		{
			var prop = expr.Type.Properties.FirstOrDefault(p => p.Definition.Name == "Item");
			if (prop == null)
			{
				throw new ParseException($"No applicable indexer exists in type '{expr.Type.Type.FullName}'", errorPos);
			}
			expressionType = prop.PropertyType;
			var method = expr.Type.Methods.Single(m => m.Definition == prop.Definition.GetMethod);
			argumentTypes = method.Parameters.Select(p => p.ParameterType).ToList();
		}

		ValidateToken(TokenId.OpenBracket, Res.OpenParenExpected);
		NextToken();
		var args = ParseArguments(argumentTypes);
		ValidateToken(TokenId.CloseBracket, Res.CloseBracketOrCommaExpected);
		NextToken();
		return new ElementAccessExpression(expressionType, expr, args);
	}

	private void SetTextPos(int pos)
	{
		_textPos = pos;
		_ch = _textPos < _textLen ? _text[_textPos] : '\0';
	}

	private void NextChar()
	{
		if (_textPos < _textLen)
		{
			_textPos++;
		}

		_ch = _textPos < _textLen ? _text[_textPos] : '\0';
	}

	private void NextToken()
	{
		while (char.IsWhiteSpace(_ch))
		{
			NextChar();
		}

		TokenId t;
		int tokenPos = _textPos;
		switch (_ch)
		{
			case '!':
				NextChar();
				if (_ch == '=')
				{
					NextChar();
					t = TokenId.ExclamationEqual;
				}
				else
				{
					t = TokenId.Exclamation;
				}
				break;
			case '%':
				NextChar();
				t = TokenId.Percent;
				break;
			case '&':
				NextChar();
				if (_ch == '&')
				{
					NextChar();
					t = TokenId.DoubleAmphersand;
				}
				else
				{
					throw new ParseException(Res.OperatorNotSupported, _textPos);
				}
				break;
			case '(':
				NextChar();
				t = TokenId.OpenParen;
				break;
			case ')':
				NextChar();
				t = TokenId.CloseParen;
				break;
			case '*':
				NextChar();
				t = TokenId.Asterisk;
				break;
			case '+':
				NextChar();
				t = TokenId.Plus;
				break;
			case ',':
				NextChar();
				t = TokenId.Comma;
				break;
			case '-':
				NextChar();
				t = TokenId.Minus;
				break;
			case '.':
				NextChar();
				t = TokenId.Dot;
				break;
			case '/':
				NextChar();
				t = TokenId.Slash;
				break;
			case ':':
				NextChar();
				t = TokenId.Colon;
				break;
			case '<':
				NextChar();
				if (_ch == '=')
				{
					NextChar();
					t = TokenId.LessThanEqual;
				}
				else if (_ch == '>')
				{
					NextChar();
					t = TokenId.LessGreater;
				}
				else
				{
					t = TokenId.LessThan;
				}
				break;
			case '=':
				NextChar();
				if (_ch == '=')
				{
					NextChar();
					t = TokenId.DoubleEqual;
				}
				else
				{
					t = TokenId.Equal;
				}
				break;
			case '>':
				NextChar();
				if (_ch == '=')
				{
					NextChar();
					t = TokenId.GreaterThanEqual;
				}
				else
				{
					t = TokenId.GreaterThan;
				}
				break;
			case '?':
				NextChar();
				if (_ch == '?')
				{
					NextChar();
					t = TokenId.DoubleQuestion;
				}
				else
				{
					t = TokenId.Question;
				}
				break;
			case '[':
				NextChar();
				t = TokenId.OpenBracket;
				break;
			case ']':
				NextChar();
				t = TokenId.CloseBracket;
				break;
			case '|':
				NextChar();
				if (_ch == '|')
				{
					NextChar();
					t = TokenId.DoubleBar;
				}
				else
				{
					t = TokenId.Bar;
				}
				break;
			case '"':
			case '\'':
				char quote = _ch;
				do
				{
					NextChar();
					while (_textPos < _textLen && _ch != quote)
					{
						NextChar();
					}

					if (_textPos == _textLen)
					{
						throw new ParseException("Unterminated string literal", _textPos);
					}

					NextChar();
				} while (_ch == quote);
				t = TokenId.StringLiteral;
				break;
			default:
				if (char.IsLetter(_ch) || _ch == '_')
				{
					do
					{
						NextChar();
					} while (char.IsLetterOrDigit(_ch) || _ch == '_');
					t = TokenId.Identifier;
					break;
				}
				if (char.IsDigit(_ch))
				{
					t = TokenId.IntegerLiteral;
					do
					{
						NextChar();
					} while (char.IsDigit(_ch));
					if (_ch == '.')
					{
						t = TokenId.RealLiteral;
						NextChar();
						ValidateDigit();
						do
						{
							NextChar();
						} while (char.IsDigit(_ch));
					}
					if (_ch == 'E' || _ch == 'e')
					{
						t = TokenId.RealLiteral;
						NextChar();
						if (_ch == '+' || _ch == '-')
						{
							NextChar();
						}

						ValidateDigit();
						do
						{
							NextChar();
						} while (char.IsDigit(_ch));
					}
					if (_ch == 'F' || _ch == 'f')
					{
						NextChar();
					}

					break;
				}
				if (_textPos == _textLen)
				{
					t = TokenId.End;
					break;
				}
				throw new ParseException($"Syntax error '{_ch}'", _textPos);
		}

		_token.id = t;
		_token.text = _text.Substring(tokenPos, _textPos - tokenPos);
		_token.pos = tokenPos;
	}

	private bool TokenIdentifierIs(string id)
	{
		return _token.id == TokenId.Identifier && string.Equals(id, _token.text, StringComparison.OrdinalIgnoreCase);
	}

	private string GetIdentifier()
	{
		ValidateToken(TokenId.Identifier, Res.IdentifierExpected);
		string id = _token.text;
		return id;
	}

	private void ValidateDigit()
	{
		if (!char.IsDigit(_ch))
		{
			throw new ParseException("Digit expected", _textPos);
		}
	}

	private void ValidateNotMethodAccess(Expression expression)
	{
		if (expression is MemberExpression me && me.Member is MethodInfo)
		{
			throw new ParseException($"'{me.Expression.Type.Type.FullName}.{me.Member.Definition.Name}()' is a method, which is not valid in the given context.");
		}
	}

	private void ValidateToken(TokenId t, string errorMessage)
	{
		if (_token.id != t)
		{
			throw new ParseException(errorMessage, _token.pos, _token.text.Length);
		}
	}

	private void ValidateToken(TokenId t)
	{
		if (_token.id != t)
		{
			throw new ParseException(Res.SyntaxError);
		}
	}

	private static TypeInfo GetNullableUnderlyingType(TypeInfo type)
	{
		if (type.Type.IsValueNullable())
		{
			type = type.GetGenericArguments()![0];
		}
		return type;
	}

	private static class Res
	{
		public const string EmptyExpression = "Empty expression.";
		public const string ExpressionExpected = "Expression expected.";
		public const string SyntaxError = "Syntax error.";
		public const string ColonExpected = "':' expected.";
		public const string OpenParenExpected = "'(' expected.";
		public const string CloseParenOrOperatorExpected = "')' or operator expected.";
		public const string CloseParenOrCommaExpected = "')' or ',' expected.";
		public const string CloseBracketOrCommaExpected = "']' or ',' expected.";
		public const string IdentifierExpected = "Identifier expected.";
		public const string OperatorNotSupported = "Operator is not supported.";
		public const string InvalidType = "Invalid type.";
	}

	private struct Token
	{
		public TokenId id;
		public string text;
		public int pos;
	}

	private enum TokenId
	{
		Unknown,
		End,
		Identifier,
		StringLiteral,
		IntegerLiteral,
		RealLiteral,
		Exclamation,
		Percent,
		Amphersand,
		OpenParen,
		CloseParen,
		Asterisk,
		Plus,
		Comma,
		Minus,
		Dot,
		Slash,
		Colon,
		LessThan,
		Equal,
		GreaterThan,
		Question,
		DoubleQuestion,
		OpenBracket,
		CloseBracket,
		Bar,
		ExclamationEqual,
		DoubleAmphersand,
		LessThanEqual,
		LessGreater,
		DoubleEqual,
		GreaterThanEqual,
		DoubleBar
	}
}

