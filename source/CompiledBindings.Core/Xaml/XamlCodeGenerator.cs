namespace CompiledBindings;

public class XamlCodeGenerator
{
	public XamlCodeGenerator(string frameworkId, string langVersion, string msbuildVersion)
	{
		var msbuildVersionParts = msbuildVersion.Split('.');
		if (!int.TryParse(msbuildVersionParts[0], out var buildVersionNum))
		{
			buildVersionNum = 0;
		}
		if (!float.TryParse(langVersion, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var langVersionNum))
		{
			if (langVersion is "latest" or "preview" && buildVersionNum >= 16)
			{
				langVersionNum = buildVersionNum == 16 ? 9.0f : 10.0f;
			}
			else
			{
				langVersionNum = 7.3f;
			}
		}
		LangVersion = langVersionNum;
		LangNullables = LangVersion >= 8;
		FrameworkId = frameworkId;
	}

	public string FrameworkId { get; }


	public float LangVersion { get; }

	public bool LangNullables { get; }

	public void GenerateSetValue(StringBuilder output, XamlObjectProperty property, Expression? expression, string? targetRootVariable, ref int localVarIndex, ref int localFuncIndex, ref bool isLineDirective, string? a)
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
			value = expression!.CSharpCode;
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

		bool isTwoWayBinding = property.Value.BindValue?.Mode == BindingMode.TwoWay;

