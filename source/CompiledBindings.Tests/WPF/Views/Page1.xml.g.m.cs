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

				var targetRoot = _targetRoot;
				var dataRoot = _targetRoot;
#line (42, 13) - (42, 52) 42 "Page1.xml"
				targetRoot.textBlock5.Text = dataRoot._viewModel.DecimalProp.ToString();
#line (64, 13) - (64, 81) 64 "Page1.xml"
				targetRoot.button1.IsEnabled = dataRoot.listView.SelectedItem != null;
#line default

				_bindingsTrackings.SetPropertyChangedEventHandler0(dataRoot._viewModel);
				_bindingsTrackings.SetPropertyChangedEventHandler1(dataRoot.listView);
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

					var targetRoot = bindings._targetRoot;
					var dataRoot = bindings._targetRoot;
					var typedSender = (global::WPFTest.ViewModels.Page1ViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "DecimalProp")
					{
#line (42, 13) - (42, 52) 42 "Page1.xml"
						targetRoot.textBlock5.Text = typedSender.DecimalProp.ToString();
#line default
					}
				}

				private void OnPropertyChanged1_SelectedItem(object sender, global::System.EventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var targetRoot = bindings._targetRoot;
					var dataRoot = bindings._targetRoot;
					var typedSender = (global::System.Windows.Controls.ListView)sender;
#line (64, 13) - (64, 81) 64 "Page1.xml"
					targetRoot.button1.IsEnabled = typedSender.SelectedItem != null;
#line default
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

				var targetRoot = _targetRoot;
				var dataRoot = _dataRoot;
				var bindings = this;
#line (58, 20) - (58, 78) 58 "Page1.xml"
				var value1 = dataRoot.ModifyViewModel;
#line (52, 19) - (52, 154) 52 "Page1.xml"
				var value2 = dataRoot.ArrayProp.Length > 0;
#line (46, 13) - (46, 67) 46 "Page1.xml"
				var value3 = dataRoot.BooleanProp;
#line (48, 13) - (48, 39) 48 "Page1.xml"
				var value4 = dataRoot.DecimalProp;
#line (68, 6) - (68, 68) 68 "Page1.xml"
				var value5 = dataRoot.IntProp;
#line (59, 20) - (59, 80) 59 "Page1.xml"
				var value6 = value1?.Input1;
#line (67, 18) - (67, 91) 67 "Page1.xml"
				var value7 = value1?.ModifyTextViewModel;
#line (46, 13) - (46, 67) 46 "Page1.xml"
				targetRoot.header1.Visibility = (value3 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed);
#line (48, 13) - (48, 39) 48 "Page1.xml"
				targetRoot.textBlock6.Text = value4.ToString();
#line (49, 13) - (49, 101) 49 "Page1.xml"
				targetRoot.textBlock6.Visibility = ((global::System.Windows.Visibility)targetRoot.TrueToVisibleConverter.Convert(value3, typeof(global::System.Windows.Visibility), null, null));
#line (50, 20) - (50, 50) 50 "Page1.xml"
				targetRoot.textBlock7.Text = (value4 + 1).ToString();
#line (51, 18) - (51, 56) 51 "Page1.xml"
				var value8 = dataRoot.OrderInput;
#line default
				if (!object.Equals(targetRoot.textBox1.Text, value8))
				{
					_settingBinding4 = true;
					try
					{
						targetRoot.textBox1.Text = value8;
					}
					finally
					{
						_settingBinding4 = false;
					}
				}
#line (52, 19) - (52, 154) 52 "Page1.xml"
				var value9 = ((global::System.Nullable<global::System.Boolean>)targetRoot.InverseBooleanConverter.Convert(dataRoot.BoolInput, typeof(global::System.Nullable<global::System.Boolean>), value2, null));
#line default
				if (!object.Equals(targetRoot.checkBox1.IsChecked, value9))
				{
					_settingBinding5 = true;
					try
					{
						targetRoot.checkBox1.IsChecked = value9;
					}
					finally
					{
						_settingBinding5 = false;
					}
				}
#line (56, 13) - (56, 43) 56 "Page1.xml"
				targetRoot.listView.ItemsSource = dataRoot.ListProp;
#line (57, 13) - (57, 56) 57 "Page1.xml"
				targetRoot.listView.SetVisible(value2);
#line (58, 20) - (58, 78) 58 "Page1.xml"
				targetRoot.textBlock8.Text = (value1 != null ? value1.Input1 : "abc");
#line (59, 20) - (59, 80) 59 "Page1.xml"
				targetRoot.textBlock9.Text = value6 ?? "aaa";
#line default
				Set0(bindings._generatedCodeDisposed.Token);
				async void Set0(CancellationToken cancellationToken)
				{
					try
					{
#line (60, 20) - (60, 71) 60 "Page1.xml"
						var task = dataRoot.TaskProp;
#line default
						if (!task.IsCompleted)
						{
#line (60, 20) - (60, 71) 60 "Page1.xml"
							targetRoot.textBlock10.Text = "Loading...";
#line default
						}
#line (60, 20) - (60, 71) 60 "Page1.xml"
						var value = await task;
#line default
						if (!cancellationToken.IsCancellationRequested)
						{
							targetRoot.textBlock10.Text = value;
						}
					}
					catch
					{
					}
				}
#line (61, 20) - (61, 79) 61 "Page1.xml"
				targetRoot.textBlock11.Text = dataRoot.Calculate(new WPFTest.ViewModels.ParamClass("text"));
#line default
				Set1(bindings._generatedCodeDisposed.Token);
				async void Set1(CancellationToken cancellationToken)
				{
					try
					{
#line (62, 16) - (62, 49) 62 "Page1.xml"
						var value = await dataRoot.LoadImageAsync();
#line default
						if (!cancellationToken.IsCancellationRequested)
						{
							targetRoot.image2.Source = value;
						}
					}
					catch
					{
					}
				}
				if (!object.Equals(targetRoot.textBox2.Text, value6))
				{
					_settingBinding14 = true;
					try
					{
						targetRoot.textBox2.Text = value6;
					}
					finally
					{
						_settingBinding14 = false;
					}
				}
#line (67, 18) - (67, 91) 67 "Page1.xml"
				var value10 = value7?.TextInput;
#line default
				if (!object.Equals(targetRoot.textBox3.Text, value10))
				{
					_settingBinding15 = true;
					try
					{
						targetRoot.textBox3.Text = value10;
					}
					finally
					{
						_settingBinding15 = false;
					}
				}
#line (68, 6) - (68, 68) 68 "Page1.xml"
				global::WPFTest.TestExtensions.SetMyProperty(targetRoot.textBox3, value5);
#line (69, 19) - (69, 77) 69 "Page1.xml"
				var value11 = value1?.BoolInput;
#line default
				if (!object.Equals(targetRoot.checkBox2.IsChecked, value11))
				{
					_settingBinding17 = true;
					try
					{
						targetRoot.checkBox2.IsChecked = value11;
					}
					finally
					{
						_settingBinding17 = false;
					}
				}
#line (71, 7) - (71, 36) 71 "Page1.xml"
				global::System.Windows.Controls.Grid.SetColumn(targetRoot.checkBox2, value5);
#line default

				_bindingsTrackings.SetPropertyChangedEventHandler1(value1);
				_bindingsTrackings.SetPropertyChangedEventHandler2(value7);
			}

			private void OnTargetChanged0(object sender, global::System.EventArgs e)
			{
				var dataRoot = _dataRoot;
				var targetRoot = _targetRoot;
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
				var targetRoot = _targetRoot;
				if (!_settingBinding5)
				{
					try
					{
#line (52, 19) - (52, 154) 52 "Page1.xml"
						dataRoot.BoolInput = (global::System.Boolean)targetRoot.InverseBooleanConverter.ConvertBack(_targetRoot.checkBox1.IsChecked, typeof(global::System.Boolean), dataRoot.ArrayProp.Length > 0, null);
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
				var targetRoot = _targetRoot;
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
				var targetRoot = _targetRoot;
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
				var targetRoot = _targetRoot;
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
				var targetRoot = _targetRoot;
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

					var targetRoot = bindings._targetRoot;
					var dataRoot = bindings._dataRoot;
					var typedSender = (global::WPFTest.ViewModels.Page1ViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "BooleanProp")
					{
#line (46, 13) - (46, 67) 46 "Page1.xml"
						var value1 = typedSender.BooleanProp;
#line (46, 13) - (46, 67) 46 "Page1.xml"
						targetRoot.header1.Visibility = (value1 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed);
#line (49, 13) - (49, 101) 49 "Page1.xml"
						targetRoot.textBlock6.Visibility = ((global::System.Windows.Visibility)targetRoot.TrueToVisibleConverter.Convert(value1, typeof(global::System.Windows.Visibility), null, null));
#line default
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "DecimalProp")
					{
#line (48, 13) - (48, 39) 48 "Page1.xml"
						var value1 = typedSender.DecimalProp;
#line (48, 13) - (48, 39) 48 "Page1.xml"
						targetRoot.textBlock6.Text = value1.ToString();
#line (50, 20) - (50, 50) 50 "Page1.xml"
						targetRoot.textBlock7.Text = (value1 + 1).ToString();
#line default
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "OrderInput")
					{
#line (51, 18) - (51, 56) 51 "Page1.xml"
						var value1 = typedSender.OrderInput;
#line default
						if (!object.Equals(targetRoot.textBox1.Text, value1))
						{
							bindings._settingBinding4 = true;
							try
							{
								targetRoot.textBox1.Text = value1;
							}
							finally
							{
								bindings._settingBinding4 = false;
							}
						}
#line default
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "BoolInput")
					{
#line (52, 19) - (52, 154) 52 "Page1.xml"
						var value1 = ((global::System.Nullable<global::System.Boolean>)targetRoot.InverseBooleanConverter.Convert(typedSender.BoolInput, typeof(global::System.Nullable<global::System.Boolean>), dataRoot.ArrayProp.Length > 0, null));
#line default
						if (!object.Equals(targetRoot.checkBox1.IsChecked, value1))
						{
							bindings._settingBinding5 = true;
							try
							{
								targetRoot.checkBox1.IsChecked = value1;
							}
							finally
							{
								bindings._settingBinding5 = false;
							}
						}
#line default
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "ArrayProp")
					{
#line (52, 19) - (52, 154) 52 "Page1.xml"
						var value1 = typedSender.ArrayProp.Length > 0;
#line (52, 19) - (52, 154) 52 "Page1.xml"
						var value2 = ((global::System.Nullable<global::System.Boolean>)targetRoot.InverseBooleanConverter.Convert(dataRoot.BoolInput, typeof(global::System.Nullable<global::System.Boolean>), value1, null));
#line default
						if (!object.Equals(targetRoot.checkBox1.IsChecked, value2))
						{
							bindings._settingBinding5 = true;
							try
							{
								targetRoot.checkBox1.IsChecked = value2;
							}
							finally
							{
								bindings._settingBinding5 = false;
							}
						}
#line (57, 13) - (57, 56) 57 "Page1.xml"
						targetRoot.listView.SetVisible(value1);
#line default
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "ListProp")
					{
#line (56, 13) - (56, 43) 56 "Page1.xml"
						targetRoot.listView.ItemsSource = typedSender.ListProp;
#line default
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "ModifyViewModel")
					{
#line (58, 20) - (58, 78) 58 "Page1.xml"
						var value1 = typedSender.ModifyViewModel;
#line (59, 20) - (59, 80) 59 "Page1.xml"
						var value2 = value1?.Input1;
#line (67, 18) - (67, 91) 67 "Page1.xml"
						var value3 = value1?.ModifyTextViewModel;
#line (58, 20) - (58, 78) 58 "Page1.xml"
						targetRoot.textBlock8.Text = (value1 != null ? value1.Input1 : "abc");
#line (59, 20) - (59, 80) 59 "Page1.xml"
						targetRoot.textBlock9.Text = value2 ?? "aaa";
						if (!object.Equals(targetRoot.textBox2.Text, value2))
						{
							bindings._settingBinding14 = true;
							try
							{
								targetRoot.textBox2.Text = value2;
							}
							finally
							{
								bindings._settingBinding14 = false;
							}
						}
#line (67, 18) - (67, 91) 67 "Page1.xml"
						var value4 = value3?.TextInput;
#line default
						if (!object.Equals(targetRoot.textBox3.Text, value4))
						{
							bindings._settingBinding15 = true;
							try
							{
								targetRoot.textBox3.Text = value4;
							}
							finally
							{
								bindings._settingBinding15 = false;
							}
						}
#line (69, 19) - (69, 77) 69 "Page1.xml"
						var value5 = value1?.BoolInput;
#line default
						if (!object.Equals(targetRoot.checkBox2.IsChecked, value5))
						{
							bindings._settingBinding17 = true;
							try
							{
								targetRoot.checkBox2.IsChecked = value5;
							}
							finally
							{
								bindings._settingBinding17 = false;
							}
						}
#line default
						SetPropertyChangedEventHandler1(value1);
						SetPropertyChangedEventHandler2(value3);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "TaskProp")
					{
#line default
						Set0(bindings._generatedCodeDisposed.Token);
						async void Set0(CancellationToken cancellationToken)
						{
							try
							{
#line (60, 20) - (60, 71) 60 "Page1.xml"
								var task = typedSender.TaskProp;
#line default
								if (!task.IsCompleted)
								{
#line (60, 20) - (60, 71) 60 "Page1.xml"
									targetRoot.textBlock10.Text = "Loading...";
#line default
								}
#line (60, 20) - (60, 71) 60 "Page1.xml"
								var value = await task;
#line default
								if (!cancellationToken.IsCancellationRequested)
								{
									targetRoot.textBlock10.Text = value;
								}
							}
							catch
							{
							}
						}
#line default
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "IntProp")
					{
#line (68, 6) - (68, 68) 68 "Page1.xml"
						var value1 = typedSender.IntProp;
#line (68, 6) - (68, 68) 68 "Page1.xml"
						global::WPFTest.TestExtensions.SetMyProperty(targetRoot.textBox3, value1);
#line (71, 7) - (71, 36) 71 "Page1.xml"
						global::System.Windows.Controls.Grid.SetColumn(targetRoot.checkBox2, value1);
#line default
					}
				}

				private void OnPropertyChanged1(object sender, System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var targetRoot = bindings._targetRoot;
					var dataRoot = bindings._dataRoot;
					var typedSender = (global::WPFTest.ViewModels.Page1ModifyViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "Input1")
					{
#line (59, 20) - (59, 80) 59 "Page1.xml"
						var value1 = typedSender.Input1;
#line (59, 20) - (59, 80) 59 "Page1.xml"
						targetRoot.textBlock9.Text = value1 ?? "aaa";
						if (!object.Equals(targetRoot.textBox2.Text, value1))
						{
							bindings._settingBinding14 = true;
							try
							{
								targetRoot.textBox2.Text = value1;
							}
							finally
							{
								bindings._settingBinding14 = false;
							}
						}
#line default
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "ModifyTextViewModel")
					{
#line (67, 18) - (67, 91) 67 "Page1.xml"
						var value1 = typedSender.ModifyTextViewModel;
#line (67, 18) - (67, 91) 67 "Page1.xml"
						var value2 = value1.TextInput;
#line default
						if (!object.Equals(targetRoot.textBox3.Text, value2))
						{
							bindings._settingBinding15 = true;
							try
							{
								targetRoot.textBox3.Text = value2;
							}
							finally
							{
								bindings._settingBinding15 = false;
							}
						}
#line default
						SetPropertyChangedEventHandler2(value1);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "BoolInput")
					{
#line (69, 19) - (69, 77) 69 "Page1.xml"
						var value1 = typedSender.BoolInput;
#line default
						if (!object.Equals(targetRoot.checkBox2.IsChecked, value1))
						{
							bindings._settingBinding17 = true;
							try
							{
								targetRoot.checkBox2.IsChecked = value1;
							}
							finally
							{
								bindings._settingBinding17 = false;
							}
						}
#line default
					}
				}

				private void OnPropertyChanged2(object sender, System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var targetRoot = bindings._targetRoot;
					var dataRoot = bindings._dataRoot;
					var typedSender = (global::WPFTest.ViewModels.Page1ModifyTextViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "TextInput")
					{
#line (67, 18) - (67, 91) 67 "Page1.xml"
						var value1 = typedSender.TextInput;
#line default
						if (!object.Equals(targetRoot.textBox3.Text, value1))
						{
							bindings._settingBinding15 = true;
							try
							{
								targetRoot.textBox3.Text = value1;
							}
							finally
							{
								bindings._settingBinding15 = false;
							}
						}
#line default
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
		global::System.Windows.Data.IValueConverter InverseBooleanConverter;

		public void Initialize(global::System.Windows.FrameworkElement rootElement)
		{
			textBlock1 = (global::System.Windows.Controls.TextBlock)rootElement.FindName("textBlock1");
			textBlock2 = (global::System.Windows.Controls.TextBlock)rootElement.FindName("textBlock2");
			textBlock3 = (global::System.Windows.Controls.TextBlock)rootElement.FindName("textBlock3");

			var root = global::CompiledBindings.DataTemplateBindings.GetRoot(rootElement);
			InverseBooleanConverter = (global::System.Windows.Data.IValueConverter)(root?.Resources["InverseBooleanConverter"] ?? global::System.Windows.Application.Current.Resources["InverseBooleanConverter"] ?? throw new global::System.Exception("Resource 'InverseBooleanConverter' not found."));

#line default

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

				var targetRoot = _targetRoot;
				var dataRoot = _dataRoot;
#line (25, 21) - (25, 47) 25 "Page1.xml"
				var value1 = dataRoot.BooleanProp;
#line (23, 28) - (23, 58) 23 "Page1.xml"
				var value2 = dataRoot.Model;
#line (23, 28) - (23, 58) 23 "Page1.xml"
				targetRoot.textBlock1.Text = value2.SByteProp.ToString();
#line (25, 21) - (25, 47) 25 "Page1.xml"
				targetRoot.textBlock2.Text = value1.ToString();
#line (26, 21) - (26, 84) 26 "Page1.xml"
				targetRoot.textBlock2.Visibility = WPFTest.Views.XamlUtils.TrueToVisible(value1);
#line (27, 28) - (27, 111) 27 "Page1.xml"
				targetRoot.textBlock3.IsEnabled = ((global::System.Boolean)targetRoot.InverseBooleanConverter.Convert(value1, typeof(global::System.Boolean), null, null));
#line default

				_bindingsTrackings.SetPropertyChangedEventHandler1(value2);
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

					var targetRoot = bindings._targetRoot;
					var dataRoot = bindings._dataRoot;
					var typedSender = (global::WPFTest.ViewModels.EntityViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "Model")
					{
#line (23, 28) - (23, 58) 23 "Page1.xml"
						var value1 = typedSender.Model;
#line (23, 28) - (23, 58) 23 "Page1.xml"
						targetRoot.textBlock1.Text = value1.SByteProp.ToString();
#line default
						SetPropertyChangedEventHandler1(value1);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "BooleanProp")
					{
#line (25, 21) - (25, 47) 25 "Page1.xml"
						var value1 = typedSender.BooleanProp;
#line (25, 21) - (25, 47) 25 "Page1.xml"
						targetRoot.textBlock2.Text = value1.ToString();
#line (26, 21) - (26, 84) 26 "Page1.xml"
						targetRoot.textBlock2.Visibility = WPFTest.Views.XamlUtils.TrueToVisible(value1);
#line (27, 28) - (27, 111) 27 "Page1.xml"
						targetRoot.textBlock3.IsEnabled = ((global::System.Boolean)targetRoot.InverseBooleanConverter.Convert(value1, typeof(global::System.Boolean), null, null));
#line default
					}
				}

				private void OnPropertyChanged1(object sender, System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var targetRoot = bindings._targetRoot;
					var dataRoot = bindings._dataRoot;
					var typedSender = (global::WPFTest.ViewModels.EntityModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "SByteProp")
					{
#line (23, 28) - (23, 58) 23 "Page1.xml"
						targetRoot.textBlock1.Text = typedSender.SByteProp.ToString();
#line default
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

#line default

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

				var targetRoot = _targetRoot;
				var dataRoot = _dataRoot;
#line (34, 24) - (34, 44) 34 "Page1.xml"
				targetRoot.textBlock4.Text = dataRoot.Title;
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

					var targetRoot = bindings._targetRoot;
					var dataRoot = bindings._dataRoot;
					var typedSender = (global::WPFTest.ViewModels.EntityViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "Title")
					{
#line (34, 24) - (34, 44) 34 "Page1.xml"
						targetRoot.textBlock4.Text = typedSender.Title;
#line default
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
