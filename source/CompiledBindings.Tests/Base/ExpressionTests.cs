namespace CompiledBindings.Tests;

public class ExpressionTests : IDisposable
{
	public ExpressionTests()
	{
		TypeInfoUtils.LoadReferences(new string[]
		{
			typeof(string).Assembly.Location,
			Assembly.GetExecutingAssembly().Location
		});
	}

	public void Dispose()
	{
		TypeInfoUtils.Cleanup();
	}

	[Test]
	public void TestExpressions()
	{
		var class1Type = new TypeInfo(TypeInfo.GetTypeThrow(typeof(Class1)), false);
		var stringType = TypeInfo.GetTypeThrow(typeof(string));
		var intType = TypeInfo.GetTypeThrow(typeof(int));

		var ns = new[] { new XamlNamespace("local", "using:CompiledBindings.Tests") };

		string expression, expectedCode;
		Expression result;

		expression = "(ListProp.Count - NullIntProp).ToString() ?? '-'";
		expectedCode = "(dataRoot.ListProp?.Count - dataRoot.NullIntProp)?.ToString() ?? \"-\"";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, stringType, true, new XamlNamespace[0], out IList<XamlNamespace> dummyNamespaces, out int dummyPos);
		Assert.That(result.CSharpCode.Equals(expectedCode));

		var expression2 = @"(RefProp/.NullStructProp + RefProp\.StructProp).TestMethod()";
		var expectedCode2 = "(dataRoot.RefProp?.NullStructProp + dataRoot.RefProp?.StructProp)?.TestMethod()";
		var res2 = ExpressionParser.Parse(class1Type, "dataRoot", expression2, intType, true, new XamlNamespace[0], out dummyNamespaces, out dummyPos);
		Assert.That(res2.CSharpCode.Equals(expectedCode2));

		var expression3 = "RefProp.StringProp == 'C' ? 1 : 0";
		var expectedCode3 = "(dataRoot.RefProp?.StringProp == \"C\" ? 1 : 0)";
		var res3 = ExpressionParser.Parse(class1Type, "dataRoot", expression3, intType, true, new XamlNamespace[0], out dummyNamespaces, out dummyPos);
		Assert.That(res3.CSharpCode.Equals(expectedCode3));

		var expression4 = "local:Class1.Instance.StringProp";
		var expectedCode4 = "CompiledBindings.Tests.Class1.Instance.StringProp";
		var res4 = ExpressionParser.Parse(class1Type, "dataRoot", expression4, intType, true, ns, out dummyNamespaces, out dummyPos);
		Assert.That(res4.CSharpCode.Equals(expectedCode4));

