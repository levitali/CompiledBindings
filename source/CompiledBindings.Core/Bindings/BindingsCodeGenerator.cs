namespace CompiledBindings;

public class BindingsCodeGenerator : XamlCodeGenerator
{
	public const string CompiledBindingsHelperBaseCode =
@"		public static void SetPropertyChangedEventHandler(ref global::System.ComponentModel.INotifyPropertyChanged cache, global::System.ComponentModel.INotifyPropertyChanged source, global::System.ComponentModel.PropertyChangedEventHandler handler)
		{
			if (cache != null && !object.ReferenceEquals(cache, source))
			{
				cache.PropertyChanged -= handler;
				cache = null;
			}
			if (cache == null && source != null)
			{
				cache = source;
				cache.PropertyChanged += handler;
			}
		}

		public static void SetPropertyChangedEventHandler(ref global::System.ComponentModel.INotifyPropertyChanged cache, object source, global::System.ComponentModel.PropertyChangedEventHandler handler)
		{
			if (cache != null && !object.ReferenceEquals(cache, source))
			{
				cache.PropertyChanged -= handler;
				cache = null;
			}
			if (cache == null && source is System.ComponentModel.INotifyPropertyChanged npc)
			{
				cache = npc;
				cache.PropertyChanged += handler;
			}
		}

		public static void SetCollectionChangedEventHandler(ref global::System.Collections.Specialized.INotifyCollectionChanged cache, global::System.Collections.Specialized.INotifyCollectionChanged source, global::System.Collections.Specialized.NotifyCollectionChangedEventHandler handler)
		{
			if (cache != null && !object.ReferenceEquals(cache, source))
			{
				cache.CollectionChanged -= handler;
				cache = null;
			}
			if (cache == null && source != null)
			{
				cache = source;
				cache.CollectionChanged += handler;
			}
		}

		public static T TryGetBindings<T>(ref global::System.WeakReference bindingsWeakReference, global::System.Action cleanup)
			where T : class
		{
			T bindings = null;
			if (bindingsWeakReference != null)
			{
				bindings = (T)bindingsWeakReference.Target;
				if (bindings == null)
				{
					bindingsWeakReference = null;
					cleanup();
				}
			}
			return bindings;
		}

