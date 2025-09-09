﻿namespace CompiledBindings;

public class ExpressionParser
{
	private static readonly Dictionary<string, Expression> _constantKeywords = new()
	{
		{ "true", new ConstantExpression(true) },
		{ "false", new ConstantExpression(false) },
		{ "null", Expression.NullExpression },
	};
	private readonly VariableExpression _root;
	private readonly IList<XamlNamespace> _namespaces;
	private readonly HashSet<string> _clrNamespaces;
	private readonly HashSet<string> _includeNamespaces = [];
	private readonly string _text;
	private int _textPos;
	private readonly int _textLen;
	private char _ch;
	private Token _token;
	private TypeInfo? _expectedType;
	private bool _parsingInterpolatedString;
	private bool _parsingIs;

	private ExpressionParser(VariableExpression root, string expression, TypeInfo resultType, IList<XamlNamespace> namespaces)
	{
		_root = root;
		_namespaces = namespaces;
		_clrNamespaces = [.. EnumerableExtensions.AsEnumerable(root.Type.Reference.Namespace).Union(namespaces.Select(n => n.ClrNamespace!))];
		_text = expression;
		_textLen = _text.Length;
		_expectedType = resultType;
		_ch = _text[0];
		NextToken();
	}

	public static Expression Parse(TypeInfo dataType, string member, string expression, TypeInfo resultType, bool validateEnd, IList<XamlNamespace> namespaces, out ICollection<string> includeNamespaces, out int textPos)
	{
		if (string.IsNullOrWhiteSpace(expression))
		{
			throw new ParseException(Res.EmptyExpression);
		}

		var parser = new ExpressionParser(new VariableExpression(dataType, member), expression, resultType, namespaces);

		var res = parser.ParseExpression();
		if (!(parser._token.id == TokenId.End || (!validateEnd && parser._token.id == TokenId.Comma)))
		{
			throw new ParseException(Res.SyntaxError, parser._token.pos);
		}

		includeNamespaces = parser._includeNamespaces;
		textPos = parser._textPos;

		return res;
	}

	// ?:, ?? operators
	private Expression ParseExpression()
	{
		var expr = ParseAs();

		if (_token.id == TokenId.Question)
		{
			return ParseConditionalExpression(expr);
		}
		else if (_token.id == TokenId.DoubleQuestion)
		{
			return ParseCoalesceExpression(expr);
		}
		return expr;
	}

	private Expression ParseCoalesceExpression(Expression expression)
	{
		NextToken();
		var right = ParseExpression();
		return new CoalesceExpression(expression, right);
	}

