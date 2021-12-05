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

#line (13, 5) - (13, 5) 13 "Page1.xml"
			var value1 = WPFTest.Strings.Instance;
#line (13, 5) - (13, 5) 13 "Page1.xml"
			Title = value1.Title;
#line default
			Set0(_generatedCodeDisposed.Token);
			async void Set0(CancellationToken cancellationToken)
			{
				try
				{
#line (31, 10) - (31, 10) 31 "Page1.xml"
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
#line (37, 13) - (37, 13) 37 "Page1.xml"
			header1.Text = value1.Header1;
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
#line (34, 13) - (34, 13) 34 "Page1.xml"
				targetRoot.textBlock4.Text = dataRoot._viewModel.DecimalProp.ToString();
#line (56, 13) - (56, 13) 56 "Page1.xml"
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
#line (34, 13) - (34, 13) 34 "Page1.xml"
						targetRoot.textBlock4.Text = typedSender.DecimalProp.ToString();
#line default
						if (!notifyAll)
						{
							return;
						}
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
#line (56, 13) - (56, 13) 56 "Page1.xml"
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

#line (57, 13) - (57, 13) 57 "Page1.xml"
				_eventHandler13 = (p1, p2) => dataRoot.ModifyViewModel?.OnClick(dataRoot.BooleanProp);
#line default
				_targetRoot.button1.Click += _eventHandler13;
#line default

				_bindingsTrackings.SetPropertyChangedEventHandler0(dataRoot);

				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::System.Windows.Controls.TextBox.TextProperty, typeof(global::System.Windows.Controls.TextBox))
					.AddValueChanged(_targetRoot.textBox1, OnTargetChanged0);
				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::System.Windows.Controls.CheckBox.IsCheckedProperty, typeof(global::System.Windows.Controls.CheckBox))
					.AddValueChanged(_targetRoot.checkBox1, OnTargetChanged1);
				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::System.Windows.Controls.TextBox.TextProperty, typeof(global::System.Windows.Controls.TextBox))
					.AddValueChanged(_targetRoot.textBox2, OnTargetChanged2);
				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::System.Windows.Controls.TextBox.TextProperty, typeof(global::System.Windows.Controls.TextBox))
					.AddValueChanged(_targetRoot.textBox3, OnTargetChanged3);
				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::System.Windows.Controls.CheckBox.IsCheckedProperty, typeof(global::System.Windows.Controls.CheckBox))
					.AddValueChanged(_targetRoot.checkBox2, OnTargetChanged4);
			}

			public void Cleanup()
			{
				if (_targetRoot != null)
				{
					_generatedCodeDisposed.Cancel();
					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::System.Windows.Controls.TextBox.TextProperty, typeof(global::System.Windows.Controls.TextBox))
						.RemoveValueChanged(_targetRoot.textBox1, OnTargetChanged0);
					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::System.Windows.Controls.CheckBox.IsCheckedProperty, typeof(global::System.Windows.Controls.CheckBox))
						.RemoveValueChanged(_targetRoot.checkBox1, OnTargetChanged1);
					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::System.Windows.Controls.TextBox.TextProperty, typeof(global::System.Windows.Controls.TextBox))
						.RemoveValueChanged(_targetRoot.textBox2, OnTargetChanged2);
					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::System.Windows.Controls.TextBox.TextProperty, typeof(global::System.Windows.Controls.TextBox))
						.RemoveValueChanged(_targetRoot.textBox3, OnTargetChanged3);
					global::System.ComponentModel.DependencyPropertyDescriptor
						.FromProperty(
							global::System.Windows.Controls.CheckBox.IsCheckedProperty, typeof(global::System.Windows.Controls.CheckBox))
						.RemoveValueChanged(_targetRoot.checkBox2, OnTargetChanged4);
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
#line (50, 20) - (50, 20) 50 "Page1.xml"
				var value1 = dataRoot.ModifyViewModel;
#line (44, 19) - (44, 19) 44 "Page1.xml"
				var value2 = dataRoot.ArrayProp.Length > 0;
