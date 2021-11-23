#nullable enable

namespace CompiledBindings;

public class XamlCodeGenerator
{
	public XamlCodeGenerator(string langVersion)
	{
		LangNullables =
			langVersion is "latest" or "preview" ||
			(float.TryParse(langVersion, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var version) && version >= 8);
	}

	public bool LangNullables { get; }

	public void GenerateSetValue(StringBuilder output, XamlObjectProperty property, Expression? expression, string? targetRootVariable, string? bindingsAccess, ref int localVarIndex, ref int localFuncIndex, string? a)
	{
		var memberExpr = targetRootVariable;
		if (property.Object.Name != null && !property.Object.IsRoot)
		{
			if (memberExpr != null)
			{
				memberExpr += ".";
			}
			memberExpr += property.Object.Name;
		}
		var setExpr = memberExpr;
		if (!property.IsAttached)
		{
			if (setExpr != null)
			{
				setExpr += ".";
			}
			setExpr += property.MemberName;
		}
		else if (property.Object.Name == null)
		{
			setExpr = "this";
		}

		string value;
		if (property.Value.BindValue != null || property.Value.StaticValue != null)
		{
			value = expression!.ToString();
		}
		else if (property.Value.CSharpValue != null)
		{
			value = property.Value.CSharpValue;
		}
		else
		{
			throw new NotSupportedException();
		}

		bool isAsync = false;
		var taskType = TypeInfo.GetTypeThrow(typeof(System.Threading.Tasks.Task));
		if (expression != null && taskType.IsAssignableFrom(expression.Type))
		{
			isAsync = !taskType.IsAssignableFrom(property.MemberType);
		}

		if (property.TargetEvent != null)
		{
			if (expression != null)
			{
				var types = property.TargetEvent.Definition.GetEventHandlerParameterTypes().ToList();

				bool wrap;
				if (expression is MemberExpression me && me.Member is MethodInfo sourceMethod)
				{
					wrap = types.Count > 0 && sourceMethod.Parameters.Count == 0;
					if (wrap)
					{
						expression = new CallExpression(me.Expression, sourceMethod, new Expression[0]);
						value = expression.ToString();
					}
				}
				else
				{
					wrap = true;
				}

				if (wrap)
				{
					value = $"({string.Join(", ", Enumerable.Range(1, types.Count).Select(i => "p" + i))}) => {value}";
				}

				if (property.Value.BindValue != null)
				{
					output.AppendLine(
$@"#line {((IXmlLineInfo)property.XamlNode.Element).LineNumber} ""{property.XamlNode.File}""
{a}			{bindingsAccess}_eventHandler{property.Value.BindValue.Index} = {value};
{a}			{setExpr} += {bindingsAccess}_eventHandler{property.Value.BindValue.Index};");
				}
				else
				{
					output.AppendLine(
$@"#line {((IXmlLineInfo)property.XamlNode.Element).LineNumber} ""{property.XamlNode.File}""
{a}			{setExpr} += {value};");
				}
			}
			else
			{
				output.AppendLine(
$@"{a}			{setExpr} += {value};");
			}
		}
		else if (property.TargetMethod != null)
		{
			if (property.IsAttached)
			{
				if (property.TargetMethod.Definition.IsStatic)
				{
					output.AppendLine(
$@"{a}			global::{property.TargetMethod.Definition.DeclaringType.GetCSharpFullName()}.{property.TargetMethod.Definition.Name}({setExpr}, {value});");
				}
				else
				{
					var attachRoot = targetRootVariable;
					if (attachRoot != null)
					{
						attachRoot += '.';
					}
					output.AppendLine(
$@"{a}			{attachRoot}{property.Object.Parent!.Name}.{property.TargetMethod.Definition.Name}({setExpr}, {value});");
				}
			}
			else
			{
				GenerateSet(true, ref localFuncIndex);
			}
		}
		// For two-way bindings set value only if the current value of the target is different.
		// For example, if it's a TextBox (EditText etc), than
		// setting the text causes moving cursor at first (or last, dependent on platform) position.
		else if (property.Value.BindValue?.Mode == BindingMode.TwoWay)
		{
			output.AppendLine(
$@"{a}			if (!{bindingsAccess}_settingBinding{property.Value.BindValue.Index})
{a}			{{");

			string varName;
			if (expression is not ParameterExpression)
			{
				varName = "value" + localVarIndex++;
				output.AppendLine(
$@"#line {((IXmlLineInfo)property.XamlNode.Element).LineNumber} ""{property.XamlNode.File}""
{a}				var {varName} = {value};");
			}
			else
			{
				varName = value;
			}
			output.AppendLine(
$@"{a}				if (!object.Equals({setExpr}, {varName}))
{a}				{{
{a}					{setExpr} = {varName};
{a}				}}
{a}			}}");
		}
		else
		{
			GenerateSet(false, ref localFuncIndex);
		}

		void GenerateSet(bool isMethodCall, ref int localFuncIndex)
		{
			if (isAsync)
			{
				var bindings = property.Value.BindValue != null ? "bindings." : null;
				output.AppendLine(
$@"{a}			Set{localFuncIndex}({bindings}_generatedCodeDisposed.Token);
{a}			async void Set{localFuncIndex++}(CancellationToken cancellationToken)
{a}			{{
{a}				try
{a}				{{");
				var fallbackValue = property.Value.BindValue?.FallbackValue;
				if (fallbackValue != null)
				{
					output.AppendLine(
$@"#line {((IXmlLineInfo)property.XamlNode.Element).LineNumber} ""{property.XamlNode.File}""
{a}					var task = {value};
{a}					if (!task.IsCompleted)
{a}					{{
{a}						{setExpr}{(isMethodCall ? $"({fallbackValue})" : $" = {fallbackValue}")};
{a}					}}");
					value = "task";
				}
				output.AppendLine(
$@"#line {((IXmlLineInfo)property.XamlNode.Element).LineNumber} ""{property.XamlNode.File}""
{a}					var value = await {value};
{a}					if (!cancellationToken.IsCancellationRequested)
{a}					{{
{a}						{setExpr}{(isMethodCall ? $"(value)" : $" = value")};
{a}					}}
{a}				}}
{a}				catch
{a}				{{
{a}				}}
{a}			}}");
			}
			else
			{
				output.AppendLine(
$@"#line {((IXmlLineInfo)property.XamlNode.Element).LineNumber} ""{property.XamlNode.File}""
{a}			{setExpr}{(isMethodCall ? $"({value})" : $" = {value}")};");
			}
		}
	}

	public void GenerateUpdateMethodBody(StringBuilder output, UpdateMethod updateMethod, string? targetRootVariable = null, string? bindingsAccess = null, string? a = null)
	{
		int localVarIndex = updateMethod.LocalVariables.Count + 1;
		int localFuncIndex = 0;

		if (updateMethod.SetProperties != null)
		{
			foreach (var prop in updateMethod.SetProperties)
			{
				GenerateSetValue(output, prop, null, targetRootVariable, bindingsAccess, ref localVarIndex, ref localFuncIndex, a);
			}
		}

		foreach (var variable in updateMethod.LocalVariables)
		{
			output.AppendLine(
$@"{a}			var {variable.Name} = {variable.Expression};");
		}

		foreach (var prop in updateMethod.SetExpressions)
		{
			GenerateSetValue(output, prop.Property, prop.Expression, targetRootVariable, bindingsAccess, ref localVarIndex, ref localFuncIndex, a);
		}
		output.AppendLine(
$@"#line default
#line hidden");
	}
}

