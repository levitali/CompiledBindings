namespace CompiledBindings;

public abstract class SimpleXamlDomCodeGenerator : XamlCodeGenerator
{
	private readonly BindingsCodeGenerator _bindingsCodeGenerator;
	private readonly string _bindingContextStart;
	private readonly string _bindingConextArgs;
	private readonly string _bindableObject;
	private readonly string _findByNameFormat;
	private readonly string _compiledBindingsHelperNs;
	private readonly bool _generateVariableDeclarations;
	private readonly bool _generateVariableInitialization;

	public SimpleXamlDomCodeGenerator(
		BindingsCodeGenerator bindingsCodeGenerator,
		string compiledBindingsHelperNs,
		string bindingContextStart,
		string bindingConextArgs,
		string bindableObject,
		string findByNameFormat,
		bool generateVariableDeclarations,
		bool generateVariableInitialization,
		string langVersion,
		string msbuildVersion)
		: base(langVersion, msbuildVersion)
	{
		_bindingsCodeGenerator = bindingsCodeGenerator;
		_bindingContextStart = bindingContextStart;
		_bindingConextArgs = bindingConextArgs;
		_bindableObject = bindableObject;
		_findByNameFormat = findByNameFormat;
		_compiledBindingsHelperNs = compiledBindingsHelperNs;
		_generateVariableDeclarations = generateVariableDeclarations;
		_generateVariableInitialization = generateVariableInitialization;
	}

