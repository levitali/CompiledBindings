using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CompiledBindings.Tests.XF;

[TestClass]
public class XFTests
{
	[TestMethod]
	public void Page1()
	{
		TestPage("Page1");
	}

	[TestMethod]
	public void Page2()
	{
		TestPage("Page2");
	}

	private void TestPage(string pageName)
	{
		int substr = "file:///".Length;
		TypeInfoUtils.LoadReferences(new string[]
		{
				typeof(object).Assembly.CodeBase.Substring(substr),
				typeof(INotifyPropertyChanged).Assembly.CodeBase.Substring(substr),
				Assembly.GetExecutingAssembly().CodeBase.Substring(substr)
		});
		try
		{
			var xamlFile = Path.Combine(Environment.CurrentDirectory, "XF", "Views", $"{pageName}.xml");
			var xdoc = XDocument.Load(xamlFile, LoadOptions.SetLineInfo);

			var xamlDomParser = new XFXamlDomParser();
			var parseResult = xamlDomParser.Parse(xamlFile, Path.GetFileName(xamlFile), xdoc);
			parseResult.Validate(xamlFile);

			var codeGenerator = new XFCodeGenerator("latest", "17.0.0");
			var code = codeGenerator.GenerateCode(parseResult);

			var csharpFile = Path.Combine(Environment.CurrentDirectory, "XF", "Views", $"{pageName}.xml.g.m.cs");
			var expectedCode = File.ReadAllText(csharpFile);

			Assert.AreEqual(code, expectedCode);
		}
		finally
		{
			TypeInfoUtils.Cleanup();
		}
	}
}

