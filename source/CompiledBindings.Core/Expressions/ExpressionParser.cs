namespace CompiledBindings;

public class ExpressionParser
{
	private static readonly Dictionary<string, Expression> _keywords = new()
	{
		{ "true", new ConstantExpression(true) },
		{ "false", new ConstantExpression(false) },
		{ "null", Expression.NullExpression },
	};
	private readonly VariableExpression _root;
	private readonly IList<XamlNamespace> _namespaces;
	private readonly List<XamlNamespace> _includeNamespaces = new();
	private readonly string _text;
	private int _textPos;
	private readonly int _textLen;
	private char _ch;
	private Token _token;
	private TypeInfo? _expectedType;
	private bool _parsingInterpolatedString;

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

	public static Expression Parse(TypeInfo dataType, string member, string expression, TypeInfo resultType, bool validateEnd, IList<XamlNamespace> namespaces, out IList<XamlNamespace> includeNamespaces, out int textPos)
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

	// ?:, ??, as, is operators
	private Expression ParseExpression()
	{
		var expr = ParseLogicalOr();

		if (_token.id == TokenId.Question)
		{
			return ParseConditionalExpression(expr);
		}
		else if (_token.id == TokenId.DoubleQuestion)
		{
			return ParseCoalesceExpression(expr);
		}
		else if (TokenIdentifierIs("as"))
		{
			return ParseAsExpression(expr);
		}
		else if (TokenIdentifierIs("is"))
		{
			return ParseIsExpression(expr);
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
			if (_token.id is TokenId.Dot or TokenId.SlashDot or TokenId.BackslashDot)
			{
				bool? isNotifiable = _token.id switch { TokenId.BackslashDot => true, TokenId.SlashDot => false, _ => null };
				ValidateNotMethodAccess(expr);
				NextToken();
				expr = ParseMemberAccess(expr, isNotifiable);
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
			else if (!CheckLastParameterArray())
			{
				return false;
			}
			if (!CheckAssignable() && (!CheckLastParameterArray() || !CheckAssignable()))
			{
				return false;
			}

			bool CheckLastParameterArray()
			{
				var lastPrm = parameters[parameters.Count - 1];
				if (!lastPrm.Definition.CustomAttributes.Any(a => a.AttributeType.FullName == "System.ParamArrayAttribute"))
				{
					return false;
				}
				prmType = lastPrm.ParameterType.GetElementType()!;
				return true;
			}

			bool CheckAssignable()
			{
				return prmType.IsAssignableFrom(args[i].Type) ||
					(args[i] is ConstantExpression ce && ce.Value is string && prmType.Reference.FullName == "System.Char");
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

	private IEnumerable<(MethodInfo method, XamlNamespace? ns)> EnumerateMethods(TypeInfo type, string methodName)
	{
		type = GetNullableUnderlyingType(type);

		return type.Methods
			.Where(m => m.Definition.Name == methodName)
			.Select(m => (m, (XamlNamespace?)null))
			.Concat(
				_namespaces.SelectMany(ns =>
					TypeInfo.FindExtensionMethods(ns.ClrNamespace!, methodName, type)
					.Select(m => (m, (XamlNamespace?)ns))));
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
					throw new InvalidProgramException();
				}
				if (!pi.ParameterType.Reference.IsArray)
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
					throw new InvalidProgramException();
				}

				if (prmIndex == method.Parameters.Count - 1)
				{
					var prm = method.Parameters[prmIndex];
					if (prm.Definition.CustomAttributes.Any(a => a.AttributeType.FullName == "System.ParamArrayAttribute"))
					{
						if (!prm.ParameterType.Reference.IsArray)
						{
							throw new InvalidProgramException();
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
			_ => throw new ParseException(Res.ExpressionExpected, _token.pos),
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
		var args = ParseArgumentList();

		var method = typeExpr.Type.Constructors.FirstOrDefault(c => CheckMethodApplicable(c, args, false));
		if (method == null)
		{
			throw new ParseException($"No applicable constructor exists in type '{typeExpr.Type.Reference.FullName}'", errorPos);
		}

		CorrectCharParameters(method, args, false, errorPos);
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
			if (_token.id == TokenId.Comma || _token.id == TokenId.Colon)
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
			if (value is >= int.MinValue and <= int.MaxValue)
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

		return ParseMemberAccess(null, null);
	}

	private Expression ParseAsExpression(Expression expression)
	{
		NextToken();

		int errorPos = _token.pos;
		string prefix = GetIdentifier();

		NextToken();
		ValidateToken(TokenId.Colon, Res.ColonExpected);

		var typeExpr = ParseTypeExpression(prefix, errorPos);

		return new AsExpression(expression, typeExpr);
	}

	private Expression ParseIsExpression(Expression expression)
	{
		var savedExpectedType = _expectedType;
		_expectedType = expression.Type;

		var result = Parse1();

		_expectedType = savedExpectedType;
		return result;

		Expression Parse1()
		{
			NextToken();

			Expression result = null!;
			string? logicalOperand = null;

			while (true)
			{
				var expr = Parse2();

				if (result == null)
				{
					result = expr;
				}
				else
				{
					result = new BinaryExpression(result, expr, logicalOperand!);
				}

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

		Expression Parse2()
		{
			if (_token.id == TokenId.OpenParen)
			{
				var expr = Parse1();
				ValidateToken(TokenId.CloseParen, Res.CloseParenOrOperatorExpected);
				NextToken();
				return new ParenExpression(expr);
			}

			if (_token.id == TokenId.Exclamation || TokenIdentifierIs("not"))
			{
				NextToken();
				var expr = Parse2();

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
			if (right is TypeExpression te)
			{
				if (isComparisonToken)
				{
					throw new ParseException("Comparison operator cannot be used for type checking.", errPos);
				}
				return new IsExpression(expression, te);
			}

			return new BinaryExpression(expression, right, GetComparisonOperand(op));
		}
	}

	private Expression ParseMemberAccess(Expression? instance, bool? isNotifiable)
	{
		int errorPos = _token.pos;
		string id = GetIdentifier();
		NextToken();

		if (instance == null)
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

		var inst = instance ?? _root;

		var type = inst.Type;
		if (type.Reference.IsValueNullable())
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
					var enumerator = EnumerateMethods(type, id).GetEnumerator();

					MethodInfo method;
					XamlNamespace? ns;

					// Get the first method with the name in order to take argument types.
					// Afterwards the more suitable member or extension method will be found.
					GetNextMethod();

					var argumentTypes = method.Parameters.Select(p => p.ParameterType).ToList();
					var args = ParseArgumentList(argumentTypes);

					while (!CheckMethodApplicable(method, args, ns != null))
					{
						GetNextMethod();
					}

					if (ns != null)
					{
						_includeNamespaces.Add(ns);
					}

					CorrectCharParameters(method, args, ns != null, errorPos);
					CorrectNotNullableParameters(method, args);

					return new CallExpression(inst, method, args);

					void GetNextMethod()
					{
						if (!enumerator.MoveNext())
						{
							throw new ParseException($"No applicable method '{id}' exists in type '{type.Reference.FullName}'", errorPos, id.Length);
						}
						(method, ns) = enumerator.Current;
					}
				}

				var method2 = type.Methods.FirstOrDefault(m => m.Definition.Name == id);
				if (method2 != null)
				{
					member = method2;
					memberType = new TypeInfo(TypeInfo.GetTypeThrow(typeof(Delegate)), false);
				}
				else
				{
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

					if (instance == null)
					{
						if (_token.id == TokenId.Colon)
						{
							return ParseTypeExpression(id, errorPos);
						}
						if (_expectedType?.Reference.Name == id)
						{
							return new TypeExpression(_expectedType);
						}
					}

					throw new ParseException($"No property, field or method '{id}' exists in type '{type.Reference.FullName}'", errorPos, id.Length);
				}
			}
		}
Label_CreateMemberExpression:
		return new MemberExpression(inst, member, memberType, isNotifiable);
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
		if (expr.Type.Reference.IsArray)
		{
			expressionType = expr.Type.GetElementType()!;
			argumentTypes = new[] { TypeInfo.GetTypeThrow(typeof(int)) };
		}
		else
		{
			var prop = expr.Type.Properties.FirstOrDefault(p => p.Definition.Name == "Item");
			if (prop == null)
			{
				throw new ParseException($"No applicable indexer exists in type '{expr.Type.Reference.FullName}'", errorPos);
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
				if (_ch == '.')
				{
					NextChar();
					t = TokenId.SlashDot;
				}
				else
				{
					t = TokenId.Slash;
				}
				break;
			case '\\':
				NextChar();
				if (_ch == '.')
				{
					NextChar();
					t = TokenId.BackslashDot;
					break;
				}
				goto default;
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
				if (_ch is ('\'' or '"'))
				{
					t = TokenId.InterpolatedString;
				}
				else
				{
					t = TokenId.Dollar;
				}
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
		SlashDot,
		BackslashDot
	}
}

