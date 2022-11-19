namespace WPFTest.Views
{
	using WPFTest.Views;

#nullable disable

	[global::System.CodeDom.Compiler.GeneratedCode("CompiledBindings", null)]
	partial class Page1
	{
		global::System.Threading.CancellationTokenSource _generatedCodeDisposed = new global::System.Threading.CancellationTokenSource();
		global::System.Windows.Data.IValueConverter TrueToVisibleConverter;
		global::System.Windows.Data.IValueConverter InverseBooleanConverter;
		private bool _generatedCodeInitialized;

		private void InitializeAfterConstructor()
		{
			if (_generatedCodeInitialized)
				return;

			_generatedCodeInitialized = true;

			TrueToVisibleConverter = (global::System.Windows.Data.IValueConverter)(this.Resources["TrueToVisibleConverter"] ?? global::System.Windows.Application.Current.Resources["TrueToVisibleConverter"] ?? throw new global::System.Exception("Resource 'TrueToVisibleConverter' not found."));
			InverseBooleanConverter = (global::System.Windows.Data.IValueConverter)(this.Resources["InverseBooleanConverter"] ?? global::System.Windows.Application.Current.Resources["InverseBooleanConverter"] ?? throw new global::System.Exception("Resource 'InverseBooleanConverter' not found."));

#line (15, 5) - (15, 50) 15 "Page1.xml"
			var value1 = WPFTest.Strings.Instance;
#line (15, 5) - (15, 50) 15 "Page1.xml"
			Title = value1.Title;
#line default
			Set0(_generatedCodeDisposed.Token);
			async void Set0(global::System.Threading.CancellationToken cancellationToken)
			{
				try
				{
#line (42, 16) - (42, 48) 42 "Page1.xml"
					var result = await this.LoadImageAsync();
#line default
					if (!cancellationToken.IsCancellationRequested)
					{
						image1.Source = result;
					}
				}
				catch
				{
				}
			}
#line (48, 13) - (48, 57) 48 "Page1.xml"
			header1.Text = value1.Header1;
#line (78, 13) - (78, 55) 78 "Page1.xml"
			global::WPFTest.TestExtensions.SetMyProperty(checkBox2, 6);
#line default

			Bindings_.Initialize(this);
			this.DataContextChanged += this_DataContextChanged;
			if (this.DataContext is global::WPFTest.ViewModels.Page1ViewModel dataRoot1)
			{
				Bindings_this.Initialize(this, dataRoot1);
			}
		}

		private void this_DataContextChanged(object sender, global::System.Windows.DependencyPropertyChangedEventArgs e)
		{
			Bindings_this.Cleanup();
			if (((global::System.Windows.FrameworkElement)sender).DataContext is global::WPFTest.ViewModels.Page1ViewModel dataRoot)
			{
				Bindings_this.Initialize(this, dataRoot);
			}
		}

		Page1_Bindings_ Bindings_ = new Page1_Bindings_();

		class Page1_Bindings_
		{
			Page1 _targetRoot;
			Page1_BindingsTrackings_ _bindingsTrackings;

			public void Initialize(Page1 dataRoot)
			{
				_targetRoot = dataRoot;
				_bindingsTrackings = new Page1_BindingsTrackings_(this);

				Update();
			}

			public void Cleanup()
			{
				if (_targetRoot != null)
				{
					_bindingsTrackings.Cleanup();
					_targetRoot = null;
				}
			}

			public void Update()
			{
				var dataRoot = _targetRoot;
				Update0_DecimalProp(dataRoot._viewModel.DecimalProp);
				Update1_SelectedItem(dataRoot.listView.SelectedItem);
				_bindingsTrackings.SetPropertyChangedEventHandler0(dataRoot._viewModel);
				_bindingsTrackings.SetPropertyChangedEventHandler1(dataRoot.listView);
			}

			private void Update0_DecimalProp(global::System.Decimal value)
			{
#line (45, 13) - (45, 52) 45 "Page1.xml"
				_targetRoot.textBlock5.Text = value.ToString();
#line default
			}

			private void Update1_SelectedItem(global::System.Object value)
			{
#line (68, 13) - (68, 81) 68 "Page1.xml"
				_targetRoot.button1.IsEnabled = value != null;
#line default
			}

			class Page1_BindingsTrackings_
			{
				global::System.WeakReference _bindingsWeakRef;
				global::WPFTest.ViewModels.Page1ViewModel _propertyChangeSource0;
				global::System.Windows.Controls.ListView _propertyChangeSource1;

				public Page1_BindingsTrackings_(Page1_Bindings_ bindings)
				{
					_bindingsWeakRef = new global::System.WeakReference(bindings);
				}

				public void Cleanup()
				{
					SetPropertyChangedEventHandler0(null);
					SetPropertyChangedEventHandler1(null);
				}

				public void SetPropertyChangedEventHandler0(global::WPFTest.ViewModels.Page1ViewModel value)
				{
					if (_propertyChangeSource0 != null && !object.ReferenceEquals(_propertyChangeSource0, value))
					{
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource0).PropertyChanged -= OnPropertyChanged0;
						_propertyChangeSource0 = null;
					}
					if (_propertyChangeSource0 == null && value != null)
					{
						_propertyChangeSource0 = value;
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource0).PropertyChanged += OnPropertyChanged0;
					}
				}

				public void SetPropertyChangedEventHandler1(global::System.Windows.Controls.ListView value)
				{
					if (_propertyChangeSource1 != null && !object.ReferenceEquals(_propertyChangeSource1, value))
					{
						global::System.ComponentModel.DependencyPropertyDescriptor
							.FromProperty(
								global::System.Windows.Controls.ListView.SelectedItemProperty, typeof(global::System.Windows.Controls.ListView))
							.RemoveValueChanged(_propertyChangeSource1, OnPropertyChanged1_SelectedItem);
						_propertyChangeSource1 = null;
					}
					if (_propertyChangeSource1 == null && value != null)
					{
						_propertyChangeSource1 = value;
						global::System.ComponentModel.DependencyPropertyDescriptor
							.FromProperty(
								global::System.Windows.Controls.ListView.SelectedItemProperty, typeof(global::System.Windows.Controls.ListView))
							.AddValueChanged(_propertyChangeSource1, OnPropertyChanged1_SelectedItem);
					}
				}

				private void OnPropertyChanged0(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::WPFTest.ViewModels.Page1ViewModel)sender;
					if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "DecimalProp")
					{
						bindings.Update0_DecimalProp(typedSender.DecimalProp);
					}
				}

				private void OnPropertyChanged1_SelectedItem(object sender, global::System.EventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::System.Windows.Controls.ListView)sender;
					bindings.Update1_SelectedItem(typedSender.SelectedItem);
				}

				Page1_Bindings_ TryGetBindings()
				{
					Page1_Bindings_ bindings = null;
					if (_bindingsWeakRef != null)
					{
						bindings = (Page1_Bindings_)_bindingsWeakRef.Target;
						if (bindings == null)
						{
							_bindingsWeakRef = null;
							Cleanup();
						}
					}
					return bindings;
				}
			}
		}

		Page1_Bindings_this Bindings_this = new Page1_Bindings_this();

		class Page1_Bindings_this
		{
			Page1 _targetRoot;
			global::WPFTest.ViewModels.Page1ViewModel _dataRoot;
			Page1_BindingsTrackings_this _bindingsTrackings;
			global::System.Windows.RoutedEventHandler _eventHandler14;
			bool _settingBinding4;
			bool _settingBinding6;
			bool _settingBinding15;
			bool _settingBinding17;
			bool _settingBinding18;
			bool _settingBinding19;
			bool _settingBinding20;
			bool _settingBinding22;
			global::System.Threading.CancellationTokenSource _generatedCodeDisposed;

			public void Initialize(Page1 targetRoot, global::WPFTest.ViewModels.Page1ViewModel dataRoot)
			{
				_targetRoot = targetRoot;
				_dataRoot = dataRoot;
				_generatedCodeDisposed = new global::System.Threading.CancellationTokenSource();
				_bindingsTrackings = new Page1_BindingsTrackings_this(this);

				Update();

#line (69, 13) - (69, 65) 69 "Page1.xml"
				_eventHandler14 = (p1, p2) => dataRoot.ModifyViewModel?.OnClick(dataRoot.BooleanProp);
#line default
				_targetRoot.button1.Click += _eventHandler14;
#line default

				_bindingsTrackings.SetPropertyChangedEventHandler0(dataRoot);

				_targetRoot.textBox4.GotFocus += OnTargetChanged0;
				_targetRoot.textBox4.LostFocus += OnTargetChanged0;
				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::System.Windows.Controls.TextBox.TextProperty,
						typeof(global::System.Windows.Controls.TextBox))
					.AddValueChanged(_targetRoot.textBox1, OnTargetChanged1);
				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty,
						typeof(global::System.Windows.Controls.Primitives.ToggleButton))
					.AddValueChanged(_targetRoot.checkBox1, OnTargetChanged2);
				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::System.Windows.Controls.TextBox.TextProperty,
						typeof(global::System.Windows.Controls.TextBox))
					.AddValueChanged(_targetRoot.textBox2, OnTargetChanged3);
				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::WPFTest.TestExtensions.MyPropertyProperty,
						typeof(global::WPFTest.TestExtensions))
					.AddValueChanged(_targetRoot.textBox3, OnTargetChanged4);
				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::System.Windows.Controls.TextBox.TextProperty,
						typeof(global::System.Windows.Controls.TextBox))
					.AddValueChanged(_targetRoot.textBox3, OnTargetChanged5);
				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::System.Windows.Controls.TextBox.TextProperty,
						typeof(global::System.Windows.Controls.TextBox))
					.AddValueChanged(_targetRoot.textBox4, OnTargetChanged6);
				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty,
						typeof(global::System.Windows.Controls.Primitives.ToggleButton))
					.AddValueChanged(_targetRoot.checkBox2, OnTargetChanged7);
			}

			public void Cleanup()
			{
				if (_targetRoot != null)
				{
					_generatedCodeDisposed.Cancel();
					_targetRoot.textBox4.GotFocus -= OnTargetChanged0;
					_targetRoot.textBox4.LostFocus -= OnTargetChanged0;
					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::System.Windows.Controls.TextBox.TextProperty,
							typeof(global::System.Windows.Controls.TextBox))
						.RemoveValueChanged(_targetRoot.textBox1, OnTargetChanged1);
					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty,
							typeof(global::System.Windows.Controls.Primitives.ToggleButton))
						.RemoveValueChanged(_targetRoot.checkBox1, OnTargetChanged2);
					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::System.Windows.Controls.TextBox.TextProperty,
							typeof(global::System.Windows.Controls.TextBox))
						.RemoveValueChanged(_targetRoot.textBox2, OnTargetChanged3);
					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::WPFTest.TestExtensions.MyPropertyProperty,
							typeof(global::WPFTest.TestExtensions))
						.RemoveValueChanged(_targetRoot.textBox3, OnTargetChanged4);
					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::System.Windows.Controls.TextBox.TextProperty,
							typeof(global::System.Windows.Controls.TextBox))
						.RemoveValueChanged(_targetRoot.textBox3, OnTargetChanged5);
					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::System.Windows.Controls.TextBox.TextProperty,
							typeof(global::System.Windows.Controls.TextBox))
						.RemoveValueChanged(_targetRoot.textBox4, OnTargetChanged6);
					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty,
							typeof(global::System.Windows.Controls.Primitives.ToggleButton))
						.RemoveValueChanged(_targetRoot.checkBox2, OnTargetChanged7);
					_targetRoot.button1.Click -= _eventHandler14;
					_eventHandler14 = null;
					_bindingsTrackings.Cleanup();
					_dataRoot = null;
					_targetRoot = null;
				}
			}

			public void Update()
			{
				var dataRoot = _dataRoot;
#line (60, 13) - (60, 43) 60 "Page1.xml"
				_targetRoot.listView.ItemsSource = dataRoot.ListProp;
#line (65, 20) - (65, 79) 65 "Page1.xml"
				_targetRoot.textBlock12.Text = dataRoot.Calculate(new WPFTest.ViewModels.ParamClass("text"));
#line default
				Set0(_generatedCodeDisposed.Token);
				async void Set0(global::System.Threading.CancellationToken cancellationToken)
				{
					try
					{
#line (66, 16) - (66, 49) 66 "Page1.xml"
						var result = await dataRoot.LoadImageAsync();
#line default
						if (!cancellationToken.IsCancellationRequested)
						{
							_targetRoot.image2.Source = result;
						}
					}
					catch
					{
					}
				}
#line default
				Update0(dataRoot);
			}

			private void Update0(global::WPFTest.ViewModels.Page1ViewModel value)
			{
#line (51, 13) - (51, 39) 51 "Page1.xml"
				var value1 = value.DecimalProp;
#line (72, 13) - (72, 75) 72 "Page1.xml"
				var value2 = value.IntProp;
#line (51, 13) - (51, 39) 51 "Page1.xml"
				_targetRoot.textBlock6.Text = value1.ToString();
#line (53, 20) - (53, 50) 53 "Page1.xml"
				_targetRoot.textBlock7.Text = (value1 + 1).ToString();
				_settingBinding17 = true;
				try
				{
#line (72, 13) - (72, 75) 72 "Page1.xml"
					global::WPFTest.TestExtensions.SetMyProperty(_targetRoot.textBox3, value2);
				}
				finally
				{
					_settingBinding17 = false;
				}
#line (77, 13) - (77, 42) 77 "Page1.xml"
				global::System.Windows.Controls.Grid.SetColumn(_targetRoot.checkBox2, value2);
#line default
				Update0_ModifyViewModel(value.ModifyViewModel);
				Update0_BooleanProp(value.BooleanProp);
				Update0_ArrayProp(value.ArrayProp);
				Update0_OrderInput(value.OrderInput);
				Update0_TaskProp(value.TaskProp);
				Update0_FocusedField(value.FocusedField);
			}

			private void Update1(global::WPFTest.ViewModels.Page1ModifyViewModel value)
			{
				Update1_Input1(value?.Input1);
				Update1_IntInput(value?.IntInput);
				Update1_CanChangeInput1(value?.CanChangeInput1);
				Update1_BoolInput(value?.BoolInput);
				Update1_ModifyTaskProp(value?.ModifyTaskProp);
			}

			private void Update2(global::WPFTest.ViewModels.Page1ModifyTextViewModel value)
			{
				Update2_TextInput(value?.TextInput);
				Update2_BoolInput(value?.BoolInput);
			}

			private void Update0_BooleanProp(global::System.Boolean value)
			{
#line (49, 13) - (49, 67) 49 "Page1.xml"
				_targetRoot.header1.Visibility = (value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed);
#line (52, 13) - (52, 101) 52 "Page1.xml"
				_targetRoot.textBlock6.Visibility = ((global::System.Windows.Visibility)_targetRoot.TrueToVisibleConverter.Convert(value, typeof(global::System.Windows.Visibility), null, null));
#line default
			}

			private void Update0_DecimalProp(global::System.Decimal value)
			{
				var dataRoot = _dataRoot;
#line (51, 13) - (51, 39) 51 "Page1.xml"
				_targetRoot.textBlock6.Text = value.ToString();
#line (53, 20) - (53, 50) 53 "Page1.xml"
				_targetRoot.textBlock7.Text = (value + 1).ToString();
#line (55, 20) - (55, 97) 55 "Page1.xml"
				_targetRoot.textBlock8.Text = (((global::System.Int32)value) + dataRoot.IntProp + dataRoot.ModifyViewModel?.IntInput)?.ToString();
#line default
			}

			private void Update0_OrderInput(global::System.String value)
			{
				if (!object.Equals(_targetRoot.textBox1.Text, value))
				{
					_settingBinding4 = true;
					try
					{
						_targetRoot.textBox1.Text = value;
					}
					finally
					{
						_settingBinding4 = false;
					}
				}
#line default
			}

			private void Update0_IntProp(global::System.Int32 value)
			{
				var dataRoot = _dataRoot;
#line (55, 20) - (55, 97) 55 "Page1.xml"
				_targetRoot.textBlock8.Text = (((global::System.Int32)dataRoot.DecimalProp) + value + dataRoot.ModifyViewModel?.IntInput)?.ToString();
				_settingBinding17 = true;
				try
				{
#line (72, 13) - (72, 75) 72 "Page1.xml"
					global::WPFTest.TestExtensions.SetMyProperty(_targetRoot.textBox3, value);
				}
				finally
				{
					_settingBinding17 = false;
				}
#line (77, 13) - (77, 42) 77 "Page1.xml"
				global::System.Windows.Controls.Grid.SetColumn(_targetRoot.checkBox2, value);
#line default
			}

			private void Update0_ModifyViewModel(global::WPFTest.ViewModels.Page1ModifyViewModel value)
			{
#line (73, 13) - (73, 86) 73 "Page1.xml"
				var value1 = value?.ModifyTextViewModel;
#line default
				Update1(value);
				Update2(value1);
				_bindingsTrackings.SetPropertyChangedEventHandler1(value);
				_bindingsTrackings.SetPropertyChangedEventHandler2(value1);
			}

			private void Update0_BoolInput(global::System.Boolean value)
			{
				var dataRoot = _dataRoot;
#line (56, 19) - (56, 154) 56 "Page1.xml"
				var value1 = ((global::System.Boolean?)_targetRoot.InverseBooleanConverter.Convert(value, typeof(global::System.Boolean?), dataRoot.ArrayProp.Length > 0, null));
#line default
				if (!object.Equals(_targetRoot.checkBox1.IsChecked, value1))
				{
					_settingBinding6 = true;
					try
					{
						_targetRoot.checkBox1.IsChecked = value1;
					}
					finally
					{
						_settingBinding6 = false;
					}
				}
#line default
			}

			private void Update0_ArrayProp(global::System.Int32[] value)
			{
				var dataRoot = _dataRoot;
#line (56, 19) - (56, 154) 56 "Page1.xml"
				var value1 = value.Length > 0;
#line (56, 19) - (56, 154) 56 "Page1.xml"
				var value2 = ((global::System.Boolean?)_targetRoot.InverseBooleanConverter.Convert(dataRoot.BoolInput, typeof(global::System.Boolean?), value1, null));
#line default
				if (!object.Equals(_targetRoot.checkBox1.IsChecked, value2))
				{
					_settingBinding6 = true;
					try
					{
						_targetRoot.checkBox1.IsChecked = value2;
					}
					finally
					{
						_settingBinding6 = false;
					}
				}
#line (61, 13) - (61, 56) 61 "Page1.xml"
				_targetRoot.listView.SetVisible(value1);
#line default
			}

			private void Update0_TaskProp(global::System.Threading.Tasks.Task<global::System.String> value)
			{
#line default
				Set0(_generatedCodeDisposed.Token);
				async void Set0(global::System.Threading.CancellationToken cancellationToken)
				{
					try
					{
#line (64, 20) - (64, 71) 64 "Page1.xml"
						var task = value;
#line default
						if (task.IsCompleted != true)
						{
#line (64, 20) - (64, 71) 64 "Page1.xml"
							_targetRoot.textBlock11.Text = "Loading...";
#line default
						}
#line (64, 20) - (64, 71) 64 "Page1.xml"
						var result = await task;
#line default
						if (!cancellationToken.IsCancellationRequested)
						{
							_targetRoot.textBlock11.Text = result;
						}
					}
					catch
					{
					}
				}
#line default
			}

			private void Update0_FocusedField(global::UI.FocusState<global::WPFTest.ViewModels.Page1ViewModel.Field> value)
			{
				_settingBinding20 = true;
				try
				{
#line (75, 10) - (75, 107) 75 "Page1.xml"
					global::UI.FocusManager.SetFocused(_targetRoot.textBox4, value[WPFTest.ViewModels.Page1ViewModel.Field.Field1]);
				}
				finally
				{
					_settingBinding20 = false;
				}
#line default
			}

			private void Update1_IntInput(global::System.Int32? value)
			{
				var dataRoot = _dataRoot;
#line (55, 20) - (55, 97) 55 "Page1.xml"
				_targetRoot.textBlock8.Text = (((global::System.Int32)dataRoot.DecimalProp) + dataRoot.IntProp + value)?.ToString();
#line default
			}

			private void Update1_Input1(global::System.String value)
			{
				var dataRoot = _dataRoot;
#line (62, 20) - (62, 78) 62 "Page1.xml"
				_targetRoot.textBlock9.Text = dataRoot.ModifyViewModel is var v0 && v0 != null ? v0.Input1 : "abc";
#line (63, 20) - (63, 80) 63 "Page1.xml"
				_targetRoot.textBlock10.Text = value ?? "aaa";
				if (!object.Equals(_targetRoot.textBox2.Text, value))
				{
					_settingBinding15 = true;
					try
					{
						_targetRoot.textBox2.Text = value;
					}
					finally
					{
						_settingBinding15 = false;
					}
				}
#line default
			}

			private void Update1_CanChangeInput1(global::System.Boolean? value)
			{
				var dataRoot = _dataRoot;
#line (70, 70) - (70, 147) 70 "Page1.xml"
				_targetRoot.textBox2.IsReadOnly = dataRoot.ModifyViewModel is var v1 && v1 != null ? !v1.CanChangeInput1 : false;
#line default
			}

			private void Update1_BoolInput(global::System.Boolean? value)
			{
				if (!object.Equals(_targetRoot.checkBox2.IsChecked, value))
				{
					_settingBinding22 = true;
					try
					{
						_targetRoot.checkBox2.IsChecked = value;
					}
					finally
					{
						_settingBinding22 = false;
					}
				}
#line default
			}

			private void Update1_ModifyTaskProp(global::System.Threading.Tasks.Task<global::System.String> value)
			{
#line default
				Set0(_generatedCodeDisposed.Token);
				async void Set0(global::System.Threading.CancellationToken cancellationToken)
				{
					try
					{
#line (80, 15) - (80, 88) 80 "Page1.xml"
						var task = value;
#line default
						if (task?.IsCompleted != true)
						{
#line (80, 15) - (80, 88) 80 "Page1.xml"
							_targetRoot.textBlock13.Text = "Loading...";
#line default
							if (task == null)
							{
								return;
							}
						}
#line (80, 15) - (80, 88) 80 "Page1.xml"
						var result = await task;
#line default
						if (!cancellationToken.IsCancellationRequested)
						{
							_targetRoot.textBlock13.Text = result;
						}
					}
					catch
					{
					}
				}
#line default
			}

			private void Update2_TextInput(global::System.String value)
			{
				if (!object.Equals(_targetRoot.textBox3.Text, value))
				{
					_settingBinding18 = true;
					try
					{
						_targetRoot.textBox3.Text = value;
					}
					finally
					{
						_settingBinding18 = false;
					}
				}
#line default
			}

			private void Update2_BoolInput(global::System.Boolean? value)
			{
#line (74, 18) - (74, 91) 74 "Page1.xml"
				var value1 = value?.ToString();
#line default
				if (!object.Equals(_targetRoot.textBox4.Text, value1))
				{
					_settingBinding19 = true;
					try
					{
						_targetRoot.textBox4.Text = value1;
					}
					finally
					{
						_settingBinding19 = false;
					}
				}
#line default
			}

			private void OnTargetChanged0(global::System.Object p0, global::System.Windows.RoutedEventArgs p1)
			{
				var dataRoot = _dataRoot;
				if (!_settingBinding20)
				{
					try
					{
#line (75, 10) - (75, 107) 75 "Page1.xml"
						dataRoot.FocusedField[WPFTest.ViewModels.Page1ViewModel.Field.Field1] = global::UI.FocusManager.GetFocused(_targetRoot.textBox4);
#line default
					}
					catch
					{
					}
				}
			}

			private void OnTargetChanged1(object sender, global::System.EventArgs e)
			{
				var dataRoot = _dataRoot;
				if (!_settingBinding4)
				{
					try
					{
#line (54, 18) - (54, 56) 54 "Page1.xml"
						dataRoot.OrderInput = _targetRoot.textBox1.Text;
#line default
					}
					catch
					{
					}
				}
			}

			private void OnTargetChanged2(object sender, global::System.EventArgs e)
			{
				var dataRoot = _dataRoot;
				if (!_settingBinding6)
				{
					try
					{
#line (56, 19) - (56, 154) 56 "Page1.xml"
						dataRoot.BoolInput = (global::System.Boolean)_targetRoot.InverseBooleanConverter.ConvertBack(_targetRoot.checkBox1.IsChecked, typeof(global::System.Boolean), dataRoot.ArrayProp.Length > 0, null);
#line default
					}
					catch
					{
					}
				}
			}

			private void OnTargetChanged3(object sender, global::System.EventArgs e)
			{
				var dataRoot = _dataRoot;
				if (!_settingBinding15)
				{
					try
					{
#line (70, 18) - (70, 68) 70 "Page1.xml"
						var value = dataRoot.ModifyViewModel;
#line default
						if (value != null)
						{
#line (70, 18) - (70, 68) 70 "Page1.xml"
							value.Input1 = _targetRoot.textBox2.Text;
#line default
						}
					}
					catch
					{
					}
				}
			}

			private void OnTargetChanged4(object sender, global::System.EventArgs e)
			{
				var dataRoot = _dataRoot;
				if (!_settingBinding17)
				{
					try
					{
#line (72, 13) - (72, 75) 72 "Page1.xml"
						dataRoot.IntProp = global::WPFTest.TestExtensions.GetMyProperty(_targetRoot.textBox3);
#line default
					}
					catch
					{
					}
				}
			}

			private void OnTargetChanged5(object sender, global::System.EventArgs e)
			{
				var dataRoot = _dataRoot;
				if (!_settingBinding18)
				{
					try
					{
#line (73, 13) - (73, 86) 73 "Page1.xml"
						var value = dataRoot.ModifyViewModel?.ModifyTextViewModel;
#line default
						if (value != null)
						{
#line (73, 13) - (73, 86) 73 "Page1.xml"
							value.TextInput = _targetRoot.textBox3.Text;
#line default
						}
					}
					catch
					{
					}
				}
			}

			private void OnTargetChanged6(object sender, global::System.EventArgs e)
			{
				var dataRoot = _dataRoot;
				if (!_settingBinding19)
				{
					try
					{
#line (74, 18) - (74, 91) 74 "Page1.xml"
						var value = dataRoot.ModifyViewModel?.ModifyTextViewModel;
#line default
						if (value != null)
						{
#line (74, 18) - (74, 91) 74 "Page1.xml"
							value.BoolInput = _targetRoot.textBox4.Text is var t19 && string.IsNullOrEmpty(t19) ? null : (global::System.Boolean?)global::System.Convert.ChangeType(t19, typeof(global::System.Boolean), null);
#line default
						}
					}
					catch
					{
					}
				}
			}

			private void OnTargetChanged7(object sender, global::System.EventArgs e)
			{
				var dataRoot = _dataRoot;
				if (!_settingBinding22)
				{
					try
					{
#line (79, 13) - (79, 71) 79 "Page1.xml"
						var value = dataRoot.ModifyViewModel;
#line default
						if (value != null)
						{
#line (79, 13) - (79, 71) 79 "Page1.xml"
							value.BoolInput = _targetRoot.checkBox2.IsChecked ?? default;
#line default
						}
					}
					catch
					{
					}
				}
			}

			class Page1_BindingsTrackings_this
			{
				global::System.WeakReference _bindingsWeakRef;
				global::WPFTest.ViewModels.Page1ViewModel _propertyChangeSource0;
				global::WPFTest.ViewModels.Page1ModifyViewModel _propertyChangeSource1;
				global::WPFTest.ViewModels.Page1ModifyTextViewModel _propertyChangeSource2;

				public Page1_BindingsTrackings_this(Page1_Bindings_this bindings)
				{
					_bindingsWeakRef = new global::System.WeakReference(bindings);
				}

				public void Cleanup()
				{
					SetPropertyChangedEventHandler0(null);
					SetPropertyChangedEventHandler1(null);
					SetPropertyChangedEventHandler2(null);
				}

				public void SetPropertyChangedEventHandler0(global::WPFTest.ViewModels.Page1ViewModel value)
				{
					if (_propertyChangeSource0 != null && !object.ReferenceEquals(_propertyChangeSource0, value))
					{
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource0).PropertyChanged -= OnPropertyChanged0;
						_propertyChangeSource0 = null;
					}
					if (_propertyChangeSource0 == null && value != null)
					{
						_propertyChangeSource0 = value;
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource0).PropertyChanged += OnPropertyChanged0;
					}
				}

				public void SetPropertyChangedEventHandler1(global::WPFTest.ViewModels.Page1ModifyViewModel value)
				{
					if (_propertyChangeSource1 != null && !object.ReferenceEquals(_propertyChangeSource1, value))
					{
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource1).PropertyChanged -= OnPropertyChanged1;
						_propertyChangeSource1 = null;
					}
					if (_propertyChangeSource1 == null && value != null)
					{
						_propertyChangeSource1 = value;
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource1).PropertyChanged += OnPropertyChanged1;
					}
				}

				public void SetPropertyChangedEventHandler2(global::WPFTest.ViewModels.Page1ModifyTextViewModel value)
				{
					if (_propertyChangeSource2 != null && !object.ReferenceEquals(_propertyChangeSource2, value))
					{
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource2).PropertyChanged -= OnPropertyChanged2;
						_propertyChangeSource2 = null;
					}
					if (_propertyChangeSource2 == null && value != null)
					{
						_propertyChangeSource2 = value;
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource2).PropertyChanged += OnPropertyChanged2;
					}
				}

				private void OnPropertyChanged0(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::WPFTest.ViewModels.Page1ViewModel)sender;
					switch (e.PropertyName)
					{
						case null:
						case "":
							bindings.Update0(typedSender);
							break;
						case "BooleanProp":
							bindings.Update0_BooleanProp(typedSender.BooleanProp);
							break;
						case "DecimalProp":
							bindings.Update0_DecimalProp(typedSender.DecimalProp);
							break;
						case "OrderInput":
							bindings.Update0_OrderInput(typedSender.OrderInput);
							break;
						case "IntProp":
							bindings.Update0_IntProp(typedSender.IntProp);
							break;
						case "ModifyViewModel":
							bindings.Update0_ModifyViewModel(typedSender.ModifyViewModel);
							break;
						case "BoolInput":
							bindings.Update0_BoolInput(typedSender.BoolInput);
							break;
						case "ArrayProp":
							bindings.Update0_ArrayProp(typedSender.ArrayProp);
							break;
						case "TaskProp":
							bindings.Update0_TaskProp(typedSender.TaskProp);
							break;
						case "FocusedField":
							bindings.Update0_FocusedField(typedSender.FocusedField);
							break;
					}
				}

				private void OnPropertyChanged1(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::WPFTest.ViewModels.Page1ModifyViewModel)sender;
					switch (e.PropertyName)
					{
						case null:
						case "":
							bindings.Update1(typedSender);
							break;
						case "IntInput":
							bindings.Update1_IntInput(typedSender.IntInput);
							break;
						case "Input1":
							bindings.Update1_Input1(typedSender.Input1);
							break;
						case "CanChangeInput1":
							bindings.Update1_CanChangeInput1(typedSender.CanChangeInput1);
							break;
						case "BoolInput":
							bindings.Update1_BoolInput(typedSender.BoolInput);
							break;
						case "ModifyTaskProp":
							bindings.Update1_ModifyTaskProp(typedSender.ModifyTaskProp);
							break;
					}
				}

				private void OnPropertyChanged2(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::WPFTest.ViewModels.Page1ModifyTextViewModel)sender;
					switch (e.PropertyName)
					{
						case null:
						case "":
							bindings.Update2(typedSender);
							break;
						case "TextInput":
							bindings.Update2_TextInput(typedSender.TextInput);
							break;
						case "BoolInput":
							bindings.Update2_BoolInput(typedSender.BoolInput);
							break;
					}
				}

				Page1_Bindings_this TryGetBindings()
				{
					Page1_Bindings_this bindings = null;
					if (_bindingsWeakRef != null)
					{
						bindings = (Page1_Bindings_this)_bindingsWeakRef.Target;
						if (bindings == null)
						{
							_bindingsWeakRef = null;
							Cleanup();
						}
					}
					return bindings;
				}
			}
		}
	}

	class Page1_DataTemplate0 : global::CompiledBindings.IGeneratedDataTemplate
	{
		private global::System.Windows.Controls.TextBlock textBlock1;
		private global::System.Windows.Controls.TextBlock textBlock2;
		private global::System.Windows.Controls.TextBlock textBlock3;
		public global::System.Windows.Data.IValueConverter InverseBooleanConverter { get; set; }

		public void Initialize(global::System.Windows.FrameworkElement rootElement)
		{
			textBlock1 = (global::System.Windows.Controls.TextBlock)rootElement.FindName("textBlock1");
			textBlock2 = (global::System.Windows.Controls.TextBlock)rootElement.FindName("textBlock2");
			textBlock3 = (global::System.Windows.Controls.TextBlock)rootElement.FindName("textBlock3");


			rootElement.DataContextChanged += rootElement_DataContextChanged;
			if (rootElement.DataContext is global::WPFTest.ViewModels.EntityViewModel dataRoot0)
			{
				Bindings_rootElement.Initialize(this, dataRoot0);
			}
		}

		public void Cleanup(global::System.Windows.FrameworkElement rootElement)
		{
			rootElement.DataContextChanged -= rootElement_DataContextChanged;
			Bindings_rootElement.Cleanup();
		}

		private void rootElement_DataContextChanged(object sender, global::System.Windows.DependencyPropertyChangedEventArgs e)
		{
			Bindings_rootElement.Cleanup();
			if (((global::System.Windows.FrameworkElement)sender).DataContext is global::WPFTest.ViewModels.EntityViewModel dataRoot)
			{
				Bindings_rootElement.Initialize(this, dataRoot);
			}
		}

		Page1_DataTemplate0_Bindings_rootElement Bindings_rootElement = new Page1_DataTemplate0_Bindings_rootElement();

		class Page1_DataTemplate0_Bindings_rootElement
		{
			Page1_DataTemplate0 _targetRoot;
			global::WPFTest.ViewModels.EntityViewModel _dataRoot;
			Page1_DataTemplate0_BindingsTrackings_rootElement _bindingsTrackings;

			public void Initialize(Page1_DataTemplate0 targetRoot, global::WPFTest.ViewModels.EntityViewModel dataRoot)
			{
				_targetRoot = targetRoot;
				_dataRoot = dataRoot;
				_bindingsTrackings = new Page1_DataTemplate0_BindingsTrackings_rootElement(this);

				Update();

				_bindingsTrackings.SetPropertyChangedEventHandler0(dataRoot);
			}

			public void Cleanup()
			{
				if (_targetRoot != null)
				{
					_bindingsTrackings.Cleanup();
					_dataRoot = null;
					_targetRoot = null;
				}
			}

			public void Update()
			{
				var dataRoot = _dataRoot;
#line (25, 28) - (25, 58) 25 "Page1.xml"
				var value1 = dataRoot.Model;
#line default
				Update0_BooleanProp(dataRoot.BooleanProp);
				Update1_SByteProp(value1.SByteProp);
				_bindingsTrackings.SetPropertyChangedEventHandler1(value1);
			}

			private void Update0_BooleanProp(global::System.Boolean value)
			{
#line (27, 21) - (27, 47) 27 "Page1.xml"
				_targetRoot.textBlock2.Text = value.ToString();
#line (28, 21) - (28, 84) 28 "Page1.xml"
				_targetRoot.textBlock2.Visibility = WPFTest.Views.XamlUtils.TrueToVisible(value);
#line (29, 28) - (29, 111) 29 "Page1.xml"
				_targetRoot.textBlock3.IsEnabled = ((global::System.Boolean)_targetRoot.InverseBooleanConverter.Convert(value, typeof(global::System.Boolean), null, null));
#line default
			}

			private void Update1_SByteProp(global::System.SByte value)
			{
#line (25, 28) - (25, 58) 25 "Page1.xml"
				_targetRoot.textBlock1.Text = value.ToString();
#line default
			}

			class Page1_DataTemplate0_BindingsTrackings_rootElement
			{
				global::System.WeakReference _bindingsWeakRef;
				global::WPFTest.ViewModels.EntityViewModel _propertyChangeSource0;
				global::WPFTest.ViewModels.EntityModel _propertyChangeSource1;

				public Page1_DataTemplate0_BindingsTrackings_rootElement(Page1_DataTemplate0_Bindings_rootElement bindings)
				{
					_bindingsWeakRef = new global::System.WeakReference(bindings);
				}

				public void Cleanup()
				{
					SetPropertyChangedEventHandler0(null);
					SetPropertyChangedEventHandler1(null);
				}

				public void SetPropertyChangedEventHandler0(global::WPFTest.ViewModels.EntityViewModel value)
				{
					if (_propertyChangeSource0 != null && !object.ReferenceEquals(_propertyChangeSource0, value))
					{
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource0).PropertyChanged -= OnPropertyChanged0;
						_propertyChangeSource0 = null;
					}
					if (_propertyChangeSource0 == null && value != null)
					{
						_propertyChangeSource0 = value;
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource0).PropertyChanged += OnPropertyChanged0;
					}
				}

				public void SetPropertyChangedEventHandler1(global::WPFTest.ViewModels.EntityModel value)
				{
					if (_propertyChangeSource1 != null && !object.ReferenceEquals(_propertyChangeSource1, value))
					{
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource1).PropertyChanged -= OnPropertyChanged1;
						_propertyChangeSource1 = null;
					}
					if (_propertyChangeSource1 == null && value != null)
					{
						_propertyChangeSource1 = value;
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource1).PropertyChanged += OnPropertyChanged1;
					}
				}

				private void OnPropertyChanged0(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::WPFTest.ViewModels.EntityViewModel)sender;
					if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "BooleanProp")
					{
						bindings.Update0_BooleanProp(typedSender.BooleanProp);
					}
				}

				private void OnPropertyChanged1(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::WPFTest.ViewModels.EntityModel)sender;
					if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "SByteProp")
					{
						bindings.Update1_SByteProp(typedSender.SByteProp);
					}
				}

				Page1_DataTemplate0_Bindings_rootElement TryGetBindings()
				{
					Page1_DataTemplate0_Bindings_rootElement bindings = null;
					if (_bindingsWeakRef != null)
					{
						bindings = (Page1_DataTemplate0_Bindings_rootElement)_bindingsWeakRef.Target;
						if (bindings == null)
						{
							_bindingsWeakRef = null;
							Cleanup();
						}
					}
					return bindings;
				}
			}
		}
	}

	class Page1_DataTemplate1 : global::CompiledBindings.IGeneratedDataTemplate
	{
		private global::System.Windows.Controls.TextBlock textBlock4;

		public void Initialize(global::System.Windows.FrameworkElement rootElement)
		{
			textBlock4 = (global::System.Windows.Controls.TextBlock)rootElement.FindName("textBlock4");


			rootElement.DataContextChanged += rootElement_DataContextChanged;
			if (rootElement.DataContext is global::WPFTest.ViewModels.EntityViewModel dataRoot0)
			{
				Bindings_rootElement.Initialize(this, dataRoot0);
			}
		}

		public void Cleanup(global::System.Windows.FrameworkElement rootElement)
		{
			rootElement.DataContextChanged -= rootElement_DataContextChanged;
			Bindings_rootElement.Cleanup();
		}

		private void rootElement_DataContextChanged(object sender, global::System.Windows.DependencyPropertyChangedEventArgs e)
		{
			Bindings_rootElement.Cleanup();
			if (((global::System.Windows.FrameworkElement)sender).DataContext is global::WPFTest.ViewModels.EntityViewModel dataRoot)
			{
				Bindings_rootElement.Initialize(this, dataRoot);
			}
		}

		Page1_DataTemplate1_Bindings_rootElement Bindings_rootElement = new Page1_DataTemplate1_Bindings_rootElement();

		class Page1_DataTemplate1_Bindings_rootElement
		{
			Page1_DataTemplate1 _targetRoot;
			global::WPFTest.ViewModels.EntityViewModel _dataRoot;
			Page1_DataTemplate1_BindingsTrackings_rootElement _bindingsTrackings;

			public void Initialize(Page1_DataTemplate1 targetRoot, global::WPFTest.ViewModels.EntityViewModel dataRoot)
			{
				_targetRoot = targetRoot;
				_dataRoot = dataRoot;
				_bindingsTrackings = new Page1_DataTemplate1_BindingsTrackings_rootElement(this);

				Update();

				_bindingsTrackings.SetPropertyChangedEventHandler0(dataRoot);
			}

			public void Cleanup()
			{
				if (_targetRoot != null)
				{
					_bindingsTrackings.Cleanup();
					_dataRoot = null;
					_targetRoot = null;
				}
			}

			public void Update()
			{
				var dataRoot = _dataRoot;
				Update0_Title(dataRoot.Title);
			}

			private void Update0_Title(global::System.String value)
			{
#line (37, 24) - (37, 44) 37 "Page1.xml"
				_targetRoot.textBlock4.Text = value;
#line default
			}

			class Page1_DataTemplate1_BindingsTrackings_rootElement
			{
				global::System.WeakReference _bindingsWeakRef;
				global::WPFTest.ViewModels.EntityViewModel _propertyChangeSource0;

				public Page1_DataTemplate1_BindingsTrackings_rootElement(Page1_DataTemplate1_Bindings_rootElement bindings)
				{
					_bindingsWeakRef = new global::System.WeakReference(bindings);
				}

				public void Cleanup()
				{
					SetPropertyChangedEventHandler0(null);
				}

				public void SetPropertyChangedEventHandler0(global::WPFTest.ViewModels.EntityViewModel value)
				{
					if (_propertyChangeSource0 != null && !object.ReferenceEquals(_propertyChangeSource0, value))
					{
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource0).PropertyChanged -= OnPropertyChanged0;
						_propertyChangeSource0 = null;
					}
					if (_propertyChangeSource0 == null && value != null)
					{
						_propertyChangeSource0 = value;
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource0).PropertyChanged += OnPropertyChanged0;
					}
				}

				private void OnPropertyChanged0(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::WPFTest.ViewModels.EntityViewModel)sender;
					if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "Title")
					{
						bindings.Update0_Title(typedSender.Title);
					}
				}

				Page1_DataTemplate1_Bindings_rootElement TryGetBindings()
				{
					Page1_DataTemplate1_Bindings_rootElement bindings = null;
					if (_bindingsWeakRef != null)
					{
						bindings = (Page1_DataTemplate1_Bindings_rootElement)_bindingsWeakRef.Target;
						if (bindings == null)
						{
							_bindingsWeakRef = null;
							Cleanup();
						}
					}
					return bindings;
				}
			}
		}
	}
}
