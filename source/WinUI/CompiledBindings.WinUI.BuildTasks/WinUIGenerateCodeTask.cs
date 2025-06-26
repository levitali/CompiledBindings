using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CompiledBindings;

public class WinUIGenerateCodeTask : Task, ICancelableTask
{
	private CancellationTokenSource? _cancellationTokenSource;
	private readonly PlatformConstants _platformConstants;

	public WinUIGenerateCodeTask() : this(new PlatformConstants())
	{
	}

	public WinUIGenerateCodeTask(PlatformConstants platformConstants)
	{
		_platformConstants = platformConstants;
	}

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

	public string? Nullable { get; init; }

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

			TypeInfo.EnableNullables = string.Compare(Nullable, "enable", true) == 0;

			_cancellationTokenSource = new CancellationTokenSource();

			TypeInfoUtils.LoadReferences(ReferenceAssemblies.Select(a => a.ItemSpec));
			var localAssembly = TypeInfoUtils.LoadLocalAssembly(LocalAssembly);

			TypeInfo.NotNullableProperties[$"{_platformConstants.BaseClrNamespace}.UI.Xaml.Controls.TextBox"] = ["Text"];
			TypeInfo.NotNullableProperties[$"{_platformConstants.BaseClrNamespace}.UI.Xaml.Controls.TextBlock"] = ["Text"];
			TypeInfo.NotNullableProperties[$"{_platformConstants.BaseClrNamespace}.UI.Xaml.Documents.Run"] = ["Text"];

