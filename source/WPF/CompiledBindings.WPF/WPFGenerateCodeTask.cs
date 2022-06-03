using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

#nullable enable

namespace CompiledBindings;

public class WPFGenerateCodeTask : Task, ICancelableTask
{
	private CancellationTokenSource? _cancellationTokenSource;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	[Required]
	public string LangVersion { get; set; }

	[Required]
	public string MSBuildVersion { get; set; }

	[Required]
	public ITaskItem[] ReferenceAssemblies { get; set; }

	[Required]
	public string LocalAssembly { get; set; }

	[Required]
	public string ProjectPath { get; set; }

	[Required]
	public string IntermediateOutputPath { get; set; }

	public ITaskItem ApplicationDefinition { get; set; }

	[Required]
	public ITaskItem[] Pages { get; set; }

	[Output]
	public ITaskItem NewApplicationDefinition { get; set; }

	[Output]
	public ITaskItem[] NewPages { get; set; }

	[Output]
	public ITaskItem[] GeneratedCodeFiles { get; private set; }

	public bool AttachDebugger { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public override bool Execute()
	{
		try
		{
			if (AttachDebugger)
			{
				System.Diagnostics.Debugger.Launch();
			}

			_cancellationTokenSource = new CancellationTokenSource();

			TypeInfoUtils.LoadReferences(ReferenceAssemblies.Select(a => a.ItemSpec));
			var localAssembly = TypeInfoUtils.LoadLocalAssembly(LocalAssembly);

			var xamlDomParser = new WpfXamlDomParser();

			var generatedCodeFiles = new List<ITaskItem>();
			var newPages = new List<ITaskItem>();
			bool generateDataTemplateBindings = false;

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

			var intermediateOutputPath = IntermediateOutputPath;
			bool isIntermediateOutputPathRooted = Path.IsPathRooted(intermediateOutputPath);
			if (!isIntermediateOutputPathRooted)
			{
				intermediateOutputPath = Path.Combine(ProjectPath, intermediateOutputPath);
			}

			foreach (var (xaml, file, xdoc) in xamlFiles)
			{
				var newXaml = xaml;

				var targetRelativePath = xaml.GetMetadata("Link");
				if (string.IsNullOrEmpty(targetRelativePath))
				{
					targetRelativePath = xaml.ItemSpec;
				}

				var targetDir = Path.Combine(IntermediateOutputPath, Path.GetDirectoryName(targetRelativePath));
				var sourceCodeTargetPath = Path.Combine(targetDir, Path.GetFileNameWithoutExtension(targetRelativePath) + ".g.m.cs");
				var xamlFile = Path.Combine(IntermediateOutputPath, targetRelativePath);

				/*
				if (File.Exists(sourceCodeTargetPath))
				{
					using (var stream = File.OpenRead(sourceCodeTargetPath))
					using (var streamReader = new StreamReader(stream))
					{
						var firstLine = streamReader.ReadLine();
						if (firstLine != null)
						{
							var parts = firstLine.Split(' ');
							if (parts.Length == 3 &&
								parts[0] == "//checksum" &&
								uint.TryParse(parts[1], out var checksum1) &&
								bool.TryParse(parts[2], out var dataTemplates))
							{
								var checksum2 = Crc32.GetCrc32(file);
								if (checksum1 == checksum2 && File.Exists(xamlFile))
								{
									generateDataTemplateBindings |= dataTemplates;

									generatedCodeFiles.Add(new TaskItem(sourceCodeTargetPath));

									newXaml = new TaskItem(xamlFile);
									xaml.CopyMetadataTo(newXaml);
									newXaml.SetMetadata("Link", targetRelativePath);

									SetNewXaml();
									continue;
								}
							}
						}
					}
				}
				*/

				try
				{
					var xclass = xdoc.Root.Attribute(xamlDomParser.xClass);
					if (xclass != null)
					{
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

						var parseResult = xamlDomParser.Parse(file, lineFile, xdoc);

						if (parseResult.GenerateCode)
						{
							parseResult.Validate(file);

							var codeGenerator = new WpfCodeGenerator(LangVersion, MSBuildVersion);
							string code = codeGenerator.GenerateCode(parseResult);

							bool dataTemplates = parseResult.DataTemplates.Count > 0;
							generateDataTemplateBindings |= dataTemplates;

							var checksum = Crc32.GetCrc32(file);
							code = $"//checksum {checksum} {dataTemplates}{Environment.NewLine}{code}";

							var dirInfo = new DirectoryInfo(targetDir);
							dirInfo.Create();

							File.WriteAllText(sourceCodeTargetPath, code);
							generatedCodeFiles.Add(new TaskItem(sourceCodeTargetPath));

							if (parseResult.DataTemplates.Count > 0)
							{
								string? rootName = null;
								if (parseResult.DataTemplates.SelectMany(dt => dt.EnumerateAllProperties()).Any(p => p.Value.BindValue?.Resources.Count > 0))
								{
									var xNameAttr = xdoc.Root.Attribute(xamlDomParser.xName);
									if (xNameAttr != null)
									{
										rootName = xNameAttr.Value;
									}
									else
									{
										rootName = XamlDomParser.GenerateName(xdoc.Root, xamlDomParser.UsedNames!);
										xdoc.Root.Add(new XAttribute(xamlDomParser.xName, rootName));
									}
								}

								var compiledBindingsNs = "clr-namespace:CompiledBindings";
								var localNs = "clr-namespace:" + parseResult.TargetType!.Type.Namespace;

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

								var mbui = (XNamespace)compiledBindingsNs;
								var local = (XNamespace)localNs;

								for (int i = 0; i < parseResult.DataTemplates.Count; i++)
								{
									var dataTemplate = parseResult.DataTemplates[i];
									var rootElement = dataTemplate.RootElement.Elements().First();
									if (dataTemplate.EnumerateAllProperties().Any(p => p.Value.BindValue?.Resources.Count > 0))
									{
										rootElement.Add(
											new XAttribute(mbui + "DataTemplateBindings.Root",
												$"{{Binding ElementName={rootName}}}"));
									}
									rootElement.Add(
										new XElement(mbui + "DataTemplateBindings.Bindings",
											new XElement(local + $"{parseResult.TargetType.Type.Name}_DataTemplate{i}")));
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

				SetNewXaml();

				void SetNewXaml()
				{
					if (xaml == ApplicationDefinition)
					{
						NewApplicationDefinition = newXaml;
					}
					else
					{
						newPages.Add(newXaml);
					}
				}
			}

			if (generateDataTemplateBindings)
			{
				var dataTemplateBindingsFile = Path.Combine(IntermediateOutputPath, "DataTemplateBindings.cs");
				File.WriteAllText(dataTemplateBindingsFile, GenerateDataTemplateBindingsClass());
				generatedCodeFiles.Add(new TaskItem(dataTemplateBindingsFile));
			}

			GeneratedCodeFiles = generatedCodeFiles.ToArray();
			NewPages = newPages.ToArray();

			foreach (var lrefFile in Directory.GetFiles(IntermediateOutputPath, "*.lref"))
			{
				File.Delete(lrefFile);
			}
			foreach (var lrefFile in Directory.GetFiles(IntermediateOutputPath, "*.cache"))
			{
				File.Delete(lrefFile);
			}

			return true;
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

	private string GenerateDataTemplateBindingsClass()
	{
		return
$@"namespace CompiledBindings
{{
	using System.Windows;

	public class DataTemplateBindings
	{{
		public static readonly DependencyProperty BindingsProperty =
			DependencyProperty.RegisterAttached(""Bindings"", typeof(IGeneratedDataTemplate), typeof(DataTemplateBindings), new PropertyMetadata(BindingsChanged));

		public static IGeneratedDataTemplate GetBindings(DependencyObject @object)
		{{
			return (IGeneratedDataTemplate)@object.GetValue(BindingsProperty);
		}}

		public static void SetBindings(DependencyObject @object, IGeneratedDataTemplate value)
		{{
			@object.SetValue(BindingsProperty, value);
		}}

		static void BindingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{{
			if (e.OldValue != null)
			{{
				((IGeneratedDataTemplate)e.OldValue).Cleanup((FrameworkElement)d);
			}}
			if (e.NewValue != null)
			{{
				((IGeneratedDataTemplate)e.NewValue).Initialize((FrameworkElement)d);
			}}
		}}

		public static readonly DependencyProperty RootProperty =
			DependencyProperty.RegisterAttached(""Root"", typeof(FrameworkElement), typeof(DataTemplateBindings), new PropertyMetadata(null));

		public static FrameworkElement GetRoot(DependencyObject @object)
		{{
			return (FrameworkElement)@object.GetValue(RootProperty);
		}}

		public static void SetRoot(DependencyObject @object, FrameworkElement value)
		{{
			@object.SetValue(RootProperty, value);
		}}
	}}

	public interface IGeneratedDataTemplate
	{{
		void Initialize(FrameworkElement rootElement);
		void Cleanup(FrameworkElement rootElement);
	}}
}}";
	}

	void ICancelableTask.Cancel()
	{
		_cancellationTokenSource?.Cancel();
	}
}

public class WpfXamlDomParser : SimpleXamlDomParser
{
	private static ILookup<string, string>? _nsMappings = null;

	public WpfXamlDomParser()
		: base("http://schemas.microsoft.com/winfx/2006/xaml/presentation",
			   "http://schemas.microsoft.com/winfx/2006/xaml",
				getClrNsFromXmlNs: xmlNs =>
				{
					if (_nsMappings == null)
					{
						_nsMappings = TypeInfoUtils.Assemblies
							.SelectMany(ass => ass.CustomAttributes.Where(at => at.AttributeType.FullName == "System.Windows.Markup.XmlnsDefinitionAttribute"))
							.Select(at => (
								XmlNamespace: (string)at.ConstructorArguments[0].Value,
								ClrNamespace: (string)at.ConstructorArguments[1].Value))
							.ToLookup(at => at.XmlNamespace, at => at.ClrNamespace);
					}

					return _nsMappings[xmlNs];
				},
				TypeInfo.GetTypeThrow("System.Windows.Data.IValueConverter"),
				TypeInfo.GetTypeThrow("System.Windows.Data.BindingBase"),
				TypeInfo.GetTypeThrow("System.Windows.DependencyObject"),
				TypeInfo.GetTypeThrow("System.Windows.DependencyProperty")
			  )
	{
	}

	public override bool IsMemExtension(XAttribute a)
	{
		return base.IsMemExtension(a) || a.Value.StartsWith("{x:Bind ");
	}

	public override (bool isSupported, string? controlName) IsElementSupported(XName elementName)
	{
		var b = base.IsElementSupported(elementName);
		if (!b.isSupported)
		{
			return b;
		}

		if (elementName == Style)
		{
			return (false, "a Style");
		}

		return (true, null);
	}
}

public class WpfCodeGenerator : SimpleXamlDomCodeGenerator
{
	public WpfCodeGenerator(string langVersion, string msbuildVersion)
		: base(new WpfBindingsCodeGenerator(langVersion, msbuildVersion),
			   "Data",
			   "System.Windows.DependencyPropertyChangedEventArgs",
			   "System.Windows.FrameworkElement",
			   "(global::{0}){1}.FindName(\"{2}\")",
			   false,
			   false,
			   langVersion,
			   msbuildVersion)
	{
	}

	protected override void GenerateInitializeResources(StringBuilder output, SimpleXamlDom parseResult, string rootElement, bool isDataTemplate)
	{
		var resources = parseResult.XamlObjects.SelectMany(o => o.Properties).Select(p => p.Value.BindValue).Where(b => b != null).SelectMany(b => b!.Resources).Distinct(b => b.name).ToList();
		if (resources.Count > 0)
		{
			string root;
			if (isDataTemplate)
			{
				output.AppendLine(
$@"			var root = global::CompiledBindings.DataTemplateBindings.GetRoot({rootElement});");
				root = "root?";
			}
			else
			{
				root = "this";
			}

			foreach (var resource in resources)
			{
				output.AppendLine(
$@"			{resource.name} = (global::{resource.type.Type.GetCSharpFullName()})({root}.Resources[""{resource.name}""] ?? global::System.Windows.Application.Current.Resources[""{resource.name}""] ?? throw new global::System.Exception(""Resource '{resource.name}' not found.""));");
			}

			output.AppendLine();
		}
	}
}

public class WpfBindingsCodeGenerator : BindingsCodeGenerator
{
	public WpfBindingsCodeGenerator(string langVersion, string msbuildVersion) : base(langVersion, msbuildVersion)
	{

	}

	protected override void GenerateSetDependencyPropertyChangedCallback(StringBuilder output, TwoWayEventData ev, string targetExpr)
	{
		var dp = ev.Bindings[0].DependencyProperty!;
		output.AppendLine(
$@"				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::{dp.Definition.DeclaringType.GetCSharpFullName()}.{dp.Definition.Name},
						typeof(global::{dp.Definition.DeclaringType.GetCSharpFullName()}))
					.AddValueChanged({targetExpr}, OnTargetChanged{ev.Index});");
	}

	protected override void GenerateUnsetDependencyPropertyChangedCallback(StringBuilder output, TwoWayEventData ev, string targetExpr)
	{
		var dp = ev.Bindings[0].DependencyProperty!;
		output.AppendLine(
$@"					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::{dp.Definition.DeclaringType.GetCSharpFullName()}.{dp.Definition.Name},
							typeof(global::{dp.Definition.DeclaringType.GetCSharpFullName()}))
						.RemoveValueChanged({targetExpr}, OnTargetChanged{ev.Index});");
	}

	protected override void GenerateDependencyPropertyChangedCallback(StringBuilder output, string methodName, string? a)
	{
		output.AppendLine(
$@"{a}			private void {methodName}(object sender, global::System.EventArgs e)");
	}

	protected override void GenerateRegisterDependencyPropertyChangeEvent(StringBuilder output, NotifyPropertyChangedData notifyGroup, NotifyPropertyChangedProperty notifyProp, string cacheVar, string methodName)
	{
		output.AppendLine(
$@"						global::System.ComponentModel.DependencyPropertyDescriptor
							.FromProperty(
								global::{notifyGroup.SourceExpression.Type.Type.GetCSharpFullName()}.{notifyProp.Property.Definition.Name}Property, typeof(global::{notifyGroup.SourceExpression.Type.Type.GetCSharpFullName()}))
							.AddValueChanged({cacheVar}, {methodName});");
	}

	protected override void GenerateUnregisterDependencyPropertyChangeEvent(StringBuilder output, NotifyPropertyChangedData notifyGroup, NotifyPropertyChangedProperty notifyProp, string cacheVar, string methodName)
	{
		output.AppendLine(
$@"						global::System.ComponentModel.DependencyPropertyDescriptor
							.FromProperty(
								global::{notifyGroup.SourceExpression.Type.Type.GetCSharpFullName()}.{notifyProp.Property.Definition.Name}Property, typeof(global::{notifyGroup.SourceExpression.Type.Type.GetCSharpFullName()}))
							.RemoveValueChanged({cacheVar}, {methodName});");
	}
}

