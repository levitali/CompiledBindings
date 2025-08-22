using Mono.Cecil;

namespace CompiledBindings.Tests.XF;

public class XFTests : IDisposable
{
	public XFTests()
	{
		TypeInfoUtils.LoadReferences(new string[]
		{
			typeof(object).Assembly.Location,
			typeof(INotifyPropertyChanged).Assembly.Location,
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
		TestPage("Page1", true);
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

	[Test]
	public void Page4()
	{
		TestPage("Page4", true);
	}

	private void TestPage(string pageName, bool enableNullables)
	{
		TypeInfo.EnableNullables = enableNullables;

		var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
		var xamlFile = Path.Combine(dir, "XF", "Views", $"{pageName}.xml");
		var xdoc = XDocument.Load(xamlFile, LoadOptions.SetLineInfo);

		var xamlDomParser = new XFXamlDomParser(new PlatformConstants());
		var parseResult = xamlDomParser.Parse(xamlFile, Path.GetFileName(xamlFile), xdoc, null!);

		var codeGenerator = new XFCodeGenerator("CompiledBindings.XF", "latest", "17.0.0", new PlatformConstants());
		var code = codeGenerator.GenerateCode(parseResult!);

		var csharpFile = Path.Combine(dir, "XF", "Views", $"{pageName}.xml.g.m.cs");
		var expectedCode = File.ReadAllText(csharpFile);

		Assert.That(code.Equals(expectedCode));
		
		using var assembly = AssemblyDefinition.ReadAssembly(Assembly.GetExecutingAssembly().Location);
		var assemblyTypes = assembly.MainModule.Types.ToDictionary(_ => _.FullName);

		var xaml = File.ReadAllText(xamlFile);

		var newXaml = XFXamlProcessor.ProcessXaml(xaml, xamlDomParser, new PlatformConstants(), "CompiledBindings.XF", assemblyTypes);
		Assert.NotNull(newXaml);

		var processedFile = Path.Combine(dir, "XF", "Views", $"{pageName}.g.xml");
		var expectedXaml = File.ReadAllText(processedFile);

		Assert.That(newXaml.Equals(expectedXaml));
	}
}

