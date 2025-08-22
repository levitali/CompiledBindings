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
		TestPage("Page1");
	}

	[Test]
	public void Page2()
	{
		TestPage("Page2");
	}

	[Test]
	public void Page3()
	{
		TestPage("Page3");
	}

	[Test]
	public void Page4()
	{
		TestPage("Page4");
	}

	private void TestPage(string pageName)
	{
		var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
		var xamlFile = Path.Combine(dir, "WPF", "Views", $"{pageName}.xml");
		var xdoc = XDocument.Load(xamlFile, LoadOptions.SetLineInfo);

		var xamlDomParser = new WpfXamlDomParser();
		var parseResult = xamlDomParser.Parse(xamlFile, Path.GetFileName(xamlFile), xdoc, null!);

		var codeGenerator = new WpfCodeGenerator("CompiledBindings.WPF", "latest", "17.0.0");
		var code = codeGenerator.GenerateCode(parseResult!);

		var csharpFile = Path.Combine(dir, "WPF", "Views", $"{pageName}.xml.g.m.cs");
		var expectedCode = File.ReadAllText(csharpFile);

		Assert.That(code.Equals(expectedCode));

		WpfXamlProcessor.ProcessXaml(xdoc, parseResult!, "CompiledBindings.WPF");

		var gFile = Path.Combine(dir, "WPF", "Views", $"{pageName}.g.xml");
		var expectedXaml = File.ReadAllText(gFile);

		Assert.That(xdoc.ToString().Equals(expectedXaml));
	}
}

