﻿using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CompiledBindings.Tests;

[TestClass]
public class ExpressionTests
{
	[TestMethod]
	public void TestExpressions()
	{
		int substr = "file:///".Length;
		TypeInfoUtils.LoadReferences(new string[]
		{
				typeof(string).Assembly.CodeBase.Substring(substr),
				Assembly.GetExecutingAssembly().CodeBase.Substring(substr)
		});

		var class1Type = new TypeInfo(TypeInfoUtils.GetTypeThrow(typeof(Class1)), false);
		var stringType = TypeInfoUtils.GetTypeThrow(typeof(string));
		var intType = TypeInfoUtils.GetTypeThrow(typeof(int));
		var boolType = TypeInfoUtils.GetTypeThrow(typeof(bool));

		var ns = new[] { new XamlNamespace("local", "using:CompiledBindings.Tests") };

		string expression, expectedCode;
		Expression result;
		IList<XamlNamespace> dummyNamespaces;
		int dummyPos;

		expression = "(ListProp.Count - NullIntProp).ToString() ?? '-'";
		expectedCode = "(dataRoot.ListProp?.Count - dataRoot.NullIntProp)?.ToString() ?? \"-\"";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, stringType, true, new XamlNamespace[0], out dummyNamespaces, out dummyPos);
		Assert.AreEqual(result.ToString(), expectedCode);

		var expression2 = "(RefProp.NullStructProp + RefProp.StructProp).TestMethod()";
		var expectedCode2 = "(dataRoot.RefProp?.NullStructProp + dataRoot.RefProp?.StructProp)?.TestMethod()";
		var res2 = ExpressionParser.Parse(class1Type, "dataRoot", expression2, intType, true, new XamlNamespace[0], out dummyNamespaces, out dummyPos);
		Assert.AreEqual(res2.ToString(), expectedCode2);

		var expression3 = "RefProp.StringProp == 'C' ? 1 : 0";
		var expectedCode3 = "(dataRoot.RefProp?.StringProp == \"C\" ? 1 : 0)";
		var res3 = ExpressionParser.Parse(class1Type, "dataRoot", expression3, intType, true, new XamlNamespace[0], out dummyNamespaces, out dummyPos);
		Assert.AreEqual(res3.ToString(), expectedCode3);

		var expression4 = "local:Class1.Instance.StringProp";
		var expectedCode4 = "CompiledBindings.Tests.Class1.Instance.StringProp";
		var res4 = ExpressionParser.Parse(class1Type, "dataRoot", expression4, intType, true, ns, out dummyNamespaces, out dummyPos);
		Assert.AreEqual(res4.ToString(), expectedCode4);

		expression = "FuncProp()";
		expectedCode = "dataRoot.FuncProp()";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, intType, true, ns, out dummyNamespaces, out dummyPos);
		Assert.AreEqual(result.ToString(), expectedCode);

		expression = "FuncProp2('test').GuidProp";
		expectedCode = "dataRoot.FuncProp2(\"test\")?.GuidProp";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, intType, true, ns, out dummyNamespaces, out dummyPos);
		Assert.AreEqual(result.ToString(), expectedCode);

		expression = "local:Class1.Method1(typeof(local:Class2))";
		expectedCode = "CompiledBindings.Tests.Class1.Method1(typeof(global::CompiledBindings.Tests.Class2))";
		result = ExpressionParser.Parse(class1Type, "dataRoot", expression, intType, true, ns, out dummyNamespaces, out dummyPos);
		Assert.AreEqual(result.ToString(), expectedCode);
	}
}