	private Expression ParseConditionalExpression(Expression expression)
	{
		NextToken();
		var expr1 = ParseExpression();
		ValidateToken(TokenId.Colon, Res.ColonExpected);
		NextToken();
		var expr2 = ParseExpression();
		return new ConditionalExpression(expression, expr1, expr2);
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
			NextToken();
			var right = ParseComparison();
			ValidateNotMethodAccess(right);
			left = new BinaryExpression(left, right, "&&");
			_expectedType = savedExpectedType;
		}
		return left;
	}

	private bool IsCompasionToken()
	{
		return _token.id is TokenId.DoubleEqual or TokenId.ExclamationEqual or TokenId.LessGreater
			 or TokenId.GreaterThan or TokenId.GreaterThanEqual or TokenId.LessThan or TokenId.LessThanEqual ||
			ReplaceTokenIdentifier("gt", TokenId.GreaterThan) || ReplaceTokenIdentifier("ge", TokenId.GreaterThanEqual) ||
			ReplaceTokenIdentifier("lt", TokenId.LessThan) || ReplaceTokenIdentifier("le", TokenId.LessThanEqual) ||
			ReplaceTokenIdentifier("eq", TokenId.DoubleEqual) || ReplaceTokenIdentifier("ne", TokenId.ExclamationEqual);
	}

	private string GetComparisonOperand(TokenId id)
	{
		return id switch
		{
			TokenId.ExclamationEqual or TokenId.LessGreater => "!=",
			TokenId.GreaterThan => ">",
			TokenId.GreaterThanEqual => ">=",
			TokenId.LessThan => "<",
			TokenId.LessThanEqual => "<=",
			_ => "==",
		};
	}

	// =, ==, !=, <>, >, >=, <, <= operators
	private Expression ParseComparison()
	{
		var left = ParseAdditive();
		while (IsCompasionToken())
		{
			var savedExpectedType = _expectedType;
			_expectedType = GetNullableUnderlyingType(left.Type);
			ValidateNotMethodAccess(left);
			var op = _token.id;
			NextToken();
			var right = ParseAdditive();
			ValidateNotMethodAccess(right);
			_expectedType = savedExpectedType;
			left = new BinaryExpression(left, right, GetComparisonOperand(op));
		}
		return left;
	}

	private bool ReplaceTokenIdentifier(string identiferId, TokenId tokenId)
	{
		if (!TokenIdentifierIs(identiferId))
		{
			return false;
		}

		_token.id = tokenId;
		return true;
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
			var operand = op.id == TokenId.Plus ? "+" : "-";
			left = new BinaryExpression(left, right, operand);
		}
		return left;
	}

	// *, /, %, mod operators
	private Expression ParseMultiplicative()
	{
		var left = ParseIs();
		while (_token.id is TokenId.Asterisk or TokenId.Slash or TokenId.Percent || TokenIdentifierIs("mod"))
		{
			var savedExpectedType = _expectedType;
			_expectedType = GetNullableUnderlyingType(left.Type);
			ValidateNotMethodAccess(left);
			var op = _token;
			NextToken();
			var right = ParseIs();
			ValidateNotMethodAccess(right);
			_expectedType = savedExpectedType;
			var operand = op.id switch
			{
				TokenId.Percent => "%",
				TokenId.Asterisk => "*",
				_ => "/"
			};
			left = new BinaryExpression(left, right, operand);
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
			expr = new UnaryExpression(expr, op.id == TokenId.Minus ? "-" : "!");
			return expr;
		}
		return ParsePrimary();
	}

	private Expression ParsePrimary()
	{
		var expr = ParsePrimaryStart();
		while (true)
		{
			if (_token.id == TokenId.OpenParen)
			{
				expr = ParseInvoke(expr);
			}
			else
			{
				bool? isNotifiable = null;
				if (_token.id is TokenId.Slash or TokenId.Backslash)
				{
					isNotifiable = _token.id == TokenId.Backslash;
					NextToken();
				}
				if (_token.id == TokenId.Dot)
				{
					ValidateNotMethodAccess(expr);
					NextToken();
					expr = ParseMemberAccess(expr, isNotifiable);
				}
				else if (_token.id == TokenId.OpenBracket)
				{
					ValidateNotMethodAccess(expr);
					expr = ParseElementAccess(expr, isNotifiable);
				}
				else
				{
					break;
				}
			}
		}
		return expr;
	}

	private static bool IsExpressionAssignable(Expression expression, TypeInfo type)
	{
		if (type.IsAssignableFrom(expression.Type))
		{
			return true;
		}

		if (expression.Type.Reference.IsValueNullable() &&
			expression.Type.Reference.GetGenericArguments()![0].FullName == type.Reference.FullName)
		{
			return true;
		}

		if (expression.TypeDefiningExpression is not ConstantExpression ce)
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

	private bool CheckMethodApplicable(MethodInfo method, Expression[] args, bool isExtension)
	{
		var parameters = method.Parameters;
		int prmIndex = isExtension ? 1 : 0;

		if (parameters.Count == prmIndex && args.Length > 0)
		{
			return false;
		}

		for (int i = 0; i < args.Length; i++, prmIndex++)
		{
			TypeInfo prmType = null!;
			if (parameters.Count > prmIndex)
			{
				prmType = parameters[prmIndex].ParameterType;
			}
			else if (!checkLastParameterArray())
			{
				return false;
			}
			if (!checkAssignable() && (!checkLastParameterArray() || !checkAssignable()))
			{
				return false;
			}

			bool checkLastParameterArray()
			{
				var lastPrm = parameters[parameters.Count - 1];
				if (!lastPrm.Definition.CustomAttributes.Any(a => a.AttributeType.FullName == "System.ParamArrayAttribute"))
				{
					return false;
				}
				prmType = lastPrm.ParameterType.GetElementType()!;
				return true;
			}

			bool checkAssignable()
			{
				return IsExpressionAssignable(args[i], prmType);
			}
		}

		if (prmIndex < parameters.Count)
		{
			if (!parameters[prmIndex].Definition.IsOptional)
			{
				return false;
			}
			// Other parameters must also be optional
		}

		return true;
	}

	private IEnumerable<(MethodInfo method, string? ns)> EnumerateMethods(TypeInfo type, string methodName, bool isStatic)
	{
		type = GetNullableUnderlyingType(type);
		return type.EnumerateAllMethods(methodName, isStatic, _clrNamespaces);
	}

	private Expression ParseInvoke(Expression expr)
	{
		int errorPos = _token.pos;

		var delegateType = TypeInfo.GetTypeThrow(typeof(MulticastDelegate));
		if (!delegateType.IsAssignableFrom(expr.Type))
		{
			throw new ParseException($"The type '{expr.Type.Reference.Name}' is not a MulticastDelegate.", errorPos);
		}

		var method = expr.Type.Methods.Single(m => m.Definition.Name == "Invoke");
		var argumentTypes = method.Parameters.Select(p => p.ParameterType).ToList();

		var args = ParseArgumentList(argumentTypes);
		if (method.Parameters.Count < args.Length)
		{
			throw new ParseException($"Delegate '{expr.Type.Reference.Name}' does not take {args.Length} arguments.", errorPos);
		}

		CorrectCharParameters(method, args, false, errorPos);
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
					// Should never happen if the correct method was chosen.
					return;
				}
				if (!pi.ParameterType.Reference.IsArray)
				{
					// Should never happen if the correct method was chosen.
					return;
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

	private static void CorrectCharParameters(MethodInfo method, Expression[] args, bool isExtension, int errorPos)
	{
		bool? isParamsChar = null;
		for (int i = 0, prmIndex = isExtension ? 1 : 0; i < args.Length; i++, prmIndex++)
		{
			var arg = args[i];
			if (arg is ConstantExpression ce && ce.Value is string)
			{
				if (isParamsChar == null && prmIndex == method.Parameters.Count)
				{
					// Should never happen if the correct method was chosen.
					return;
				}

				if (prmIndex == method.Parameters.Count - 1)
				{
					var prm = method.Parameters[prmIndex];
					if (prm.Definition.CustomAttributes.Any(a => a.AttributeType.FullName == "System.ParamArrayAttribute"))
					{
						if (!prm.ParameterType.Reference.IsArray)
						{
							// Should never happen if the correct method was chosen.
							return;
						}

						isParamsChar = prm.ParameterType.Reference.GetElementType().FullName == "System.Char";
						if (isParamsChar == false)
						{
							return;
						}
					}
				}
				if (isParamsChar == true || method.Parameters[prmIndex].ParameterType.Reference.FullName == "System.Char")
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
		return _token.id switch
		{
			TokenId.Identifier => ParseIdentifier(),
			TokenId.Dollar => ParseStaticResource(),
			TokenId.InterpolatedString => ParseInterpolatedString(),
			TokenId.StringLiteral => ParseStringLiteral(),
			TokenId.IntegerLiteral => ParseIntegerLiteral(),
			TokenId.RealLiteral => ParseRealLiteral(),
			TokenId.OpenParen => ParseParenExpression(),
			_ => throw new ParseException($"Invalid expression term {_token.text}", _token.pos),
		};
	}

	private Expression ParseNewExpression()
	{
		NextToken();

		int errorPos = _token.pos;
		string prefix = GetIdentifier();

		NextToken();
		ValidateToken(TokenId.Colon, Res.ColonExpected);

		var typeExpr = ParseTypeExpression(prefix, errorPos);

		// Get the first constructor for argument types.
		// Later more suitable constructor will be taken
		var ctor = typeExpr.Type.Constructors.FirstOrDefault();
		if (ctor == null)
		{
			throw new ParseException($"No constructor exists in type '{typeExpr.Type.Reference.FullName}'", errorPos);
		}
		var argumentTypes = ctor.Parameters.Select(p => p.ParameterType).ToList();
		var args = ParseArgumentList(argumentTypes);

		// Try to find contructor for the argments.
		foreach (var ctor2 in typeExpr.Type.Constructors)
		{
			if (CheckMethodApplicable(ctor2, args, false))
			{
				ctor = ctor2;
				break;
			}
		}

		CorrectCharParameters(ctor, args, false, errorPos);
		CorrectNotNullableParameters(ctor, args);

		return new NewExpression(typeExpr, args);
	}

	private Expression ParseTypeofExpression()
	{
		NextToken();
		int errorPos = _token.pos;
		var args = ParseArgumentList(new[] { TypeInfo.GetTypeThrow(typeof(Type)) });
		if (args.Length != 1 || args[0] is not TypeExpression typeExpression)
		{
			throw new ParseException(Res.InvalidType, errorPos);
		}
		return new TypeofExpression(typeExpression);
	}

	private Expression ParseStaticResource()
	{
		NextToken();
		string id = GetIdentifier();
		NextToken();
		return new StaticResourceExpression(id, _expectedType ?? TypeInfo.GetTypeThrow(typeof(object)));
	}

	private Expression ParseInterpolatedString()
	{
		var errorPos = _textPos;
		var quote = _text[_textPos];

		int endPos;
		var expressions = new List<Expression>();
		// Start the format string
		var format = "$\"";

		for (int i = 0; ; i++)
		{
			// Find the end quote symbol
			_textPos++;
			endPos = _text.IndexOf(quote, _textPos);
			if (endPos == -1)
			{
				throw new ParseException(Res.UnterminatedStringLiteral, errorPos);
			}

			// Find the start of interpolated expression
			var pos = _text.IndexOf('{', _textPos);
			if (endPos < pos || pos == -1)
			{
				break;
			}

			// Add the text before the interpolated expression to the format
			format += _text.Substring(_textPos, pos - _textPos);
			// Add double { for generating single one in the C# code
			format += "{{";
			// Add placeholder for setting generated C# expression
			format += "{" + i + "}";

			// Prepare parsing the interpolated expression
			_textPos = pos;
			NextChar();
			NextToken();

			// Parse the expression
			_parsingInterpolatedString = true;
			var expression = ParseExpression();
			expressions.Add(expression);
			_parsingInterpolatedString = false;

			// If the current token is comma or colon, there is an alignment and/or format part
			if (_token.id is TokenId.Comma or TokenId.Colon)
			{
				// Find the closing }
				pos = _text.IndexOf('}', _textPos);
				if (pos == -1)
				{
					throw new ParseException(Res.CloseBracketExpected, _textPos);
				}
				// Ensure that there is no openning { before
				var pos2 = _text.IndexOf('{', _textPos);
				if (pos2 != -1 && pos2 < pos)
				{
					throw new ParseException(Res.CloseBracketExpected, pos2);
				}
				// Add the format part
				format += (_token.id == TokenId.Comma ? "," : ":") + _text.Substring(_textPos, pos - _textPos);
				_textPos = pos;
			}
			else if (_token.id != TokenId.End || _ch != '}')
			{
				throw new ParseException(Res.CloseBracketExpected, _token.pos);
			}
			// Add closing } for generated C# code
			format += "}}";
		}

		// Add possible text after the last interpolated expression
		format += _text.Substring(_textPos, endPos - _textPos);
		// Close the format string
		format += "\"";

		// Prepare parsing next expression
		_textPos = endPos;
		NextChar();
		NextToken();

		// The resulting format can still be invalid.
		// Check it now by trying to generate C# code.
		// Checking now will give the correct error position.
		var result = new InterpolatedStringExpression(format, expressions);
		try
		{
			var test = result.CSharpCode;
		}
		catch (FormatException)
		{
			throw new ParseException(Res.IncorrectInterpolatedString, errorPos);
		}
		return result;
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
		return new ConstantExpression(s);
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
			return new ConstantExpression(value);
		}
		else
		{
			if (!long.TryParse(text, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out long value))
			{
				throw new ParseException($"Invalid integer literal '{text}'", _token.pos);
			}

			NextToken();
			return new ConstantExpression(value);
		}
	}

	private Expression ParseRealLiteral()
	{
		ValidateToken(TokenId.RealLiteral);
		string text = _token.text;
		object? value = null;
		char last = text[text.Length - 1];
		if (last is 'F' or 'f')
		{
			if (float.TryParse(text.Substring(0, text.Length - 1), NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out float f))
			{
				value = f;
			}
		}
		else if (last is 'M' or 'm')
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
		return new ConstantExpression(value);
	}

	private Expression ParseParenExpression()
	{
		ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
		NextToken();
		var e = ParseExpression();
		ValidateToken(TokenId.CloseParen, Res.CloseParenOrOperatorExpected);
		NextToken();
		if (e is AsExpression)
		{
			return e;
		}
		else if (e is TypeExpression te)
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

		if (_constantKeywords.TryGetValue(_token.text, out var value))
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

		return ParseMemberAccess(null, null);
	}

	private Expression ParseAs()
	{
		var expression = ParseLogicalOr();
		while (TokenIdentifierIs("as"))
		{
			NextToken();

			int errorPos = _token.pos;
			string prefix = GetIdentifier();

			NextToken();
			ValidateToken(TokenId.Colon, Res.ColonExpected);

			var typeExpr = ParseTypeExpression(prefix, errorPos);
			var type = typeExpr.Type;
			if (type.Reference.IsValueType && !type.Reference.IsValueNullable())
			{
				type = TypeInfo.GetTypeThrow(typeof(Nullable<>)).MakeGenericInstanceType(type);
				typeExpr = new TypeExpression(type);
			}

			expression = new AsExpression(expression, typeExpr);
		}
		return expression;
	}

	private Expression ParseIs()
	{
		var expression = ParseUnary();
		while (!_parsingIs && TokenIdentifierIs("is"))
		{
			_parsingIs = true;
			var savedExpectedType = _expectedType;
			_expectedType = GetNullableUnderlyingType(expression.Type);

			expression = parse1();
			if (expression is not ParenExpression)
			{
				expression = new ParenExpression(expression);
			}

			_parsingIs = false;
			_expectedType = savedExpectedType;

			Expression parse1()
			{
				NextToken();

				Expression result = null!;
				string logicalOperand = null!;

				while (true)
				{
					var expr = parse2();

					result = result == null ? expr : new BinaryExpression(result, expr, logicalOperand);

					if (_token.id != TokenId.DoubleAmphersand &&
						 !ReplaceTokenIdentifier("and", TokenId.DoubleAmphersand) &&
						 _token.id != TokenId.DoubleBar &&
						 !ReplaceTokenIdentifier("or", TokenId.DoubleBar))
					{
						break;
					}

					logicalOperand = _token.id == TokenId.DoubleAmphersand ? "&&" : "||";
					NextToken();
				}

				return result;
			}

			Expression parse2()
			{
				if (_token.id == TokenId.OpenParen)
				{
					_parsingIs = false;
					var expr = parse1();
					ValidateToken(TokenId.CloseParen, Res.CloseParenOrOperatorExpected);
					NextToken();
					_parsingIs = true;
					return new ParenExpression(expr);
				}

				if (_token.id == TokenId.Exclamation || TokenIdentifierIs("not"))
				{
					NextToken();
					var expr = parse2();

					if (expr is BinaryExpression be && be.Operand == "==")
					{
						return new BinaryExpression(be.Left, be.Right, "!=");
					}
					if (expr is not ParenExpression)
					{
						expr = new ParenExpression(expr);
					}
					return new UnaryExpression(expr, "!");
				}

				var errPos = _token.pos;
				bool isComparisonToken;
				TokenId op;
				if (isComparisonToken = IsCompasionToken())
				{
					op = _token.id;
					NextToken();
				}
				else
				{
					op = TokenId.DoubleEqual;
				}

				var right = ParseComparison();
				return right is TypeExpression te
					? isComparisonToken
						? throw new ParseException("Comparison operator cannot be used for type checking.", errPos)
						: new IsExpression(expression, te)
					: new BinaryExpression(expression, right, GetComparisonOperand(op));
			}

		}
		return expression;
	}

	private Expression ParseMemberAccess(Expression? instance, bool? isNotifiable)
	{
		int errorPos = _token.pos;
		string id = GetIdentifier();
		NextToken();

		if (instance == null)
		{
			// The static members of an expected type have priority.
			// If there a member of the data root type with the same name,
			// it's possible to force using it by the "this" keyword.

			if (_expectedType != null)
			{
				IMemberInfo? staticFieldOrProp =
					_expectedType.Fields.FirstOrDefault(f => f.Definition.Name == id && f.Definition.IsStatic);
				if (staticFieldOrProp == null)
				{
					var (prop2, ns2) = _expectedType.FindProperty(id, true, _clrNamespaces);
					if (ns2 != null)
					{
						_includeNamespaces.Add(ns2);
					}
					staticFieldOrProp = prop2;
				}
				if (staticFieldOrProp != null)
				{
					return new MemberExpression(new TypeExpression(_expectedType), staticFieldOrProp, staticFieldOrProp.MemberType);
				}
			}
		}

		var inst = instance ?? _root;
		var isStatic = instance is TypeExpression;

		var type = inst.Type;
		if (type.Reference.IsValueNullable())
		{
			type = type.GetGenericArguments()![0];
		}

		IMemberInfo member;
		TypeInfo memberType;

		// Try to find a property
		var (prop, ns) = type.FindProperty(id, isStatic, _clrNamespaces);
		if (prop != null)
		{
			member = prop;
			memberType = prop.PropertyType;
			if (ns != null)
			{
				_includeNamespaces.Add(ns);
			}
		}
		else
		{
			// Tryp to find a field
			var field = type.Fields.FirstOrDefault(f => f.Definition.Name == id);
			if (field != null)
			{
				member = field;
				memberType = field.FieldType;
			}
			else
			{
				// If the next token after the member name is the open paren,
				// it must be a method call.
				if (_token.id == TokenId.OpenParen)
				{
					return ParseCall(inst, type, id, errorPos, isNotifiable);
				}

				// The expression can still be a method without parens (delegate).
				(var method, ns) = type.EnumerateAllMethods(id, isStatic, _clrNamespaces).FirstOrDefault();
				if (method != null)
				{
					member = method;
					memberType = new TypeInfo(TypeInfo.GetTypeThrow(typeof(Delegate)), false);
					if (ns != null)
					{
						_includeNamespaces.Add(ns);
					}
				}
				else
				{
					// If the type is a ValueTuple, try to find its elements by names
					if (type.Reference.FullName.StartsWith("System.ValueTuple"))
					{
						var attrs = inst switch
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
								var itemName = "Item" + (index + 1);
								var field2 = type.Fields.FirstOrDefault(f => f.Definition.Name == itemName);
								if (field2 != null)
								{
									member = field2;
									memberType = field2.FieldType;
									goto Label_CreateMemberExpression;
								}
							}
						}
					}

					if (instance == null)
					{
						// If it's not a member access (instance is null),
						// and the next token is colon,
						// the id must be a namespace prefix.
						if (_token.id == TokenId.Colon)
						{
							return ParseTypeExpression(id, errorPos);
						}

						// Allow using name of the expected type without namespace prefix.
						// Note! For static properties and fields of the expected type
						// the type name is optional.						
						if (_expectedType?.Reference.Name == id)
						{
							return new TypeExpression(_expectedType);
						}
					}

					// The id can be name of a nested type
					else if (instance is TypeExpression)
					{
						var nestedType = type.NestedTypes.FirstOrDefault(t => t.Reference.Name == id);
						if (nestedType != null)
						{
							return new TypeExpression(nestedType);
						}
					}

					throw new ParseException($"No property, field or method '{id}' exists in type '{type.Reference.FullName}'", errorPos, id.Length);
				}
			}
		}
Label_CreateMemberExpression:
		return new MemberExpression(inst, member, memberType, isNotifiable);
	}

	private Expression ParseCall(Expression inst, TypeInfo type, string methodName, int errorPos, bool? isNotifiable)
	{
		bool isStatic = inst is TypeExpression;
		var enumerator = EnumerateMethods(type, methodName, isStatic).GetEnumerator();

		MethodInfo method;
		string? ns;

		// Get the first method with the name in order to take argument types.
		// Afterwards the more suitable member or extension method will be found.
		getNextMethod();

		var argumentTypes = method.Parameters.Select(p => p.ParameterType).ToList();
		var args = ParseArgumentList(argumentTypes);

		while (!CheckMethodApplicable(method, args, ns != null))
		{
			getNextMethod();
		}

		if (ns != null)
		{
			_includeNamespaces.Add(ns);
		}

		CorrectCharParameters(method, args, ns != null, errorPos);
		CorrectNotNullableParameters(method, args);

		return new CallExpression(inst, method, args, isNotifiable);

		void getNextMethod()
		{
			if (!enumerator.MoveNext())
			{
				throw new ParseException($"No applicable method '{methodName}' exists in type '{type.Reference.FullName}'", errorPos, methodName.Length);
			}
			(method, ns) = enumerator.Current;
		}
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
			_expectedType = i < argumentTypes?.Count ? GetNullableUnderlyingType(argumentTypes[i]) : null;
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

	private Expression ParseElementAccess(Expression expr, bool? isNotifiable)
	{
		int errorPos = _token.pos;

		TypeInfo expressionType;
		IList<TypeInfo> argumentTypes;
		IList<PropertyInfo>? indexerProperties;
		PropertyInfo? indexerProperty;

		if (expr.Type.Reference.IsArray)
		{
			expressionType = expr.Type.GetElementType()!;
			argumentTypes = new[] { TypeInfo.GetTypeThrow(typeof(int)) };
			indexerProperties = null;
			indexerProperty = null;
		}
		else
		{
			var indexerName = expr.Type.GetIndexerName();
			if (indexerName == null)
			{
				ThrowException();
			}
			// Get the indexer property with the getter.
			// Note, there can be indexer property only with setter.
			indexerProperties = expr.Type.Properties
				.Where(p => p.Definition.GetMethod != null && p.Definition.Name == indexerName)
				.ToList();
			if (indexerProperties.Count == 0)
			{
				ThrowException();
			}

			// Use first indexer for argument types.
			// Later more applicable property will be found.
			indexerProperty = indexerProperties[0];
			expressionType = indexerProperty.PropertyType;
			var method = expr.Type.Methods.Single(m => m.Definition == indexerProperty.Definition.GetMethod);
			argumentTypes = method.Parameters.Select(p => p.ParameterType).ToList();
		}

		ValidateToken(TokenId.OpenBracket, Res.OpenParenExpected);
		NextToken();
		var args = ParseArguments(argumentTypes);
		ValidateToken(TokenId.CloseBracket, Res.CloseBracketOrCommaExpected);
		NextToken();

		if (indexerProperties?.Count > 1)
		{
			// Try to find more applicable indexer.
			// Note! Even if no applicable indexer is found,
			// still the first one ist use.
			// If it's not correct, there will be an error in the generated C# code,
			// which will refer to the position in XAML.
			foreach (var prop in indexerProperties)
			{
				var method = expr.Type.Methods.Single(m => m.Definition == prop.Definition.GetMethod);
				if (CheckMethodApplicable(method, args, false))
				{
					indexerProperty = prop;
					expressionType = prop.PropertyType;
					break;
				}
			}
		}

		return new ElementAccessExpression(expressionType, expr, args, indexerProperty, isNotifiable);

		void ThrowException()
		{
			throw new ParseException($"No applicable indexer exists in type '{expr.Type.Reference.FullName}'", errorPos);
		}
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
			case '\\':
				NextChar();
				t = TokenId.Backslash;
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
						if (_parsingInterpolatedString)
						{
							throw new ParseException(Res.CloseBracketExpected, tokenPos);
						}
						else
						{
							throw new ParseException(Res.UnterminatedStringLiteral, _textPos);
						}
					}

					NextChar();
				} while (_ch == quote);
				t = TokenId.StringLiteral;
				break;
			case '$':
				NextChar();
				t = _ch is '\'' or '"' ? TokenId.InterpolatedString : TokenId.Dollar;
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
					if (_ch is 'E' or 'e')
					{
						t = TokenId.RealLiteral;
						NextChar();
						if (_ch is '+' or '-')
						{
							NextChar();
						}

						ValidateDigit();
						do
						{
							NextChar();
						} while (char.IsDigit(_ch));
					}
					if (_ch is 'F' or 'f')
					{
						NextChar();
					}

					break;
				}
				if (_textPos == _textLen || (_parsingInterpolatedString && _ch == '}'))
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
			throw new ParseException($"'{me.Expression.Type.Reference.FullName}.{me.Member.Definition.Name}()' is a method, which is not valid in the given context.");
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
		if (type.Reference.IsValueNullable())
		{
			type = type.GetGenericArguments()![0];
		}
		return type;
	}

	private static class Res
	{
		public const string EmptyExpression = "Empty expression.";
		public const string SyntaxError = "Syntax error.";
		public const string ColonExpected = "':' expected.";
		public const string OpenParenExpected = "'(' expected.";
		public const string CloseParenOrOperatorExpected = "')' or operator expected.";
		public const string CloseParenOrCommaExpected = "')' or ',' expected.";
		public const string CloseBracketOrCommaExpected = "']' or ',' expected.";
		public const string IdentifierExpected = "Identifier expected.";
		public const string OperatorNotSupported = "Operator is not supported.";
		public const string InvalidType = "Invalid type.";
		public const string UnterminatedStringLiteral = "Unterminated string literal.";
		public const string CloseBracketExpected = "Exprected }";
		public const string IncorrectInterpolatedString = "Incorrect interpolated string.";
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
		Backslash,
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
		DoubleBar,
		Dollar,
		InterpolatedString,
	}
}
