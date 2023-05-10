using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CompiledBindings;

public class WinUIGenerateCodeTask : Task
{
	[Required]
	public required string LangVersion { get; init; }

	[Required]
	public required string MSBuildVersion { get; init; }

	[Required]
	public required ITaskItem[] ReferenceAssemblies { get; init; }

	[Required]
	public required string LocalAssembly { get; init; }

	[Required]
	public required string ProjectPath { get; init; }

	[Required]
	public required string IntermediateOutputPath { get; init; }

	public required ITaskItem ApplicationDefinition { get; init; }

	[Required]
	public required ITaskItem[] Pages { get; init; }

	[Output]
	public ITaskItem NewApplicationDefinition { get; private set; } = null!;

	[Output]
	public ITaskItem[] NewPages { get; private set; } = null!;

	[Output]
	public ITaskItem[] GeneratedCodeFiles { get; private set; } = null!;

	public bool AttachDebugger { get; set; }

	public override bool Execute()
	{
		try
		{
			if (AttachDebugger)
			{
				System.Diagnostics.Debugger.Launch();
			}

			TypeInfoUtils.LoadReferences(ReferenceAssemblies.Select(a => a.ItemSpec));
			var localAssembly = TypeInfoUtils.LoadLocalAssembly(LocalAssembly);

			TypeInfo.NotNullableProperties["Microsoft.UI.Xaml.Controls.TextBox"] = new HashSet<string> { "Text" };
			TypeInfo.NotNullableProperties["Microsoft.UI.Xaml.Controls.TextBlock"] = new HashSet<string> { "Text" };
			TypeInfo.NotNullableProperties["Microsoft.UI.Xaml.Documents.Run"] = new HashSet<string> { "Text" };

			var xamlDomParser = new WinUIXamlDomParser();

			var generatedCodeFiles = new List<ITaskItem>();
			var newPages = new List<ITaskItem>();
			bool result = true;

			var intermediateOutputPath = IntermediateOutputPath;
			bool isIntermediateOutputPathRooted = Path.IsPathRooted(intermediateOutputPath);
			if (!isIntermediateOutputPathRooted)
			{
				intermediateOutputPath = Path.Combine(ProjectPath, intermediateOutputPath);
			}

			var allXaml = Pages.ToList();
			if (ApplicationDefinition != null)
			{
				allXaml.Add(ApplicationDefinition);
			}

			var xamlFiles = allXaml
				.Distinct(f => f.GetMetadata("FullPath"))
				.Select(f => (xaml: f, file: f.GetMetadata("FullPath")))
				.Select(e => (e.xaml, e.file, xdoc: XDocument.Load(e.file, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo)))
				.ToList();

			var globalNamespaces = XamlNamespace.GetGlobalNamespaces(xamlFiles.Select(e => e.xdoc));
			xamlDomParser.KnownNamespaces = globalNamespaces;

			foreach (var (xaml, file, xdoc) in xamlFiles)
			{
				var newXaml = xaml;

				try
				{
					var xclass = xdoc.Root.Attribute(xamlDomParser.xClass);
					if (xclass != null)
					{
						var targetRelativePath = xaml.GetMetadata("Link");
						if (string.IsNullOrEmpty(targetRelativePath))
						{
							targetRelativePath = xaml.ItemSpec;
						}

						string lineFile;
						if (!isIntermediateOutputPathRooted)
						{
							var realPath = Path.Combine(ProjectPath, Path.GetDirectoryName(targetRelativePath));
							var intermediatePath = Path.Combine(intermediateOutputPath, Path.GetDirectoryName(targetRelativePath));
							var relativePath = PathUtils.GetRelativePath(intermediatePath, realPath);
							lineFile = Path.Combine(relativePath, Path.GetFileName(targetRelativePath));
						}
						else
						{
							lineFile = targetRelativePath;
						}

						var parseResult = xamlDomParser.Parse(file, lineFile, xdoc, (line, startColumn, endColumn, message) => Log.LogError(null, null, null, file, line, startColumn, line, endColumn, message));
						if (parseResult == null)
						{
							result = false;
						}
						else if (parseResult.GenerateCode)
						{
							var codeGenerator = new WinUICodeGenerator(LangVersion, MSBuildVersion);
							string code = codeGenerator.GenerateCode(parseResult);

							var targetDir = Path.Combine(IntermediateOutputPath, Path.GetDirectoryName(targetRelativePath));
							var dirInfo = new DirectoryInfo(targetDir);
							dirInfo.Create();

							var sourceCodeTargetPath = Path.Combine(targetDir, Path.GetFileNameWithoutExtension(targetRelativePath) + ".g.m.cs");
							File.WriteAllText(sourceCodeTargetPath, code);

							generatedCodeFiles.Add(new TaskItem(sourceCodeTargetPath));

							bool generateDataTemplates = parseResult.DataTemplates.Any(d => d.GenerateClass);

							if (generateDataTemplates)
							{
								var compiledBindingsNs = "using:CompiledBindings.WinUI";
								var localNs = "using:" + parseResult.TargetType!.Reference.Namespace;

								EnsureNamespaceDeclared(compiledBindingsNs);
								EnsureNamespaceDeclared(localNs);

								void EnsureNamespaceDeclared(string searchedClrNs)
								{
									var attr = xdoc.Root.Attributes().FirstOrDefault(a => a.Name.Namespace == XNamespace.Xmlns && a.Value == searchedClrNs);
									if (attr == null)
									{
										string classNsPrefix;
										int nsIndex = 0;
										do
										{
											classNsPrefix = "g" + nsIndex++;
										}
										while (xdoc.Root.Attributes().Any(a =>
											a.Name.Namespace == XNamespace.Xmlns && a.Name.LocalName == classNsPrefix));

										xdoc.Root.Add(new XAttribute(XNamespace.Xmlns + classNsPrefix, searchedClrNs));
									}
								}

								var compiledBindings = (XNamespace)compiledBindingsNs;
								var local = (XNamespace)localNs;

								for (int i = 0; i < parseResult.DataTemplates.Count; i++)
								{
									var dataTemplate = parseResult.DataTemplates[i];
									if (dataTemplate.GenerateClass)
									{
										var rootElement = dataTemplate.RootElement.Elements().First();
										rootElement.Add(
											new XElement(compiledBindings + "BindingsHelper.Bindings",
												new XElement(local + $"{parseResult.TargetType.Reference.Name}_DataTemplate{i}",
													dataTemplate.EnumerateResources().Select(r => new XAttribute(r.name, $"{{StaticResource {r.name}}}")))));
									}
								}
							}

							foreach (var obj in parseResult.EnumerateAllObjects().Where(o => !o.NameExplicitlySet && o.Name != null))
							{
								((XElement)obj.XamlNode.Element).Add(new XAttribute(xamlDomParser.xNamespace + "Name", obj.Name));
							}

							foreach (var prop in parseResult.EnumerateAllProperties())
							{
								var prop2 = prop.XamlNode.Element.Parent.Attribute(prop.MemberName);
								prop2?.Remove();
							}

							var xamlFile = Path.Combine(IntermediateOutputPath, "intermediate", targetRelativePath);

							dirInfo = new DirectoryInfo(Path.GetDirectoryName(xamlFile));
							dirInfo.Create();

							xdoc.Save(xamlFile);

							newXaml = new TaskItem(xamlFile);
							xaml.CopyMetadataTo(newXaml);
							newXaml.SetMetadata("Link", targetRelativePath);
						}
					}
				}
				catch (Exception ex) when (ex is not GeneratorException)
				{
					throw new GeneratorException(ex.Message, file, 0, 0, 0);
				}

				if (xaml == ApplicationDefinition)
				{
					NewApplicationDefinition = newXaml;
				}
				else
				{
					newPages.Add(newXaml);
				}
			}

			GeneratedCodeFiles = generatedCodeFiles.ToArray();
			NewPages = newPages.ToArray();

			return result;
		}
		catch (GeneratorException ex)
		{
			Log.LogError(null, null, null, ex.File, ex.LineNumber, ex.ColumnNumber, ex.EndLineNumber, ex.EndColumnNumber, ex.Message);
			return false;
		}
		catch (Exception ex)
		{
			Log.LogError(ex.Message);
			return false;
		}
		finally
		{
			TypeInfoUtils.Cleanup();
		}
	}

