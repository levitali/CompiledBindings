using Mono.Cecil;

namespace CompiledBindings.Tests.XF;

public class XFTests
{
	[Test]
	public void Page1()
	{
		TestCodeGeneration("Page1");
	}

	[Test]
	public void Page2()
	{
		TestCodeGeneration("Page2");
	}

	[Test]
	public void Page3()
	{
		TestCodeGeneration("Page3");
	}

	[Test]
	public void Page4()
	{
		TestProcessXaml("Page4");
	}

	private void TestCodeGeneration(string pageName)
	{
		TypeInfoUtils.LoadReferences(new string[]
		{
			typeof(object).Assembly.Location,
			typeof(INotifyPropertyChanged).Assembly.Location,
			Assembly.GetExecutingAssembly().Location
		});
		try
		{
			var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
			var xamlFile = Path.Combine(dir, "XF", "Views", $"{pageName}.xml");
			var xdoc = XDocument.Load(xamlFile, LoadOptions.SetLineInfo);

			var xamlDomParser = new XFXamlDomParser(new PlatformConstants());
			var parseResult = xamlDomParser.Parse(xamlFile, Path.GetFileName(xamlFile), xdoc, null!);

			var codeGenerator = new XFCodeGenerator("latest", "17.0.0", new PlatformConstants());
			var code = codeGenerator.GenerateCode(parseResult!);

			var csharpFile = Path.Combine(dir, "XF", "Views", $"{pageName}.xml.g.m.cs");
			var expectedCode = File.ReadAllText(csharpFile);

			Assert.That(code.Equals(expectedCode));
		}
		finally
		{
			TypeInfoUtils.Cleanup();
		}
	}

	private void TestProcessXaml(string pageName)
	{
		TypeInfoUtils.LoadReferences(new string[]
		{
			typeof(object).Assembly.Location,
			typeof(INotifyPropertyChanged).Assembly.Location,
			Assembly.GetExecutingAssembly().Location
		});
		using var assembly = AssemblyDefinition.ReadAssembly(Assembly.GetExecutingAssembly().Location);
		try
		{
			var assemblyTypes = assembly.MainModule.Types.ToDictionary(_ => _.FullName);

			var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
			var xamlFile = Path.Combine(dir, "XF", "Views", $"{pageName}.xml");
			var xaml = File.ReadAllText(xamlFile);

			var xamlDomParser = new XFXamlDomParser(new PlatformConstants());
			var newXaml = XFXamlProcessor.ProcessXaml(xaml, xamlDomParser, new PlatformConstants(), assemblyTypes, null);
			Assert.NotNull(newXaml);

			var processedFile = Path.Combine(dir, "XF", "Views", $"{pageName}.g.xml");
			var expectedXaml = File.ReadAllText(processedFile);

			Assert.That(newXaml.Equals(expectedXaml));
		}
		finally
		{
			TypeInfoUtils.Cleanup();
		}
	}
}

