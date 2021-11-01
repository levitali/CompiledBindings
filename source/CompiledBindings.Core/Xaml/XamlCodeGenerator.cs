using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Mono.Cecil;

#nullable enable

namespace CompiledBindings
{
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
			var taskType = TypeInfoUtils.GetTypeThrow(typeof(System.Threading.Tasks.Task));
			if (expression != null && taskType.IsAssignableFrom(expression.Type))
			{
				isAsync = !taskType.IsAssignableFrom(property.MemberType);
			}

			if (property.TargetEvent != null)
			{
				if (expression != null)
				{
					var me = (MemberExpression)expression;
					var sourceMethod = (MethodDefinition)me.Member;
					bool isNullable = me.Expression.IsNullable;
					bool isParameterExpr = me.Expression is ParameterExpression;
					string? a2 = a;
					string valueVar = isParameterExpr ? me.Expression.ToString() : "value" + localVarIndex++;

					if (property.Value.BindValue != null)
					{
						output.AppendLine(
$@"{a}			if ({bindingsAccess}_eventHandler{property.Value.BindValue.Index} != null)
{a}			{{
{a}				{setExpr} -= {bindingsAccess}_eventHandler{property.Value.BindValue.Index};
{a}				{bindingsAccess}_eventHandler{property.Value.BindValue.Index} = null;
{a}			}}");
					}

					if (isNullable)
					{
						if (!isParameterExpr)
						{
							output.AppendLine(
$@"{a}			var {valueVar} = {me.Expression};");
						}

						output.AppendLine(
$@"{a}			if ({valueVar} != null)
{a}			{{");

						value = $"{valueVar}.{me.Member.Name}";
						a2 += "\t";
					}
					var types = property.TargetEvent.GetEventHandlerParameterTypes().ToList();
					if (types.Count > 0 && sourceMethod.Parameters.Count == 0)
					{
						value = $"({string.Join(", ", Enumerable.Range(1, types.Count).Select(i => "p" + i))}) => {value}()";
					}

					if (property.Value.BindValue != null)
					{
						output.AppendLine(
$@"{a2}			{bindingsAccess}_eventHandler{property.Value.BindValue.Index} = {value};
{a2}			{setExpr} += {bindingsAccess}_eventHandler{property.Value.BindValue.Index};");
					}
					else
					{
						output.AppendLine(
$@"{a2}			{setExpr} += {value};");
					}

					if (isNullable)
					{
						output.AppendLine(
$@"{a}			}}");
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
				if (expression is not ParameterExpression) //TODO!!
				{
					varName = "value" + localVarIndex++;
					output.AppendLine(
$@"{a}				var {varName} = {value};");
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
					output.AppendLine(
$@"{a}			Set{localFuncIndex}();
{a}			async void Set{localFuncIndex++}()
{a}			{{
{a}				try
{a}				{{");
					var fallbackValue = property.Value.BindValue?.FallbackValue;
					if (fallbackValue != null)
					{
						output.AppendLine(
$@"{a}					var task = {value};
{a}					if (!task.IsCompleted)
{a}					{{
{a}						{setExpr}{(isMethodCall ? $"({fallbackValue})" : $" = {fallbackValue}")};
{a}					}}");
						value = "task";
					}
					output.AppendLine(
$@"{a}					{setExpr}{(isMethodCall ? $"(await {value})" : $" = await {value}")};
{a}				}}
{a}				catch
{a}				{{
{a}				}}
{a}			}}");
				}
				else
				{
					output.AppendLine(
$@"{a}			{setExpr}{(isMethodCall ? $"({value})" : $" = {value}")};");
				}
			}
		}

		public void GenerateUnsetEventHandler(StringBuilder output, XamlObjectProperty property)
		{
			if (property.Value.CSharpValue != null && property.TargetEvent != null)
			{
				string? setExpr = property.Object.Name;
				if (setExpr != null)
				{
					setExpr += ".";
				}
				setExpr += property.MemberName;

				if (property.Object.Name != null)
				{
					output.AppendLine(
$@"			if ({property.Object.Name} != null)
			{{");
				}
				output.AppendLine(
$@"				{setExpr} -= {property.Value.CSharpValue};");
				if (property.Object.Name != null)
				{
					output.AppendLine(
$@"			}}");
				}
			}
		}

		public void GenerateUpdateMethodBody(StringBuilder output, UpdateMethod updateMethod, string? targetRootVariable = null, string? bindingsAccess = null, string? align = null)
		{
			int localVarIndex = updateMethod.LocalVariables.Count + 1;
			int localFuncIndex = 0;

			if (updateMethod.SetProperties != null)
			{
				foreach (var prop in updateMethod.SetProperties)
				{
					GenerateSetValue(output, prop, null, targetRootVariable, bindingsAccess, ref localVarIndex, ref localFuncIndex, align);
				}
			}

			foreach (var variable in updateMethod.LocalVariables)
			{
				output.AppendLine(
$@"{align}			var {variable.Name} = {variable.Expression};");
			}

			foreach (var prop in updateMethod.SetExpressions)
			{
				GenerateSetValue(output, prop.Property, prop.Expression, targetRootVariable, bindingsAccess, ref localVarIndex, ref localFuncIndex, align);
			}
		}
	}
}
