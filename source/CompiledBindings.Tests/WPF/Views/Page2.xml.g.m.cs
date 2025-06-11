namespace WPFTest.Views
{
#nullable disable

	[global::System.CodeDom.Compiler.GeneratedCode("CompiledBindings", null)]
	partial class Page2
	{
		private bool _generatedCodeInitialized;

		private void InitializeAfterConstructor()
		{
			if (_generatedCodeInitialized)
				return;

			_generatedCodeInitialized = true;

#line (18, 17) - (18, 40) 18 "Page2.xml"
			button3.Click += (p1, p2) => this.OnClick2();
#line (19, 17) - (19, 46) 19 "Page2.xml"
			button4.Click += (p1, p2) => this.Confirm(false);
#line (20, 17) - (20, 46) 20 "Page2.xml"
			button5.Click += (p1, p2) => this.Confirm(false);
#line default

			Bindings_.Initialize(this);
		}

		Page2_Bindings_ Bindings_ = new Page2_Bindings_();

		class Page2_Bindings_
		{
			Page2 _targetRoot;
			Page2_BindingsTrackings_ _bindingsTrackings;
			global::System.Windows.RoutedEventHandler _eventHandler2;
			global::System.Windows.RoutedEventHandler _eventHandler3;
			global::System.Threading.CancellationTokenSource _cts1;

			public void Initialize(Page2 dataRoot)
			{
				_targetRoot = dataRoot;
				_bindingsTrackings = new Page2_BindingsTrackings_(this);

				Update();

#line (16, 17) - (16, 41) 16 "Page2.xml"
				_eventHandler2 = dataRoot.OnClick1;
#line default
				_targetRoot.button1.Click += _eventHandler2;
#line (17, 17) - (17, 41) 17 "Page2.xml"
				_eventHandler3 = dataRoot.OnClick1;
#line default
				_targetRoot.button2.Click += _eventHandler3;

				_bindingsTrackings.SetPropertyChangedEventHandler0(dataRoot);
			}

			public void Cleanup()
			{
				if (_targetRoot != null)
				{
					_cts1?.Cancel();
					_targetRoot.button1.Click -= _eventHandler2;
					_eventHandler2 = null;
					_targetRoot.button2.Click -= _eventHandler3;
					_eventHandler3 = null;
					_bindingsTrackings.Cleanup();
					_targetRoot = null;
				}
			}

			public void Update()
			{
				var dataRoot = _targetRoot;
				Update0(dataRoot);
			}

			private void Update0(global::WPFTest.Views.Page2 value)
			{
				Update0_ObjProp(value);
				Update0_Foreground(value);
				Update0_IntProp(value);
			}

			private void Update1(global::WPFTest.Views.Class2 value)
			{
				Update1_Prop1(value);
				Update1_Prop2(value);
			}

			private void Update0_ObjProp(global::WPFTest.Views.Page2 value)
			{
#line (14, 20) - (14, 121) 14 "Page2.xml"
				var value1 = value.ObjProp;
#line default
				Update1(value1);
				_bindingsTrackings.SetPropertyChangedEventHandler1(value1);
			}

			private void Update0_Foreground(global::WPFTest.Views.Page2 value)
			{
#line (22, 13) - (22, 44) 22 "Page2.xml"
				_targetRoot.textBlock3.Foreground = value.Foreground;
#line default
			}

			private void Update0_IntProp(global::WPFTest.Views.Page2 value)
			{
#line (23, 13) - (23, 35) 23 "Page2.xml"
				_targetRoot.textBlock3.Text = value.IntProp.ToString();
#line default
			}

			private void Update1_Prop1(global::WPFTest.Views.Class2 value)
			{
#line (14, 20) - (14, 121) 14 "Page2.xml"
				_targetRoot.textBlock1.Text = (value != null ? $"{value.Prop1 ?? "Hi"}, World" : "no text");
#line default
			}

			private void Update1_Prop2(global::WPFTest.Views.Class2 value)
			{
				_cts1?.Cancel();
				_cts1 = new System.Threading.CancellationTokenSource();
				Set0(_cts1.Token);
				async void Set0(global::System.Threading.CancellationToken cancellationToken)
				{
					try
					{
#line (15, 20) - (15, 124) 15 "Page2.xml"
						var task = value?.Prop2;
#line default
						if (task?.IsCompleted != true)
						{
#line (15, 20) - (15, 124) 15 "Page2.xml"
							_targetRoot.textBlock2.Text = "loading...";
#line default
							if (task == null)
							{
								return;
							}
						}
#line (15, 20) - (15, 124) 15 "Page2.xml"
						var result = await task;
#line default
						if (!cancellationToken.IsCancellationRequested)
						{
							_targetRoot.textBlock2.Text = $"{result ?? "Hi"}, World";
						}
					}
					catch
					{
					}
				}
			}

			class Page2_BindingsTrackings_
			{
				global::System.WeakReference _bindingsWeakRef;
				object _propertyChangeSource0;
				global::System.ComponentModel.INotifyPropertyChanged _propertyChangeSource1;

				public Page2_BindingsTrackings_(Page2_Bindings_ bindings)
				{
					_bindingsWeakRef = new global::System.WeakReference(bindings);
				}

				public void Cleanup()
				{
					SetPropertyChangedEventHandler0(null);
					SetPropertyChangedEventHandler1(null);
				}

				public void SetPropertyChangedEventHandler0(global::WPFTest.Views.Page2 value)
				{
					if (_propertyChangeSource0 != null && !object.ReferenceEquals(_propertyChangeSource0, value))
					{
						((global::System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource0).PropertyChanged -= OnPropertyChanged0;
						global::System.ComponentModel.DependencyPropertyDescriptor
							.FromProperty(
								global::WPFTest.Views.Page2.ForegroundProperty, typeof(global::WPFTest.Views.Page2))
							.RemoveValueChanged(_propertyChangeSource0, OnPropertyChanged0_Foreground);
						_propertyChangeSource0 = null;
					}
					if (_propertyChangeSource0 == null && value != null)
					{
						_propertyChangeSource0 = value;
						((global::System.ComponentModel.INotifyPropertyChanged)value).PropertyChanged += OnPropertyChanged0;
						global::System.ComponentModel.DependencyPropertyDescriptor
							.FromProperty(
								global::WPFTest.Views.Page2.ForegroundProperty, typeof(global::WPFTest.Views.Page2))
							.AddValueChanged(_propertyChangeSource0, OnPropertyChanged0_Foreground);
					}
				}

				public void SetPropertyChangedEventHandler1(global::WPFTest.Views.Class2 value)
				{
					global::CompiledBindings.WPF.CompiledBindingsHelper.SetPropertyChangedEventHandler(ref _propertyChangeSource1, value, OnPropertyChanged1);
				}

				private void OnPropertyChanged0(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = global::CompiledBindings.WPF.CompiledBindingsHelper.TryGetBindings<Page2_Bindings_>(ref _bindingsWeakRef, Cleanup);
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::WPFTest.Views.Page2)sender;
					switch (e.PropertyName)
					{
						case null:
						case "":
							bindings.Update0(typedSender);
							break;
						case "ObjProp":
							bindings.Update0_ObjProp(typedSender);
							break;
						case "IntProp":
							bindings.Update0_IntProp(typedSender);
							break;
					}
				}

				private void OnPropertyChanged0_Foreground(object sender, global::System.EventArgs e)
				{
					var bindings = global::CompiledBindings.WPF.CompiledBindingsHelper.TryGetBindings<Page2_Bindings_>(ref _bindingsWeakRef, Cleanup);
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::WPFTest.Views.Page2)sender;
					bindings.Update0_Foreground(typedSender);
				}

				private void OnPropertyChanged1(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = global::CompiledBindings.WPF.CompiledBindingsHelper.TryGetBindings<Page2_Bindings_>(ref _bindingsWeakRef, Cleanup);
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::WPFTest.Views.Class2)sender;
					switch (e.PropertyName)
					{
						case null:
						case "":
							bindings.Update1(typedSender);
							break;
						case "Prop1":
							bindings.Update1_Prop1(typedSender);
							break;
						case "Prop2":
							bindings.Update1_Prop2(typedSender);
							break;
					}
				}
			}
		}
	}
}
