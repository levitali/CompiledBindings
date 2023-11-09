using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CompiledBindings;

public class XFGenerateCodeTask : Task, ICancelableTask
{
	private CancellationTokenSource? _cancellationTokenSource;
	private readonly PlatformConstants _platformConstants;

	public XFGenerateCodeTask() : this(new PlatformConstants())
	{
	}

	public XFGenerateCodeTask(PlatformConstants platformConstants)
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

	[Required]
	public required ITaskItem[] XamlFiles { get; init; }

	[Output]
	public ITaskItem[] GeneratedCodeFiles { get; private set; } = null!;

	public string? Nullable { get; init; }

	public bool AttachDebugger { get; init; }

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

			var xamlDomParser = new XFXamlDomParser(_platformConstants);

			var generatedCodeFiles = new List<TaskItem>();
			bool result = true;

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

						var parseResult = xamlDomParser.Parse(file, lineFile, xdoc, (line, startColumn, endColumn, message) => Log.LogError(null, null, null, file, line, startColumn, line, endColumn, message));
						if (parseResult == null)
						{
							result = false;
						}
						else if (parseResult.GenerateCode)
						{
							var codeGenerator = new XFCodeGenerator(LangVersion, MSBuildVersion, _platformConstants);
							string code = codeGenerator.GenerateCode(parseResult);

							code = GenerateUtils.GeneratedCodeHeader + Environment.NewLine + code;

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

			if (generatedCodeFiles.Count > 0 &&
				!TypeInfoUtils.TryGetType($"CompiledBindings.{_platformConstants.FrameworkId}.CompiledBindingsHelper", out _))
			{
				var dataTemplateBindingsFile = Path.Combine(IntermediateOutputPath, $"CompiledBindingsHelper.{_platformConstants.FrameworkId}.cs");
				File.WriteAllText(dataTemplateBindingsFile, GenerateCompiledBindingsHelper());
				generatedCodeFiles.Add(new TaskItem(dataTemplateBindingsFile));
			}

			GeneratedCodeFiles = generatedCodeFiles.ToArray();

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

	private string GenerateCompiledBindingsHelper()
	{
		return
$@"namespace CompiledBindings.{_platformConstants.FrameworkId}
{{
	public class CompiledBindingsHelper
	{{
		public static void SetPropertyChangedEventHandler(ref global::System.ComponentModel.INotifyPropertyChanged? cache, global::System.ComponentModel.INotifyPropertyChanged? source, global::System.ComponentModel.PropertyChangedEventHandler handler)
		{{
			if (cache != null && !object.ReferenceEquals(cache, source))
			{{
				cache.PropertyChanged -= handler;
				cache = null;
			}}
			if (cache == null && source != null)
			{{
				cache = source;
				cache.PropertyChanged += handler;
			}}
		}}

		public static void SetPropertyChangedEventHandler(ref global::System.ComponentModel.INotifyPropertyChanged? cache, object? source, global::System.ComponentModel.PropertyChangedEventHandler handler)
		{{
			if (cache != null && !object.ReferenceEquals(cache, source))
			{{
				cache.PropertyChanged -= handler;
				cache = null;
			}}
			if (cache == null && source is System.ComponentModel.INotifyPropertyChanged npc)
			{{
				cache = npc;
				cache.PropertyChanged += handler;
			}}
		}}

		public static T? TryGetBindings<T>(ref global::System.WeakReference? bindingsWeakReference, global::System.Action cleanup)
			where T : class
		{{
			T? bindings = null;
			if (bindingsWeakReference != null)
			{{
				bindings = (T?)bindingsWeakReference.Target;
				if (bindings == null)
				{{
					bindingsWeakReference = null;
					cleanup();
				}}
			}}
			return bindings;
		}}

		public static readonly global::{_platformConstants.BaseClrNamespace}.BindableProperty BindingsProperty =
			global::{_platformConstants.BaseClrNamespace}.BindableProperty.CreateAttached(""Bindings"", typeof(IGeneratedDataTemplate), typeof(CompiledBindingsHelper), null, propertyChanged: BindingsChanged);

		public static IGeneratedDataTemplate GetBindings(global::{_platformConstants.BaseClrNamespace}.BindableObject @object)
		{{
			return (IGeneratedDataTemplate)@object.GetValue(BindingsProperty);
		}}

		public static void SetBindings(global::{_platformConstants.BaseClrNamespace}.BindableObject @object, IGeneratedDataTemplate value)
		{{
			@object.SetValue(BindingsProperty, value);
		}}

		static void BindingsChanged(global::{_platformConstants.BaseClrNamespace}.BindableObject bindable, object oldValue, object newValue)
		{{
			if (oldValue != null)
			{{
				((IGeneratedDataTemplate)oldValue).Cleanup((global::{_platformConstants.BaseClrNamespace}.Element)bindable);
			}}
			if (newValue != null)
			{{
				((IGeneratedDataTemplate)newValue).Initialize((global::{_platformConstants.BaseClrNamespace}.Element)bindable);
			}}
		}}
	}}

	public interface IGeneratedDataTemplate
	{{
		void Initialize(global::{_platformConstants.BaseClrNamespace}.Element rootElement);
		void Cleanup(global::{_platformConstants.BaseClrNamespace}.Element rootElement);
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
	private readonly static XNamespace xNs = "http://schemas.microsoft.com/winfx/2009/xaml";
	private PlatformConstants _platformConstants;
	private ILookup<string, string>? _nsMappings;

	public XFXamlDomParser(PlatformConstants platformConstants) : base(
		platformConstants.DefaultNamespace,
		xNs,
		TypeInfo.GetTypeThrow($"{platformConstants.BaseClrNamespace}.IValueConverter"),
		TypeInfo.GetTypeThrow($"{platformConstants.BaseClrNamespace}.BindingBase"))
	{
		_platformConstants = platformConstants;
	}

	protected override IEnumerable<string> GetClrNsFromXmlNs(string xmlNs)
	{
		if (_nsMappings == null)
		{
			string attrName = $"{_platformConstants.BaseClrNamespace}.XmlnsDefinitionAttribute";
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
	}	

	protected override ExtenstionType? IsMemExtension(XName name)
	{
		var res = base.IsMemExtension(name);
		if (res == null)
		{
			if (name.Namespace == xNs)
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
}

public class XFCodeGenerator : SimpleXamlDomCodeGenerator
{
	private readonly PlatformConstants _platformConstants;

	public XFCodeGenerator(string langVersion, string msbuildVersion, PlatformConstants platformConstants)
		: base(new BindingsCodeGenerator(platformConstants.FrameworkId, langVersion, msbuildVersion),
			   "Binding",
			   "System.EventArgs",
			   $"{platformConstants.BaseClrNamespace}.Element",
			   "global::" + platformConstants.BaseClrNamespace + ".NameScopeExtensions.FindByName<global::{0}>({1}, \"{2}\")",
			   true,
			   true,
			   platformConstants.FrameworkId,
			   langVersion,
			   msbuildVersion)
	{
		_platformConstants = platformConstants;
	}

	protected override string CreateGetResourceCode(string resourceName, int varIndex)
	{
		return $@"this.Resources.TryGetValue(""{resourceName}"", out var r{varIndex}) || global::{_platformConstants.BaseClrNamespace}.Application.Current.Resources.TryGetValue(""{resourceName}"", out r{varIndex}) ? r{varIndex} : throw new global::System.Exception(""Resource '{resourceName}' not found."")";
	}

	protected override void GenerateAdditionalClassCode(StringBuilder output, GeneratedClass parseResult, string className)
	{
		var iNotifyPropertyChangedType = TypeInfo.GetTypeThrow(typeof(INotifyPropertyChanged));
		bool isLineDirective = false;

		foreach (var bind in parseResult.EnumerateBindings())
		{
			output.AppendLine();
			output.AppendLine(
$@"	class {className}_{bind.Property.Object.Name}_{bind.Property.MemberName} : global::{_platformConstants.BaseClrNamespace}.Xaml.IMarkupExtension
	{{");
			var resultType = bind.Expression!.Type.Reference.GetCSharpFullName();
			if (bind.Expression!.Type.Reference.IsValueType && bind.Expression.IsNullable)
			{
				resultType += '?';
			}

			output.AppendLine(
$@"		global::{_platformConstants.BaseClrNamespace}.Internals.TypedBindingBase _binding = new global::{_platformConstants.BaseClrNamespace}.Internals.TypedBinding<global::{bind.SourceType.Reference.GetCSharpFullName()}, global::{resultType}>(");

			var checkNull = bind.SourceType.Reference.IsValueType ? null : "dataRoot == null ? (default, false) : ";
			output.AppendLine(
$@"			dataRoot => {checkNull}(
{LineDirective(bind.Property.XamlNode, ref isLineDirective)}
				{bind.SourceExpression!.CSharpCode},
{ResetLineDirective(ref isLineDirective)}
				true),");

			output.AppendLine(
$@"			null,");

			var notifySources = BindingParser.GetNotifySources(new[] { bind }, iNotifyPropertyChangedType, null);
			if (notifySources.Count > 0)
			{
				output.AppendLine(
$@"			new[]
			{{");

				foreach (var source in notifySources)
				{
					foreach (var prop in source.Properties)
					{
						output.AppendLine(
$@"				new global::System.Tuple<global::System.Func<global::{bind.SourceType.Reference.GetCSharpFullName()}, object>, string>(dataRoot =>
{LineDirective(bind.Property.XamlNode, ref isLineDirective)}
					{source.SourceExpression},
{ResetLineDirective(ref isLineDirective)}
					""{prop.Member!.Definition.Name}""),");
					}
				}

				output.Append(
$@"			}}");
			}
			else
			{
				output.Append(
$@"			null");
			}
			output.AppendLine(");");

			output.AppendLine();
			output.AppendLine(
$@"		public object ProvideValue(global::System.IServiceProvider serviceProvider)
		{{
			return _binding;
		}}");

			output.AppendLine(
$@"	}}");
		}
	}
}

public class PlatformConstants
{
	public virtual string FrameworkId => "XF";
	public virtual string BaseClrNamespace => "Xamarin.Forms";
	public virtual string DefaultNamespace => "http://xamarin.com/schemas/2014/forms";
}

