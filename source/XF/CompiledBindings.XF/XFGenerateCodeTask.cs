using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Cecil;

#nullable enable
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace CompiledBindings
{
	public class XFGenerateCodeTask : Task, ICancelableTask
	{
		private CancellationTokenSource? _cancellationTokenSource;

		[Required]
		public string LangVersion { get; set; }
		
		[Required]
		public ITaskItem[] ReferenceAssemblies { get; set; }

		[Required]
		public string LocalAssembly { get; set; }

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

				var xamlDomParser = new XFXamlDomParser();

				var generatedCodeFiles = new List<TaskItem>();
				bool generateDataTemplateBindings = false;

				foreach (var xaml in XamlFiles.Distinct(f => f.GetMetadata("FullPath")))
				{
					if (_cancellationTokenSource.IsCancellationRequested)
					{
						return true;
					}

					var file = xaml.GetMetadata("FullPath");
					try
					{
						var xdoc = XDocument.Load(file, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
						var xclass = xdoc.Root.Attribute(xamlDomParser.xClass);
						if (xclass != null)
						{
							var parseResult = xamlDomParser.Parse(file, xdoc);

							if (parseResult.GenerateCode)
							{
								var codeGenerator = new XFCodeGenerator(LangVersion);
								string code = codeGenerator.GenerateCode(parseResult);

								bool dataTemplates = parseResult.DataTemplates.Count > 0;
								generateDataTemplateBindings |= dataTemplates;

								var targetRelativePath = xaml.GetMetadata("Link");
								if (string.IsNullOrEmpty(targetRelativePath))
								{
									targetRelativePath = xaml.ItemSpec;
								}

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
@"namespace CompiledBindings
{
	using Xamarin.Forms;

	public class DataTemplateBindings
	{
		public static readonly BindableProperty BindingsProperty =
			BindableProperty.CreateAttached(""Bindings"", typeof(IGeneratedDataTemplate), typeof(DataTemplateBindings), null, propertyChanged: BindingsChanged);

		public static IGeneratedDataTemplate GetBindings(BindableObject @object)
		{
			return (IGeneratedDataTemplate)@object.GetValue(BindingsProperty);
		}

		public static void SetBindings(BindableObject @object, IGeneratedDataTemplate value)
		{
			@object.SetValue(BindingsProperty, value);
		}

		public static readonly BindableProperty RootProperty =
			BindableProperty.CreateAttached(""Root"", typeof(VisualElement), typeof(DataTemplateBindings), null);

		public static VisualElement GetRoot(BindableObject @object)
		{
			return (VisualElement)@object.GetValue(RootProperty);
		}

		public static void SetRoot(BindableObject @object, VisualElement value)
		{
			@object.SetValue(RootProperty, value);
		}

		static void BindingsChanged(BindableObject bindable, object oldValue, object newValue)
		{
			((IGeneratedDataTemplate)newValue).Initialize((Element)bindable);
		}
	}

	public interface IGeneratedDataTemplate
	{
		void Initialize(global::Xamarin.Forms.Element rootElement);
	}
}";
		}

		void ICancelableTask.Cancel()
		{
			_cancellationTokenSource?.Cancel();
		}
	}

	public class XFXamlDomParser : SimpleXamlDomParser
	{
		private static ILookup<string, string>? _nsMappings;

		public XFXamlDomParser() : base(
			"http://xamarin.com/schemas/2014/forms",
			"http://schemas.microsoft.com/winfx/2009/xaml",
			getClrNsFromXmlNs: xmlNs =>
			{
				if (_nsMappings == null)
				{
					_nsMappings = TypeInfoUtils.Assemblies
						.SelectMany(ass => ass.CustomAttributes.Where(at => at.AttributeType.FullName == "Xamarin.Forms.XmlnsDefinitionAttribute"))
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
			})
		{
		}

		public override bool IsMemExtension(XAttribute a)
		{
			return base.IsMemExtension(a) || a.Value.StartsWith("{x:Bind ");
		}

		protected override Expression GenerateConvertExpression(Bind bind)
		{
			var converterType = TypeInfoUtils.GetTypeThrow("Xamarin.Forms.IValueConverter");
			var convertMethod = converterType.Methods.First(m => m.Name == "Convert");
			var converterField = new FieldDefinition(bind.Converter, FieldAttributes.Private, converterType);

			Expression expression = new ParameterExpression(new TypeInfo(TargetType, false), "targetRoot");
			expression = new MemberExpression(expression, converterField, new TypeInfo(converterType, false));
			expression = new CallExpression(expression, convertMethod, new Expression[]
			{
				bind.Expression,
				new TypeofExpression(new TypeExpression(bind.Property.MemberType)),
				bind.ConverterParameter ?? Expression.NullExpression,
				Expression.NullExpression
			});
			if (bind.Property.MemberType.Type.FullName != "System.Object")
			{
				expression = new CastExpression(expression, bind.Property.MemberType, false);
			}

			return expression;
		}
	}

	public class XFCodeGenerator : SimpleXamlDomCodeGenerator
	{
		public XFCodeGenerator(string langVersion)
			: base(new BindingsCodeGenerator(langVersion),
				   "Binding",
				   "System.EventArgs",
				   "Xamarin.Forms.Element",
				   "global::Xamarin.Forms.NameScopeExtensions.FindByName<global::{0}>({1}, \"{2}\")",
				   true,
				   true,
				   langVersion)
		{
		}

		protected override void GenerateConverterDeclarations(StringBuilder output, SimpleXamlDom parseResult)
		{
			var converters = parseResult.XamlObjects.SelectMany(o => o.Properties).Select(p => p.Value.BindValue?.Converter).Where(c => c != null).Distinct();
			foreach (var converter in converters)
			{
				output.AppendLine(
$@"		global::Xamarin.Forms.IValueConverter {converter};");
			}
		}

		protected override void GenerateInitializeConverters(StringBuilder output, SimpleXamlDom parseResult, string rootElement, bool isDataTemplate)
		{
			var converters = parseResult.XamlObjects.SelectMany(o => o.Properties).Select(p => p.Value.BindValue?.Converter).Where(c => c != null).Distinct().ToList();
			if (converters.Count > 0)
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

				foreach (var converter in converters)
				{
					output.AppendLine(
$@"			{converter} = (global::Xamarin.Forms.IValueConverter)({root1}.Resources.ContainsKey(""{converter}"") == true ? {root2}.Resources[""{converter}""] : global::Xamarin.Forms.Application.Current.Resources[""{converter}""]);");
				}

				output.AppendLine();
			}
		}
	}
}