	public class WinUIXamlDomParser : SimpleXamlDomParser
	{
		private static readonly XNamespace _xmlns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

		private static readonly string[] _defaultClrNamespaces = new string[]
		{
			"Microsoft.UI.Xaml.Documents",
			"Microsoft.UI.Xaml.Controls",
			"Microsoft.UI.Xaml.Shapes",
		};

		public WinUIXamlDomParser() : base(
			_xmlns,
			"http://schemas.microsoft.com/winfx/2006/xaml",
			TypeInfo.GetTypeThrow("Microsoft.UI.Xaml.Data.IValueConverter"),
			TypeInfo.GetTypeThrow("Microsoft.UI.Xaml.Data.BindingBase"),
			TypeInfo.GetTypeThrow("Microsoft.UI.Xaml.DependencyObject"),
			TypeInfo.GetTypeThrow("Microsoft.UI.Xaml.DependencyProperty"))
		{
		}

		protected override IEnumerable<string> GetClrNsFromXmlNs(string xmlNs)
		{
			if (xmlNs == _xmlns)
				return _defaultClrNamespaces;
			return Enumerable.Empty<string>();
		}

		public override bool IsDataContextSupported(TypeInfo type)
		{
			return TypeInfo.GetTypeThrow("Microsoft.UI.Xaml.FrameworkElement").IsAssignableFrom(type);
		}
	}

