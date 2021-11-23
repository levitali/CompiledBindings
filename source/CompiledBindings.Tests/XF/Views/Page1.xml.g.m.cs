namespace XFTest.Views
{
	using System.Threading;
	using UI;

#nullable disable

	[System.CodeDom.Compiler.GeneratedCode("CompiledBindings", null)]
	partial class Page1
	{
		private global::Xamarin.Forms.Label label4;
		private global::Xamarin.Forms.Label label5;
		private global::Xamarin.Forms.Label label6;
		private global::Xamarin.Forms.Label label7;
		private global::Xamarin.Forms.Label label8;
		private global::Xamarin.Forms.Entry entry1;
		private global::Xamarin.Forms.Button button1;
		private bool _generatedCodeInitialized;

		private void InitializeAfterConstructor()
		{
			if (_generatedCodeInitialized)
				return;

			_generatedCodeInitialized = true;

			label4 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label4");
			label5 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label5");
			label6 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label6");
			label7 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label7");
			label8 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label8");
			entry1 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Entry>(this, "entry1");
			button1 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Button>(this, "button1");

			var value1 = XFTest.Strings.Instance;
#line 0 "Page1.xml"
			Title = value1.Title;
#line 0 "Page1.xml"
			label4.Text = value1.Header1;
#line 0 "Page1.xml"
			button1.Clicked += (p1, p2) => this.Save();
#line default
#line hidden

			Bindings.Initialize(this);
			this.BindingContextChanged += this_BindingContextChanged;
			if (this.BindingContext is global::XFTest.ViewModels.Page1ViewModel dataRoot1)
			{
				Bindings_this.Initialize(this, dataRoot1);
			}
		}

		~Page1()
		{
			if (Bindings != null)
			{
				Bindings.Cleanup();
			}
			if (Bindings_this != null)
			{
				Bindings_this.Cleanup();
			}
		}

		private void this_BindingContextChanged(object sender, global::System.EventArgs e)
		{
			Bindings_this.Cleanup();
			if (((global::Xamarin.Forms.Element)sender).BindingContext is global::XFTest.ViewModels.Page1ViewModel dataRoot)
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
				var value1 = (((global::XFTest.ViewModels.EntityViewModel)dataRoot.list.SelectedItem));
#line 0 "Page1.xml"
				targetRoot.button1.IsEnabled = value1?.BooleanProp ?? default;
#line default
#line hidden

				_bindingsTrackings.SetPropertyChangedEventHandler0(value1);
				_bindingsTrackings.SetPropertyChangedEventHandler1(dataRoot.list);
			}

			class Page1_BindingsTrackings
			{
				global::System.WeakReference _bindingsWeakRef;
				global::XFTest.ViewModels.EntityViewModel _propertyChangeSource0;
				global::Xamarin.Forms.CollectionView _propertyChangeSource1;

				public Page1_BindingsTrackings(Page1_Bindings bindings)
				{
					_bindingsWeakRef = new global::System.WeakReference(bindings);
				}

				public void Cleanup()
				{
					SetPropertyChangedEventHandler0(null);
					SetPropertyChangedEventHandler1(null);
				}

				public void SetPropertyChangedEventHandler0(global::XFTest.ViewModels.EntityViewModel value)
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

				public void SetPropertyChangedEventHandler1(global::Xamarin.Forms.CollectionView value)
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
					var dataRoot = bindings._targetRoot;
					var typedSender = (global::XFTest.ViewModels.EntityViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "BooleanProp")
					{
#line 0 "Page1.xml"
						targetRoot.button1.IsEnabled = typedSender.BooleanProp;
#line default
#line hidden
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
					var dataRoot = bindings._targetRoot;
					var typedSender = (global::Xamarin.Forms.CollectionView)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "SelectedItem")
					{
						var value1 = (((global::XFTest.ViewModels.EntityViewModel)typedSender.SelectedItem));
#line 0 "Page1.xml"
						targetRoot.button1.IsEnabled = value1?.BooleanProp ?? default;
#line default
#line hidden
						SetPropertyChangedEventHandler0(value1);
						if (!notifyAll)
						{
							return;
						}
					}
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
			global::XFTest.ViewModels.Page1ViewModel _dataRoot;
			Page1_BindingsTrackings_this _bindingsTrackings;
			bool _settingBinding6;

			public void Initialize(Page1 targetRoot, global::XFTest.ViewModels.Page1ViewModel dataRoot)
			{
				if (_targetRoot != null)
					throw new System.InvalidOperationException();
				if (targetRoot == null)
					throw new System.ArgumentNullException(nameof(targetRoot));
				if (dataRoot == null)
					throw new System.ArgumentNullException(nameof(dataRoot));

				_targetRoot = targetRoot;
				_dataRoot = dataRoot;
				_bindingsTrackings = new Page1_BindingsTrackings_this(this);

				Update();

				_bindingsTrackings.SetPropertyChangedEventHandler0(dataRoot);

				_targetRoot.entry1.PropertyChanged += OnTargetChanged0;
			}

			public void Cleanup()
			{
				if (_targetRoot != null)
				{
					_targetRoot.entry1.PropertyChanged -= OnTargetChanged0;
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
				var value2 = dataRoot.DecimalProp;
#line 0 "Page1.xml"
				targetRoot.label5.Text = value2.ToString();
#line 0 "Page1.xml"
				targetRoot.label5.TextColor = (value1 ? Xamarin.Forms.Color.Green : Xamarin.Forms.Color.Red);
#line 0 "Page1.xml"
				targetRoot.label6.Text = (value2 + 1).ToString();
#line 0 "Page1.xml"
				targetRoot.label7.Text = (dataRoot.NullableIntProp ?? 0).ToString();
#line 0 "Page1.xml"
				targetRoot.label8.Text = (!value1).ToString();
				if (!_settingBinding6)
				{
#line 0 "Page1.xml"
					var value3 = value1.ToString();
					if (!object.Equals(targetRoot.entry1.Text, value3))
					{
						targetRoot.entry1.Text = value3;
					}
				}
#line 0 "Page1.xml"
				targetRoot.entry1.SetFocused(dataRoot.FocusedField[XFTest.ViewModels.Page1ViewModel.Field.Field1]);
#line 0 "Page1.xml"
				targetRoot.list.IsVisible = dataRoot.ArrayProp?.Length > 0;
#line 0 "Page1.xml"
				targetRoot.list.ItemsSource = dataRoot.ListProp;
#line default
#line hidden

			}

			private void OnTargetChanged0(global::System.Object p0, global::System.ComponentModel.PropertyChangedEventArgs p1)
			{
				var dataRoot = _dataRoot;
				var targetRoot = _targetRoot;
				switch (p1.PropertyName)
				{
					case "IsFocused":
						try
						{
							dataRoot.FocusedField[XFTest.ViewModels.Page1ViewModel.Field.Field1] = _targetRoot.entry1.IsFocused;
						}
						catch
						{
						}
						break;
					case "Text":
						if (!_settingBinding6)
						{
							_settingBinding6 = true;
							try
							{
								dataRoot.SetValue(_targetRoot.entry1.Text);
							}
							catch
							{
							}
							finally
							{
								_settingBinding6 = false;
							}
						}
						break;
				}
			}

			class Page1_BindingsTrackings_this
			{
				global::System.WeakReference _bindingsWeakRef;
				global::XFTest.ViewModels.Page1ViewModel _propertyChangeSource0;

				public Page1_BindingsTrackings_this(Page1_Bindings_this bindings)
				{
					_bindingsWeakRef = new global::System.WeakReference(bindings);
				}

				public void Cleanup()
				{
					SetPropertyChangedEventHandler0(null);
				}

				public void SetPropertyChangedEventHandler0(global::XFTest.ViewModels.Page1ViewModel value)
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
					var typedSender = (global::XFTest.ViewModels.Page1ViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "DecimalProp")
					{
						var value1 = typedSender.DecimalProp;
#line 0 "Page1.xml"
						targetRoot.label5.Text = value1.ToString();
#line 0 "Page1.xml"
						targetRoot.label6.Text = (value1 + 1).ToString();
#line default
#line hidden
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "BooleanProp")
					{
						var value1 = typedSender.BooleanProp;
#line 0 "Page1.xml"
						targetRoot.label5.TextColor = (value1 ? Xamarin.Forms.Color.Green : Xamarin.Forms.Color.Red);
#line 0 "Page1.xml"
						targetRoot.label8.Text = (!value1).ToString();
						if (!bindings._settingBinding6)
						{
#line 0 "Page1.xml"
							var value2 = value1.ToString();
							if (!object.Equals(targetRoot.entry1.Text, value2))
							{
								targetRoot.entry1.Text = value2;
							}
						}
#line default
#line hidden
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "NullableIntProp")
					{
#line 0 "Page1.xml"
						targetRoot.label7.Text = (typedSender.NullableIntProp ?? 0).ToString();
#line default
#line hidden
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "FocusedField")
					{
#line 0 "Page1.xml"
						targetRoot.entry1.SetFocused(typedSender.FocusedField[XFTest.ViewModels.Page1ViewModel.Field.Field1]);
#line default
#line hidden
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "ArrayProp")
					{
#line 0 "Page1.xml"
						targetRoot.list.IsVisible = typedSender.ArrayProp?.Length > 0;
#line default
#line hidden
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "ListProp")
					{
#line 0 "Page1.xml"
						targetRoot.list.ItemsSource = typedSender.ListProp;
#line default
#line hidden
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
		private global::Xamarin.Forms.Label label1;
		private global::Xamarin.Forms.Label label2;

		public void Initialize(global::Xamarin.Forms.Element rootElement)
		{
			label1 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(rootElement, "label1");
			label2 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(rootElement, "label2");

#line default
#line hidden

			rootElement.BindingContextChanged += rootElement_BindingContextChanged;
			if (rootElement.BindingContext is global::XFTest.ViewModels.EntityViewModel dataRoot0)
			{
				Bindings_rootElement.Initialize(this, dataRoot0);
			}
		}

		private void rootElement_BindingContextChanged(object sender, global::System.EventArgs e)
		{
			Bindings_rootElement.Cleanup();
			if (((global::Xamarin.Forms.Element)sender).BindingContext is global::XFTest.ViewModels.EntityViewModel dataRoot)
			{
				Bindings_rootElement.Initialize(this, dataRoot);
			}
		}

		Page1_DataTemplate0_Bindings_rootElement Bindings_rootElement = new Page1_DataTemplate0_Bindings_rootElement();

		class Page1_DataTemplate0_Bindings_rootElement
		{
			Page1_DataTemplate0 _targetRoot;
			global::XFTest.ViewModels.EntityViewModel _dataRoot;
			Page1_DataTemplate0_BindingsTrackings_rootElement _bindingsTrackings;

			public void Initialize(Page1_DataTemplate0 targetRoot, global::XFTest.ViewModels.EntityViewModel dataRoot)
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
				var value1 = dataRoot.Model;
#line 0 "Page1.xml"
				targetRoot.label1.Text = value1?.SByteProp.ToString();
#line 0 "Page1.xml"
				targetRoot.label2.Text = dataRoot.BooleanProp.ToString();
#line default
#line hidden

				_bindingsTrackings.SetPropertyChangedEventHandler1(value1);
			}

			class Page1_DataTemplate0_BindingsTrackings_rootElement
			{
				global::System.WeakReference _bindingsWeakRef;
				global::XFTest.ViewModels.EntityViewModel _propertyChangeSource0;
				global::XFTest.ViewModels.EntityModel _propertyChangeSource1;

				public Page1_DataTemplate0_BindingsTrackings_rootElement(Page1_DataTemplate0_Bindings_rootElement bindings)
				{
					_bindingsWeakRef = new global::System.WeakReference(bindings);
				}

				public void Cleanup()
				{
					SetPropertyChangedEventHandler0(null);
					SetPropertyChangedEventHandler1(null);
				}

				public void SetPropertyChangedEventHandler0(global::XFTest.ViewModels.EntityViewModel value)
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

				public void SetPropertyChangedEventHandler1(global::XFTest.ViewModels.EntityModel value)
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
					var typedSender = (global::XFTest.ViewModels.EntityViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "Model")
					{
						var value1 = typedSender.Model;
#line 0 "Page1.xml"
						targetRoot.label1.Text = value1?.SByteProp.ToString();
#line default
#line hidden
						SetPropertyChangedEventHandler1(value1);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "BooleanProp")
					{
#line 0 "Page1.xml"
						targetRoot.label2.Text = typedSender.BooleanProp.ToString();
#line default
#line hidden
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
					var typedSender = (global::XFTest.ViewModels.EntityModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "SByteProp")
					{
#line 0 "Page1.xml"
						targetRoot.label1.Text = typedSender.SByteProp.ToString();
#line default
#line hidden
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

	class Page1_DataTemplate1 : global::CompiledBindings.IGeneratedDataTemplate
	{
		private global::Xamarin.Forms.Label label3;

		public void Initialize(global::Xamarin.Forms.Element rootElement)
		{
			label3 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(rootElement, "label3");

#line default
#line hidden

			rootElement.BindingContextChanged += rootElement_BindingContextChanged;
			if (rootElement.BindingContext is global::XFTest.ViewModels.EntityViewModel dataRoot0)
			{
				Bindings_rootElement.Initialize(this, dataRoot0);
			}
		}

		private void rootElement_BindingContextChanged(object sender, global::System.EventArgs e)
		{
			Bindings_rootElement.Cleanup();
			if (((global::Xamarin.Forms.Element)sender).BindingContext is global::XFTest.ViewModels.EntityViewModel dataRoot)
			{
				Bindings_rootElement.Initialize(this, dataRoot);
			}
		}

		Page1_DataTemplate1_Bindings_rootElement Bindings_rootElement = new Page1_DataTemplate1_Bindings_rootElement();

		class Page1_DataTemplate1_Bindings_rootElement
		{
			Page1_DataTemplate1 _targetRoot;
			global::XFTest.ViewModels.EntityViewModel _dataRoot;
			Page1_DataTemplate1_BindingsTrackings_rootElement _bindingsTrackings;

			public void Initialize(Page1_DataTemplate1 targetRoot, global::XFTest.ViewModels.EntityViewModel dataRoot)
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
#line 0 "Page1.xml"
				targetRoot.label3.Text = dataRoot.BooleanProp.ToString();
#line default
#line hidden

			}

			class Page1_DataTemplate1_BindingsTrackings_rootElement
			{
				global::System.WeakReference _bindingsWeakRef;
				global::XFTest.ViewModels.EntityViewModel _propertyChangeSource0;

				public Page1_DataTemplate1_BindingsTrackings_rootElement(Page1_DataTemplate1_Bindings_rootElement bindings)
				{
					_bindingsWeakRef = new global::System.WeakReference(bindings);
				}

				public void Cleanup()
				{
					SetPropertyChangedEventHandler0(null);
				}

				public void SetPropertyChangedEventHandler0(global::XFTest.ViewModels.EntityViewModel value)
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
					var typedSender = (global::XFTest.ViewModels.EntityViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "BooleanProp")
					{
#line 0 "Page1.xml"
						targetRoot.label3.Text = typedSender.BooleanProp.ToString();
#line default
#line hidden
						if (!notifyAll)
						{
							return;
						}
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

	class Page1_DataTemplate2 : global::CompiledBindings.IGeneratedDataTemplate
	{
		private global::Xamarin.Forms.Label label9;

		public void Initialize(global::Xamarin.Forms.Element rootElement)
		{
			label9 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(rootElement, "label9");

#line default
#line hidden

			rootElement.BindingContextChanged += rootElement_BindingContextChanged;
			if (rootElement.BindingContext is global::XFTest.ViewModels.EntityViewModel dataRoot0)
			{
				Bindings_rootElement.Initialize(this, dataRoot0);
			}
		}

		private void rootElement_BindingContextChanged(object sender, global::System.EventArgs e)
		{
			Bindings_rootElement.Cleanup();
			if (((global::Xamarin.Forms.Element)sender).BindingContext is global::XFTest.ViewModels.EntityViewModel dataRoot)
			{
				Bindings_rootElement.Initialize(this, dataRoot);
			}
		}

		Page1_DataTemplate2_Bindings_rootElement Bindings_rootElement = new Page1_DataTemplate2_Bindings_rootElement();

		class Page1_DataTemplate2_Bindings_rootElement
		{
			Page1_DataTemplate2 _targetRoot;
			global::XFTest.ViewModels.EntityViewModel _dataRoot;
			Page1_DataTemplate2_BindingsTrackings_rootElement _bindingsTrackings;

			public void Initialize(Page1_DataTemplate2 targetRoot, global::XFTest.ViewModels.EntityViewModel dataRoot)
			{
				if (_targetRoot != null)
					throw new System.InvalidOperationException();
				if (targetRoot == null)
					throw new System.ArgumentNullException(nameof(targetRoot));
				if (dataRoot == null)
					throw new System.ArgumentNullException(nameof(dataRoot));

				_targetRoot = targetRoot;
				_dataRoot = dataRoot;
				_bindingsTrackings = new Page1_DataTemplate2_BindingsTrackings_rootElement(this);

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
#line 0 "Page1.xml"
				targetRoot.label9.Text = dataRoot.DecimalProp.ToString();
#line default
#line hidden

			}

			class Page1_DataTemplate2_BindingsTrackings_rootElement
			{
				global::System.WeakReference _bindingsWeakRef;
				global::XFTest.ViewModels.EntityViewModel _propertyChangeSource0;

				public Page1_DataTemplate2_BindingsTrackings_rootElement(Page1_DataTemplate2_Bindings_rootElement bindings)
				{
					_bindingsWeakRef = new global::System.WeakReference(bindings);
				}

				public void Cleanup()
				{
					SetPropertyChangedEventHandler0(null);
				}

				public void SetPropertyChangedEventHandler0(global::XFTest.ViewModels.EntityViewModel value)
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
					var typedSender = (global::XFTest.ViewModels.EntityViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "DecimalProp")
					{
#line 0 "Page1.xml"
						targetRoot.label9.Text = typedSender.DecimalProp.ToString();
#line default
#line hidden
						if (!notifyAll)
						{
							return;
						}
					}
				}

				Page1_DataTemplate2_Bindings_rootElement TryGetBindings()
				{
					Page1_DataTemplate2_Bindings_rootElement bindings = null;
					if (_bindingsWeakRef != null)
					{
						bindings = (Page1_DataTemplate2_Bindings_rootElement)_bindingsWeakRef.Target;
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