#line (38, 13) - (38, 13) 38 "Page1.xml"
				var value3 = dataRoot.BooleanProp;
#line (40, 13) - (40, 13) 40 "Page1.xml"
				var value4 = dataRoot.DecimalProp;
#line (51, 20) - (51, 20) 51 "Page1.xml"
				var value5 = value1?.Input1;
#line (59, 12) - (59, 12) 59 "Page1.xml"
				var value6 = value1?.ModifyTextViewModel;
#line (38, 13) - (38, 13) 38 "Page1.xml"
				targetRoot.header1.Visibility = (value3 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed);
#line (40, 13) - (40, 13) 40 "Page1.xml"
				targetRoot.textBlock5.Text = value4.ToString();
#line (41, 13) - (41, 13) 41 "Page1.xml"
				targetRoot.textBlock5.Visibility = ((global::System.Windows.Visibility)targetRoot.TrueToVisibleConverter.Convert(value3, typeof(global::System.Windows.Visibility), null, null));
#line (42, 20) - (42, 20) 42 "Page1.xml"
				targetRoot.textBlock6.Text = (value4 + 1).ToString();
#line default
				if (!_settingBinding4)
				{
#line (43, 18) - (43, 18) 43 "Page1.xml"
					var value7 = dataRoot.OrderInput;
#line default
					if (!object.Equals(targetRoot.textBox1.Text, value7))
					{
						targetRoot.textBox1.Text = value7;
					}
				}
#line default
				if (!_settingBinding5)
				{
#line (44, 19) - (44, 19) 44 "Page1.xml"
					var value8 = ((global::System.Nullable<global::System.Boolean>)targetRoot.InverseBooleanConverter.Convert(dataRoot.BoolInput, typeof(global::System.Nullable<global::System.Boolean>), value2, null));
#line default
					if (!object.Equals(targetRoot.checkBox1.IsChecked, value8))
					{
						targetRoot.checkBox1.IsChecked = value8;
					}
				}
#line (48, 13) - (48, 13) 48 "Page1.xml"
				targetRoot.listView.ItemsSource = dataRoot.ListProp;
#line (49, 13) - (49, 13) 49 "Page1.xml"
				targetRoot.listView.SetVisible(value2);
#line (50, 20) - (50, 20) 50 "Page1.xml"
				targetRoot.textBlock7.Text = (value1 != null ? value1.Input1 : "abc");
#line (51, 20) - (51, 20) 51 "Page1.xml"
				targetRoot.textBlock8.Text = value5 ?? "aaa";
#line default
				Set0(bindings._generatedCodeDisposed.Token);
				async void Set0(CancellationToken cancellationToken)
				{
					try
					{
#line (52, 20) - (52, 20) 52 "Page1.xml"
						var task = dataRoot.TaskProp;
#line default
						if (!task.IsCompleted)
						{
#line (52, 20) - (52, 20) 52 "Page1.xml"
							targetRoot.textBlock9.Text = "Loading...";
#line default
						}
#line (52, 20) - (52, 20) 52 "Page1.xml"
						var value = await task;
#line default
						if (!cancellationToken.IsCancellationRequested)
						{
							targetRoot.textBlock9.Text = value;
						}
					}
					catch
					{
					}
				}
#line (53, 20) - (53, 20) 53 "Page1.xml"
				targetRoot.textBlock10.Text = dataRoot.Calculate(new WPFTest.ViewModels.ParamClass("text"));
