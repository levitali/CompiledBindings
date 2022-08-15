namespace WPFTest.Views
{
	using System.Threading;
	using WPFTest.Views;

#nullable disable

	[System.CodeDom.Compiler.GeneratedCode("CompiledBindings", null)]
	partial class Page1
	{
		CancellationTokenSource _generatedCodeDisposed = new CancellationTokenSource();
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

#line (13, 5) - (13, 50) 13 "Page1.xml"
			var value1 = WPFTest.Strings.Instance;
#line (13, 5) - (13, 50) 13 "Page1.xml"
			Title = value1.Title;
#line default
			Set0(_generatedCodeDisposed.Token);
			async void Set0(CancellationToken cancellationToken)
			{
				try
				{
#line (39, 16) - (39, 48) 39 "Page1.xml"
					var value = await this.LoadImageAsync();
#line default
					if (!cancellationToken.IsCancellationRequested)
					{
						image1.Source = value;
					}
				}
				catch
				{
				}
			}
#line (45, 13) - (45, 57) 45 "Page1.xml"
			header1.Text = value1.Header1;
#line (70, 7) - (70, 49) 70 "Page1.xml"
			global::WPFTest.TestExtensions.SetMyProperty(checkBox2, 6);
#line default

			Bindings.Initialize(this);
			this.DataContextChanged += this_DataContextChanged;
			if (this.DataContext is global::WPFTest.ViewModels.Page1ViewModel dataRoot1)
			{
				Bindings_this.Initialize(this, dataRoot1);
			}
		}

		~Page1()
		{
			_generatedCodeDisposed.Cancel();
			if (Bindings != null)
			{
				Bindings.Cleanup();
			}
			if (Bindings_this != null)
			{
				Bindings_this.Cleanup();
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

		Page1_Bindings Bindings = new Page1_Bindings();

		class Page1_Bindings
		{
			Page1 _targetRoot;
			Page1_BindingsTrackings _bindingsTrackings;

			public void Initialize(Page1 dataRoot)
			{
				if (_targetRoot != null)
					throw new System.InvalidOperationException();
				if (dataRoot == null)
					throw new System.ArgumentNullException(nameof(dataRoot));

				_targetRoot = dataRoot;
				_bindingsTrackings = new Page1_BindingsTrackings(this);

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
				if (_targetRoot == null)
				{
					throw new System.InvalidOperationException();
				}

				var dataRoot = _targetRoot;
				Update0_DecimalProp(dataRoot._viewModel.DecimalProp);
				Update1_SelectedItem(dataRoot.listView.SelectedItem);

				_bindingsTrackings.SetPropertyChangedEventHandler0(dataRoot._viewModel);
				_bindingsTrackings.SetPropertyChangedEventHandler1(dataRoot.listView);
			}

			private void Update0_DecimalProp(global::System.Decimal value)
			{
#line (42, 13) - (42, 52) 42 "Page1.xml"
				_targetRoot.textBlock5.Text = value.ToString();
#line default
			}

			private void Update1_SelectedItem(global::System.Object value)
			{
#line (64, 13) - (64, 81) 64 "Page1.xml"
				_targetRoot.button1.IsEnabled = value != null;
#line default
			}

			class Page1_BindingsTrackings
			{
				global::System.WeakReference _bindingsWeakRef;
				global::WPFTest.ViewModels.Page1ViewModel _propertyChangeSource0;
				global::System.Windows.Controls.ListView _propertyChangeSource1;

				public Page1_BindingsTrackings(Page1_Bindings bindings)
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

				private void OnPropertyChanged0(object sender, System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::WPFTest.ViewModels.Page1ViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "DecimalProp")
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
					bindings.Update1_SelectedItem(typedSender.SelectedItem)
				}

				Page1_Bindings TryGetBindings()
				{
					Page1_Bindings bindings = null;
					if (_bindingsWeakRef != null)
					{
						bindings = (Page1_Bindings)_bindingsWeakRef.Target;
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
			global::System.Windows.RoutedEventHandler _eventHandler13;
			bool _settingBinding4;
			bool _settingBinding5;
			bool _settingBinding14;
			bool _settingBinding15;
			bool _settingBinding16;
			bool _settingBinding17;
			CancellationTokenSource _generatedCodeDisposed;

			public void Initialize(Page1 targetRoot, global::WPFTest.ViewModels.Page1ViewModel dataRoot)
			{
				if (_targetRoot != null)
					throw new System.InvalidOperationException();
				if (targetRoot == null)
					throw new System.ArgumentNullException(nameof(targetRoot));
				if (dataRoot == null)
					throw new System.ArgumentNullException(nameof(dataRoot));

				_targetRoot = targetRoot;
				_dataRoot = dataRoot;
				_generatedCodeDisposed = new CancellationTokenSource();
				_bindingsTrackings = new Page1_BindingsTrackings_this(this);

				Update();

#line (65, 13) - (65, 65) 65 "Page1.xml"
				_eventHandler13 = (p1, p2) => dataRoot.ModifyViewModel?.OnClick(dataRoot.BooleanProp);
#line default
				_targetRoot.button1.Click += _eventHandler13;
#line default

				_bindingsTrackings.SetPropertyChangedEventHandler0(dataRoot);

				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::System.Windows.Controls.TextBox.TextProperty,
						typeof(global::System.Windows.Controls.TextBox))
					.AddValueChanged(_targetRoot.textBox1, OnTargetChanged0);
				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty,
						typeof(global::System.Windows.Controls.Primitives.ToggleButton))
					.AddValueChanged(_targetRoot.checkBox1, OnTargetChanged1);
				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::System.Windows.Controls.TextBox.TextProperty,
						typeof(global::System.Windows.Controls.TextBox))
					.AddValueChanged(_targetRoot.textBox2, OnTargetChanged2);
				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::System.Windows.Controls.TextBox.TextProperty,
						typeof(global::System.Windows.Controls.TextBox))
					.AddValueChanged(_targetRoot.textBox3, OnTargetChanged3);
				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::WPFTest.TestExtensions.MyPropertyProperty,
						typeof(global::WPFTest.TestExtensions))
					.AddValueChanged(_targetRoot.textBox3, OnTargetChanged4);
				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty,
						typeof(global::System.Windows.Controls.Primitives.ToggleButton))
					.AddValueChanged(_targetRoot.checkBox2, OnTargetChanged5);
			}

			public void Cleanup()
			{
				if (_targetRoot != null)
				{
					_generatedCodeDisposed.Cancel();
					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::System.Windows.Controls.TextBox.TextProperty,
							typeof(global::System.Windows.Controls.TextBox))
						.RemoveValueChanged(_targetRoot.textBox1, OnTargetChanged0);
					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty,
							typeof(global::System.Windows.Controls.Primitives.ToggleButton))
						.RemoveValueChanged(_targetRoot.checkBox1, OnTargetChanged1);
					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::System.Windows.Controls.TextBox.TextProperty,
							typeof(global::System.Windows.Controls.TextBox))
						.RemoveValueChanged(_targetRoot.textBox2, OnTargetChanged2);
					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::System.Windows.Controls.TextBox.TextProperty,
							typeof(global::System.Windows.Controls.TextBox))
						.RemoveValueChanged(_targetRoot.textBox3, OnTargetChanged3);
					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::WPFTest.TestExtensions.MyPropertyProperty,
							typeof(global::WPFTest.TestExtensions))
						.RemoveValueChanged(_targetRoot.textBox3, OnTargetChanged4);
					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty,
							typeof(global::System.Windows.Controls.Primitives.ToggleButton))
						.RemoveValueChanged(_targetRoot.checkBox2, OnTargetChanged5);
					_targetRoot.button1.Click -= _eventHandler13;
					_eventHandler13 = null;
					_bindingsTrackings.Cleanup();
					_dataRoot = null;
					_targetRoot = null;
				}
			}

			public void Update()
			{
				if (_targetRoot == null)
				{
					throw new System.InvalidOperationException();
				}

				var dataRoot = _dataRoot;
#line (61, 20) - (61, 79) 61 "Page1.xml"
				_targetRoot.textBlock11.Text = dataRoot.Calculate(new WPFTest.ViewModels.ParamClass("text"));
#line default
				Set0(_generatedCodeDisposed.Token);
				async void Set0(CancellationToken cancellationToken)
				{
					try
					{
#line (62, 16) - (62, 49) 62 "Page1.xml"
						var value = await dataRoot.LoadImageAsync();
#line default
						if (!cancellationToken.IsCancellationRequested)
						{
							_targetRoot.image2.Source = value;
						}
					}
					catch
					{
					}
				}
#line default
				Update0_ModifyViewModel(dataRoot.ModifyViewModel);
				Update0_BooleanProp(dataRoot.BooleanProp);
				Update0_DecimalProp(dataRoot.DecimalProp);
				Update0_ArrayProp(dataRoot.ArrayProp);
				Update0_IntProp(dataRoot.IntProp);
				Update0_OrderInput(dataRoot.OrderInput);
				Update0_ListProp(dataRoot.ListProp);
				Update0_TaskProp(dataRoot.TaskProp);
			}

			private void Update0_BooleanProp(global::System.Boolean value)
			{
#line (46, 13) - (46, 67) 46 "Page1.xml"
				_targetRoot.header1.Visibility = (value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed);
#line (49, 13) - (49, 101) 49 "Page1.xml"
				_targetRoot.textBlock6.Visibility = ((global::System.Windows.Visibility)_targetRoot.TrueToVisibleConverter.Convert(value, typeof(global::System.Windows.Visibility), null, null));
#line default
			}

			private void Update0_DecimalProp(global::System.Decimal value)
			{
#line (48, 13) - (48, 39) 48 "Page1.xml"
				_targetRoot.textBlock6.Text = value.ToString();
#line (50, 20) - (50, 50) 50 "Page1.xml"
				_targetRoot.textBlock7.Text = (value + 1).ToString();
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

			private void Update0_BoolInput(global::System.Boolean value)
			{
				var dataRoot = _dataRoot;
#line (52, 19) - (52, 154) 52 "Page1.xml"
				var value1 = ((global::System.Nullable<global::System.Boolean>)_targetRoot.InverseBooleanConverter.Convert(value, typeof(global::System.Nullable<global::System.Boolean>), dataRoot.ArrayProp.Length > 0, null));
#line default
				if (!object.Equals(_targetRoot.checkBox1.IsChecked, value1))
				{
					_settingBinding5 = true;
					try
					{
						_targetRoot.checkBox1.IsChecked = value1;
					}
					finally
					{
						_settingBinding5 = false;
					}
				}
#line default
			}

			private void Update0_ArrayProp(global::System.Int32[] value)
			{
				var dataRoot = _dataRoot;
#line (52, 19) - (52, 154) 52 "Page1.xml"
				var value1 = value.Length > 0;
#line (52, 19) - (52, 154) 52 "Page1.xml"
				var value2 = ((global::System.Nullable<global::System.Boolean>)_targetRoot.InverseBooleanConverter.Convert(dataRoot.BoolInput, typeof(global::System.Nullable<global::System.Boolean>), value1, null));
#line default
				if (!object.Equals(_targetRoot.checkBox1.IsChecked, value2))
				{
					_settingBinding5 = true;
					try
					{
						_targetRoot.checkBox1.IsChecked = value2;
					}
					finally
					{
						_settingBinding5 = false;
					}
				}
#line (57, 13) - (57, 56) 57 "Page1.xml"
				_targetRoot.listView.SetVisible(value1);
#line default
			}

			private void Update0_ListProp(global::System.Collections.Generic.IList<global::WPFTest.ViewModels.EntityViewModel> value)
			{
#line (56, 13) - (56, 43) 56 "Page1.xml"
				_targetRoot.listView.ItemsSource = value;
#line default
			}

			private void Update0_ModifyViewModel(global::WPFTest.ViewModels.Page1ModifyViewModel value)
			{
#line (58, 20) - (58, 78) 58 "Page1.xml"
				_targetRoot.textBlock8.Text = (value != null ? value.Input1 : "abc");
#line default
				Update1_Input1(value?.Input1);
				Update1_ModifyTextViewModel(value?.ModifyTextViewModel);
				Update1_BoolInput(value?.BoolInput ?? default);
				_bindingsTrackings.SetPropertyChangedEventHandler1(value);
			}

			private void Update0_TaskProp(global::System.Threading.Tasks.Task<global::System.String> value)
			{
#line default
				Set0(_generatedCodeDisposed.Token);
				async void Set0(CancellationToken cancellationToken)
				{
					try
					{
#line (60, 20) - (60, 71) 60 "Page1.xml"
						var task = value;
#line default
						if (!task.IsCompleted)
						{
#line (60, 20) - (60, 71) 60 "Page1.xml"
							_targetRoot.textBlock10.Text = "Loading...";
#line default
						}
#line (60, 20) - (60, 71) 60 "Page1.xml"
						var value = await task;
#line default
						if (!cancellationToken.IsCancellationRequested)
						{
							_targetRoot.textBlock10.Text = value;
						}
					}
					catch
					{
					}
				}
#line default
			}

			private void Update0_IntProp(global::System.Int32 value)
			{
#line (68, 6) - (68, 68) 68 "Page1.xml"
				global::WPFTest.TestExtensions.SetMyProperty(_targetRoot.textBox3, value);
#line (71, 7) - (71, 36) 71 "Page1.xml"
				global::System.Windows.Controls.Grid.SetColumn(_targetRoot.checkBox2, value);
#line default
			}

			private void Update1_Input1(global::System.String value)
			{
#line (59, 20) - (59, 80) 59 "Page1.xml"
				_targetRoot.textBlock9.Text = value ?? "aaa";
				if (!object.Equals(_targetRoot.textBox2.Text, value))
				{
					_settingBinding14 = true;
					try
					{
						_targetRoot.textBox2.Text = value;
					}
					finally
					{
						_settingBinding14 = false;
					}
				}
#line default
			}

			private void Update1_ModifyTextViewModel(global::WPFTest.ViewModels.Page1ModifyTextViewModel value)
			{
				Update2_TextInput(value?.TextInput);
				_bindingsTrackings.SetPropertyChangedEventHandler2(value);
			}

			private void Update1_BoolInput(global::System.Boolean value)
			{
				if (!object.Equals(_targetRoot.checkBox2.IsChecked, value))
				{
					_settingBinding17 = true;
					try
					{
						_targetRoot.checkBox2.IsChecked = value;
					}
					finally
					{
						_settingBinding17 = false;
					}
				}
#line default
			}

			private void Update2_TextInput(global::System.String value)
			{
				if (!object.Equals(_targetRoot.textBox3.Text, value))
				{
					_settingBinding15 = true;
					try
					{
						_targetRoot.textBox3.Text = value;
					}
					finally
					{
						_settingBinding15 = false;
					}
				}
#line default
			}

			private void OnTargetChanged0(object sender, global::System.EventArgs e)
			{
				var dataRoot = _dataRoot;
				if (!_settingBinding4)
				{
					try
					{
#line (51, 18) - (51, 56) 51 "Page1.xml"
						dataRoot.OrderInput = _targetRoot.textBox1.Text;
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
				if (!_settingBinding5)
				{
					try
					{
#line (52, 19) - (52, 154) 52 "Page1.xml"
						dataRoot.BoolInput = (global::System.Boolean)_targetRoot.InverseBooleanConverter.ConvertBack(_targetRoot.checkBox1.IsChecked, typeof(global::System.Boolean), dataRoot.ArrayProp.Length > 0, null);
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
				if (!_settingBinding14)
				{
					try
					{
#line (66, 18) - (66, 68) 66 "Page1.xml"
						var value = dataRoot.ModifyViewModel;
#line default
						if (value != null)
						{
#line (66, 18) - (66, 68) 66 "Page1.xml"
							value.Input1 = _targetRoot.textBox2.Text;
#line default
						}
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
#line (67, 18) - (67, 91) 67 "Page1.xml"
						var value = dataRoot.ModifyViewModel?.ModifyTextViewModel;
#line default
						if (value != null)
						{
#line (67, 18) - (67, 91) 67 "Page1.xml"
							value.TextInput = _targetRoot.textBox3.Text;
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
				if (!_settingBinding16)
				{
					try
					{
#line (68, 6) - (68, 68) 68 "Page1.xml"
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
				if (!_settingBinding17)
				{
					try
					{
#line (69, 19) - (69, 77) 69 "Page1.xml"
						var value = dataRoot.ModifyViewModel;
#line default
						if (value != null)
						{
#line (69, 19) - (69, 77) 69 "Page1.xml"
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

				private void OnPropertyChanged0(object sender, System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::WPFTest.ViewModels.Page1ViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "BooleanProp")
					{
						bindings.Update0_BooleanProp(typedSender.BooleanProp);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "DecimalProp")
					{
						bindings.Update0_DecimalProp(typedSender.DecimalProp);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "OrderInput")
					{
						bindings.Update0_OrderInput(typedSender.OrderInput);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "BoolInput")
					{
						bindings.Update0_BoolInput(typedSender.BoolInput);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "ArrayProp")
					{
						bindings.Update0_ArrayProp(typedSender.ArrayProp);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "ListProp")
					{
						bindings.Update0_ListProp(typedSender.ListProp);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "ModifyViewModel")
					{
						bindings.Update0_ModifyViewModel(typedSender.ModifyViewModel);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "TaskProp")
					{
						bindings.Update0_TaskProp(typedSender.TaskProp);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "IntProp")
					{
						bindings.Update0_IntProp(typedSender.IntProp);
					}
				}

				private void OnPropertyChanged1(object sender, System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::WPFTest.ViewModels.Page1ModifyViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "Input1")
					{
						bindings.Update1_Input1(typedSender.Input1);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "ModifyTextViewModel")
					{
						bindings.Update1_ModifyTextViewModel(typedSender.ModifyTextViewModel);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "BoolInput")
					{
						bindings.Update1_BoolInput(typedSender.BoolInput);
					}
				}

				private void OnPropertyChanged2(object sender, System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::WPFTest.ViewModels.Page1ModifyTextViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "TextInput")
					{
						bindings.Update2_TextInput(typedSender.TextInput);
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
				if (_targetRoot != null)
					throw new System.InvalidOperationException();
				if (targetRoot == null)
					throw new System.ArgumentNullException(nameof(targetRoot));
				if (dataRoot == null)
					throw new System.ArgumentNullException(nameof(dataRoot));

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
				if (_targetRoot == null)
				{
					throw new System.InvalidOperationException();
				}

				var dataRoot = _dataRoot;
				Update0_BooleanProp(dataRoot.BooleanProp);
				Update0_Model(dataRoot.Model);
			}

			private void Update0_Model(global::WPFTest.ViewModels.EntityModel value)
			{
				Update1_SByteProp(value.SByteProp);
				_bindingsTrackings.SetPropertyChangedEventHandler1(value);
			}

			private void Update0_BooleanProp(global::System.Boolean value)
			{
#line (25, 21) - (25, 47) 25 "Page1.xml"
				_targetRoot.textBlock2.Text = value.ToString();
#line (26, 21) - (26, 84) 26 "Page1.xml"
				_targetRoot.textBlock2.Visibility = WPFTest.Views.XamlUtils.TrueToVisible(value);
#line (27, 28) - (27, 111) 27 "Page1.xml"
				_targetRoot.textBlock3.IsEnabled = ((global::System.Boolean)_targetRoot.InverseBooleanConverter.Convert(value, typeof(global::System.Boolean), null, null));
#line default
			}

			private void Update1_SByteProp(global::System.SByte value)
			{
#line (23, 28) - (23, 58) 23 "Page1.xml"
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

				private void OnPropertyChanged0(object sender, System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::WPFTest.ViewModels.EntityViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "Model")
					{
						bindings.Update0_Model(typedSender.Model);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "BooleanProp")
					{
						bindings.Update0_BooleanProp(typedSender.BooleanProp);
					}
				}

				private void OnPropertyChanged1(object sender, System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::WPFTest.ViewModels.EntityModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "SByteProp")
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
				if (_targetRoot != null)
					throw new System.InvalidOperationException();
				if (targetRoot == null)
					throw new System.ArgumentNullException(nameof(targetRoot));
				if (dataRoot == null)
					throw new System.ArgumentNullException(nameof(dataRoot));

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
				if (_targetRoot == null)
				{
					throw new System.InvalidOperationException();
				}

				var dataRoot = _dataRoot;
				Update0_Title(dataRoot.Title);
			}

			private void Update0_Title(global::System.String value)
			{
#line (34, 24) - (34, 44) 34 "Page1.xml"
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

				private void OnPropertyChanged0(object sender, System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::WPFTest.ViewModels.EntityViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "Title")
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
