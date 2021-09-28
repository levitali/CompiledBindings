using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable

namespace CompiledBindings
{
	public class SimpleXamlDomCodeGenerator
	{
		BindingsCodeGenerator _bindingsCodeGenerator;
		string _bindingContextStart;
		string _bindingConextArgs;
		string _bindableObject;
		string _findByNameFormat;
		bool _generateVariableDeclarations;
		bool _generateVariableInitialization;

		public SimpleXamlDomCodeGenerator(BindingsCodeGenerator bindingsCodeGenerator,
										  string bindingContextStart,
										  string bindingConextArgs,
										  string bindableObject,
										  string findByNameFormat,
										  bool generateVariableDeclarations,
										  bool generateVariableInitialization)
		{
			_bindingsCodeGenerator = bindingsCodeGenerator;
			_bindingContextStart = bindingContextStart;
			_bindingConextArgs = bindingConextArgs;
			_bindableObject = bindableObject;
			_findByNameFormat = findByNameFormat;
			_generateVariableDeclarations = generateVariableDeclarations;
			_generateVariableInitialization = generateVariableInitialization;
		}

		public string GenerateCode(SimpleXamlDom parseResult)
		{
			var output = new StringBuilder();

			if (parseResult.TargetType!.Type.Namespace != null)
			{
				output.AppendLine(
$@"namespace {parseResult.TargetType.Type.Namespace}
{{");
			}

			var usings = parseResult.EnumerateAllProperties().SelectMany(p => p.IncludeNamespaces).Distinct().ToList();
			if (usings.Count > 0)
			{
				foreach (string ns in usings)
				{
					output.AppendLine(
$@"	using {ns};");
				}
				output.AppendLine();
			}

			output.AppendLine(
$@"#pragma warning disable 8600
#pragma warning disable 8603
#pragma warning disable 8618
#pragma warning disable 8625

	[System.CodeDom.Compiler.GeneratedCode(""CompiledBindings"", null)]
	partial class {parseResult.TargetType.Type.Name}
	{{");

			if (parseResult.GenerateInitializeMethod)
			{
				GenerateInitializeMethod(output, parseResult);
			}
			GenerateBindings(output, parseResult, parseResult.TargetType.Type.Namespace, parseResult.TargetType.Type.Name);

			output.AppendLine(
$@"	}}");

			GenerateDataTemplates(output, parseResult, parseResult.TargetType.Type.Namespace, parseResult.TargetType.Type.Name);

			if (parseResult.TargetType.Type.Namespace != null)
			{
				output.AppendLine(
"}");
			}

			return output.ToString();
		}

		protected virtual void GenerateConverterDeclarations(StringBuilder output, SimpleXamlDom parseResult)
		{
		}

		protected virtual void GenerateInitializeConverters(StringBuilder output, SimpleXamlDom parseResult, string rootElement, bool isDataTemplate)
		{
		}

		private void GenerateInitializeMethod(StringBuilder output, SimpleXamlDom parseResult)
		{
			if (_generateVariableDeclarations)
			{
				GenerateVariablesDeclarations(output, parseResult, true);
			}
			GenerateConverterDeclarations(output, parseResult);

			output.AppendLine(
$@"		private bool _generatedCodeInitialized;

		private void InitializeAfterConstructor()
		{{
			if (_generatedCodeInitialized)
				return;

			_generatedCodeInitialized = true;
");

			GenerateInitializeMethodBody(output, parseResult, "this", false);

			output.AppendLine(
$@"		}}");

			GenerateDestructorMethod(output, parseResult, "this");

			GenerateBindingContextChangedHandlers(output, parseResult);
		}

		private void GenerateDestructorMethod(StringBuilder output, SimpleXamlDom parseResult, string rootElement)
		{
			if (parseResult.BindingScopes.Count > 0)
			{
				output.AppendLine();

				if (parseResult.HasDestructor)
				{
					output.AppendLine(
$@"		private void DeinitializeAfterDestructor()");
				}
				else
				{
					output.AppendLine(
$@"		~{parseResult.TargetType!.Type.Name}()");
				}
				output.AppendLine(
$@"		{{");

				foreach (var bs in parseResult.BindingScopes)
				{
					var viewName = bs.ViewName ?? (bs.DataType != null ? rootElement : null);
					output.AppendLine(
$@"			if (Bindings_{viewName} != null)
			{{
				Bindings_{viewName}.Cleanup();
			}}");
				}

				output.AppendLine(
$@"		}}");
			}
		}

		private void GenerateBindingContextChangedHandlers(StringBuilder output, SimpleXamlDom parseResult)
		{
			foreach (var bs in parseResult.BindingScopes.Where(b => b.DataType != null))
			{
				var viewName = bs.ViewName ?? "this";
				var prm = bs.DataType!.Type.FullName == parseResult.TargetType?.Type.FullName ? null : $", dataRoot";

				output.AppendLine(
$@"
		private void {bs.ViewName ?? "this"}_{_bindingContextStart}ContextChanged(object sender, {_bindingConextArgs} e)
		{{
			Bindings_{viewName}.Cleanup();");

				string? a = null;
				if (bs.DataType != null)
				{
					output.AppendLine(
$@"			if (((global::{_bindableObject})sender).{_bindingContextStart}Context is {bs.DataType.Type.FullName} dataRoot)
			{{");
					a = "\t";
				}
				output.AppendLine(
$@"{a}			Bindings_{viewName}.Initialize(this{prm});");
				if (bs.DataType != null)
				{
					output.AppendLine(
$@"			}}");
				}

				output.AppendLine(
$@"		}}");
			}

		}

		private void GenerateInitializeMethodBody(StringBuilder output, SimpleXamlDom parseResult, string rootElement, bool isDataTemplate)
		{
			if (isDataTemplate || _generateVariableInitialization)
			{
				IEnumerable<XamlObject> objects = parseResult.XamlObjects!;
				if (!isDataTemplate)
				{
					objects = objects.Where(o => !o.NameExplicitlySet);
				}

				foreach (var obj in objects.Where(o => o.Name != null))
				{
					output.AppendLine(
$@"			{obj.Name} = {string.Format(_findByNameFormat, obj.Type.Type.GetCSharpFullName(), rootElement, obj.Name)};");
				}
				output.AppendLine();
			}

			GenerateInitializeConverters(output, parseResult, rootElement, isDataTemplate);

			_bindingsCodeGenerator.GenerateUpdateMethodBody(output, parseResult.StaticUpdate);
			output.AppendLine();

			for (int i = 0; i < parseResult.BindingScopes.Count; i++)
			{
				var bs = parseResult.BindingScopes[i];
				var viewName = bs.ViewName ?? (bs.DataType != null ? rootElement : null);
				if (bs.DataType == null)
				{
					output.AppendLine(
$@"			Bindings_{viewName}.Initialize(this);");
				}
				else
				{
					var prm = bs.DataType.Type.FullName == parseResult.TargetType?.Type.FullName ? null : $", dataRoot" + i;
					output.AppendLine(
$@"			{viewName}.{_bindingContextStart}ContextChanged += {viewName}_{_bindingContextStart}ContextChanged;
			if ({viewName}.{_bindingContextStart}Context is {bs.DataType.Type.FullName} dataRoot{i})
			{{
				Bindings_{viewName}.Initialize(this{prm});
			}}");
				}
			}
		}

		private void GenerateBindings(StringBuilder output, SimpleXamlDom parseResult, string? ns, string className)
		{
			foreach (var bs in parseResult.BindingScopes)
			{
				output.AppendLine();
				_bindingsCodeGenerator.GenerateBindingsClass(output, bs.BindingsData!, ns, className, nameSuffix: "_" + (bs.ViewName ?? (bs.DataType != null ? "this" : null)));
			}
		}

		private void GenerateDataTemplates(StringBuilder output, SimpleXamlDom parseResult, string? ns, string classBaseName)
		{
			for (int i = 0; i < parseResult.DataTemplates.Count; i++)
			{
				GenerateDataTemplateClass(output, parseResult.DataTemplates[i], ns, classBaseName + "_DataTemplate" + i);
			}
		}

		private void GenerateDataTemplateClass(StringBuilder output, SimpleXamlDom parseResult, string? ns, string dataTemplateClassName)
		{
			parseResult.BindingScopes.Where(bs => bs.ViewName == null).ForEach(bs => bs.ViewName = "rootElement");

			output.AppendLine(
$@"
	class {dataTemplateClassName} : global::CompiledBindings.IGeneratedDataTemplate
	{{");

			GenerateVariablesDeclarations(output, parseResult, false);
			GenerateConverterDeclarations(output, parseResult);

			output.AppendLine(
$@"
		public void Initialize(global::{_bindableObject} rootElement)
		{{");

			GenerateInitializeMethodBody(output, parseResult, "rootElement", true);

			output.AppendLine(
$@"		}}");

			GenerateBindingContextChangedHandlers(output, parseResult);

			GenerateBindings(output, parseResult, ns, dataTemplateClassName);

			output.AppendLine(
$@"	}}");
		}

		private void GenerateVariablesDeclarations(StringBuilder output, SimpleXamlDom parseResult, bool notExplicitlySet)
		{
			IEnumerable<XamlObject> objects = parseResult.XamlObjects!;
			if (notExplicitlySet)
			{
				objects = objects.Where(o => !o.NameExplicitlySet);
			}
			foreach (var obj in objects.Where(o => o.Name != null))
			{
				output.AppendLine(
$@"		private global::{obj.Type.Type.GetCSharpFullName()} {obj.Name};");
			}
		}
	}
}
