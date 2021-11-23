//checksum 2891290075 True
namespace WPFTest.Views
{
	using System.Threading;
	using WPFTest;

#nullable disable

	[System.CodeDom.Compiler.GeneratedCode("CompiledBindings", null)]
	partial class MainWindow
	{
		CancellationTokenSource _generatedCodeDisposed = new CancellationTokenSource();
		global::System.Windows.Data.IValueConverter booleanToVisibilityConverter;
		global::System.Windows.Media.ImageSource defaultImage;
		private bool _generatedCodeInitialized;

		private void InitializeAfterConstructor()
		{
			if (_generatedCodeInitialized)
				return;

			_generatedCodeInitialized = true;

			booleanToVisibilityConverter = (global::System.Windows.Data.IValueConverter)(this.Resources["booleanToVisibilityConverter"] ?? global::System.Windows.Application.Current.Resources["booleanToVisibilityConverter"] ?? throw new global::System.Exception("Resource 'booleanToVisibilityConverter' not found."));
			defaultImage = (global::System.Windows.Media.ImageSource)(this.Resources["defaultImage"] ?? global::System.Windows.Application.Current.Resources["defaultImage"] ?? throw new global::System.Exception("Resource 'defaultImage' not found."));

			var value1 = WPFTest.Strings.Instance;
#line 16 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
			Title = value1.Title;
#line 43 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
			Set0(_generatedCodeDisposed.Token);
			async void Set0(CancellationToken cancellationToken)
			{
				try
				{
					var value = await this._viewModel?.LoadImageAsync();
					if (!cancellationToken.IsCancellationRequested)
					{
						image1.Source = value;
					}
				}
				catch
				{
				}
			}
#line 45 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
			textBlock3.Text = value1.Header1;

			Bindings.Initialize(this);
			this.DataContextChanged += this_DataContextChanged;
			if (this.DataContext is global::WPFTest.ViewModels.MainViewModel dataRoot1)
			{
				Bindings_this.Initialize(this, dataRoot1);
			}
		}

		~MainWindow()
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
			if (((global::System.Windows.FrameworkElement)sender).DataContext is global::WPFTest.ViewModels.MainViewModel dataRoot)
			{
				Bindings_this.Initialize(this, dataRoot);
			}
		}

		MainWindow_Bindings Bindings = new MainWindow_Bindings();

		class MainWindow_Bindings
		{
			MainWindow _targetRoot;
			MainWindow_BindingsTrackings _bindingsTrackings;
			global::System.Windows.RoutedEventHandler _eventHandler2;

			public void Initialize(MainWindow dataRoot)
			{
				if (_targetRoot != null)
					throw new System.InvalidOperationException();
				if (dataRoot == null)
					throw new System.ArgumentNullException(nameof(dataRoot));

				_targetRoot = dataRoot;
				_bindingsTrackings = new MainWindow_BindingsTrackings(this);

				Update();

				_eventHandler2 = (p1, p2) => dataRoot._viewModel?.ModifyViewModel?.OnClick(dataRoot.button?.CommandParameter ?? default);
				_targetRoot.button.Click += _eventHandler2;
			}

