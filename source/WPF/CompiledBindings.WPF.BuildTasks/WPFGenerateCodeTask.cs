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

public class WPFGenerateCodeTask : Task, ICancelableTask
{
	private CancellationTokenSource? _cancellationTokenSource;

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

	[Required]
	public required string RootNamespace { get; init; }

	[Required]
	public required string AssemblyName { get; init; }

	public required ITaskItem ApplicationDefinition { get; init; }

	[Required]
	public required ITaskItem[] Pages { get; init; }

	[Output]
	public ITaskItem NewApplicationDefinition { get; private set; } = null!;

	[Output]
	public ITaskItem[] NewPages { get; private set; } = null!;

	[Output]
	public ITaskItem[] GeneratedCodeFiles { get; private set; } = null!;

	public string? Nullable { get; init; }

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

			var compiledBindingsHelperNs = GenerateUtils.GenerateCompiledBindingsHelperNs(RootNamespace, AssemblyName, "WPF");

			var xamlDomParser = new WpfXamlDomParser();
			var codeGenerator = new WpfCodeGenerator(compiledBindingsHelperNs, LangVersion, MSBuildVersion);

			var generatedCodeFiles = new List<ITaskItem>();
			var newPages = new List<ITaskItem>();
			bool result = true;

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

						var parseResult = xamlDomParser.Parse(file, lineFile, xdoc, (line, startColumn, endColumn, message) => Log.LogError(null, null, null, file, line, startColumn, line, endColumn, message));
						if (parseResult == null)
						{
							result = false;
						}
						else if (parseResult.GenerateCode)
						{
							string code = codeGenerator.GenerateCode(parseResult);

							var dirInfo = new DirectoryInfo(targetDir);
							dirInfo.Create();

							File.WriteAllText(sourceCodeTargetPath, code);
							generatedCodeFiles.Add(new TaskItem(sourceCodeTargetPath));

							WpfXamlProcessor.ProcessXaml(xdoc, parseResult, compiledBindingsHelperNs);
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

				setNewXaml();

				void setNewXaml()
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

			if (result)
			{
				if (generatedCodeFiles.Count > 0)
				{
					var dataTemplateBindingsFile = Path.Combine(IntermediateOutputPath, "CompiledBindingsHelper.WPF.cs");
					File.WriteAllText(dataTemplateBindingsFile, codeGenerator.GenerateCompiledBindingsHelper());
					generatedCodeFiles.Add(new TaskItem(dataTemplateBindingsFile));
				}

				GeneratedCodeFiles = generatedCodeFiles.ToArray();
				NewPages = newPages.ToArray();
			}

			foreach (var lrefFile in Directory.GetFiles(IntermediateOutputPath, "*.lref"))
			{
				File.Delete(lrefFile);
			}
			foreach (var lrefFile in Directory.GetFiles(IntermediateOutputPath, "*.cache"))
			{
				File.Delete(lrefFile);
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
}

public class WpfXamlDomParser : SimpleXamlDomParser
{
	private ILookup<string, string>? _nsMappings = null;

	public WpfXamlDomParser()
		: base("http://schemas.microsoft.com/winfx/2006/xaml/presentation",
			   WpfConstants.xNs,
			   TypeInfo.GetTypeThrow("System.Windows.Data.IValueConverter"),
			   TypeInfo.GetTypeThrow("System.Windows.Data.BindingBase"),
			   TypeInfo.GetTypeThrow("System.Windows.DependencyObject"),
			   TypeInfo.GetTypeThrow("System.Windows.DependencyProperty"))
	{
	}

	protected override IEnumerable<string> GetClrNsFromXmlNs(string xmlNs)
	{
		_nsMappings ??= TypeInfoUtils.Assemblies
							.SelectMany(ass => ass.CustomAttributes.Where(at => at.AttributeType.FullName == "System.Windows.Markup.XmlnsDefinitionAttribute"))
							.Select(at => (
								XmlNamespace: (string)at.ConstructorArguments[0].Value,
								ClrNamespace: (string)at.ConstructorArguments[1].Value))
							.ToLookup(at => at.XmlNamespace, at => at.ClrNamespace);

		return _nsMappings[xmlNs];
	}

	protected override ExtenstionType? IsMemExtension(XName name)
	{
		var res = base.IsMemExtension(name);
		if (res == null)
		{
			if (name.Namespace == WpfConstants.xNs)
			{
				res = name.LocalName switch
				{
					"Bind" or "BindExtension" => ExtenstionType.Bind,
					"Set" or "SetExtension" => ExtenstionType.Set,
					_ => null
				};
			}
		}
		return res;
	}

	protected override bool CanSetBindingTypeProperty => true;

	protected override void CheckBinding(Bind bind, XamlNode xamlNode)
	{
		if (bind.Mode != BindingMode.OneTime)
		{
			throw new GeneratorException($"Only OneTime Mode is supported if the property type is a standard Binding.", CurrentFile, xamlNode);
		}
	}
}

public class WpfCodeGenerator : SimpleXamlDomCodeGenerator
{
	private readonly string _compiledBindingsHelperNs;

	public WpfCodeGenerator(string compiledBindingsHelperNs, string langVersion, string msbuildVersion)
		: base(new WpfBindingsCodeGenerator(compiledBindingsHelperNs, langVersion, msbuildVersion),
			   compiledBindingsHelperNs,
			   "Data",
			   "System.Windows.DependencyPropertyChangedEventArgs",
			   "System.Windows.FrameworkElement",
			   "(global::{0}){1}.FindName(\"{2}\")",
			   false,
			   false,
			   langVersion,
			   msbuildVersion)
	{
		_compiledBindingsHelperNs = compiledBindingsHelperNs;
	}

	public string GenerateCompiledBindingsHelper()
	{
		return
$@"{GenerateFileHeader()}

namespace {_compiledBindingsHelperNs}
{{
	internal class CompiledBindingsHelper
	{{
{BindingsCodeGenerator.CompiledBindingsHelperBaseCode}		

		public static readonly global::System.Windows.DependencyProperty BindingsProperty =
			global::System.Windows.DependencyProperty.RegisterAttached(""Bindings"", typeof(IGeneratedDataTemplate), typeof(CompiledBindingsHelper), new global::System.Windows.PropertyMetadata(BindingsChanged));

		public static IGeneratedDataTemplate GetBindings(global::System.Windows.DependencyObject @object)
		{{
			return (IGeneratedDataTemplate)@object.GetValue(BindingsProperty);
		}}

		public static void SetBindings(global::System.Windows.DependencyObject @object, IGeneratedDataTemplate value)
		{{
			@object.SetValue(BindingsProperty, value);
		}}

		static void BindingsChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e)
		{{
			if (e.OldValue != null)
			{{
				((IGeneratedDataTemplate)e.OldValue).Cleanup((global::System.Windows.FrameworkElement)d);
			}}
			if (e.NewValue != null)
			{{
				((IGeneratedDataTemplate)e.NewValue).Initialize((global::System.Windows.FrameworkElement)d);
			}}
		}}
	}}

	internal interface IGeneratedDataTemplate
	{{
		void Initialize(global::System.Windows.FrameworkElement rootElement);
		void Cleanup(global::System.Windows.FrameworkElement rootElement);
	}}
}}";
	}


	protected override string CreateGetResourceCode(string resourceName, int varIndex)
	{
		return $@"this.Resources[""{resourceName}""] ?? global::System.Windows.Application.Current.Resources[""{resourceName}""] ?? throw new global::System.Exception(""Resource '{resourceName}' not found."")";
	}

	protected override void GenerateAdditionalClassCode(StringBuilder output, GeneratedClass parseResult, string className)
	{
		foreach (var bind in parseResult.EnumerateBindings())
		{
			output.AppendLine();
			output.AppendLine(
$@"	class {className}_{bind.Property.Object.Name}_{bind.Property.MemberName} : global::System.Windows.Markup.MarkupExtension, global::System.Windows.Data.IValueConverter
	{{");
			output.AppendLine(
$@"		public override object ProvideValue(global::System.IServiceProvider serviceProvider)
		{{
			return new global::System.Windows.Data.Binding
			{{
				Converter = this,
				Mode = global::System.Windows.Data.BindingMode.OneTime
			}};
		}}");

			output.AppendLine();
			output.AppendLine(
$@"		public object Convert(object value, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
		{{");
			if (bind.SourceType.Reference.FullName != "System.Object")
			{
				if (bind.SourceType.Reference.IsValueType)
				{
					output.AppendLine(
$@"			{bind.SourceType.Reference.GetCSharpFullName()} dataRoot;
			if (value is {bind.SourceType.Reference.GetCSharpFullName()})
			{{
				dataRoot = {bind.SourceType.Reference.GetCSharpFullName()};
			}}
			else
			{{
				return global::System.Windows.DependencyProperty.UnsetValue;
			}}");
				}
				else
				{
					output.AppendLine(
$@"			if (value is not {bind.SourceType.Reference.GetCSharpFullName()} dataRoot)
			{{
				return global::System.Windows.DependencyProperty.UnsetValue;
			}}");
				}
			}
			output.AppendLine(
$@"			return {bind.BindExpression!.ToString()};
		}}");

			output.AppendLine();
			output.AppendLine(
$@"		public object ConvertBack(object value, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
		{{
			throw new global::System.NotSupportedException();
		}}");
			output.AppendLine(
$@"	}}");
		}
	}
}

public class WpfBindingsCodeGenerator : BindingsCodeGenerator
{
	public WpfBindingsCodeGenerator(string compiledBindingsHelperNs, string langVersion, string msbuildVersion)
		: base(compiledBindingsHelperNs, langVersion, msbuildVersion)
	{
	}

	protected override string DependencyObjectType => "global::System.Windows.DependencyObject";

	protected override void GenerateSetDependencyPropertyChangedCallback(StringBuilder output, TwoWayBinding ev, string targetExpr)
	{
		var dp = ev.Bindings[0].DependencyProperty!;
		output.AppendLine(
$@"				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::{dp.Definition.DeclaringType.GetCSharpFullName()}.{dp.Definition.Name},
						typeof(global::{dp.Definition.DeclaringType.GetCSharpFullName()}))
					.AddValueChanged({targetExpr}, OnTargetChanged{ev.Index});");
	}

	protected override void GenerateUnsetDependencyPropertyChangedCallback(StringBuilder output, TwoWayBinding ev, string targetExpr)
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

	protected override void GenerateRegisterDependencyPropertyChangeEvent(StringBuilder output, NotifySource notifySource, NotifyProperty notifyProp, string cacheVar, string methodName)
	{
		output.AppendLine(
$@"						global::System.ComponentModel.DependencyPropertyDescriptor
							.FromProperty(
								global::{notifySource.Expression.Type.Reference.GetCSharpFullName()}.{notifyProp.Member!.Definition.Name}Property, typeof(global::{notifySource.Expression.Type.Reference.GetCSharpFullName()}))
							.AddValueChanged({cacheVar}, {methodName});");
	}

	protected override void GenerateUnregisterDependencyPropertyChangeEvent(StringBuilder output, NotifySource notifySource, NotifyProperty notifyProp, string cacheVar, string methodName)
	{
		output.AppendLine(
$@"						global::System.ComponentModel.DependencyPropertyDescriptor
							.FromProperty(
								global::{notifySource.Expression.Type.Reference.GetCSharpFullName()}.{notifyProp.Member!.Definition.Name}Property, typeof(global::{notifySource.Expression.Type.Reference.GetCSharpFullName()}))
							.RemoveValueChanged({cacheVar}, {methodName});");
	}
}

public static class WpfXamlProcessor
{
	public static void ProcessXaml(XDocument xdoc, SimpleXamlDom parseResult, string compiledBindingsHelperNs)
	{
		var localNs = "clr-namespace:" + parseResult.TargetType!.Reference.Namespace;
		string? localPrefix = null;

		bool generateDataTemplates = parseResult.DataTemplates.Any(dt => dt.GenerateClass);

		if (generateDataTemplates)
		{
			var compiledBindingsNs = $"clr-namespace:{compiledBindingsHelperNs}";

			ensureNamespaceDeclared(compiledBindingsNs);
			localPrefix = ensureNamespaceDeclared(localNs);

			var mbui = (XNamespace)compiledBindingsNs;
			var local = (XNamespace)localNs;

			for (int i = 0; i < parseResult.DataTemplates.Count; i++)
			{
				var dataTemplate = parseResult.DataTemplates[i];
				if (dataTemplate.GenerateClass)
				{
					var rootElement = dataTemplate.RootElement.Elements().First();

					rootElement.Add(
						new XElement(mbui + "CompiledBindingsHelper.Bindings",
							new XElement(local + $"{parseResult.TargetType.Reference.Name}_DataTemplate{i}",
								dataTemplate.EnumerateResources().Select(r => new XAttribute(r.name, $"{{StaticResource {r.name}}}")))));
				}
			}
		}

		replaceNativeBindings(parseResult.GeneratedClass, parseResult.TargetType.Reference.Name);
		for (int i = 0; i < parseResult.DataTemplates.Count; i++)
		{
			replaceNativeBindings(parseResult.DataTemplates[i], parseResult.TargetType.Reference.Name + "_DataTemplate" + i);
		}

		foreach (var obj in parseResult.EnumerateAllObjects().Where(o => !o.NameExplicitlySet && o.Name != null))
		{
			((XElement)obj.XamlNode.Element).Add(new XAttribute(WpfConstants.xName, obj.Name));
		}

		foreach (var prop in parseResult.EnumerateAllProperties()
			.Where(p => p.Value.BindingValue == null || ((XAttribute)p.XamlNode.Element).Name.Namespace != XNamespace.None))
		{
			var prop2 = prop.XamlNode.Element.Parent.Attribute(prop.XamlNode.Name);
			prop2?.Remove();
		}

		void replaceNativeBindings(GeneratedClass generatedClass, string className)
		{
			foreach (var bind in generatedClass.EnumerateBindings())
			{
				localPrefix ??= ensureNamespaceDeclared(localNs);
				var name = $"{className}";
				var value = $"{{{localPrefix}:{className}_{bind.Property.Object.Name}_{bind.Property.MemberName}}}";
				var attr = (XAttribute)bind.Property.XamlNode.Element;
				var element = attr.Parent;
				if (attr.Name.Namespace == XNamespace.None)
				{
					attr.Value = value;
				}
				else
				{
					element.Add(new XAttribute(attr.Name.LocalName, value));
				}
			}
		}

		string ensureNamespaceDeclared(string searchedClrNs)
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
				return classNsPrefix;
			}

			return attr.Name.LocalName;
		}
	}
}

public static class WpfConstants
{
	public readonly static XNamespace xNs = "http://schemas.microsoft.com/winfx/2006/xaml";
	public readonly static XName xName = xNs + "Name";
}

