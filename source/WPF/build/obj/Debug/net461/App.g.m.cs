//checksum 25681081 True
namespace WPFTest
{
	using System.Threading;
#nullable disable

	[System.CodeDom.Compiler.GeneratedCode("CompiledBindings", null)]
	partial class App
	{
	}

	class App_DataTemplate0 : global::CompiledBindings.IGeneratedDataTemplate
	{
		private global::System.Windows.Controls.TextBlock textBlock1;
		private global::System.Windows.Controls.TextBlock textBlock2;

		public void Initialize(global::System.Windows.FrameworkElement rootElement)
		{
			textBlock1 = (global::System.Windows.Controls.TextBlock)rootElement.FindName("textBlock1");
			textBlock2 = (global::System.Windows.Controls.TextBlock)rootElement.FindName("textBlock2");


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

		App_DataTemplate0_Bindings_rootElement Bindings_rootElement = new App_DataTemplate0_Bindings_rootElement();

		class App_DataTemplate0_Bindings_rootElement
		{
			App_DataTemplate0 _targetRoot;
			global::WPFTest.ViewModels.EntityViewModel _dataRoot;
			App_DataTemplate0_BindingsTrackings_rootElement _bindingsTrackings;

			public void Initialize(App_DataTemplate0 targetRoot, global::WPFTest.ViewModels.EntityViewModel dataRoot)
			{
				if (_targetRoot != null)
					throw new System.InvalidOperationException();
				if (targetRoot == null)
					throw new System.ArgumentNullException(nameof(targetRoot));
				if (dataRoot == null)
					throw new System.ArgumentNullException(nameof(dataRoot));

				_targetRoot = targetRoot;
				_dataRoot = dataRoot;
				_bindingsTrackings = new App_DataTemplate0_BindingsTrackings_rootElement(this);

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
				var value1 = dataRoot.Model;
#line 19 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\App.xaml"
				targetRoot.textBlock1.Text = value1.SByteProp.ToString();
#line 20 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\App.xaml"
				targetRoot.textBlock2.Text = dataRoot.BooleanProp.ToString();

				_bindingsTrackings.SetPropertyChangedEventHandler1(value1);
			}

			class App_DataTemplate0_BindingsTrackings_rootElement
			{
				global::System.WeakReference _bindingsWeakRef;
				global::WPFTest.ViewModels.EntityViewModel _propertyChangeSource0;
				global::WPFTest.ViewModels.EntityModel _propertyChangeSource1;

				public App_DataTemplate0_BindingsTrackings_rootElement(App_DataTemplate0_Bindings_rootElement bindings)
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
#line 19 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\App.xaml"
						targetRoot.textBlock1.Text = value1.SByteProp.ToString();
						SetPropertyChangedEventHandler1(value1);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "BooleanProp")
					{
#line 20 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\App.xaml"
						targetRoot.textBlock2.Text = typedSender.BooleanProp.ToString();
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
#line 19 "W:\MyProjects\CompiledBindings\source\WPF\WPFTest\App.xaml"
						targetRoot.textBlock1.Text = typedSender.SByteProp.ToString();
						if (!notifyAll)
						{
							return;
						}
					}
				}

				App_DataTemplate0_Bindings_rootElement TryGetBindings()
				{
					App_DataTemplate0_Bindings_rootElement bindings = null;
					if (_bindingsWeakRef != null)
					{
						bindings = (App_DataTemplate0_Bindings_rootElement)_bindingsWeakRef.Target;
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