	public class WinUICodeGenerator : SimpleXamlDomCodeGenerator
	{
		public WinUICodeGenerator(string langVersion, string msbuildVersion)
			: base(new WinUIBindingsCodeGenerator(langVersion, msbuildVersion),
				   "Data",
				   "Microsoft.UI.Xaml.DataContextChangedEventArgs",
				   "Microsoft.UI.Xaml.FrameworkElement",
				   "(global::{0}){1}.FindName(\"{2}\")",
				   true,
				   false,
				   "WinUI",
				   langVersion,
				   msbuildVersion)
		{
		}

		protected override string CreateGetResourceCode(string resourceName)
		{
			return $@"this.Resources.TryGetValue(""{resourceName}"", out var r) ? r : global::Microsoft.UI.Xaml.Application.Current.Resources[""{resourceName}""]";
		}
	}

	public class WinUIBindingsCodeGenerator : BindingsCodeGenerator
	{
		public WinUIBindingsCodeGenerator(string langVersion, string msbuildVersion) : base("WinUI", langVersion, msbuildVersion)
		{
		}

		protected override void GenerateBindingsExtraFieldDeclarations(StringBuilder output, BindingsData bindingsData)
		{
			// Generate _callbackTokenXXX fields for dependency property notifications
			foreach (var ev in bindingsData.TwoWayEvents.Where(e => e.Bindings[0].DependencyProperty != null))
			{
				output.AppendLine(
$@"			private long _targetCallbackToken{ev.Index};");
			}
		}

		protected override void GenerateSetDependencyPropertyChangedCallback(StringBuilder output, TwoWayBindingsData ev, string targetExpr)
		{
			var first = ev.Bindings[0];
			output.AppendLine(
$@"				_targetCallbackToken{ev.Index} = {targetExpr}.RegisterPropertyChangedCallback({first.Property.Object.Type.Reference.FullName}.{first.Property.MemberName}Property, OnTargetChanged{ev.Index});");
		}

		protected override void GenerateUnsetDependencyPropertyChangedCallback(StringBuilder output, TwoWayBindingsData ev, string targetExpr)
		{
			var first = ev.Bindings[0];
			output.AppendLine(
$@"					{targetExpr}.UnregisterPropertyChangedCallback({first.Property.Object.Type.Reference.FullName}.{first.Property.MemberName}Property, _targetCallbackToken{ev.Index});");
		}

		protected override void GenerateDependencyPropertyChangedCallback(StringBuilder output, string methodName, string? a)
		{
			output.AppendLine(
$@"{a}			private void {methodName}(global::Microsoft.UI.Xaml.DependencyObject sender, Microsoft.UI.Xaml.DependencyProperty dp)");
		}

		protected override void GenerateDependencyPropertyChangeCacheVariables(StringBuilder output, NotifySource notifySource, NotifyProperty notifyProp, string cacheVar)
		{
			if (notifyProp == notifySource.Properties[0])
			{
				output.AppendLine(
$@"				private global::Microsoft.UI.Xaml.DependencyObject {cacheVar};");
			}
			output.AppendLine(
$@"				private long _sourceCallbackToken{notifySource.Index}_{notifyProp.Member!.Definition.Name};");
		}

		protected override void GenerateDependencyPropertySetPropertyHandler(StringBuilder output, NotifySource notifySource, NotifyProperty notifyProp, string cacheVar, string methodName)
		{
			output.AppendLine(
$@"					global::CompiledBindings.WinUI.BindingsHelper.SetPropertyChangedEventHandler(ref {cacheVar}, value, global::{notifySource.Expression.Type.Reference.GetCSharpFullName()}.{notifyProp.Member!.Definition.Name}Property, ref _sourceCallbackToken{notifySource.Index}_{notifyProp.Member!.Definition.Name}, OnPropertyChanged{notifySource.Index}_{notifyProp.PropertyCodeName});");
		}
	}
}
