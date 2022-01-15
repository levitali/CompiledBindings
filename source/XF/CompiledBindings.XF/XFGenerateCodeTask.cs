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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace CompiledBindings;

public class XFGenerateCodeTask : Task, ICancelableTask
{
	private CancellationTokenSource? _cancellationTokenSource;
	private PlatformConstants _platformConstants;

	public XFGenerateCodeTask() : this(new PlatformConstants())
	{
	}

	public XFGenerateCodeTask(PlatformConstants platformConstants)
	{
		_platformConstants = platformConstants;
	}

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

	[Required]
	public ITaskItem[] XamlFiles { get; set; }

	[Output]
	public ITaskItem[] GeneratedCodeFiles { get; private set; }

	public bool AttachDebugger { get; set; }

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

			var xamlDomParser = new XFXamlDomParser(_platformConstants);

			var generatedCodeFiles = new List<TaskItem>();
			bool generateDataTemplateBindings = false;

			var xamlFiles = XamlFiles
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
				if (_cancellationTokenSource.IsCancellationRequested)
				{
					return true;
				}

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

						var parseResult = xamlDomParser.Parse(file, lineFile, xdoc);

						if (parseResult.GenerateCode)
						{
							var codeGenerator = new XFCodeGenerator(LangVersion, MSBuildVersion, _platformConstants);
							string code = codeGenerator.GenerateCode(parseResult);

							bool dataTemplates = parseResult.DataTemplates.Count > 0;
							generateDataTemplateBindings |= dataTemplates;

							var targetDir = Path.Combine(IntermediateOutputPath, Path.GetDirectoryName(targetRelativePath));
							var dirInfo = new DirectoryInfo(targetDir);
							dirInfo.Create();

							var sourceCodeTargetPath = Path.Combine(targetDir, Path.GetFileNameWithoutExtension(targetRelativePath) + ".g.m.cs");
							File.WriteAllText(sourceCodeTargetPath, code);

							generatedCodeFiles.Add(new TaskItem(sourceCodeTargetPath));
						}
					}
				}
				catch (Exception ex) when (ex is not GeneratorException)
				{
					Log.LogError(null, null, null, file, 0, 0, 0, 0, ex.Message);
					return false;
				}
			}

			if (generateDataTemplateBindings)
			{
				var dataTemplateBindingsFile = Path.Combine(IntermediateOutputPath, "DataTemplateBindings.cs");
				File.WriteAllText(dataTemplateBindingsFile, GenerateDataTemplateBindingsClass());
				generatedCodeFiles.Add(new TaskItem(dataTemplateBindingsFile));
			}

			GeneratedCodeFiles = generatedCodeFiles.ToArray();

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
	using {_platformConstants.BaseClrNamespace};

	public class DataTemplateBindings
	{{
		public static readonly BindableProperty BindingsProperty =
			BindableProperty.CreateAttached(""Bindings"", typeof(IGeneratedDataTemplate), typeof(DataTemplateBindings), null, propertyChanged: BindingsChanged);

		public static IGeneratedDataTemplate GetBindings(BindableObject @object)
		{{
			return (IGeneratedDataTemplate)@object.GetValue(BindingsProperty);
		}}

		public static void SetBindings(BindableObject @object, IGeneratedDataTemplate value)
		{{
			@object.SetValue(BindingsProperty, value);
		}}

		public static readonly BindableProperty RootProperty =
			BindableProperty.CreateAttached(""Root"", typeof(VisualElement), typeof(DataTemplateBindings), null);

		public static VisualElement GetRoot(BindableObject @object)
		{{
			return (VisualElement)@object.GetValue(RootProperty);
		}}

		public static void SetRoot(BindableObject @object, VisualElement value)
		{{
			@object.SetValue(RootProperty, value);
		}}

		static void BindingsChanged(BindableObject bindable, object oldValue, object newValue)
		{{
			if (oldValue != null)
			{{
				((IGeneratedDataTemplate)oldValue).Cleanup((Element)bindable);
			}}
			if (newValue != null)
			{{
				((IGeneratedDataTemplate)newValue).Initialize((Element)bindable);
			}}			
		}}
	}}

	public interface IGeneratedDataTemplate
	{{
		void Initialize(Element rootElement);
		void Cleanup(Element rootElement);
	}}
}}";
	}

	void ICancelableTask.Cancel()
	{
		_cancellationTokenSource?.Cancel();
	}
}