		expression = "FuncProp()";
		expectedCode = "dataRoot.FuncProp()";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, intType, true, ns, out dummyNamespaces, out dummyPos);
		Assert.That(result.CSharpCode.Equals(expectedCode));

		expression = "FuncProp2('test').GuidProp";
		expectedCode = "dataRoot.FuncProp2?.Invoke(\"test\")?.GuidProp";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, intType, true, ns, out dummyNamespaces, out dummyPos);
		Assert.That(result.CSharpCode.Equals(expectedCode));

		expression = "local:Class1.Method1(typeof(local:Class2))";
		expectedCode = "CompiledBindings.Tests.Class1.Method1(typeof(global::CompiledBindings.Tests.Class2))";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, intType, true, ns, out dummyNamespaces, out dummyPos);
		Assert.That(result.CSharpCode.Equals(expectedCode));

		expression = "NullDateTimeProp.ToString('g')";
		expectedCode = "dataRoot.NullDateTimeProp?.ToString(\"g\")";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, intType, true, ns, out dummyNamespaces, out dummyPos);
		Assert.That(result.CSharpCode.Equals(expectedCode));

		expression = "$'{IntProp,2} {RefProp.DecimalProp:0.###}'";
		expectedCode = "$\"{dataRoot.IntProp,2} {dataRoot.RefProp?.DecimalProp:0.###}\"";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, intType, true, ns, out dummyNamespaces, out dummyPos);
		Assert.That(result.CSharpCode.Equals(expectedCode));

		expression = "((local:Class1)RefProp.ObjProp).Mode3 eq null";
		expectedCode = "(((global::CompiledBindings.Tests.Class1)dataRoot.RefProp?.ObjProp)) is var var1 && var1 != null ? var1.Mode3 == null : false";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, intType, true, ns, out dummyNamespaces, out dummyPos);
		result = new FallbackExpression(result, new ConstantExpression(false), "var1");
		Assert.That(result.CSharpCode.Equals(expectedCode));
	}

	[Test]
	public void TestIsExpression()
	{
		var class1Type = new TypeInfo(TypeInfo.GetTypeThrow(typeof(Class1)), false);
		var stringType = TypeInfo.GetTypeThrow(typeof(string));
		var intType = TypeInfo.GetTypeThrow(typeof(int));

		var ns = new[]
		{
			new XamlNamespace("system", "using:System"),
			new XamlNamespace("local", "using:CompiledBindings.Tests")
		};

		string expression, expectedCode;
		Expression result;

		expression = "IntProp is 0 or 1";
		expectedCode = "dataRoot.IntProp == 0 || dataRoot.IntProp == 1";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, stringType, true, new XamlNamespace[0], out var _, out var _);
		Assert.That(result.CSharpCode.Equals(expectedCode));

		expression = "IntProp is gt 0 and lt 10";
		expectedCode = "dataRoot.IntProp > 0 && dataRoot.IntProp < 10";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, stringType, true, new XamlNamespace[0], out var _, out var _);
		Assert.That(result.CSharpCode.Equals(expectedCode));

		expression = "IntProp is not 0 and lt 10";
		expectedCode = "dataRoot.IntProp != 0 && dataRoot.IntProp < 10";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, stringType, true, new XamlNamespace[0], out var _, out var _);
		Assert.That(result.CSharpCode.Equals(expectedCode));

		expression = "IntProp is not (0 or 10)";
		expectedCode = "!(dataRoot.IntProp == 0 || dataRoot.IntProp == 10)";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, stringType, true, new XamlNamespace[0], out var _, out var _);
		Assert.That(result.CSharpCode.Equals(expectedCode));


		expression = "IntProp is ge 0 and not NullIntProp";
		expectedCode = "dataRoot.IntProp >= 0 && dataRoot.IntProp != dataRoot.NullIntProp";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, stringType, true, new XamlNamespace[0], out var _, out var _);
		Assert.That(result.CSharpCode.Equals(expectedCode));

		expression = "(Mode is Mode1 or Mode2) and RefProp.DecimalProp ne 4";
		expectedCode = "(dataRoot.Mode == CompiledBindings.Tests.TestMode.Mode1 || dataRoot.Mode == CompiledBindings.Tests.TestMode.Mode2) && dataRoot.RefProp?.DecimalProp != 4";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, stringType, true, new XamlNamespace[0], out var _, out var _);
		Assert.That(result.CSharpCode.Equals(expectedCode));

		expression = "ListProp.Count is not > 0";
		expectedCode = "!(dataRoot.ListProp?.Count > 0)";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, stringType, true, new XamlNamespace[0], out var _, out var _);
		Assert.That(result.CSharpCode.Equals(expectedCode));

		expression = "ObjProp is system:String or not system:Int32";
		expectedCode = "dataRoot.ObjProp is global::System.String || !(dataRoot.ObjProp is global::System.Int32)";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, stringType, true, ns, out var _, out var _);
		Assert.That(result.CSharpCode.Equals(expectedCode));
	}

	[Test]
	public void ExpectedTypeAsMember()
	{
		var class1Type = new TypeInfo(TypeInfo.GetTypeThrow(typeof(Class1)), false);
		var stringType = TypeInfo.GetTypeThrow(typeof(string));
		var intType = TypeInfo.GetTypeThrow(typeof(int));

		var ns = new[]
		{
			new XamlNamespace("system", "using:System"),
			new XamlNamespace("local", "using:CompiledBindings.Tests")
		};

		string expression, expectedCode;
		Expression result;

		expression = "Mode eq Mode3";
		expectedCode = "dataRoot.Mode == CompiledBindings.Tests.TestMode.Mode3";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, stringType, true, new XamlNamespace[0], out var _, out var _);
		Assert.That(result.CSharpCode.Equals(expectedCode));

		expression = "Mode eq this.Mode3";
		expectedCode = "dataRoot.Mode == dataRoot.Mode3";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, stringType, true, new XamlNamespace[0], out var _, out var _);
		Assert.That(result.CSharpCode.Equals(expectedCode));

		expression = "Check(TestMode)";
		expectedCode = "dataRoot.Check(dataRoot.TestMode)";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, stringType, true, new XamlNamespace[0], out var _, out var _);
		Assert.That(result.CSharpCode.Equals(expectedCode));
	}
}