	public string GenerateCode(SimpleXamlDom parseResult)
	{
		var output = new StringBuilder();

		output.AppendLine(GenerateFileHeader());
		output.AppendLine();

		if (parseResult.TargetType.Reference.Namespace != null)
		{
			output.AppendLine(
$@"namespace {parseResult.TargetType.Reference.Namespace}
{{");
		}

		if (parseResult.IncludeNamespaces.Count > 0)
		{
			foreach (string ns in parseResult.IncludeNamespaces)
			{
				output.AppendLine(
$@"	using {ns};");
			}
			output.AppendLine();
		}

		if (parseResult.GeneratedClass.GenerateClass)
		{
			output.AppendLine(
$@"	[global::System.CodeDom.Compiler.GeneratedCode(""CompiledBindings"", null)]
	partial class {parseResult.TargetType.Reference.Name}
	{{");
			var taskType = TypeInfo.GetTypeThrow(typeof(System.Threading.Tasks.Task));
			var asyncFunctions = parseResult.GeneratedClass.UpdateMethod.SetExpressions.Any(e => taskType.IsAssignableFrom(e.Expression.Type));
			if (asyncFunctions)
			{
				output.AppendLine(
$@"		global::System.Threading.CancellationTokenSource _generatedCodeDisposed = new global::System.Threading.CancellationTokenSource();");
			}

			GenerateInitializeMethod(output, parseResult);
			GenerateBindings(output, parseResult.GeneratedClass, parseResult.TargetType.Reference.Name);

			output.AppendLine(
$@"	}}");
		}

		GenerateAdditionalClassCode(output, parseResult.GeneratedClass, parseResult.TargetType.Reference.Name);
		GenerateDataTemplates(output, parseResult, parseResult.TargetType.Reference.Name);

		if (parseResult.TargetType.Reference.Namespace != null)
		{
			output.AppendLine(
"}");
		}

		return output.ToString();
	}

	protected virtual void GenerateAdditionalClassCode(StringBuilder output, GeneratedClass parseResult, string className)
	{
	}

	private void GenerateResourceDeclarations(StringBuilder output, GeneratedClass parseResult, bool isDataTemplate)
	{
		foreach (var (name, type) in parseResult.EnumerateResources())
		{
			if (isDataTemplate)
			{
				output.AppendLine(
$@"		public global::{type.Reference.GetCSharpFullName()} {name} {{ get; set; }}");
			}
			else
			{
				output.AppendLine(
$@"		global::{type.Reference.GetCSharpFullName()} {name};");
			}
		}
	}

	private void GenerateInitializeResources(StringBuilder output, GeneratedClass parseResult)
	{
		var resources = parseResult.EnumerateResources().ToList();
		if (resources.Count > 0)
		{
			int varIndex = 0;
			foreach (var (name, type) in resources)
			{
				output.AppendLine(
$@"			{name} = (global::{type.Reference.GetCSharpFullName()})({CreateGetResourceCode(name, varIndex++)});");
			}

			output.AppendLine();
		}
	}

	protected abstract string CreateGetResourceCode(string resourceName, int varIndex);

	private void GenerateInitializeMethod(StringBuilder output, SimpleXamlDom parseResult)
	{
		if (_generateVariableDeclarations)
		{
			GenerateVariablesDeclarations(output, parseResult.GeneratedClass, true);
		}
		GenerateResourceDeclarations(output, parseResult.GeneratedClass, false);

		output.AppendLine(
$@"		private bool _generatedCodeInitialized;

		private void InitializeAfterConstructor()
		{{
			if (_generatedCodeInitialized)
				return;

			_generatedCodeInitialized = true;
");

		GenerateInitializeMethodBody(output, parseResult.GeneratedClass, "this", false, parseResult.TargetType);

		output.AppendLine(
$@"		}}");

		GenerateBindingContextChangedHandlers(output, parseResult.GeneratedClass, parseResult.TargetType);
	}

	private void GenerateBindingContextChangedHandlers(StringBuilder output, GeneratedClass parseResult, TypeInfo? targetType)
	{
		foreach (var bs in parseResult.BindingScopes.Where(b => b.DataType != null))
		{
			var viewName = bs.ViewName ?? "this";
			var prm = bs.DataType!.Reference.FullName == targetType?.Reference.FullName ? null : $", dataRoot";

			output.AppendLine();
			output.AppendLine(
$@"		private void {viewName}_{_bindingContextStart}ContextChanged(object sender, global::{_bindingConextArgs} e)
		{{
			Bindings_{viewName}.Cleanup();
			if (((global::{_bindableObject})sender).{_bindingContextStart}Context is global::{bs.DataType.Reference.GetCSharpFullName()} dataRoot)
			{{
				Bindings_{viewName}.Initialize(this{prm});
			}}
		}}");
		}
	}

	private void GenerateInitializeMethodBody(StringBuilder output, GeneratedClass parseResult, string rootElement, bool isDataTemplate, TypeInfo? targetType)
	{
		if (isDataTemplate || _generateVariableInitialization)
		{
			GenerateVariablesInitializations(output, parseResult, rootElement, isDataTemplate);
		}

		if (!isDataTemplate)
		{
			GenerateInitializeResources(output, parseResult);
		}

		bool isLineDirective = false;
		_bindingsCodeGenerator.GenerateSetExpressions(output, parseResult.UpdateMethod, ref isLineDirective);
		ResetLineDirective(output, ref isLineDirective);

		output.AppendLine();

		for (int i = 0; i < parseResult.BindingScopes.Count; i++)
		{
			var bs = parseResult.BindingScopes[i];
			var viewName = bs.ViewName ?? rootElement;
			if (bs.DataType == null)
			{
				output.AppendLine(
$@"			Bindings_.Initialize(this);");
			}
			else
			{
				var prm = bs.DataType.Reference.FullName == targetType?.Reference.FullName ? null : $", dataRoot" + i;
				output.AppendLine(
$@"			{viewName}.{_bindingContextStart}ContextChanged += {viewName}_{_bindingContextStart}ContextChanged;
			if ({viewName}.{_bindingContextStart}Context is global::{bs.DataType.Reference.GetCSharpFullName()} dataRoot{i})
			{{
				Bindings_{viewName}.Initialize(this{prm});
			}}");
			}
		}
	}

	protected virtual void GenerateVariablesInitializations(StringBuilder output, GeneratedClass parseResult, string rootElement, bool isDataTemplate)
	{
		IEnumerable<XamlObject> objects = parseResult.XamlObjects!;
		if (!isDataTemplate)
		{
			objects = objects.Where(o => !o.NameExplicitlySet);
		}

		foreach (var obj in objects.Where(o => o.Name != null))
		{
			output.AppendLine(
$@"			{obj.Name} = {string.Format(_findByNameFormat, obj.Type.Reference.GetCSharpFullName(), rootElement, obj.Name)};");
		}
		output.AppendLine();
	}

	private void GenerateBindings(StringBuilder output, GeneratedClass parseResult, string className)
	{
		foreach (var bs in parseResult.BindingScopes)
		{
			output.AppendLine();
			var nameSuffix = "_";
			if (bs.DataType != null)
			{
				nameSuffix += bs.ViewName ?? "this";
			}
			_bindingsCodeGenerator.GenerateBindingsClass(output, bs.BindingsData, className, nameSuffix: nameSuffix);
		}
	}

	private void GenerateDataTemplates(StringBuilder output, SimpleXamlDom parseResult, string classBaseName)
	{
		for (int i = 0; i < parseResult.DataTemplates.Count; i++)
		{
			GenerateDataTemplateClass(output, parseResult.DataTemplates[i], classBaseName + "_DataTemplate" + i);
		}
	}

	private void GenerateDataTemplateClass(StringBuilder output, GeneratedClass parseResult, string dataTemplateClassName)
	{
		parseResult.BindingScopes.Where(bs => bs.ViewName == null).ForEach(bs => bs.ViewName = "rootElement");

		if (parseResult.GenerateClass)
		{
			output.AppendLine(
$@"
	class {dataTemplateClassName} : global::{_compiledBindingsHelperNs}.IGeneratedDataTemplate
	{{");

			GenerateVariablesDeclarations(output, parseResult, false);

			var taskType = TypeInfo.GetTypeThrow(typeof(System.Threading.Tasks.Task));
			var asyncFunctions = parseResult.UpdateMethod.SetExpressions.Any(e => taskType.IsAssignableFrom(e.Expression.Type));
			if (asyncFunctions)
			{
				output.AppendLine(
$@"		private global::System.Threading.CancellationTokenSource _generatedCodeDisposed;");
			}

			GenerateResourceDeclarations(output, parseResult, true);

			// Initialize method
			output.AppendLine(
$@"
		public void Initialize(global::{_bindableObject} rootElement)
		{{");
			if (asyncFunctions)
			{
				output.AppendLine(
$@"			_generatedCodeDisposed = new global::System.Threading.CancellationTokenSource();");
			}

			GenerateInitializeMethodBody(output, parseResult, "rootElement", true, null);

			output.AppendLine(
$@"		}}");

			// Cleanup method
			output.AppendLine();
			output.AppendLine(
$@"		public void Cleanup(global::{_bindableObject} rootElement)
		{{");
			if (asyncFunctions)
			{
				output.AppendLine(
$@"			_generatedCodeDisposed.Cancel();");
			}

			foreach (var bs in parseResult.BindingScopes)
			{
				if (bs.DataType == null)
				{
					output.AppendLine(
$@"			Bindings.Cleanup();");
				}
				else
				{
					var viewName = bs.ViewName ?? "rootElement";
					output.AppendLine(
$@"			{viewName}.{_bindingContextStart}ContextChanged -= {viewName}_{_bindingContextStart}ContextChanged;
			Bindings_{viewName}.Cleanup();");
				}
			}

			output.AppendLine(
$@"		}}");


			GenerateBindingContextChangedHandlers(output, parseResult, null);
			GenerateBindings(output, parseResult, dataTemplateClassName);

			output.AppendLine(
$@"	}}");
		}

		GenerateAdditionalClassCode(output, parseResult, dataTemplateClassName);
	}

	private void GenerateVariablesDeclarations(StringBuilder output, GeneratedClass parseResult, bool notExplicitlySet)
	{
		IEnumerable<XamlObject> objects = parseResult.XamlObjects!;
		if (notExplicitlySet)
		{
			objects = objects.Where(o => !o.NameExplicitlySet);
		}
		foreach (var obj in objects.Where(o => o.Name != null))
		{
			output.AppendLine(
$@"		private global::{obj.Type.Reference.GetCSharpFullName()} {obj.Name};");
		}
	}
}