			var xamlDomParser = new WinUIXamlDomParser(_platformConstants);
			var codeGenerator = new WinUICodeGenerator(_platformConstants, LangVersion, MSBuildVersion);

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
				if (_cancellationTokenSource.IsCancellationRequested)
				{
					return true;
				}

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
								var compiledBindingsNs = $"using:CompiledBindings.{_platformConstants.FrameworkId}";
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
											new XElement(compiledBindings + "CompiledBindingsHelper.Bindings",
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
								var prop2 = prop.XamlNode.Element.Parent.Attribute(prop.XamlNode.Name);
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

			if (result)
			{
				if (generatedCodeFiles.Count > 0)
				{
					var dataTemplateBindingsFile = Path.Combine(IntermediateOutputPath, $"CompiledBindingsHelper.{_platformConstants.FrameworkId}.cs");
					File.WriteAllText(dataTemplateBindingsFile, codeGenerator.GenerateCompiledBindingsHelper());
					generatedCodeFiles.Add(new TaskItem(dataTemplateBindingsFile));
				}

				GeneratedCodeFiles = generatedCodeFiles.ToArray();
				NewPages = newPages.ToArray();
			}


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

	void ICancelableTask.Cancel()
	{
		_cancellationTokenSource?.Cancel();
	}

	public class WinUIXamlDomParser : SimpleXamlDomParser
	{
		private static readonly XNamespace _xmlns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

		private readonly string[] _defaultClrNamespaces;

		private readonly PlatformConstants _platformConstants;

		public WinUIXamlDomParser(PlatformConstants platformConstants) : base(
			_xmlns,
			"http://schemas.microsoft.com/winfx/2006/xaml",
			TypeInfo.GetTypeThrow($"{platformConstants.BaseClrNamespace}.UI.Xaml.Data.IValueConverter"),
			TypeInfo.GetTypeThrow($"{platformConstants.BaseClrNamespace}.UI.Xaml.Data.BindingBase"),
			TypeInfo.GetTypeThrow($"{platformConstants.BaseClrNamespace}.UI.Xaml.DependencyObject"),
			TypeInfo.GetTypeThrow($"{platformConstants.BaseClrNamespace}.UI.Xaml.DependencyProperty"))
		{
			_platformConstants = platformConstants;
			_defaultClrNamespaces =
			[
				$"{platformConstants.BaseClrNamespace}.UI.Xaml.Data",
				$"{platformConstants.BaseClrNamespace}.UI.Xaml.Documents",
				$"{platformConstants.BaseClrNamespace}.UI.Xaml.Controls",
				$"{platformConstants.BaseClrNamespace}.UI.Xaml.Shapes",
			];
		}

		protected override IEnumerable<string> GetClrNsFromXmlNs(string xmlNs)
		{
			return xmlNs == _xmlns ? _defaultClrNamespaces : Enumerable.Empty<string>();
		}

		public override bool IsDataContextSupported(TypeInfo type)
		{
			return TypeInfo.GetTypeThrow($"{_platformConstants.BaseClrNamespace}.UI.Xaml.FrameworkElement").IsAssignableFrom(type);
		}
	}

	public class WinUICodeGenerator : SimpleXamlDomCodeGenerator
	{
		private readonly PlatformConstants _platformConstants;

		public WinUICodeGenerator(PlatformConstants platformConstants, string langVersion, string msbuildVersion)
			: base(new WinUIBindingsCodeGenerator(platformConstants, langVersion, msbuildVersion),
				   "Data",
				   $"{platformConstants.BaseClrNamespace}.UI.Xaml.DataContextChangedEventArgs",
				   $"{platformConstants.BaseClrNamespace}.UI.Xaml.FrameworkElement",
				   "(global::{0}){1}.FindName(\"{2}\")",
				   true,
				   false,
				   platformConstants.FrameworkId,
				   langVersion,
				   msbuildVersion)
		{
			_platformConstants = platformConstants;
		}

		public string GenerateCompiledBindingsHelper()
		{
			return
$@"{GenerateFileHeader()}

namespace CompiledBindings.{_platformConstants.FrameworkId}
{{
	internal class CompiledBindingsHelper
	{{
{BindingsCodeGenerator.CompiledBindingsHelperBaseCode}

		public static readonly global::{_platformConstants.BaseClrNamespace}.UI.Xaml.DependencyProperty BindingsProperty =
				global::{_platformConstants.BaseClrNamespace}.UI.Xaml.DependencyProperty.RegisterAttached(""Bindings"", typeof(IGeneratedDataTemplate), typeof(CompiledBindingsHelper), new global::{_platformConstants.BaseClrNamespace}.UI.Xaml.PropertyMetadata(null, BindingsChanged));

		public static IGeneratedDataTemplate GetBindings(global::{_platformConstants.BaseClrNamespace}.UI.Xaml.DependencyObject @object)
		{{
			return (IGeneratedDataTemplate)@object.GetValue(BindingsProperty);
		}}

		public static void SetBindings(global::{_platformConstants.BaseClrNamespace}.UI.Xaml.DependencyObject @object, IGeneratedDataTemplate value)
		{{
			@object.SetValue(BindingsProperty, value);
		}}

		static void BindingsChanged(global::{_platformConstants.BaseClrNamespace}.UI.Xaml.DependencyObject d, global::{_platformConstants.BaseClrNamespace}.UI.Xaml.DependencyPropertyChangedEventArgs e)
		{{
			if (e.OldValue != null)
			{{
				((IGeneratedDataTemplate)e.OldValue).Cleanup((global::{_platformConstants.BaseClrNamespace}.UI.Xaml.FrameworkElement)d);
			}}
			if (e.NewValue != null)
			{{
				((IGeneratedDataTemplate)e.NewValue).Initialize((global::{_platformConstants.BaseClrNamespace}.UI.Xaml.FrameworkElement)d);
			}}
		}}
	}}

	public interface IGeneratedDataTemplate
	{{
		void Initialize(global::{_platformConstants.BaseClrNamespace}.UI.Xaml.FrameworkElement rootElement);
		void Cleanup(global::{_platformConstants.BaseClrNamespace}.UI.Xaml.FrameworkElement rootElement);
	}}
}}";
		}

		protected override string CreateGetResourceCode(string resourceName, int varIndex)
		{
			return $@"this.Resources.TryGetValue(""{resourceName}"", out var r{varIndex}) || global::{_platformConstants.BaseClrNamespace}.UI.Xaml.Application.Current.Resources.TryGetValue(""{resourceName}"", out r{varIndex}) ? r{varIndex} : throw new global::System.Exception(""Resource '{resourceName}' not found."")";
		}
	}

	public class WinUIBindingsCodeGenerator : BindingsCodeGenerator
	{
		private readonly PlatformConstants _platformConstants;

		public WinUIBindingsCodeGenerator(PlatformConstants platformConstants, string langVersion, string msbuildVersion) : base(platformConstants.FrameworkId, langVersion, msbuildVersion)
		{
			_platformConstants = platformConstants;
		}

		protected override string DependencyObjectType => $"global::{_platformConstants.BaseClrNamespace}.UI.Xaml.DependencyObject";

		protected override void GenerateBindingsExtraFieldDeclarations(StringBuilder output, BindingsClass bindingsData)
		{
			// Generate _callbackTokenXXX fields for dependency property notifications
			foreach (var ev in bindingsData.TwoWayEvents.Where(e => e.Bindings[0].DependencyProperty != null))
			{
				output.AppendLine(
$@"			private long _targetCallbackToken{ev.Index};");
			}
		}

		protected override void GenerateSetDependencyPropertyChangedCallback(StringBuilder output, TwoWayBinding ev, string targetExpr)
		{
			var first = ev.Bindings[0];
			output.AppendLine(
$@"				_targetCallbackToken{ev.Index} = {targetExpr}.RegisterPropertyChangedCallback({first.Property.Object.Type.Reference.FullName}.{first.Property.MemberName}Property, OnTargetChanged{ev.Index});");
		}

		protected override void GenerateUnsetDependencyPropertyChangedCallback(StringBuilder output, TwoWayBinding ev, string targetExpr)
		{
			var first = ev.Bindings[0];
			output.AppendLine(
$@"					{targetExpr}.UnregisterPropertyChangedCallback({first.Property.Object.Type.Reference.FullName}.{first.Property.MemberName}Property, _targetCallbackToken{ev.Index});");
		}

		protected override void GenerateDependencyPropertyChangedCallback(StringBuilder output, string methodName, string? a)
		{
			output.AppendLine(
$@"{a}			private void {methodName}(global::{_platformConstants.BaseClrNamespace}.UI.Xaml.DependencyObject sender, {_platformConstants.BaseClrNamespace}.UI.Xaml.DependencyProperty dp)");
		}

		protected override void GenerateDependencyPropertyChangeExtraVariables(StringBuilder output, NotifySource notifySource)
		{
			foreach (var notifyProp in notifySource.Properties)
			{
				output.AppendLine(
$@"				long _sourceCallbackToken{notifySource.Index}_{notifyProp.Member!.Definition.Name};");
			}
		}

		protected override void GenerateRegisterDependencyPropertyChangeEvent(StringBuilder output, NotifySource notifySource, NotifyProperty notifyProp, string cacheVar, string methodName)
		{
			if (notifySource.AnyINotifyPropertyChangedProperty)
			{
				cacheVar = $"((global::{_platformConstants.BaseClrNamespace}.UI.Xaml.DependencyObject){cacheVar})";
			}
			output.AppendLine(
$@"						_sourceCallbackToken{notifySource.Index}_{notifyProp.Member!.Definition.Name} = {cacheVar}.RegisterPropertyChangedCallback({notifySource.SourceExpression.Type.Reference.GetCSharpFullName()}.{notifyProp.Member!.Definition.Name}Property, {methodName});");
		}

		protected override void GenerateUnregisterDependencyPropertyChangeEvent(StringBuilder output, NotifySource notifySource, NotifyProperty notifyProp, string cacheVar, string methodName)
		{
			if (notifySource.AnyINotifyPropertyChangedProperty)
			{
				cacheVar = $"((global::{_platformConstants.BaseClrNamespace}.UI.Xaml.DependencyObject){cacheVar})";
			}
			output.AppendLine(
$@"						{cacheVar}.UnregisterPropertyChangedCallback({notifySource.SourceExpression.Type.Reference.GetCSharpFullName()}.{notifyProp.Member!.Definition.Name}Property, _sourceCallbackToken{notifySource.Index}_{notifyProp.Member!.Definition.Name});");
		}
	}
}

public class PlatformConstants
{
	public virtual string FrameworkId => "WinUI";
	public virtual string BaseClrNamespace => "Microsoft";
}