public class XFXamlDomParser : SimpleXamlDomParser
{
	private static ILookup<string, string>? _nsMappings;

	public XFXamlDomParser(PlatformConstants platformConstants) : base(
		platformConstants.DefaultNamespace,
		"http://schemas.microsoft.com/winfx/2009/xaml",
		getClrNsFromXmlNs: xmlNs =>
		{
			if (_nsMappings == null)
			{
				string attrName = $"{platformConstants.BaseClrNamespace}.XmlnsDefinitionAttribute";
				_nsMappings = TypeInfoUtils.Assemblies
					.SelectMany(ass => ass.CustomAttributes.Where(at => at.AttributeType.FullName == attrName))
					.Select(at => (
						XmlNamespace: (string)at.ConstructorArguments[0].Value,
						ClrNamespace: (string)at.ConstructorArguments[1].Value))
					.ToLookup(at => at.XmlNamespace, at => at.ClrNamespace);
				// For old XF versions
				if (_nsMappings.Count == 0)
				{
					_nsMappings = new[] { (XmlNamespace: "http://xamarin.com/schemas/2014/forms", ClrNamespace: "Xamarin.Forms") }
					.ToLookup(at => at.XmlNamespace, at => at.ClrNamespace);
				}
			}
			return _nsMappings[xmlNs];
		},
		TypeInfo.GetTypeThrow($"{platformConstants.BaseClrNamespace}.IValueConverter"))
	{
	}

	public override bool IsMemExtension(XAttribute a)
	{
		return base.IsMemExtension(a) || a.Value.StartsWith("{x:Bind ");
	}
}

public class XFCodeGenerator : SimpleXamlDomCodeGenerator
{
	PlatformConstants _platformConstants;

	public XFCodeGenerator(string langVersion, string msbuildVersion, PlatformConstants platformConstants)
		: base(new BindingsCodeGenerator(langVersion, msbuildVersion),
			   "Binding",
			   "System.EventArgs",
			   $"{platformConstants.BaseClrNamespace}.Element",
			   "global::" + platformConstants.BaseClrNamespace + ".NameScopeExtensions.FindByName<global::{0}>({1}, \"{2}\")",
			   true,
			   true,
			   langVersion,
			   msbuildVersion)
	{
		_platformConstants = platformConstants;
	}

	protected override void GenerateInitializeResources(StringBuilder output, SimpleXamlDom parseResult, string rootElement, bool isDataTemplate)
	{
		var resources = parseResult.XamlObjects.SelectMany(o => o.Properties).Select(p => p.Value.BindValue).Where(b => b != null).SelectMany(b => b!.Resources).Distinct(b => b.name).ToList();
		if (resources.Count > 0)
		{
			string root1, root2;
			if (isDataTemplate)
			{
				output.AppendLine(
$@"			var root = global::CompiledBindings.DataTemplateBindings.GetRoot({rootElement});");
				root1 = "root?";
				root2 = "root";
			}
			else
			{
				root1 = "this";
				root2 = "this";
			}

			foreach (var resource in resources)
			{
				output.AppendLine(
$@"			{resource.name} = (global::{resource.type.Type.GetCSharpFullName()})({root1}.Resources.ContainsKey(""{resource.name}"") == true ? {root2}.Resources[""{resource.name}""] : global::{_platformConstants.BaseClrNamespace}.Application.Current.Resources[""{resource.name}""]);");
			}

			output.AppendLine();
		}
	}
}

public class PlatformConstants
{
	public virtual string BaseClrNamespace => "Xamarin.Forms";
	public virtual string DefaultNamespace => "http://xamarin.com/schemas/2014/forms";
}