		if (property.TargetEvent != null)
		{
			if (expression != null)
			{
				var types = property.TargetEvent.GetEventHandlerParameterTypes().ToList();

				bool wrap;
				if (expression is MemberExpression me && me.Member is MethodInfo sourceMethod)
				{
					wrap = types.Count > 0 && sourceMethod.Parameters.Count == 0;
					if (wrap)
					{
						expression = new CallExpression(me.Expression, sourceMethod, new Expression[0]);
						value = expression.CSharpCode;
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
$@"{LineDirective(property.XamlNode, ref isLineDirective)}
{a}			_eventHandler{property.Value.BindValue.Index} = {value};
{ResetLineDirective(ref isLineDirective)}
{a}			{setExpr} += _eventHandler{property.Value.BindValue.Index};");
				}
				else
				{
					output.AppendLine(
$@"{LineDirective(property.XamlNode, ref isLineDirective)}
{a}			{setExpr} += {value};");
				}
			}
			else
			{
				output.AppendLine(
$@"{LineDirective(property.XamlNode, ref isLineDirective)}
{a}			{setExpr} += {value};");
			}
		}
		else if (property.TargetMethod != null)
		{
			var a2 = a;
			if (isTwoWayBinding)
			{
				ResetLineDirective(output, ref isLineDirective);
				output.AppendLine(
$@"{a}			_settingBinding{property.Value.BindValue!.Index} = true;
{a}			try
{a}			{{");
				a2 += '\t';
			}
			if (property.IsAttached)
			{
				output.AppendLine(
$@"{LineDirective(property.XamlNode, ref isLineDirective)}");
				if (property.TargetMethod.Definition.IsStatic)
				{
					output.AppendLine(
$@"{a2}			global::{property.TargetMethod.Definition.DeclaringType.GetCSharpFullName()}.{property.TargetMethod.Definition.Name}({setExpr}, {value});");
				}
				else
				{
					var attachRoot = targetRootVariable;
					if (attachRoot != null)
					{
						attachRoot += '.';
					}
					output.AppendLine(
$@"{a2}			{attachRoot}{property.Object.Parent!.Name}.{property.TargetMethod.Definition.Name}({setExpr}, {value});");
				}
			}
			else
			{
				GenerateSet(true, ref localFuncIndex, ref isLineDirective, a2);
			}
			if (isTwoWayBinding)
			{
				ResetLineDirective(output, ref isLineDirective);
				output.AppendLine(
$@"{a}			}}
{a}			finally
{a}			{{
{a}				_settingBinding{property.Value.BindValue!.Index} = false;
{a}			}}");
			}
		}
		// For two-way bindings set value only if the current value of the target is different.
		// For example, if it's a TextBox (EditText etc), than
		// setting the text causes moving cursor at first (or last, dependent on platform) position.
		else if (isTwoWayBinding)
		{
			string varName;
			if (expression is not VariableExpression)
			{
				varName = "value" + localVarIndex++;
				output.AppendLine(
$@"{LineDirective(property.XamlNode, ref isLineDirective)}
{a}			var {varName} = {value};
{ResetLineDirective(ref isLineDirective)}");
			}
			else
			{
				varName = value;
			}
			output.AppendLine(
$@"{a}			if (!object.Equals({setExpr}, {varName}))
{a}			{{
{a}				_settingBinding{property.Value.BindValue!.Index} = true;
{a}				try
{a}				{{
{LineDirective(property.XamlNode, ref isLineDirective)}
{a}					{setExpr} = {varName};
{ResetLineDirective(ref isLineDirective)}
{a}				}}
{a}				finally
{a}				{{
{a}					_settingBinding{property.Value.BindValue.Index} = false;
{a}				}}
{a}			}}");
		}
		else
		{
			GenerateSet(false, ref localFuncIndex, ref isLineDirective, a);
		}

		void GenerateSet(bool isMethodCall, ref int localFuncIndex, ref bool isLineDirective, string? a)
		{
			if (isAsync)
			{
				ResetLineDirective(output, ref isLineDirective);
				output.AppendLine(
$@"{a}			Set{localFuncIndex}(_generatedCodeDisposed.Token);
{a}			async void Set{localFuncIndex++}(global::System.Threading.CancellationToken cancellationToken)
{a}			{{
{a}				try
{a}				{{");
				var fallbackValue = property.Value.BindValue?.FallbackValue;
				if (fallbackValue != null)
				{
					output.AppendLine(
$@"{LineDirective(property.XamlNode, ref isLineDirective)}
{a}					var task = {value};
{ResetLineDirective(ref isLineDirective)}");

					var taskVar = "task";
					var isTaskNullable = expression!.IsNullable;
					if (isTaskNullable)
					{
						taskVar += '?';
					}
					output.AppendLine(
$@"{a}					if ({taskVar}.IsCompleted != true)
{a}					{{
{LineDirective(property.XamlNode, ref isLineDirective)}
{a}						{setExpr}{(isMethodCall ? $"({fallbackValue})" : $" = {fallbackValue}")};
{ResetLineDirective(ref isLineDirective)}");
					if (isTaskNullable)
					{
						output.AppendLine(
$@"{a}						if (task == null)
{a}						{{
{a}							return;
{a}						}}");
					}
					output.AppendLine(
$@"{a}					}}");
					value = "task";
				}
				output.AppendLine(
$@"{LineDirective(property.XamlNode, ref isLineDirective)}
{a}					var result = await {value};
{ResetLineDirective(ref isLineDirective)}
{a}					if (!cancellationToken.IsCancellationRequested)
{a}					{{
{a}						{setExpr}{(isMethodCall ? "(result)" : " = result")};
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
$@"{LineDirective(property.XamlNode, ref isLineDirective)}
{a}			{setExpr}{(isMethodCall ? $"({value})" : $" = {value}")};");
			}
		}
	}

	public void GenerateSetExpressions(StringBuilder output, ExpressionGroup expressionGroup, ref bool isLineDirective, string? targetRootVariable = null, string? a = null)
	{
		int localVarIndex = expressionGroup.LocalVariables.Count + 1;
		int localFuncIndex = 0;

		if (expressionGroup.SetProperties != null)
		{
			foreach (var prop in expressionGroup.SetProperties)
			{
				GenerateSetValue(output, prop, null, targetRootVariable, ref localVarIndex, ref localFuncIndex, ref isLineDirective, a);
			}
		}

		if (expressionGroup.LocalVariables.Count > 0 || expressionGroup.SetExpressions.Count > 0)
		{
			foreach (var variable in expressionGroup.LocalVariables)
			{
				output.AppendLine(
$@"{LineDirective(variable.XamlNode, ref isLineDirective)}
{a}			var {variable.Name} = {variable.Expression};");
			}

			foreach (var prop in expressionGroup.SetExpressions)
			{
				GenerateSetValue(output, prop.Property, prop.Expression, targetRootVariable, ref localVarIndex, ref localFuncIndex, ref isLineDirective, a);
			}
			ResetLineDirective(output, ref isLineDirective);
		}
	}

	public string LineDirective(XamlNode xamlNode, ref bool isLineDirective)
	{
		isLineDirective = true;
		var li = (IXmlLineInfo)xamlNode.Element;
		var line = li.LineNumber;
		if (LangVersion >= 10)
		{
			var column1 = li.LinePosition;
			var column2 = li.LinePosition;
			if (xamlNode.Element is XAttribute attribute)
			{
				column2 +=
					(attribute.Parent.GetPrefixOfNamespace(attribute.Name.Namespace)?.Length + 1 ?? 0) +
					attribute.Name.LocalName.Length +
					attribute.Value.Length + 2;
			}
			//TODO Somehow it doesn't work as expected.
			//Experimentally was found out, that it works if the "column offset" is the line number
			return $"#line ({line}, {column1}) - ({line}, {column2}) {line} \"{xamlNode.File}\"";
		}
		return $"#line {line} \"{xamlNode.File}\"";
	}
	
	public string ResetLineDirective(ref bool isLineDirective)
	{
		isLineDirective = false;
		return "#line default";
	}

	public void ResetLineDirective(StringBuilder output, ref bool isLineDirective)
	{
		if (isLineDirective)
		{
			output.AppendLine(ResetLineDirective(ref isLineDirective));
		}
	}
}

