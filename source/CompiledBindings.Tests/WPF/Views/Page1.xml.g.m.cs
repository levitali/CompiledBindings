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

			var value1 = WPFTest.Strings.Instance;
			Title = value1.Title;
			Set0(_generatedCodeDisposed.Token);
			async void Set0(CancellationToken cancellationToken)
			{
				try
				{
					var value = await this.LoadImageAsync();
					if (!cancellationToken.IsCancellationRequested)
					{
						image1.Source = value;
					}
				}
				catch
				{
				}
			}
			header1.Text = value1.Header1;

			Bindings_.Initialize(this);
			this.DataContextChanged += this_DataContextChanged;
			if (this.DataContext is global::WPFTest.ViewModels.Page1ViewModel dataRoot1)
			{
				Bindings_this.Initialize(this, dataRoot1);
			}
		}

		~Page1()
		{
			_generatedCodeDisposed.Cancel();
			if (Bindings_ != null)
			{
				Bindings_.Cleanup();
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

		Page1_Bindings_ Bindings_ = new Page1_Bindings_();

		class Page1_Bindings_
		{
			Page1 _targetRoot;
			Page1_BindingsTrackings_ _bindingsTrackings;

			public void Initialize(Page1 dataRoot)
			{
				if (_targetRoot != null)
					throw new System.InvalidOperationException();
				if (dataRoot == null)
					throw new System.ArgumentNullException(nameof(dataRoot));

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
				if (_targetRoot == null)
				{
					throw new System.InvalidOperationException();
				}

				var targetRoot = _targetRoot;
				var dataRoot = _targetRoot;
				targetRoot.textBlock4.Text = dataRoot._viewModel.DecimalProp.ToString();
				targetRoot.button1.IsEnabled = dataRoot.listView.SelectedItem != null;

				_bindingsTrackings.SetPropertyChangedEventHandler0(dataRoot._viewModel);
				_bindingsTrackings.SetPropertyChangedEventHandler1(dataRoot.listView);
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
					if (_propertyChangeSource0 != null)
					{
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource0).PropertyChanged -= OnPropertyChanged0;
						_propertyChangeSource0 = null;
					}
					if (_propertyChangeSource1 != null)
					{
						global::System.ComponentModel.DependencyPropertyDescriptor
							.FromProperty(
								global::System.Windows.Controls.ListView.SelectedItemProperty, typeof(global::System.Windows.Controls.ListView))
							.RemoveValueChanged(_propertyChangeSource1, OnPropertyChanged1_SelectedItem);
						_propertyChangeSource1 = null;
					}
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
						targetRoot.textBlock4.Text = typedSender.DecimalProp.ToString();
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
					targetRoot.button1.IsEnabled = typedSender.SelectedItem != null;
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
			bool _settingBinding4;
			bool _settingBinding5;
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

				_bindingsTrackings.SetPropertyChangedEventHandler0(dataRoot);

				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::System.Windows.Controls.TextBox.TextProperty, typeof(global::System.Windows.Controls.TextBox))
					.AddValueChanged(_targetRoot.textBox1, OnTargetChanged0);
				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::System.Windows.Controls.CheckBox.IsCheckedProperty, typeof(global::System.Windows.Controls.CheckBox))
					.AddValueChanged(_targetRoot.checkBox1, OnTargetChanged1);
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
				var value1 = dataRoot.ArrayProp?.Length > 0;
				var value2 = dataRoot.BooleanProp;
				var value3 = dataRoot.DecimalProp;
				var value4 = dataRoot.ModifyViewModel;
				targetRoot.header1.Visibility = (value2 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed);
				targetRoot.textBlock5.Visibility = ((global::System.Windows.Visibility)targetRoot.TrueToVisibleConverter.Convert(value2, typeof(global::System.Windows.Visibility), null, null));
				targetRoot.textBlock5.Text = value3.ToString();
				targetRoot.textBlock6.Text = (value3 + 1).ToString();
				if (!_settingBinding4)
				{
					var value5 = dataRoot.OrderInput;
					if (!object.Equals(targetRoot.textBox1.Text, value5))
					{
						targetRoot.textBox1.Text = value5;
					}
				}
				if (!_settingBinding5)
				{
					var value6 = ((global::System.Nullable<global::System.Boolean>)targetRoot.InverseBooleanConverter.Convert(dataRoot.BoolInput, typeof(global::System.Nullable<global::System.Boolean>), value1, null));
					if (!object.Equals(targetRoot.checkBox1.IsChecked, value6))
					{
						targetRoot.checkBox1.IsChecked = value6;
					}
				}
				targetRoot.listView.ItemsSource = dataRoot.ListProp;
				targetRoot.listView.SetVisible(value1);
				targetRoot.textBlock7.Text = dataRoot.ModifyViewModel is var v0 && v0 != null ? v0.Input1 : "abc";
				targetRoot.textBlock8.Text = value4?.Input1 ?? "aaa";
				Set0(bindings._generatedCodeDisposed.Token);
				async void Set0(CancellationToken cancellationToken)
				{
					try
					{
						var task = dataRoot.TaskProp;
						if (!task.IsCompleted)
						{
							targetRoot.textBlock9.Text = "Loading...";
						}
						var value = await task;
						if (!cancellationToken.IsCancellationRequested)
						{
							targetRoot.textBlock9.Text = value;
						}
					}
					catch
					{
					}
				}
				Set1(bindings._generatedCodeDisposed.Token);
				async void Set1(CancellationToken cancellationToken)
				{
					try
					{
						var value = await dataRoot.LoadImageAsync();
						if (!cancellationToken.IsCancellationRequested)
						{
							targetRoot.image2.Source = value;
						}
					}
					catch
					{
					}
				}

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
						dataRoot.OrderInput = _targetRoot.textBox1.Text;
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
						dataRoot.BoolInput = (global::System.Boolean)targetRoot.InverseBooleanConverter.ConvertBack(_targetRoot.checkBox1.IsChecked, typeof(global::System.Boolean), dataRoot.ArrayProp?.Length > 0, null);
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

			class Page1_BindingsTrackings_this
			{
				global::System.WeakReference _bindingsWeakRef;
				global::WPFTest.ViewModels.Page1ViewModel _propertyChangeSource0;

				public Page1_BindingsTrackings_this(Page1_Bindings_this bindings)
				{
					_bindingsWeakRef = new global::System.WeakReference(bindings);
				}

				public void Cleanup()
				{
					if (_propertyChangeSource0 != null)
					{
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource0).PropertyChanged -= OnPropertyChanged0;
						_propertyChangeSource0 = null;
					}
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
						var value1 = typedSender.BooleanProp;
						targetRoot.header1.Visibility = (value1 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed);
						targetRoot.textBlock5.Visibility = ((global::System.Windows.Visibility)targetRoot.TrueToVisibleConverter.Convert(value1, typeof(global::System.Windows.Visibility), null, null));
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "DecimalProp")
					{
						var value1 = typedSender.DecimalProp;
						targetRoot.textBlock5.Text = value1.ToString();
						targetRoot.textBlock6.Text = (value1 + 1).ToString();
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "OrderInput")
					{
						if (!bindings._settingBinding4)
						{
							var value1 = typedSender.OrderInput;
							if (!object.Equals(targetRoot.textBox1.Text, value1))
							{
								targetRoot.textBox1.Text = value1;
							}
						}
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "BoolInput")
					{
						if (!bindings._settingBinding5)
						{
							var value1 = ((global::System.Nullable<global::System.Boolean>)targetRoot.InverseBooleanConverter.Convert(typedSender.BoolInput, typeof(global::System.Nullable<global::System.Boolean>), dataRoot.ArrayProp?.Length > 0, null));
							if (!object.Equals(targetRoot.checkBox1.IsChecked, value1))
							{
								targetRoot.checkBox1.IsChecked = value1;
							}
						}
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "ArrayProp")
					{
						var value1 = typedSender.ArrayProp?.Length > 0;
						if (!bindings._settingBinding5)
						{
							var value2 = ((global::System.Nullable<global::System.Boolean>)targetRoot.InverseBooleanConverter.Convert(dataRoot.BoolInput, typeof(global::System.Nullable<global::System.Boolean>), value1, null));
							if (!object.Equals(targetRoot.checkBox1.IsChecked, value2))
							{
								targetRoot.checkBox1.IsChecked = value2;
							}
						}
						targetRoot.listView.SetVisible(value1);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "ListProp")
					{
						targetRoot.listView.ItemsSource = typedSender.ListProp;
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "ModifyViewModel")
					{
						var value1 = typedSender.ModifyViewModel;
						targetRoot.textBlock7.Text = dataRoot.ModifyViewModel is var v0 && v0 != null ? v0.Input1 : "abc";
						targetRoot.textBlock8.Text = value1?.Input1 ?? "aaa";
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "TaskProp")
					{
						Set0(bindings._generatedCodeDisposed.Token);
						async void Set0(CancellationToken cancellationToken)
						{
							try
							{
								var task = typedSender.TaskProp;
								if (!task.IsCompleted)
								{
									targetRoot.textBlock9.Text = "Loading...";
								}
								var value = await task;
								if (!cancellationToken.IsCancellationRequested)
								{
									targetRoot.textBlock9.Text = value;
								}
							}
							catch
							{
							}
						}
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
				var value1 = dataRoot.BooleanProp;
				var value2 = dataRoot.Model;
				targetRoot.textBlock1.Text = value2?.SByteProp.ToString();
				targetRoot.textBlock2.Text = value1.ToString();
				targetRoot.textBlock3.IsEnabled = ((global::System.Boolean)targetRoot.InverseBooleanConverter.Convert(value1, typeof(global::System.Boolean), null, null));

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
					if (_propertyChangeSource0 != null)
					{
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource0).PropertyChanged -= OnPropertyChanged0;
						_propertyChangeSource0 = null;
					}
					if (_propertyChangeSource1 != null)
					{
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource1).PropertyChanged -= OnPropertyChanged1;
						_propertyChangeSource1 = null;
					}
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
						var value1 = typedSender.Model;
						targetRoot.textBlock1.Text = value1?.SByteProp.ToString();
						SetPropertyChangedEventHandler1(value1);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "BooleanProp")
					{
						var value1 = typedSender.BooleanProp;
						targetRoot.textBlock2.Text = value1.ToString();
						targetRoot.textBlock3.IsEnabled = ((global::System.Boolean)targetRoot.InverseBooleanConverter.Convert(value1, typeof(global::System.Boolean), null, null));
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
						targetRoot.textBlock1.Text = typedSender.SByteProp.ToString();
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