		public static bool IsCollectionChangedAtIndex(global::System.Collections.Specialized.NotifyCollectionChangedEventArgs args, int index)
		{
			if (args.Action == global::System.Collections.Specialized.NotifyCollectionChangedAction.Add)
			{
				return index >= args.NewStartingIndex && index < args.NewStartingIndex + args.NewItems.Count;
			}
			if (args.Action == global::System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
			{
				return index >= args.OldStartingIndex && index < args.OldStartingIndex + args.OldItems.Count;
			}
			if (args.Action == global::System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
			{
				return index >= args.NewStartingIndex && 
					((index < args.NewStartingIndex + args.NewItems.Count) ||
					 (index < args.OldStartingIndex + args.OldItems.Count));
			}
			if (args.Action == global::System.Collections.Specialized.NotifyCollectionChangedAction.Move)
			{
				return 
					(index >= args.NewStartingIndex && index < args.NewStartingIndex + args.NewItems.Count) ||
					(index >= args.OldStartingIndex && index < args.OldStartingIndex + args.OldItems.Count);
			}
			return true;
		}";

	private readonly string _compileBindingsHelderNs;

	public BindingsCodeGenerator(string compileBindingsHelderNs, string langVersion, string msbuildVersion) : base(langVersion, msbuildVersion)
	{
		_compileBindingsHelderNs = compileBindingsHelderNs;
	}

	public void GenerateBindingsClass(StringBuilder output, BindingsClass bindingsClass, string? targetClassName, string? nameSuffix = null)
	{
		bool isDiffDataRoot = bindingsClass.DataType.Reference.FullName != bindingsClass.TargetType?.Reference.FullName;
		var rootNotifySource = bindingsClass.NotifySources.SingleOrDefault(g => g.Expression is VariableExpression pe && pe.Name == "dataRoot");

		bool isLineDirective = false;

		#region Bindings Variable

		output.AppendLine(
$@"		{targetClassName}_Bindings{nameSuffix} Bindings{nameSuffix} = new {targetClassName}_Bindings{nameSuffix}();
");

		#endregion

		#region Class Begin

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
$@"			global::{bindingsClass.DataType.Reference.GetCSharpFullName()} _dataRoot;");
		}

		if (bindingsClass.NotifySources.Count > 0)
		{
			output.AppendLine(
$@"			{targetClassName}_BindingsTrackings{nameSuffix} _bindingsTrackings;");
		}

		// Generate _eventHandlerXXX fields for event bindings
		foreach (var binding in bindingsClass.Bindings.Where(b => b.Property.TargetEvent != null))
		{
			output.AppendLine(
$@"			global::{binding.Property.TargetEvent!.EventType.Reference.GetCSharpFullName()} _eventHandler{binding.Index};");
		}

		// Generate flags for two-way bindings
		foreach (var bind in bindingsClass.Bindings.Where(b => b.Mode == BindingMode.TwoWay))
		{
			output.AppendLine(
$@"			bool _settingBinding{bind.Index};");
		}

		// Generate CancellationTokenSources for cancelling asynchronous setting of properties
		var taskType = TypeInfo.GetTypeThrow(typeof(System.Threading.Tasks.Task));
		var taskBindings = bindingsClass.Bindings.Where(b => b.Expression != null && taskType.IsAssignableFrom(b.Expression.Type)).ToList();
		foreach (var bind in taskBindings)
		{
			output.AppendLine(
$@"			global::System.Threading.CancellationTokenSource _cts{bind.Index};");
		}

		GenerateBindingsExtraFieldDeclarations(output, bindingsClass);

		#endregion // Fields declaration

		#region Initialize Method

		// Start Initialize method
		if (isDiffDataRoot)
		{
			output.AppendLine(
$@"
			public void Initialize({targetClassName} targetRoot, global::{bindingsClass.DataType.Reference.GetCSharpFullName()} dataRoot)
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

		if (bindingsClass.NotifySources.Count > 0)
		{
			output.AppendLine(
$@"				_bindingsTrackings = new {targetClassName}_BindingsTrackings{nameSuffix}(this);");
		}

		output.AppendLine(
$@"
				Update();");

		// Generate code of event bindings
		var eventBindings = bindingsClass.Bindings.Where(b => b.Property.TargetEvent != null).ToList();
		if (eventBindings.Count > 0)
		{
			int dummyLocalVar = 0, dummyLocalFunc = 0;
			output.AppendLine();
			foreach (var binding in eventBindings)
			{
				GenerateSetValue(output, binding.Property, binding.Expression, "_targetRoot", ref dummyLocalVar, ref dummyLocalFunc, ref isLineDirective, "\t");
			}
			ResetLineDirective(output, ref isLineDirective);
		}

		// Generate setting PropertyChanged event handler for data root
		if (rootNotifySource != null)
		{
			int index = bindingsClass.NotifySources.IndexOf(rootNotifySource);
			output.AppendLine(
$@"
				_bindingsTrackings.SetPropertyChangedEventHandler{index}(dataRoot);");
		}

		// Set event handlers for two-way bindings
		if (bindingsClass.TwoWayEvents.Count > 0)
		{
			output.AppendLine();
			foreach (var ev in bindingsClass.TwoWayEvents)
			{
				var first = ev.Bindings[0];
				string targetExpr = "_targetRoot";
				if (first.Property.Object.Name != null)
				{
					targetExpr += "." + first.Property.Object.Name;
				}

				if (ev.TargetChangedEvents != null)
				{
					foreach (var evnt in ev.TargetChangedEvents)
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

		output.AppendLine(
$@"
			public void Cleanup()
			{{
				if (_targetRoot != null)
				{{");

		// Cancel asynchronous bindings
		foreach (var bind in taskBindings)
		{
			output.AppendLine(
$@"					_cts{bind.Index}?.Cancel();");
		}

		// Unset event handlers for two-way bindings
		foreach (var ev in bindingsClass.TwoWayEvents)
		{
			var first = ev.Bindings[0];
			string targetExpr = "_targetRoot";
			if (first.Property.Object.Name != null)
			{
				targetExpr += "." + first.Property.Object.Name;
			}

			if (ev.TargetChangedEvents != null)
			{
				foreach (var evnt in ev.TargetChangedEvents)
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
		foreach (var binding in bindingsClass.Bindings.Where(b => b.Property.TargetEvent != null))
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

		if (bindingsClass.NotifySources.Count > 0)
		{
			output.AppendLine(
$@"					_bindingsTrackings.Cleanup();");
		}

		if (isDiffDataRoot && bindingsClass.DataType.Reference.IsNullable())
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

		output.AppendLine(
$@"
			public void Update()
			{{");
		generateUpdateMethodBody(bindingsClass.UpdateMethod);
		output.AppendLine(
$@"			}}");

		#endregion

		#region UpateXX Methods

		foreach (var notifySource in bindingsClass.NotifySources.Where(s => s.UpdateMethod != null))
		{
			generateUpdateMethod(notifySource.UpdateMethod!, $"Update{notifySource.Index}");
		}

		#endregion

		#region UpdateXX_XXX Methods

		foreach (var notifySource in bindingsClass.NotifySources)
		{
			foreach (var prop in notifySource.Properties.Where(_ => _.UpdateMethod != null))
			{
				generateUpdateMethod(prop.UpdateMethod!, $"Update{notifySource.Index}_{prop.PropertyCodeName}");
			}
		}

		#endregion UpdateXX_XXX Methods

		#region OnTargetChanged methods

		foreach (var ev in bindingsClass.TwoWayEvents)
		{
			output.AppendLine();

			if (ev.TargetChangedEvents != null)
			{
				output.AppendLine(
$@"			private void OnTargetChanged{ev.Index}({string.Join(", ", ev.TargetChangedEvents[0].GetEventHandlerParameterTypes().Select((t, i) => $"global::{t.Reference.GetCSharpFullName()} p{i}"))})");
			}
			else if (ev.Bindings[0].DependencyProperty != null)
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

			if (ev.TargetChangedEvents?[0].Definition.Name == "PropertyChanged")
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
						generateSetSource(bind, "\t\t");
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
					generateSetSource(bind, null);
				}
			}

			output.AppendLine(
$@"			}}");
		}

		#endregion

		#region Trackings Class

		if (bindingsClass.NotifySources.Count > 0)
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
			foreach (var notifySource in bindingsClass.NotifySources)
			{
				var type = getDiffTypesCount(notifySource) > 1
					? "object"
					: notifySource.AnyINotifyPropertyChangedProperty
						? "global::System.ComponentModel.INotifyPropertyChanged"
						: notifySource.AnyCollectionChangedElementAccess
							? "global::System.Collections.Specialized.INotifyCollectionChanged"
							: DependencyObjectType;
				output.AppendLine(
$@"				{type} _propertyChangeSource{notifySource.Index};");
				if (notifySource.AnyDependencyProperty)
				{
					GenerateDependencyPropertyChangeExtraVariables(output, notifySource);
				}
			}

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
			foreach (var group in bindingsClass.NotifySources)
			{
				output.AppendLine(
$@"					SetPropertyChangedEventHandler{group.Index}(null);");
			}

			// Close the Cleanup method
			output.AppendLine(
$@"				}}");

			#endregion

			#region SetPropertyChangedEventHandler methods

			foreach (var notifySource in bindingsClass.NotifySources)
			{
				string cacheVar = "_propertyChangeSource" + notifySource.Index;

				output.AppendLine();
				output.AppendLine(
$@"				public void SetPropertyChangedEventHandler{notifySource.Index}(global::{notifySource.Expression.Type.Reference.GetCSharpFullName()} value)
				{{");
				if (!notifySource.AnyDependencyProperty && getDiffTypesCount(notifySource) == 1)
				{
					string method, handler;
					if (notifySource.AnyINotifyPropertyChangedProperty)
					{
						method = "SetPropertyChangedEventHandler";
						handler = $"OnPropertyChanged{notifySource.Index}";
					}
					else
					{
						method = "SetCollectionChangedEventHandler";
						handler = $"OnCollectionChanged{notifySource.Index}";
					}
					output.AppendLine(
$@"					global::{_compileBindingsHelderNs}.CompiledBindingsHelper.{method}(ref {cacheVar}, value, {handler});");
				}
				else
				{
					output.AppendLine(
$@"					if ({cacheVar} != null && !object.ReferenceEquals({cacheVar}, value))
					{{");
					if (notifySource.AnyINotifyPropertyChangedProperty)
					{
						if (notifySource.IsINotifyPropertyChanged)
						{
							output.AppendLine(
$@"						((global::System.ComponentModel.INotifyPropertyChanged){cacheVar}).PropertyChanged -= OnPropertyChanged{notifySource.Index};");
						}
						else
						{
							output.AppendLine(
$@"						if ({cacheVar} is global::System.ComponentModel.INotifyPropertyChanged npc)
						{{
							npc.PropertyChanged -= OnPropertyChanged{notifySource.Index};
						}}");
						}
					}

					if (notifySource.AnyCollectionChangedElementAccess)
					{
						output.AppendLine(
$@"						((global::System.Collections.Specialized.INotifyCollectionChanged){cacheVar}).CollectionChanged -= OnCollectionChanged{notifySource.Index};");
					}

					foreach (var notifyProp in notifySource.Properties.Where(p => p.IsDependencyProp))
					{
						GenerateUnregisterDependencyPropertyChangeEvent(output, notifySource, notifyProp, cacheVar, $"OnPropertyChanged{notifySource.Index}_{notifyProp.PropertyCodeName}");
					}
					output.AppendLine(
$@"						{cacheVar} = null;
					}}
					if ({cacheVar} == null && value != null)
					{{");
					output.AppendLine(
$@"						{cacheVar} = value;");
					if (notifySource.AnyINotifyPropertyChangedProperty)
					{
						if (notifySource.IsINotifyPropertyChanged)
						{
							output.AppendLine(
$@"						((global::System.ComponentModel.INotifyPropertyChanged)value).PropertyChanged += OnPropertyChanged{notifySource.Index};");
						}
						else
						{
							output.AppendLine(
$@"						if (value is global::System.ComponentModel.INotifyPropertyChanged npc)
						{{
							npc.PropertyChanged += OnPropertyChanged{notifySource.Index};
						}}");
						}
					}

					if (notifySource.AnyCollectionChangedElementAccess)
					{
						output.AppendLine(
$@"						((global::System.Collections.Specialized.INotifyCollectionChanged)value).CollectionChanged += OnCollectionChanged{notifySource.Index};");
					}

					foreach (var notifyProp in notifySource.Properties.Where(p => p.IsDependencyProp))
					{
						GenerateRegisterDependencyPropertyChangeEvent(output, notifySource, notifyProp, cacheVar, $"OnPropertyChanged{notifySource.Index}_{notifyProp.PropertyCodeName}");
					}
					output.AppendLine(
$@"					}}");
				}
				output.AppendLine(
$@"				}}");
			}

			#endregion

			#region OnPropertyChange methods

			foreach (var notifySource in bindingsClass.NotifySources)
			{
				if (notifySource.AnyINotifyPropertyChangedProperty)
				{
					output.AppendLine(
$@"
				private void OnPropertyChanged{notifySource.Index}(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
				{{");
					output.AppendLine(
$@"					var bindings = global::{_compileBindingsHelderNs}.CompiledBindingsHelper.TryGetBindings<{targetClassName}_Bindings{nameSuffix}>(ref _bindingsWeakRef, Cleanup);
					if (bindings == null)
					{{
						return;
					}}

					var typedSender = (global::{notifySource.Expression.Type.Reference.GetCSharpFullName()})sender;");

					if (notifySource.ManyINotifyPropertyChangedProperties)
					{
						output.AppendLine(
$@"					switch (e.PropertyName)
					{{
						case null:
						case """":
							bindings.Update{notifySource.Index}(typedSender);
							break;");

						foreach (var prop in notifySource.INotifyPropChangedProperties)
						{
							foreach (var propName in prop.PropertyNames)
							{
								output.AppendLine(
$@"						case ""{propName}"":");
							}
							output.AppendLine(
$@"							bindings.Update{notifySource.Index}_{prop.PropertyCodeName}(typedSender);
							break;");
						}
						output.AppendLine(
$@"					}}");
					}
					else
					{
						var prop = notifySource.Properties.Single(p => !p.IsDependencyProp);
						output.AppendLine(
$@"					if (string.IsNullOrEmpty(e.PropertyName) || {string.Join(" || ", prop.PropertyNames.Select(n => $"e.PropertyName == \"{n}\""))})
					{{
						bindings.Update{notifySource.Index}_{prop.PropertyCodeName}(typedSender);
					}}");
					}
					output.AppendLine(
$@"				}}");

				}

				if (notifySource.AnyCollectionChangedElementAccess)
				{
					output.AppendLine(
$@"
				private void OnCollectionChanged{notifySource.Index}(object sender, global::System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
				{{");
					output.AppendLine(
$@"					var bindings = global::{_compileBindingsHelderNs}.CompiledBindingsHelper.TryGetBindings<{targetClassName}_Bindings{nameSuffix}>(ref _bindingsWeakRef, Cleanup);
					if (bindings == null)
					{{
						return;
					}}

					var typedSender = (global::{notifySource.Expression.Type.Reference.GetCSharpFullName()})sender;");

					string @else = "";
					foreach (var prop in notifySource.Properties
						.Where(p => p.IsCollectionChangedElementAccess && p.PropertyCodeName != "Item"))
					{
						var index = ((ElementAccessExpression)prop.Expression).Parameters[0].CSharpCode;
						output.AppendLine(
$@"					{@else}if (global::{_compileBindingsHelderNs}.CompiledBindingsHelper.IsCollectionChangedAtIndex(e, {index}))
					{{
						bindings.Update{notifySource.Index}_{prop.PropertyCodeName}(typedSender);
					}}");
						@else = "else ";
					}

					if (notifySource.Properties.Any(p => p.IsCollectionChangedElementAccess && p.PropertyCodeName == "Item"))
					{
						output.AppendLine(
$@"					bindings.Update{notifySource.Index}_Item(typedSender);");
					}

					output.AppendLine(
$@"				}}");
				}

				foreach (var prop in notifySource.Properties.Where(p => p.IsDependencyProp))
				{
					output.AppendLine();
					GenerateDependencyPropertyChangedCallback(output, $"OnPropertyChanged{notifySource.Index}_{prop.PropertyCodeName}", "\t");
					output.AppendLine(
$@"				{{");
					output.AppendLine(
$@"					var bindings = global::{_compileBindingsHelderNs}.CompiledBindingsHelper.TryGetBindings<{targetClassName}_Bindings{nameSuffix}>(ref _bindingsWeakRef, Cleanup);
					if (bindings == null)
					{{
						return;
					}}

					var typedSender = (global::{notifySource.Expression.Type.Reference.GetCSharpFullName()})sender;");

					output = output.AppendLine(
$@"					bindings.Update{notifySource.Index}_{prop.PropertyCodeName}(typedSender);");
					output.AppendLine(
$@"				}}");
				}
			}

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

		static int getDiffTypesCount(NotifySource notifySource)
		{
			int diffTypes = 0;
			if (notifySource.AnyINotifyPropertyChangedProperty)
				diffTypes++;
			if (notifySource.AnyCollectionChangedElementAccess)
				diffTypes++;
			if (notifySource.AnyDependencyProperty)
				diffTypes++;
			return diffTypes;
		}

		void generateUpdateMethod(UpdateMethod updateMethod, string name)
		{
			output.AppendLine();
			output.AppendLine(
$@"			private void {name}({string.Join(", ", updateMethod.Parameters.Select(p => $"global::{p.Type.Reference.GetCSharpFullName()} {p.Name}"))})
			{{");
			generateUpdateMethodBody(updateMethod);
			output.AppendLine(
$@"			}}");
		}

		void generateUpdateMethodBody(UpdateMethod updateMethod)
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

			GenerateSetExpressions(output, updateMethod.Expressions, ref isLineDirective, targetRootVariable: "_targetRoot", a: "\t");

			foreach (var notifSource in updateMethod.UpdateNotifySources)
			{
				output.AppendLine(
$@"				Update{notifSource.Index}({notifSource.SourceExpression});");
			}

			foreach (var notifProp in updateMethod.UpdateNotifyProperties)
			{
				output.AppendLine(
$@"				Update{notifProp.Parent.Index}_{notifProp.PropertyCodeName}({notifProp.SourceExpression});");
			}

			foreach (var notifSource in updateMethod.SetEventHandlers.Where(g => g != rootNotifySource))
			{
				output.AppendLine(
$@"				_bindingsTrackings.SetPropertyChangedEventHandler{notifSource.Index}({notifSource.SourceExpression});");
			}
		}

		void generateSetSource(Bind bind, string? a)
		{
			var expr = bind.BindBackExpression ?? bind.Expression!;
			var me = expr as MemberExpression;
			var ae = expr.EnumerateTree().OrderByDescending(e => e.CSharpCode.Length).OfType<IAccessExpression>().FirstOrDefault(e => e.Expression.IsNullable);

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
				string sourceTypeFullName = sourceType.Reference.GetCSharpFullName();
				string? cast = sourceTypeFullName == "System.Object" ? null : $"(global::{sourceTypeFullName})";
				string parameter = bind.ConverterParameter?.CSharpCode ?? "null";
				setExpr = GenerateConvertBackCall(bind.Converter, setExpr, sourceTypeFullName, parameter, cast);
			}
			else if (sourceType.Reference.FullName == "System.String" && bind.Property.MemberType.Reference.FullName != "System.String")
			{
				if (bind.Property.MemberType.IsNullable)
				{
					setExpr += "?";
				}
				setExpr += ".ToString()";
			}
			else if (!sourceType.IsAssignableFrom(bind.Property.MemberType))
			{
				var sourceType2 = sourceType.Reference;
				var targetType = bind.Property.MemberType.Reference;
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

					if (bind.Property.MemberType.Reference.IsNullable())
					{
						var checkNull = bind.Property.MemberType.Reference.FullName == "System.String" ? $"string.IsNullOrEmpty(t{bind.Index})" : $"t{bind.Index} == null";
						var @default = sourceType.Reference.IsNullable() ? "null" : "default";
						setExpr = $"{setExpr} is var t{bind.Index} && {checkNull} ? {@default} : (global::{sourceType.Reference.GetCSharpFullName()})global::System.Convert.ChangeType(t{bind.Index}, typeof(global::{sourceType2.GetCSharpFullName()}), null)";
					}
					else
					{
						setExpr = $"(global::{sourceType.Reference.GetCSharpFullName()})global::System.Convert.ChangeType({setExpr}, typeof(global::{sourceType2.GetCSharpFullName()}), null)";
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
$@"{LineDirective(bind.Property.XamlNode, ref isLineDirective)}
{a2}					var value = {ae!.Expression};
{ResetLineDirective(ref isLineDirective)}
{a2}					if (value != null)
{a2}					{{");
				generateSetSource(memberExpr2.CSharpCode, a2 + '\t');
				output.AppendLine(
$@"{a2}					}}");
			}
			else
			{
				generateSetSource(expr.CSharpCode, a2);
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

			void generateSetSource(string expression, string? a)
			{
				if (me?.Member is MethodInfo)
				{
					output.AppendLine(
$@"{LineDirective(bind.Property.XamlNode, ref isLineDirective)}
{a}					{expression}({setExpr});
{ResetLineDirective(ref isLineDirective)}");
				}
				else
				{
					output.AppendLine(
$@"{LineDirective(bind.Property.XamlNode, ref isLineDirective)}
{a}					{expression} = {setExpr};
{ResetLineDirective(ref isLineDirective)}");
				}
			}
		}

		#endregion
	}

	protected virtual void GenerateBindingsExtraFieldDeclarations(StringBuilder output, BindingsClass bindingsData)
	{
	}

	protected virtual void GenerateSetDependencyPropertyChangedCallback(StringBuilder output, TwoWayBinding binding, string targetExpr)
	{
	}

	protected virtual void GenerateUnsetDependencyPropertyChangedCallback(StringBuilder output, TwoWayBinding binding, string targetExpr)
	{
	}

	protected virtual string DependencyObjectType => string.Empty;

	protected virtual void GenerateDependencyPropertyChangeExtraVariables(StringBuilder output, NotifySource notifySource)
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