			public void Cleanup()
			{
				if (_targetRoot != null)
				{
					_targetRoot.button.Click -= _eventHandler2;
					_eventHandler2 = null;
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
#line 55 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
				targetRoot.textBlock9.Text = dataRoot.InstanceFunction(1, 2);
#line 70 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
				targetRoot.button.IsEnabled = dataRoot.listView?.SelectedItem != null;

				_bindingsTrackings.SetPropertyChangedEventHandler0(dataRoot.listView);
			}

			class MainWindow_BindingsTrackings
			{
				global::System.WeakReference _bindingsWeakRef;
				global::System.Windows.Controls.ListView _propertyChangeSource0;

				public MainWindow_BindingsTrackings(MainWindow_Bindings bindings)
				{
					_bindingsWeakRef = new global::System.WeakReference(bindings);
				}

				public void Cleanup()
				{
					SetPropertyChangedEventHandler0(null);
				}

				public void SetPropertyChangedEventHandler0(global::System.Windows.Controls.ListView value)
				{
					if (_propertyChangeSource0 != null && !object.ReferenceEquals(_propertyChangeSource0, value))
					{
						global::System.ComponentModel.DependencyPropertyDescriptor
							.FromProperty(
								global::System.Windows.Controls.ListView.SelectedItemProperty, typeof(global::System.Windows.Controls.ListView))
							.RemoveValueChanged(_propertyChangeSource0, OnPropertyChanged0_SelectedItem);
						_propertyChangeSource0 = null;
					}
					if (_propertyChangeSource0 == null && value != null)
					{
						_propertyChangeSource0 = value;
						global::System.ComponentModel.DependencyPropertyDescriptor
							.FromProperty(
								global::System.Windows.Controls.ListView.SelectedItemProperty, typeof(global::System.Windows.Controls.ListView))
							.AddValueChanged(_propertyChangeSource0, OnPropertyChanged0_SelectedItem);
					}
				}

				private void OnPropertyChanged0_SelectedItem(object sender, global::System.EventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var targetRoot = bindings._targetRoot;
					var dataRoot = bindings._targetRoot;
					var typedSender = (global::System.Windows.Controls.ListView)sender;
#line 70 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
					targetRoot.button.IsEnabled = typedSender.SelectedItem != null;
				}

				MainWindow_Bindings TryGetBindings()
				{
					MainWindow_Bindings bindings = null;
					if (_bindingsWeakRef != null)
					{
						bindings = (MainWindow_Bindings)_bindingsWeakRef.Target;
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

		MainWindow_Bindings_this Bindings_this = new MainWindow_Bindings_this();

		class MainWindow_Bindings_this
		{
			MainWindow _targetRoot;
			global::WPFTest.ViewModels.MainViewModel _dataRoot;
			MainWindow_BindingsTrackings_this _bindingsTrackings;
			bool _settingBinding0;
			CancellationTokenSource _generatedCodeDisposed;

			public void Initialize(MainWindow targetRoot, global::WPFTest.ViewModels.MainViewModel dataRoot)
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
				_bindingsTrackings = new MainWindow_BindingsTrackings_this(this);

				Update();

				_bindingsTrackings.SetPropertyChangedEventHandler0(dataRoot);

				global::System.ComponentModel.DependencyPropertyDescriptor
					.FromProperty(
						global::System.Windows.Controls.TextBox.TextProperty, typeof(global::System.Windows.Controls.TextBox))
					.AddValueChanged(_targetRoot.textBox1, OnTargetChanged0);
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
				var value1 = dataRoot.ModifyViewModel;
				if (!_settingBinding0)
				{
					var value2 = dataRoot.OrderInput;
					if (!object.Equals(targetRoot.textBox1.Text, value2))
					{
						targetRoot.textBox1.Text = value2;
					}
				}
#line 47 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
				targetRoot.textBlock4.Text = dataRoot.DecimalProp.ToString();
#line 48 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
				targetRoot.textBlock4.Visibility = ((global::System.Windows.Visibility)targetRoot.booleanToVisibilityConverter.Convert(dataRoot.BooleanProp, typeof(global::System.Windows.Visibility), null, null));
#line 49 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
				targetRoot.textBlock5.Text = dataRoot.ModifyViewModel is var v0 && v0 != null ? v0.Input1 : "aaa";
#line 50 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
				targetRoot.textBlock6.Text = dataRoot.ModifyViewModel is var v1 && v1 != null ? v1.Input2 : "aaa";
#line 51 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
				targetRoot.textBlock7.Text = value1?.Input1 ?? "ccc";
#line 52 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
				Set0(bindings._generatedCodeDisposed.Token);
				async void Set0(CancellationToken cancellationToken)
				{
					try
					{
						var value = await dataRoot.TaskProp;
						if (!cancellationToken.IsCancellationRequested)
						{
							targetRoot.textBlock8.Text = value;
						}
					}
					catch
					{
					}
				}
#line 59 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
				Set1(bindings._generatedCodeDisposed.Token);
				async void Set1(CancellationToken cancellationToken)
				{
					try
					{
						var task = dataRoot.LoadImageAsync() + 1;
						if (!task.IsCompleted)
						{
							targetRoot.image2.Source = targetRoot.defaultImage;
						}
						var value = await task;
						if (!cancellationToken.IsCancellationRequested)
						{
							targetRoot.image2.Source = value;
						}
					}
					catch
					{
					}
				}
#line 64 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
				targetRoot.listView.ItemsSource = dataRoot.ListProp;
#line 65 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
				targetRoot.listView.SetVisible(dataRoot.ArrayProp.Length > 0);

			}

			private void OnTargetChanged0(object sender, global::System.EventArgs e)
			{
				var dataRoot = _dataRoot;
				var targetRoot = _targetRoot;
				if (!_settingBinding0)
				{
					_settingBinding0 = true;
					try
					{
						dataRoot.OrderInput = _targetRoot.textBox1.Text;
					}
					catch
					{
					}
					finally
					{
						_settingBinding0 = false;
					}
				}
			}

			class MainWindow_BindingsTrackings_this
			{
				global::System.WeakReference _bindingsWeakRef;
				global::WPFTest.ViewModels.MainViewModel _propertyChangeSource0;

				public MainWindow_BindingsTrackings_this(MainWindow_Bindings_this bindings)
				{
					_bindingsWeakRef = new global::System.WeakReference(bindings);
				}

				public void Cleanup()
				{
					SetPropertyChangedEventHandler0(null);
				}

				public void SetPropertyChangedEventHandler0(global::WPFTest.ViewModels.MainViewModel value)
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
					var typedSender = (global::WPFTest.ViewModels.MainViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "OrderInput")
					{
						if (!bindings._settingBinding0)
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
					if (notifyAll || e.PropertyName == "DecimalProp")
					{
#line 47 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
						targetRoot.textBlock4.Text = typedSender.DecimalProp.ToString();
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "BooleanProp")
					{
#line 48 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
						targetRoot.textBlock4.Visibility = ((global::System.Windows.Visibility)targetRoot.booleanToVisibilityConverter.Convert(typedSender.BooleanProp, typeof(global::System.Windows.Visibility), null, null));
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "ModifyViewModel")
					{
						var value1 = typedSender.ModifyViewModel;
#line 49 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
						targetRoot.textBlock5.Text = dataRoot.ModifyViewModel is var v0 && v0 != null ? v0.Input1 : "aaa";
#line 50 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
						targetRoot.textBlock6.Text = dataRoot.ModifyViewModel is var v1 && v1 != null ? v1.Input2 : "aaa";
#line 51 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
						targetRoot.textBlock7.Text = value1?.Input1 ?? "ccc";
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "TaskProp")
					{
#line 52 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
						Set0(bindings._generatedCodeDisposed.Token);
						async void Set0(CancellationToken cancellationToken)
						{
							try
							{
								var value = await typedSender.TaskProp;
								if (!cancellationToken.IsCancellationRequested)
								{
									targetRoot.textBlock8.Text = value;
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
					if (notifyAll || e.PropertyName == "ListProp")
					{
#line 64 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
						targetRoot.listView.ItemsSource = typedSender.ListProp;
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "ArrayProp")
					{
#line 65 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
						targetRoot.listView.SetVisible(typedSender.ArrayProp.Length > 0);
						if (!notifyAll)
						{
							return;
						}
					}
				}

				MainWindow_Bindings_this TryGetBindings()
				{
					MainWindow_Bindings_this bindings = null;
					if (_bindingsWeakRef != null)
					{
						bindings = (MainWindow_Bindings_this)_bindingsWeakRef.Target;
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

	class MainWindow_DataTemplate0 : global::CompiledBindings.IGeneratedDataTemplate
	{
		private global::System.Windows.Controls.TextBlock textBlock1;
		private global::System.Windows.Controls.TextBlock textBlock2;
		global::System.Windows.Data.IValueConverter booleanToVisibilityConverter;

		public void Initialize(global::System.Windows.FrameworkElement rootElement)
		{
			textBlock1 = (global::System.Windows.Controls.TextBlock)rootElement.FindName("textBlock1");
			textBlock2 = (global::System.Windows.Controls.TextBlock)rootElement.FindName("textBlock2");

			var root = global::CompiledBindings.DataTemplateBindings.GetRoot(rootElement);
			booleanToVisibilityConverter = (global::System.Windows.Data.IValueConverter)(root?.Resources["booleanToVisibilityConverter"] ?? global::System.Windows.Application.Current.Resources["booleanToVisibilityConverter"] ?? throw new global::System.Exception("Resource 'booleanToVisibilityConverter' not found."));


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

		MainWindow_DataTemplate0_Bindings_rootElement Bindings_rootElement = new MainWindow_DataTemplate0_Bindings_rootElement();

		class MainWindow_DataTemplate0_Bindings_rootElement
		{
			MainWindow_DataTemplate0 _targetRoot;
			global::WPFTest.ViewModels.EntityViewModel _dataRoot;
			MainWindow_DataTemplate0_BindingsTrackings_rootElement _bindingsTrackings;

			public void Initialize(MainWindow_DataTemplate0 targetRoot, global::WPFTest.ViewModels.EntityViewModel dataRoot)
			{
				if (_targetRoot != null)
					throw new System.InvalidOperationException();
				if (targetRoot == null)
					throw new System.ArgumentNullException(nameof(targetRoot));
				if (dataRoot == null)
					throw new System.ArgumentNullException(nameof(dataRoot));

				_targetRoot = targetRoot;
				_dataRoot = dataRoot;
				_bindingsTrackings = new MainWindow_DataTemplate0_BindingsTrackings_rootElement(this);

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
#line 31 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
				targetRoot.textBlock1.Text = value2.SByteProp.ToString();
#line 32 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
				targetRoot.textBlock1.Visibility = ((global::System.Windows.Visibility)targetRoot.booleanToVisibilityConverter.Convert(value1, typeof(global::System.Windows.Visibility), null, null));
#line 33 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
				targetRoot.textBlock2.Text = value1.ToString();

				_bindingsTrackings.SetPropertyChangedEventHandler1(value2);
			}

			class MainWindow_DataTemplate0_BindingsTrackings_rootElement
			{
				global::System.WeakReference _bindingsWeakRef;
				global::WPFTest.ViewModels.EntityViewModel _propertyChangeSource0;
				global::WPFTest.ViewModels.EntityModel _propertyChangeSource1;

				public MainWindow_DataTemplate0_BindingsTrackings_rootElement(MainWindow_DataTemplate0_Bindings_rootElement bindings)
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
						var value1 = typedSender.Model;
#line 31 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
						targetRoot.textBlock1.Text = value1.SByteProp.ToString();
						SetPropertyChangedEventHandler1(value1);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "BooleanProp")
					{
						var value1 = typedSender.BooleanProp;
#line 32 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
						targetRoot.textBlock1.Visibility = ((global::System.Windows.Visibility)targetRoot.booleanToVisibilityConverter.Convert(value1, typeof(global::System.Windows.Visibility), null, null));
#line 33 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
						targetRoot.textBlock2.Text = value1.ToString();
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
#line 31 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\Views\MainWindow.xaml"
						targetRoot.textBlock1.Text = typedSender.SByteProp.ToString();
						if (!notifyAll)
						{
							return;
						}
					}
				}

				MainWindow_DataTemplate0_Bindings_rootElement TryGetBindings()
				{
					MainWindow_DataTemplate0_Bindings_rootElement bindings = null;
					if (_bindingsWeakRef != null)
					{
						bindings = (MainWindow_DataTemplate0_Bindings_rootElement)_bindingsWeakRef.Target;
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
