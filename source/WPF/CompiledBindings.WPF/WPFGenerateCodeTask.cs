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

namespace CompiledBindings
{
	public class WPFGenerateCodeTask : Task, ICancelableTask
	{
		private CancellationTokenSource? _cancellationTokenSource;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		[Required]
		public ITaskItem[] ReferenceAssemblies { get; set; }

		[Required]
		public string LocalAssembly { get; set; }

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

				if (ApplicationDefinition != null)
				{
					NewApplicationDefinition = ProcessXaml(ApplicationDefinition);
				}
				foreach (var page in Pages)
				{
					if (_cancellationTokenSource.IsCancellationRequested)
					{
						return true;
					}

					newPages.Add(ProcessXaml(page));
				}

				ITaskItem ProcessXaml(ITaskItem xaml)
				{
					var newXaml = xaml;

					var file = xaml.GetMetadata("FullPath");

					var targetRelativePath = xaml.GetMetadata("Link");
					if (string.IsNullOrEmpty(targetRelativePath))
					{
						targetRelativePath = xaml.ItemSpec;
					}

					var targetDir = Path.Combine(IntermediateOutputPath, Path.GetDirectoryName(targetRelativePath));
					var sourceCodeTargetPath = Path.Combine(targetDir, Path.GetFileNameWithoutExtension(targetRelativePath) + ".g.m.cs");
					var xamlFile = Path.Combine(IntermediateOutputPath, targetRelativePath);

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

										return newXaml;
									}
								}
							}
						}
					}

					try
					{
						var xdoc = XDocument.Load(file, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
						var xclass = xdoc.Root.Attribute(xamlDomParser.xClass);
						if (xclass != null)
						{
							var parseResult = xamlDomParser.Parse(file, xdoc);

							if (parseResult.GenerateCode)
							{
								parseResult.SetDependecyPropertyChangedEventHandlers("System.Windows.DependencyProperty");

								var codeGenerator = new WpfCodeGenerator();
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

					return newXaml;
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
			((IGeneratedDataTemplate)e.NewValue).Initialize((FrameworkElement)d);
		}}
	}}

	public interface IGeneratedDataTemplate
	{{
		void Initialize(FrameworkElement rootElement);
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
		private static readonly XNamespace _xmlns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
		private static ILookup<string, string>? _nsMappings = null;

		public WpfXamlDomParser()
			: base(_xmlns,
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
					})
		{
			DependencyObjectType = TypeInfoUtils.GetTypeThrow("System.Windows.DependencyObject");
		}

		public override bool IsMemExtension(XAttribute a)
		{
			return base.IsMemExtension(a) || a.Value.StartsWith("{x:Bind ");
		}

		protected override Expression GenerateConvertExpression(Bind bind)
		{
			var objectType = TypeInfoUtils.GetTypeThrow(typeof(object));
			var frameworkElementType = TypeInfoUtils.GetTypeThrow("System.Windows.FrameworkElement");
			var resourceDictionaryType = TypeInfoUtils.GetTypeThrow("System.Windows.ResourceDictionary");
			var resourcesProp = frameworkElementType.Properties.Single(p => p.Name == "Resources");
			var converterType = TypeInfoUtils.GetTypeThrow("System.Windows.Data.IValueConverter");
			var convertMethod = converterType.Methods.First(m => m.Name == "Convert");
			var objectField = new FieldDefinition(bind.Property.Object.Name, FieldAttributes.Private, TargetType);

			Expression expression = new ParameterExpression(new TypeInfo(TargetType, false), "targetRoot");
			expression = new MemberExpression(expression, objectField, new TypeInfo(converterType, false));
			expression = new MemberExpression(expression, resourcesProp, new TypeInfo(resourceDictionaryType, false));
			expression = new ElementAccessExpression(new TypeInfo(objectType, false), expression, new Expression[] { new ConstantExpression(bind.Converter) });
			expression = new CastExpression(expression, new TypeInfo(converterType, false));
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

	public class WpfCodeGenerator : SimpleXamlDomCodeGenerator
	{
		public WpfCodeGenerator()
			: base(new WpfBindingsCodeGenerator(),
				   "Data",
				   "System.Windows.DependencyPropertyChangedEventArgs",
				   "System.Windows.FrameworkElement",
				   "(global::{0}){1}.FindName(\"{2}\")",
				   false,
				   false)
		{
		}
	}

	public class WpfBindingsCodeGenerator : BindingsCodeGenerator
	{
		protected override void GenerateSetDependencyPropertyChangedCallback(StringBuilder output, TwoWayEventData ev, string targetExpr)
		{
			var prop = ev.Bindings[0].Property;
			output.AppendLine(
$@"				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::{prop.Object.Type.Type.GetCSharpFullName()}.{prop.MemberName}Property, typeof(global::{prop.Object.Type.Type.GetCSharpFullName()}))
					.AddValueChanged({targetExpr}, OnTargetChanged{ev.Index});");
		}

		protected override void GenerateUnsetDependencyPropertyChangedCallback(StringBuilder output, TwoWayEventData ev, string targetExpr)
		{
			var prop = ev.Bindings[0].Property;
			output.AppendLine(
$@"					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::{prop.Object.Type.Type.GetCSharpFullName()}.{prop.MemberName}Property, typeof(global::{prop.Object.Type.Type.GetCSharpFullName()}))
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

		protected override string GenerateConvertBackCall(string memberExpr, string converterName, string value, string targetType, string parameter, string? cast)
		{
			return $"{cast}((global::System.Windows.Data.IValueConverter){memberExpr}.Resources[\"{converterName}\"]).ConvertBack({value}, typeof(global::{targetType}), {parameter}, null)";
		}
	}
}