#line default
				Set1(bindings._generatedCodeDisposed.Token);
				async void Set1(CancellationToken cancellationToken)
				{
					try
					{
#line (54, 16) - (54, 16) 54 "Page1.xml"
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
#line default
				if (!_settingBinding14)
				{
					if (!object.Equals(targetRoot.textBox2.Text, value5))
					{
						targetRoot.textBox2.Text = value5;
					}
				}
#line default
				if (!_settingBinding15)
				{
#line (59, 12) - (59, 12) 59 "Page1.xml"
					var value9 = value6?.TextInput;
#line default
					if (!object.Equals(targetRoot.textBox3.Text, value9))
					{
						targetRoot.textBox3.Text = value9;
					}
				}
#line default
				if (!_settingBinding16)
				{
#line (60, 13) - (60, 13) 60 "Page1.xml"
					var value10 = value1?.BoolInput;
#line default
					if (!object.Equals(targetRoot.checkBox2.IsChecked, value10))
					{
						targetRoot.checkBox2.IsChecked = value10;
					}
				}
#line default

				_bindingsTrackings.SetPropertyChangedEventHandler1(value1);
				_bindingsTrackings.SetPropertyChangedEventHandler2(value6);
			}

			private void OnTargetChanged0(object sender, global::System.EventArgs e)
			{
				var dataRoot = _dataRoot;
				var targetRoot = _targetRoot;
				if (!_settingBinding4)
				{
					_settingBinding4 = true;
					try
					{
#line (43, 18) - (43, 18) 43 "Page1.xml"
						dataRoot.OrderInput = _targetRoot.textBox1.Text;
#line default
					}
					catch
					{
					}
					finally
					{
						_settingBinding4 = false;
					}
				}
			}

			private void OnTargetChanged1(object sender, global::System.EventArgs e)
			{
				var dataRoot = _dataRoot;
				var targetRoot = _targetRoot;
				if (!_settingBinding5)
				{
					_settingBinding5 = true;
					try
					{
#line (44, 19) - (44, 19) 44 "Page1.xml"
						dataRoot.BoolInput = (global::System.Boolean)targetRoot.InverseBooleanConverter.ConvertBack(_targetRoot.checkBox1.IsChecked, typeof(global::System.Boolean), dataRoot.ArrayProp.Length > 0, null);
#line default
					}
					catch
					{
					}
					finally
					{
						_settingBinding5 = false;
					}
				}
			}

			private void OnTargetChanged2(object sender, global::System.EventArgs e)
			{
				var dataRoot = _dataRoot;
				var targetRoot = _targetRoot;
				if (!_settingBinding14)
				{
					_settingBinding14 = true;
					try
					{
#line (58, 12) - (58, 12) 58 "Page1.xml"
						var value = dataRoot.ModifyViewModel;
#line default
						if (value != null)
						{
#line (58, 12) - (58, 12) 58 "Page1.xml"
							value.Input1 = _targetRoot.textBox2.Text;
#line default
						}
					}
					catch
					{
					}
					finally
					{
						_settingBinding14 = false;
					}
				}
			}

			private void OnTargetChanged3(object sender, global::System.EventArgs e)
			{
				var dataRoot = _dataRoot;
				var targetRoot = _targetRoot;
				if (!_settingBinding15)
				{
					_settingBinding15 = true;
					try
					{
#line (59, 12) - (59, 12) 59 "Page1.xml"
						var value = dataRoot.ModifyViewModel;
#line default
						if (value != null)
						{
#line (59, 12) - (59, 12) 59 "Page1.xml"
							value.ModifyTextViewModel.TextInput = _targetRoot.textBox3.Text;
#line default
						}
					}
					catch
					{
					}
					finally
					{
						_settingBinding15 = false;
					}
				}
			}

			private void OnTargetChanged4(object sender, global::System.EventArgs e)
			{
				var dataRoot = _dataRoot;
				var targetRoot = _targetRoot;
				if (!_settingBinding16)
				{
					_settingBinding16 = true;
					try
					{
#line (60, 13) - (60, 13) 60 "Page1.xml"
						var value = dataRoot.ModifyViewModel;
#line default
						if (value != null)
						{
#line (60, 13) - (60, 13) 60 "Page1.xml"
							value.BoolInput = _targetRoot.checkBox2.IsChecked ?? default;
#line default
						}
					}
					catch
					{
					}
					finally
					{
						_settingBinding16 = false;
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
#line (38, 13) - (38, 13) 38 "Page1.xml"
						var value1 = typedSender.BooleanProp;
#line (38, 13) - (38, 13) 38 "Page1.xml"
						targetRoot.header1.Visibility = (value1 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed);
#line (41, 13) - (41, 13) 41 "Page1.xml"
						targetRoot.textBlock5.Visibility = ((global::System.Windows.Visibility)targetRoot.TrueToVisibleConverter.Convert(value1, typeof(global::System.Windows.Visibility), null, null));
#line default
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "DecimalProp")
					{
#line (40, 13) - (40, 13) 40 "Page1.xml"
						var value1 = typedSender.DecimalProp;
#line (40, 13) - (40, 13) 40 "Page1.xml"
						targetRoot.textBlock5.Text = value1.ToString();
#line (42, 20) - (42, 20) 42 "Page1.xml"
						targetRoot.textBlock6.Text = (value1 + 1).ToString();
#line default
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "OrderInput")
					{
#line default
						if (!bindings._settingBinding4)
						{
#line (43, 18) - (43, 18) 43 "Page1.xml"
							var value1 = typedSender.OrderInput;
#line default
							if (!object.Equals(targetRoot.textBox1.Text, value1))
							{
								targetRoot.textBox1.Text = value1;
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
#line default
						if (!bindings._settingBinding5)
						{
#line (44, 19) - (44, 19) 44 "Page1.xml"
							var value1 = ((global::System.Nullable<global::System.Boolean>)targetRoot.InverseBooleanConverter.Convert(typedSender.BoolInput, typeof(global::System.Nullable<global::System.Boolean>), dataRoot.ArrayProp.Length > 0, null));
#line default
							if (!object.Equals(targetRoot.checkBox1.IsChecked, value1))
							{
								targetRoot.checkBox1.IsChecked = value1;
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
#line (44, 19) - (44, 19) 44 "Page1.xml"
						var value1 = typedSender.ArrayProp.Length > 0;
#line default
						if (!bindings._settingBinding5)
						{
#line (44, 19) - (44, 19) 44 "Page1.xml"
							var value2 = ((global::System.Nullable<global::System.Boolean>)targetRoot.InverseBooleanConverter.Convert(dataRoot.BoolInput, typeof(global::System.Nullable<global::System.Boolean>), value1, null));
#line default
							if (!object.Equals(targetRoot.checkBox1.IsChecked, value2))
							{
								targetRoot.checkBox1.IsChecked = value2;
							}
						}
#line (49, 13) - (49, 13) 49 "Page1.xml"
						targetRoot.listView.SetVisible(value1);
#line default
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "ListProp")
					{
#line (48, 13) - (48, 13) 48 "Page1.xml"
						targetRoot.listView.ItemsSource = typedSender.ListProp;
#line default
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "ModifyViewModel")
					{
#line (50, 20) - (50, 20) 50 "Page1.xml"
						var value1 = typedSender.ModifyViewModel;
#line (51, 20) - (51, 20) 51 "Page1.xml"
						var value2 = value1?.Input1;
#line (59, 12) - (59, 12) 59 "Page1.xml"
						var value3 = value1?.ModifyTextViewModel;
#line (50, 20) - (50, 20) 50 "Page1.xml"
						targetRoot.textBlock7.Text = (value1 != null ? value1.Input1 : "abc");
#line (51, 20) - (51, 20) 51 "Page1.xml"
						targetRoot.textBlock8.Text = value2 ?? "aaa";
#line default
						if (!bindings._settingBinding14)
						{
							if (!object.Equals(targetRoot.textBox2.Text, value2))
							{
								targetRoot.textBox2.Text = value2;
							}
						}
#line default
						if (!bindings._settingBinding15)
						{
#line (59, 12) - (59, 12) 59 "Page1.xml"
							var value4 = value3?.TextInput;
#line default
							if (!object.Equals(targetRoot.textBox3.Text, value4))
							{
								targetRoot.textBox3.Text = value4;
							}
						}
#line default
						if (!bindings._settingBinding16)
						{
#line (60, 13) - (60, 13) 60 "Page1.xml"
							var value5 = value1?.BoolInput;
#line default
							if (!object.Equals(targetRoot.checkBox2.IsChecked, value5))
							{
								targetRoot.checkBox2.IsChecked = value5;
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
#line (52, 20) - (52, 20) 52 "Page1.xml"
								var task = typedSender.TaskProp;
#line default
								if (!task.IsCompleted)
								{
#line (52, 20) - (52, 20) 52 "Page1.xml"
									targetRoot.textBlock9.Text = "Loading...";
#line default
								}
#line (52, 20) - (52, 20) 52 "Page1.xml"
								var value = await task;
#line default
								if (!cancellationToken.IsCancellationRequested)
								{
									targetRoot.textBlock9.Text = value;
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
#line (51, 20) - (51, 20) 51 "Page1.xml"
						var value1 = typedSender.Input1;
#line (51, 20) - (51, 20) 51 "Page1.xml"
						targetRoot.textBlock8.Text = value1 ?? "aaa";
#line default
						if (!bindings._settingBinding14)
						{
							if (!object.Equals(targetRoot.textBox2.Text, value1))
							{
								targetRoot.textBox2.Text = value1;
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
#line (59, 12) - (59, 12) 59 "Page1.xml"
						var value1 = typedSender.ModifyTextViewModel;
#line default
						if (!bindings._settingBinding15)
						{
#line (59, 12) - (59, 12) 59 "Page1.xml"
							var value2 = value1.TextInput;
#line default
							if (!object.Equals(targetRoot.textBox3.Text, value2))
							{
								targetRoot.textBox3.Text = value2;
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
#line default
						if (!bindings._settingBinding16)
						{
#line (60, 13) - (60, 13) 60 "Page1.xml"
							var value1 = typedSender.BoolInput;
#line default
							if (!object.Equals(targetRoot.checkBox2.IsChecked, value1))
							{
								targetRoot.checkBox2.IsChecked = value1;
							}
						}
#line default
						if (!notifyAll)
						{
							return;
						}
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
#line default
						if (!bindings._settingBinding15)
						{
#line (59, 12) - (59, 12) 59 "Page1.xml"
							var value1 = typedSender.TextInput;
#line default
							if (!object.Equals(targetRoot.textBox3.Text, value1))
							{
								targetRoot.textBox3.Text = value1;
							}
						}
#line default
						if (!notifyAll)
						{
							return;
						}
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
#line (24, 28) - (24, 28) 24 "Page1.xml"
				var value1 = dataRoot.BooleanProp;
#line (23, 28) - (23, 28) 23 "Page1.xml"
				var value2 = dataRoot.Model;
#line (23, 28) - (23, 28) 23 "Page1.xml"
				targetRoot.textBlock1.Text = value2.SByteProp.ToString();
#line (24, 28) - (24, 28) 24 "Page1.xml"
				targetRoot.textBlock2.Text = value1.ToString();
#line (25, 28) - (25, 28) 25 "Page1.xml"
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
#line (23, 28) - (23, 28) 23 "Page1.xml"
						var value1 = typedSender.Model;
#line (23, 28) - (23, 28) 23 "Page1.xml"
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
#line (24, 28) - (24, 28) 24 "Page1.xml"
						var value1 = typedSender.BooleanProp;
#line (24, 28) - (24, 28) 24 "Page1.xml"
						targetRoot.textBlock2.Text = value1.ToString();
#line (25, 28) - (25, 28) 25 "Page1.xml"
						targetRoot.textBlock3.IsEnabled = ((global::System.Boolean)targetRoot.InverseBooleanConverter.Convert(value1, typeof(global::System.Boolean), null, null));
#line default
						if (!notifyAll)
						{
							return;
						}
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
#line (23, 28) - (23, 28) 23 "Page1.xml"
						targetRoot.textBlock1.Text = typedSender.SByteProp.ToString();
#line default
						if (!notifyAll)
						{
							return;
						}
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
}
