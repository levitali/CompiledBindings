namespace CompiledBindings.Tests.XF;

public class XFTests
{
	[Test]
	public void Page1()
	{
		TestPage("Page1");
	}

	[Test]
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
			var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var xamlFile = Path.Combine(dir, "XF", "Views", $"{pageName}.xml");
			var xdoc = XDocument.Load(xamlFile, LoadOptions.SetLineInfo);

			var xamlDomParser = new XFXamlDomParser(new PlatformConstants());
			var parseResult = xamlDomParser.Parse(xamlFile, Path.GetFileName(xamlFile), xdoc);
			parseResult.Validate(xamlFile);

			var codeGenerator = new XFCodeGenerator("latest", "17.0.0", new PlatformConstants());
			var code = codeGenerator.GenerateCode(parseResult);

			var csharpFile = Path.Combine(dir, "XF", "Views", $"{pageName}.xml.g.m.cs");
			var expectedCode = File.ReadAllText(csharpFile);

			Assert.AreEqual(code, expectedCode);
		}
		finally
		{
			TypeInfoUtils.Cleanup();
		}
	}
}

