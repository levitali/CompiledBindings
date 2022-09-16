namespace CompiledBindings;

public class BindingsCodeGenerator : XamlCodeGenerator
{
	public BindingsCodeGenerator(string langVersion, string msbuildVersion) : base(langVersion, msbuildVersion)
	{
	}

	public void GenerateBindingsClass(StringBuilder output, BindingsData bindingsData, string? targetClassName, string? nameSuffix = null)
	{
		bool isDiffDataRoot = bindingsData.DataType.Type.FullName != bindingsData.TargetType?.Type.FullName;
		var rootGroup = bindingsData.NotifySources.SingleOrDefault(g => g.Expression is VariableExpression pe && pe.Name == "dataRoot");

		var iNotifyPropertyChangedType = TypeInfo.GetTypeThrow(typeof(INotifyPropertyChanged));

		#region Class Begin

		output.AppendLine(
$@"		{targetClassName}_Bindings{nameSuffix} Bindings{nameSuffix} = new {targetClassName}_Bindings{nameSuffix}();
");

		output.AppendLine(
$@"		class {targetClassName}_Bindings{nameSuffix}
		{{");

		#endregion

		#region Fields declaration

		output.AppendLine(
$@"			{targetClassName} _targetRoot;");

		if (isDiffDataRoot)
		{
			output.AppendLine(
$@"			global::{bindingsData.DataType.Type.GetCSharpFullName()} _dataRoot;");
		}

		if (bindingsData.NotifySources.Count > 0)
		{
			output.AppendLine(
$@"			{targetClassName}_BindingsTrackings{nameSuffix} _bindingsTrackings;");
		}

		// Generate _eventHandlerXXX fields for event bindings
		foreach (var binding in bindingsData.Bindings.Where(b => b.Property.TargetEvent != null))
		{
			output.AppendLine(
$@"			global::{binding.Property.TargetEvent!.EventType.Type.GetCSharpFullName()} _eventHandler{binding.Index};");
		}

		// Generate flags for two-way bindings
		foreach (var bind in bindingsData.Bindings.Where(b => b.Mode == BindingMode.TwoWay))
		{
			output.AppendLine(
$@"			bool _settingBinding{bind.Index};");
		}

		var taskType = TypeInfo.GetTypeThrow(typeof(System.Threading.Tasks.Task));
		bool asyncFunctions = bindingsData.Bindings.Any(b => b.Expression != null && taskType.IsAssignableFrom(b.Expression.Type));

		if (asyncFunctions)
		{
			output.AppendLine(
$@"			CancellationTokenSource _generatedCodeDisposed;");
		}

		GenerateBindingsExtraFieldDeclarations(output, bindingsData);

		#endregion // Fields declaration

		#region Initialize Method

		// Create Initialize method
		if (isDiffDataRoot)
		{
			output.AppendLine(
$@"
			public void Initialize({targetClassName} targetRoot, global::{bindingsData.DataType.Type.GetCSharpFullName()} dataRoot)
			{{
				_targetRoot = targetRoot;
				_dataRoot = dataRoot;");
		}
		else
		{
			output.AppendLine(
$@"
			public void Initialize({targetClassName} dataRoot)
			{{
				_targetRoot = dataRoot;");
		}

		if (asyncFunctions)
		{
			output.AppendLine(
$@"				_generatedCodeDisposed = new CancellationTokenSource();");
		}

		if (bindingsData.NotifySources.Count > 0)
		{
			output.AppendLine(
$@"				_bindingsTrackings = new {targetClassName}_BindingsTrackings{nameSuffix}(this);");
		}

		output.AppendLine(
$@"
				Update();");

		// Generate code of event bindings
		var eventBindings = bindingsData.Bindings.Where(b => b.Property.TargetEvent != null).ToList();
		if (eventBindings.Count > 0)
		{
			int dummyLocalVar = 0, dummyLocalFunc = 0;
			output.AppendLine();
			foreach (var binding in eventBindings)
			{
				GenerateSetValue(output, binding.Property, binding.Expression, "_targetRoot", ref dummyLocalVar, ref dummyLocalFunc, "\t");
			}
			output.AppendLine(
$@"#line default");
		}

		// Generate setting PropertyChanged event handler for data root
		if (rootGroup != null)
		{
			int index = bindingsData.NotifySources.IndexOf(rootGroup);
			output.AppendLine(
$@"
				_bindingsTrackings.SetPropertyChangedEventHandler{index}(dataRoot);");
		}

		// Set event handlers for two-way bindings
		if (bindingsData.TwoWayEvents.Count > 0)
		{
			output.AppendLine();
			foreach (var ev in bindingsData.TwoWayEvents)
			{
				var first = ev.Bindings[0];
				string targetExpr = "_targetRoot";
				if (first.Property.Object.Name != null)
				{
					targetExpr += "." + first.Property.Object.Name;
				}

				if (ev.Events != null)
				{
					foreach (var evnt in ev.Events)
					{
						var targetExpr2 = targetExpr + "." + evnt.Definition.Name;

						output.AppendLine(
$@"				{targetExpr2} += OnTargetChanged{ev.Index};");
					}
				}
				else if (first.DependencyProperty != null)
				{
					GenerateSetDependencyPropertyChangedCallback(output, ev, targetExpr);
				}
				else
				{
					Debug.Assert(false);
				}
			}
		}

		// Close Initialize method
		output.AppendLine(
$@"			}}");

		#endregion Initialize Method

		#region Cleanup Method

		// Generate Cleanup method
		output.AppendLine(
$@"
			public void Cleanup()
			{{");

		output.AppendLine(
$@"				if (_targetRoot != null)
				{{");
		if (asyncFunctions)
		{
			output.AppendLine(
$@"					_generatedCodeDisposed.Cancel();");
		}

		// Unset event handlers for two-way bindings
		foreach (var ev in bindingsData.TwoWayEvents)
		{
			var first = ev.Bindings[0];
			string targetExpr = "_targetRoot";
			if (first.Property.Object.Name != null)
			{
				targetExpr += "." + first.Property.Object.Name;
			}

			if (ev.Events != null)
			{
				foreach (var evnt in ev.Events)
				{
					var targetExpr2 = targetExpr + "." + evnt.Definition.Name;
					output.AppendLine(
$@"					{targetExpr2} -= OnTargetChanged{ev.Index};");
				}
			}
			else if (first.DependencyProperty != null)
			{
				GenerateUnsetDependencyPropertyChangedCallback(output, ev, targetExpr);
			}
			else
			{
				Debug.Assert(false);
			}
		}

		// Unset event handlers for event bindings
		foreach (var binding in bindingsData.Bindings.Where(b => b.Property.TargetEvent != null))
		{
			string targetExpr = "_targetRoot";
			if (binding.Property.Object.Name != null)
			{
				targetExpr += $".{binding.Property.Object.Name}";
			}
			targetExpr += $".{binding.Property.MemberName}";
			output.AppendLine(
$@"					{targetExpr} -= _eventHandler{binding.Index};
					_eventHandler{binding.Index} = null;");
		}

		if (bindingsData.NotifySources.Count > 0)
		{
			output.AppendLine(
$@"					_bindingsTrackings.Cleanup();");
		}

		if (isDiffDataRoot && bindingsData.DataType.Type.IsNullable())
		{
			output.AppendLine(
$@"					_dataRoot = null;");
		}

		output.AppendLine(
$@"					_targetRoot = null;
				}}
			}}");

		#endregion Cleanup Method

		#region Update Method

		// Generate Update method
		output.AppendLine(
$@"
			public void Update()
			{{");
		GenerateUpdateMethodBody(bindingsData.UpdateMethod);
		output.AppendLine(
$@"			}}");

		#endregion

		#region UpateXX Methods

		foreach (var notifySource in bindingsData.NotifySources.Where(s => s.UpdateMethod != null))
		{
			GenerateUpdateMethod(notifySource.UpdateMethod!, notifySource.Expression.Type, $"Update{notifySource.Index}");
		}

		#endregion

		#region UpdateXX_XXX Methods

		foreach (var notifySource in bindingsData.NotifySources)
		{
			foreach (var prop in notifySource.Properties.Where(_ => _.UpdateMethod != null))
			{
				GenerateUpdateMethod(prop.UpdateMethod!, prop.Property.PropertyType, $"Update{notifySource.Index}_{prop.Property.Definition.Name}");
			}
		}

		#endregion UpdateXX_XXX Methods

		#region OnTargetChanged methods

		foreach (var ev in bindingsData.TwoWayEvents)
		{
			output.AppendLine();

			var first = ev.Bindings[0];
			if (ev.Events != null)
			{
				output.AppendLine(
$@"			private void OnTargetChanged{ev.Index}({string.Join(", ", ev.Events[0].GetEventHandlerParameterTypes().Select((t, i) => $"global::{t.Type.GetCSharpFullName()} p{i}"))})");
			}
			else if (first.DependencyProperty != null)
			{
				GenerateDependencyPropertyChangedCallback(output, $"OnTargetChanged{ev.Index}");
			}
			else
			{
				Debug.Assert(false);
			}

			output.AppendLine(
$@"			{{
				var dataRoot = {(isDiffDataRoot ? "_dataRoot" : "_targetRoot")};");

			if (ev.Events?[0].Definition.Name == "PropertyChanged")
			{
				output.AppendLine(
$@"				switch (p1.PropertyName)
				{{");
				foreach (var group in ev.Bindings.Where(b => b.Property.TargetProperty != null).GroupBy(b => b.Property.MemberName))
				{
					output.AppendLine(
$@"					case ""{group.Key}"":");
					foreach (var bind in group)
					{
						GenerateSetSource(bind, "\t\t");
					}
					output.AppendLine(
$@"						break;");
				}
				output.AppendLine(
$@"				}}");
			}
			else
			{
				foreach (var bind in ev.Bindings)
				{
					GenerateSetSource(bind, null);
				}
			}

			output.AppendLine(
$@"			}}");
		}

		#endregion

		#region Trackings Class

		if (bindingsData.NotifySources.Count > 0)
		{
			#region Tracking Class Begin

			output.AppendLine(
$@"
			class {targetClassName}_BindingsTrackings{nameSuffix}
			{{");

			#endregion

			#region Fields

			output.AppendLine(
$@"				global::System.WeakReference _bindingsWeakRef;");

			// Generate _propertyChangeSourceXXX fields
			foreach (var group in bindingsData.NotifySources)
			{
				output.AppendLine(
$@"				global::{group.Expression.Type.Type.GetCSharpFullName()} _propertyChangeSource{group.Index};");
			}

			GenerateTrackingsExtraFieldDeclarations(output, bindingsData);

			#endregion

			#region Constructor

			output.AppendLine(
$@"
				public {targetClassName}_BindingsTrackings{nameSuffix}({targetClassName}_Bindings{nameSuffix} bindings)
				{{
					_bindingsWeakRef = new global::System.WeakReference(bindings);
				}}");

			#endregion

			#region Cleanup Method

			output.AppendLine(
$@"
				public void Cleanup()
				{{");

			// Unset property changed event handlers
			foreach (var group in bindingsData.NotifySources)
			{
				output.AppendLine(
$@"					SetPropertyChangedEventHandler{group.Index}(null);");
			}

			// Close the Cleanup method
			output.AppendLine(
$@"				}}");

			#endregion

			#region SetPropertyChangedEventHandler methods

			foreach (var notifySource in bindingsData.NotifySources)
			{
				string cacheVar = "_propertyChangeSource" + notifySource.Index;
				output.AppendLine(
$@"
				public void SetPropertyChangedEventHandler{notifySource.Index}(global::{notifySource.Expression.Type.Type.GetCSharpFullName()} value)
				{{
					if ({cacheVar} != null && !object.ReferenceEquals({cacheVar}, value))
					{{");
				GenerateUnsetPropertyChangedEventHandler(notifySource, cacheVar, "\t");
				output.AppendLine(
$@"						{cacheVar} = null;
					}}
					if ({cacheVar} == null && value != null)
					{{
						{cacheVar} = value;");
				GenerateSetPropertyChangedEventHandler(notifySource, cacheVar, "\t");
				output.AppendLine(
$@"					}}
				}}");
			}

			#endregion

			#region OnPropertyChange methods

			foreach (var notifySource in bindingsData.NotifySources)
			{
				if (iNotifyPropertyChangedType.IsAssignableFrom(notifySource.Expression.Type))
				{
					output.AppendLine(
$@"
				private void OnPropertyChanged{notifySource.Index}(object sender, System.ComponentModel.PropertyChangedEventArgs e)
				{{");
					output.AppendLine(
$@"					var bindings = TryGetBindings();
					if (bindings == null)
					{{
						return;
					}}

					var typedSender = (global::{notifySource.Expression.Type.Type.GetCSharpFullName()})sender;");

					if (notifySource.Properties.Count > 1)
					{
						output.AppendLine(
$@"					switch (e.PropertyName)
					{{
						case null:
						case """":
							bindings.Update{notifySource.Index}(typedSender);
							break;");

						for (int i = 0; i < notifySource.Properties.Count; i++)
						{
							var prop = notifySource.Properties[i];
							output.AppendLine(
$@"						case ""{prop.Property.Definition.Name}"":
							bindings.Update{notifySource.Index}_{prop.Property.Definition.Name}(typedSender.{prop.Property.Definition.Name});
							break;");
						}
						output.AppendLine(
$@"					}}");
					}
					else
					{
						var prop = notifySource.Properties[0];
						var propName = prop.Property.Definition.Name;
						output.AppendLine(
$@"					if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == ""{propName}"")
					{{
						bindings.Update{notifySource.Index}_{propName}(typedSender.{propName});
					}}");
					}
					output.AppendLine(
$@"				}}");

				}
				else
				{
					foreach (var prop in notifySource.Properties)
					{
						output.AppendLine();
						GenerateDependencyPropertyChangedCallback(output, $"OnPropertyChanged{notifySource.Index}_{prop.Property.Definition.Name}", "\t");
						output.AppendLine(
$@"				{{");
						output.AppendLine(
$@"					var bindings = TryGetBindings();
					if (bindings == null)
					{{
						return;
					}}

					var typedSender = (global::{notifySource.Expression.Type.Type.GetCSharpFullName()})sender;");

						output = output.AppendLine(
$@"					bindings.Update{notifySource.Index}_{prop.Property.Definition.Name}(typedSender.{prop.Property.Definition.Name});");
						output.AppendLine(
$@"				}}");
					}
				}
			}

			#endregion

			#region TryGetBindings method

			output.AppendLine(
$@"
				{targetClassName}_Bindings{nameSuffix} TryGetBindings()
				{{
					{targetClassName}_Bindings{nameSuffix} bindings = null;
					if (_bindingsWeakRef != null)
					{{
						bindings = ({targetClassName}_Bindings{nameSuffix})_bindingsWeakRef.Target;
						if (bindings == null)
						{{
							_bindingsWeakRef = null;
							Cleanup();
						}}
					}}
					return bindings;
				}}");

			#endregion

			#region Tracking Class End

			output.AppendLine(
$@"			}}");
			#endregion
		}

		#endregion // Trackings Class

		#region Class End

		output.AppendLine(
$@"		}}");

		#endregion

		#region Local Functions

		void GenerateUpdateMethod(UpdateMethodData updateMethod, TypeInfo parameterType, string name)
		{
			output.AppendLine();
			output.AppendLine(
$@"			private void {name}(global::{parameterType.Type.GetCSharpFullName()} value)
			{{");
			GenerateUpdateMethodBody(updateMethod);
			output.AppendLine(
$@"			}}");
		}

		void GenerateUpdateMethodBody(UpdateMethodData updateMethod)
		{
			if (updateMethod!.Expressions.SetExpressions.Select(d => d.Expression)
				.Concat(updateMethod!.Expressions.LocalVariables.Select(v => v.Expression))
				.Concat(updateMethod.UpdateNotifySources.Select(s => s.SourceExpression))
				.Concat(updateMethod.UpdateNotifyProperties.Select(p => p.SourceExpression))
				.SelectMany(e => e.EnumerateTree())
				.OfType<VariableExpression>()
				.Any(e => e.Name == "dataRoot"))
			{

				output.AppendLine(
$@"				var dataRoot = {(isDiffDataRoot ? "_dataRoot" : "_targetRoot")};");
			}

			GenerateSetExpressions(output, updateMethod.Expressions, targetRootVariable: "_targetRoot", a: "\t");

			foreach (var notifSource in updateMethod.UpdateNotifySources)
			{
				output.AppendLine(
$@"				Update{notifSource.Index}({notifSource.SourceExpression});");
			}

			foreach (var propUpdate in updateMethod.UpdateNotifyProperties)
			{
				output.AppendLine(
$@"				Update{propUpdate.Parent.Index}_{propUpdate.Property.Definition.Name}({propUpdate.SourceExpression});");
			}

			foreach (var group in updateMethod.SetEventHandlers.Where(g => g != rootGroup))
			{
				output.AppendLine(
$@"				_bindingsTrackings.SetPropertyChangedEventHandler{group.Index}({group.SourceExpression});");
			}
		}

		void GenerateSetPropertyChangedEventHandler(NotifySource notifySource, string cacheVar, string? a)
		{
			if (iNotifyPropertyChangedType.IsAssignableFrom(notifySource.Expression.Type))
			{
				output.AppendLine(
$@"{a}					((System.ComponentModel.INotifyPropertyChanged){cacheVar}).PropertyChanged += OnPropertyChanged{notifySource.Index};");
			}
			else
			{
				foreach (var notifyProp in notifySource.Properties)
				{
					GenerateRegisterDependencyPropertyChangeEvent(output, notifySource, notifyProp, cacheVar, $"OnPropertyChanged{notifySource.Index}_{notifyProp.Property.Definition.Name}");
				}
			}
		}

		void GenerateUnsetPropertyChangedEventHandler(NotifySource notifySource, string cacheVar, string? a)
		{
			if (iNotifyPropertyChangedType.IsAssignableFrom(notifySource.Expression.Type))
			{
				output.AppendLine(
$@"{a}					((System.ComponentModel.INotifyPropertyChanged){cacheVar}).PropertyChanged -= OnPropertyChanged{notifySource.Index};");
			}
			else
			{
				foreach (var notifyProp in notifySource.Properties)
				{
					GenerateUnregisterDependencyPropertyChangeEvent(output, notifySource, notifyProp, cacheVar, $"OnPropertyChanged{notifySource.Index}_{notifyProp.Property.Definition.Name}");
				}
			}
		}

		void GenerateSetSource(Bind bind, string? a)
		{
			var expr = bind.BindBackExpression ?? bind.Expression!;
			var me = expr as MemberExpression;
			var ae = expr.EnumerateTree().OrderByDescending(e => e.CSharpCode).OfType<IAccessExpression>().FirstOrDefault(e => e.Expression.IsNullable);

			string memberExpr = "_targetRoot";
			if (bind.Property.Object.Name != null)
			{
				memberExpr += "." + bind.Property.Object.Name;
			}

			TypeInfo sourceType;
			if (me?.Member is MethodInfo mi)
			{
				sourceType = mi.Parameters.Last().ParameterType;
			}
			else
			{
				sourceType = expr.Type;
			}

			string setExpr;
			if (bind.Property.IsAttached)
			{
				if (bind.Property.TargetMethod!.Definition.IsStatic)
				{
					setExpr = $"global::{bind.Property.TargetMethod.Definition.DeclaringType.GetCSharpFullName()}.Get{bind.Property.MemberName}({memberExpr})";
				}
				else
				{
					throw new NotImplementedException();
				}
			}
			else
			{
				setExpr = $"{memberExpr}.{bind.Property.MemberName}";
			}
			if (bind.Converter != null)
			{
				string sourceTypeFullName = sourceType.Type.GetCSharpFullName();
				string? cast = sourceTypeFullName == "System.Object" ? null : $"(global::{sourceTypeFullName})";
				string parameter = bind.ConverterParameter?.CSharpCode ?? "null";
				setExpr = GenerateConvertBackCall(bind.Converter, setExpr, sourceTypeFullName, parameter, cast);
			}
			else if (sourceType.Type.FullName == "System.String" && bind.Property.MemberType.Type.FullName != "System.String")
			{
				if (bind.Property.MemberType.IsNullable)
				{
					setExpr += "?";
				}
				setExpr += ".ToString()";
			}
			else if (!sourceType.IsAssignableFrom(bind.Property.MemberType))
			{
				var sourceType2 = sourceType.Type;
				var targetType = bind.Property.MemberType.Type;
				if (targetType.FullName == "System.Object")
				{
					setExpr = $"(global::{sourceType2.GetCSharpFullName()}){setExpr}";
				}
				else if (targetType.IsValueNullable() && targetType.GetGenericArguments()[0].FullName == sourceType2.FullName)
				{
					setExpr = $"{setExpr} ?? default";
				}
				else
				{
					if (sourceType2.IsValueNullable())
					{
						sourceType2 = sourceType2.GetGenericArguments()[0];
					}

					if (bind.Property.MemberType.Type.IsNullable())
					{
						var checkNull = bind.Property.MemberType.Type.FullName == "System.String" ? $"string.IsNullOrEmpty(t{bind.Index})" : $"t{bind.Index} == null";
						var @default = sourceType.Type.IsNullable() ? "null" : "default";
						setExpr = $"{setExpr} is var t{bind.Index} && {checkNull} ? {@default} : (global::{sourceType.Type.GetCSharpFullName()})global::System.Convert.ChangeType(t{bind.Index}, typeof(global::{sourceType2.GetCSharpFullName()}), null)";
					}
					else
					{
						setExpr = $"(global::{sourceType.Type.GetCSharpFullName()})global::System.Convert.ChangeType({setExpr}, typeof(global::{sourceType2.GetCSharpFullName()}), null)";
					}
				}
			}

			string? a2 = a;
			if (bind.Mode == BindingMode.TwoWay)
			{
				output.AppendLine(
$@"{a}				if (!_settingBinding{bind.Index})
{a}				{{");
				a2 = a + '\t';
			}
			output.AppendLine(
$@"{a2}				try
{a2}				{{");
			if (ae != null)
			{
				var memberExpr2 = expr.CloneReplace(ae.Expression, new VariableExpression(new TypeInfo(ae.Expression.Type, false), "value"));
				output.AppendLine(
$@"{LineDirective(bind.Property.XamlNode)}
{a2}					var value = {ae!.Expression};
#line default
{a2}					if (value != null)
{a2}					{{");
				GenerateSetSource(memberExpr2.CSharpCode, a2 + '\t');
				output.AppendLine(
$@"{a2}					}}");
			}
			else
			{
				GenerateSetSource(expr.CSharpCode, a2);
			}
			output.AppendLine(
$@"{a2}				}}
{a2}				catch
{a2}				{{
{a2}				}}");
			if (bind.Mode == BindingMode.TwoWay)
			{
				output.AppendLine(
$@"{a}				}}");
			}

			void GenerateSetSource(string expression, string? a)
			{
				if (me?.Member is MethodInfo)
				{
					output.AppendLine(
$@"{LineDirective(bind.Property.XamlNode)}
{a}					{expression}({setExpr});
#line default");
				}
				else
				{
					output.AppendLine(
$@"{LineDirective(bind.Property.XamlNode)}
{a}					{expression} = {setExpr};
#line default");
				}
			}
		}

		#endregion
	}

	protected virtual void GenerateBindingsExtraFieldDeclarations(StringBuilder output, BindingsData bindingsData)
	{
	}

	protected virtual void GenerateTrackingsExtraFieldDeclarations(StringBuilder output, BindingsData bindingsData)
	{
	}

	protected virtual void GenerateSetDependencyPropertyChangedCallback(StringBuilder output, TwoWayEventData ev, string targetExpr)
	{
	}

	protected virtual void GenerateUnsetDependencyPropertyChangedCallback(StringBuilder output, TwoWayEventData ev, string targetExpr)
	{
	}

	protected virtual void GenerateRegisterDependencyPropertyChangeEvent(StringBuilder output, NotifySource notifySource, NotifyProperty notifyProp, string cacheVar, string methodName)
	{
	}

	protected virtual void GenerateUnregisterDependencyPropertyChangeEvent(StringBuilder output, NotifySource notifySource, NotifyProperty notifyProp, string cacheVar, string methodName)
	{
	}

	protected virtual void GenerateDependencyPropertyChangedCallback(StringBuilder output, string methodName, string? a = null)
	{
	}

	protected virtual string GenerateConvertBackCall(Expression converter, string value, string targetType, string parameter, string? cast)
	{
		return $"{cast}{converter}.ConvertBack({value}, typeof(global::{targetType}), {parameter}, null)";
	}
}

