namespace CompiledBindings.Tests.WPF;

public class WPFTests : IDisposable
{
	public WPFTests()
	{
		TypeInfoUtils.LoadReferences(new string[]
		{
			typeof(object).Assembly.Location,
			typeof(INotifyPropertyChanged).Assembly.Location,
			typeof(System.Windows.Controls.Control).Assembly.Location,
			typeof(System.Windows.UIElement).Assembly.Location,
			typeof(System.Windows.DependencyObject).Assembly.Location,
			Assembly.GetExecutingAssembly().Location
		});
	}

	public void Dispose()
	{
		TypeInfoUtils.Cleanup();
	}

	[Test]
	public void Page1()
	{
		TestPage("Page1", false);
	}

	[Test]
	public void Page2()
	{
		TestPage("Page2", false);
	}

	[Test]
	public void Page3()
	{
		TestPage("Page3", true);
	}

	private void TestPage(string pageName, bool testXaml)
	{
		var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
		var xamlFile = Path.Combine(dir, "WPF", "Views", $"{pageName}.xml");
		var xdoc = XDocument.Load(xamlFile, LoadOptions.SetLineInfo);

		var xamlDomParser = new WpfXamlDomParser();
		var parseResult = xamlDomParser.Parse(xamlFile, Path.GetFileName(xamlFile), xdoc, null!);

		var codeGenerator = new WpfCodeGenerator("latest", "17.0.0");
		var code = codeGenerator.GenerateCode(parseResult!);

		var csharpFile = Path.Combine(dir, "WPF", "Views", $"{pageName}.xml.g.m.cs");
		var expectedCode = File.ReadAllText(csharpFile);

		Assert.That(code.Equals(expectedCode));

		if (testXaml)
		{
			WpfXamlProcessor.ProcessXaml(xdoc, parseResult!, null);
			
			var gFile = Path.Combine(dir, "WPF", "Views", $"{pageName}.g.xml");
			var expectedXaml = File.ReadAllText(gFile);

			Assert.That(xdoc.ToString().Equals(expectedXaml));
		}
	}
}

