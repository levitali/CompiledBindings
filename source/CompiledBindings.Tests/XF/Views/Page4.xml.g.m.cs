namespace XFTest.Views
{
#nullable disable

	[global::System.CodeDom.Compiler.GeneratedCode("CompiledBindings", null)]
	partial class Page4
	{
		private global::Xamarin.Forms.Label label3;
		private global::Xamarin.Forms.Label label4;
		private global::Xamarin.Forms.Label label5;
		private global::Xamarin.Forms.Picker picker2;
		private global::Xamarin.Forms.Label label6;
		private global::Xamarin.Forms.Label label7;
		private global::Xamarin.Forms.Label label8;
		private global::Xamarin.Forms.Entry entry1;
		private bool _generatedCodeInitialized;

		private void InitializeAfterConstructor()
		{
			if (_generatedCodeInitialized)
				return;

			_generatedCodeInitialized = true;

			label3 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label3");
			label4 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label4");
			label5 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label5");
			picker2 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Picker>(this, "picker2");
			label6 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label6");
			label7 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label7");
			label8 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label8");
			entry1 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Entry>(this, "entry1");

#line (48, 13) - (48, 28) 48 "Page4.xml"
			label6.Text = 3.ToString();
#line default

			this.BindingContextChanged += this_BindingContextChanged;
			if (this.BindingContext is global::XFTest.ViewModels.Page3ViewModel dataRoot0)
			{
				Bindings_this.Initialize(this, dataRoot0);
			}
		}

		private void this_BindingContextChanged(object sender, global::System.EventArgs e)
		{
			Bindings_this.Cleanup();
			if (((global::Xamarin.Forms.Element)sender).BindingContext is global::XFTest.ViewModels.Page3ViewModel dataRoot)
			{
				Bindings_this.Initialize(this, dataRoot);
			}
		}

		Page4_Bindings_this Bindings_this = new Page4_Bindings_this();

		class Page4_Bindings_this
		{
			Page4 _targetRoot;
			global::XFTest.ViewModels.Page3ViewModel _dataRoot;
			Page4_BindingsTrackings_this _bindingsTrackings;
			bool _settingBinding6;

			public void Initialize(Page4 targetRoot, global::XFTest.ViewModels.Page3ViewModel dataRoot)
			{
				_targetRoot = targetRoot;
				_dataRoot = dataRoot;
				_bindingsTrackings = new Page4_BindingsTrackings_this(this);

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
				var dataRoot = _dataRoot;
#line (47, 13) - (47, 46) 47 "Page4.xml"
				_targetRoot.picker2.ItemsSource = dataRoot.PickedItems;
#line default
				Update0(dataRoot);
			}

			private void Update0(global::XFTest.ViewModels.Page3ViewModel value)
			{
				Update0_Entity(value);
				Update0_State(value);
				Update0_QuantityInput(value);
			}

			private void Update1(global::XFTest.ViewModels.EntityModel value)
			{
				Update1_SByteProp(value);
				Update1_UShortProp(value);
				Update1__field1(value);
			}

			private void Update0_Entity(global::XFTest.ViewModels.Page3ViewModel value)
			{
#line (42, 16) - (42, 64) 42 "Page4.xml"
				var value1 = value.Entity;
#line (42, 16) - (42, 64) 42 "Page4.xml"
				_targetRoot.label3.IsVisible = value1 != null && value.IsLoading;
#line default
				Update1(value1);
				_bindingsTrackings.SetPropertyChangedEventHandler1(value1);
			}

			private void Update0_IsLoading(global::XFTest.ViewModels.Page3ViewModel value)
			{
#line (42, 16) - (42, 64) 42 "Page4.xml"
				_targetRoot.label3.IsVisible = value.Entity != null && value.IsLoading;
#line default
			}

			private void Update0_State(global::XFTest.ViewModels.Page3ViewModel value)
			{
#line (49, 13) - (49, 43) 49 "Page4.xml"
				_targetRoot.label7.Text = value[1, "test"].ToString();
#line default
			}

			private void Update0_QuantityInput(global::XFTest.ViewModels.Page3ViewModel value)
			{
#line (51, 13) - (51, 74) 51 "Page4.xml"
				var value1 = $"{value.QuantityInput:0.###}";
#line default
				if (!object.Equals(_targetRoot.entry1.Text, value1))
				{
					_settingBinding6 = true;
					try
					{
#line (51, 13) - (51, 74) 51 "Page4.xml"
						_targetRoot.entry1.Text = value1;
#line default
					}
					finally
					{
						_settingBinding6 = false;
					}
				}
			}

			private void Update1_SByteProp(global::XFTest.ViewModels.EntityModel value)
			{
#line (43, 16) - (43, 47) 43 "Page4.xml"
				_targetRoot.label4.Text = value?.SByteProp.ToString();
#line default
			}

			private void Update1_UShortProp(global::XFTest.ViewModels.EntityModel value)
			{
#line (44, 16) - (44, 48) 44 "Page4.xml"
				_targetRoot.label5.Text = value?.UShortProp.ToString();
#line default
			}

			private void Update1__field1(global::XFTest.ViewModels.EntityModel value)
			{
#line (50, 13) - (50, 43) 50 "Page4.xml"
				_targetRoot.label8.Text = value?._field1.ToString();
#line default
			}

			private void OnTargetChanged0(global::System.Object p0, global::System.ComponentModel.PropertyChangedEventArgs p1)
			{
				var dataRoot = _dataRoot;
				switch (p1.PropertyName)
				{
					case "Text":
						if (!_settingBinding6)
						{
							try
							{
#line (51, 13) - (51, 74) 51 "Page4.xml"
								dataRoot.QuantityInput = _targetRoot.entry1.Text is var t6 && string.IsNullOrEmpty(t6) ? null : (global::System.Decimal?)global::System.Convert.ChangeType(t6, typeof(global::System.Decimal), null);
#line default
							}
							catch
							{
							}
						}
						break;
				}
			}

			class Page4_BindingsTrackings_this
			{
				global::System.WeakReference _bindingsWeakRef;
				global::System.ComponentModel.INotifyPropertyChanged _propertyChangeSource0;
				global::System.ComponentModel.INotifyPropertyChanged _propertyChangeSource1;

				public Page4_BindingsTrackings_this(Page4_Bindings_this bindings)
				{
					_bindingsWeakRef = new global::System.WeakReference(bindings);
				}

				public void Cleanup()
				{
					SetPropertyChangedEventHandler0(null);
					SetPropertyChangedEventHandler1(null);
				}

				public void SetPropertyChangedEventHandler0(global::XFTest.ViewModels.Page3ViewModel value)
				{
					global::CompiledBindings.XF.CompiledBindingsHelper.SetPropertyChangedEventHandler(ref _propertyChangeSource0, value, OnPropertyChanged0);
				}

				public void SetPropertyChangedEventHandler1(global::XFTest.ViewModels.EntityModel value)
				{
					global::CompiledBindings.XF.CompiledBindingsHelper.SetPropertyChangedEventHandler(ref _propertyChangeSource1, value, OnPropertyChanged1);
				}

				private void OnPropertyChanged0(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = global::CompiledBindings.XF.CompiledBindingsHelper.TryGetBindings<Page4_Bindings_this>(ref _bindingsWeakRef, Cleanup);
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::XFTest.ViewModels.Page3ViewModel)sender;
					switch (e.PropertyName)
					{
						case null:
						case "":
							bindings.Update0(typedSender);
							break;
						case "Entity":
							bindings.Update0_Entity(typedSender);
							break;
						case "IsLoading":
							bindings.Update0_IsLoading(typedSender);
							break;
						case "State":
						case "Items[]":
							bindings.Update0_State(typedSender);
							break;
						case "QuantityInput":
							bindings.Update0_QuantityInput(typedSender);
							break;
					}
				}

				private void OnPropertyChanged1(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = global::CompiledBindings.XF.CompiledBindingsHelper.TryGetBindings<Page4_Bindings_this>(ref _bindingsWeakRef, Cleanup);
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::XFTest.ViewModels.EntityModel)sender;
					switch (e.PropertyName)
					{
						case null:
						case "":
							bindings.Update1(typedSender);
							break;
						case "SByteProp":
							bindings.Update1_SByteProp(typedSender);
							break;
						case "UShortProp":
							bindings.Update1_UShortProp(typedSender);
							break;
						case "_field1":
							bindings.Update1__field1(typedSender);
							break;
					}
				}
			}
		}
	}

	class Page4_picker2_ItemDisplayBinding : global::Xamarin.Forms.Xaml.IMarkupExtension
	{
		global::Xamarin.Forms.Internals.TypedBindingBase _binding = new global::Xamarin.Forms.Internals.TypedBinding<global::XFTest.ViewModels.PickItem, global::System.String>(
			dataRoot => dataRoot == null ? (default, false) : (
#line (46, 13) - (46, 83) 46 "Page4.xml"
				dataRoot.Description,
#line default
				true),
			null,
			new[]
			{
				new global::System.Tuple<global::System.Func<global::XFTest.ViewModels.PickItem, object>, string>(dataRoot =>
#line (46, 13) - (46, 83) 46 "Page4.xml"
					dataRoot,
#line default
					"Description"),
			});

		public object ProvideValue(global::System.IServiceProvider serviceProvider)
		{
			return _binding;
		}
	}

	class Page4_DataTemplate0 : global::CompiledBindings.XF.IGeneratedDataTemplate
	{
		private global::Xamarin.Forms.Picker picker1;
		private global::Xamarin.Forms.Label label1;
		private global::Xamarin.Forms.Label label2;
		public global::Xamarin.Forms.IValueConverter inverseBooleanConverter { get; set; }
		public global::Xamarin.Forms.Color Color1 { get; set; }
		public global::Xamarin.Forms.Color Color2 { get; set; }

		public void Initialize(global::Xamarin.Forms.Element rootElement)
		{
			picker1 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Picker>(rootElement, "picker1");
			label1 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(rootElement, "label1");
			label2 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(rootElement, "label2");


			rootElement.BindingContextChanged += rootElement_BindingContextChanged;
			if (rootElement.BindingContext is global::XFTest.ViewModels.EntityViewModel dataRoot0)
			{
				Bindings_rootElement.Initialize(this, dataRoot0);
			}
		}

		public void Cleanup(global::Xamarin.Forms.Element rootElement)
		{
			rootElement.BindingContextChanged -= rootElement_BindingContextChanged;
			Bindings_rootElement.Cleanup();
		}

		private void rootElement_BindingContextChanged(object sender, global::System.EventArgs e)
		{
			Bindings_rootElement.Cleanup();
			if (((global::Xamarin.Forms.Element)sender).BindingContext is global::XFTest.ViewModels.EntityViewModel dataRoot)
			{
				Bindings_rootElement.Initialize(this, dataRoot);
			}
		}

		Page4_DataTemplate0_Bindings_rootElement Bindings_rootElement = new Page4_DataTemplate0_Bindings_rootElement();

		class Page4_DataTemplate0_Bindings_rootElement
		{
			Page4_DataTemplate0 _targetRoot;
			global::XFTest.ViewModels.EntityViewModel _dataRoot;
			Page4_DataTemplate0_BindingsTrackings_rootElement _bindingsTrackings;

			public void Initialize(Page4_DataTemplate0 targetRoot, global::XFTest.ViewModels.EntityViewModel dataRoot)
			{
				_targetRoot = targetRoot;
				_dataRoot = dataRoot;
				_bindingsTrackings = new Page4_DataTemplate0_BindingsTrackings_rootElement(this);

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
				Update0(dataRoot);
			}

			private void Update0(global::XFTest.ViewModels.EntityViewModel value)
			{
				Update0_BooleanProp(value);
				Update0_Item(value);
			}

			private void Update0_Item(global::XFTest.ViewModels.EntityViewModel value)
			{
#line (25, 9) - (25, 76) 25 "Page4.xml"
				_targetRoot.label1.Text = value[0];
#line default
			}

			private void Update0_BooleanProp(global::XFTest.ViewModels.EntityViewModel value)
			{
#line (34, 11) - (34, 94) 34 "Page4.xml"
				var value1 = value.BooleanProp;
#line (34, 11) - (34, 94) 34 "Page4.xml"
				_targetRoot.label2.IsVisible = ((global::System.Boolean)_targetRoot.inverseBooleanConverter.Convert(value1, typeof(global::System.Boolean), null, null));
#line (35, 11) - (35, 37) 35 "Page4.xml"
				_targetRoot.label2.Text = value1.ToString();
#line (36, 11) - (36, 62) 36 "Page4.xml"
				_targetRoot.label2.TextColor = (value1 ? _targetRoot.Color1 : _targetRoot.Color2);
#line default
			}

			class Page4_DataTemplate0_BindingsTrackings_rootElement
			{
				global::System.WeakReference _bindingsWeakRef;
				global::System.ComponentModel.INotifyPropertyChanged _propertyChangeSource0;

				public Page4_DataTemplate0_BindingsTrackings_rootElement(Page4_DataTemplate0_Bindings_rootElement bindings)
				{
					_bindingsWeakRef = new global::System.WeakReference(bindings);
				}

				public void Cleanup()
				{
					SetPropertyChangedEventHandler0(null);
				}

				public void SetPropertyChangedEventHandler0(global::XFTest.ViewModels.EntityViewModel value)
				{
					global::CompiledBindings.XF.CompiledBindingsHelper.SetPropertyChangedEventHandler(ref _propertyChangeSource0, value, OnPropertyChanged0);
				}

				private void OnPropertyChanged0(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = global::CompiledBindings.XF.CompiledBindingsHelper.TryGetBindings<Page4_DataTemplate0_Bindings_rootElement>(ref _bindingsWeakRef, Cleanup);
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::XFTest.ViewModels.EntityViewModel)sender;
					switch (e.PropertyName)
					{
						case null:
						case "":
							bindings.Update0(typedSender);
							break;
						case "Item":
						case "Items[]":
							bindings.Update0_Item(typedSender);
							break;
						case "BooleanProp":
							bindings.Update0_BooleanProp(typedSender);
							break;
					}
				}
			}
		}
	}

	class Page4_DataTemplate0_picker1_ItemDisplayBinding : global::Xamarin.Forms.Xaml.IMarkupExtension
	{
		global::Xamarin.Forms.Internals.TypedBindingBase _binding = new global::Xamarin.Forms.Internals.TypedBinding<global::XFTest.ViewModels.EntityModel, global::System.SByte>(
			dataRoot => dataRoot == null ? (default, false) : (
#line (23, 23) - (23, 94) 23 "Page4.xml"
				dataRoot.SByteProp,
#line default
				true),
			null,
			new[]
			{
				new global::System.Tuple<global::System.Func<global::XFTest.ViewModels.EntityModel, object>, string>(dataRoot =>
#line (23, 23) - (23, 94) 23 "Page4.xml"
					dataRoot,
#line default
					"SByteProp"),
			});

		public object ProvideValue(global::System.IServiceProvider serviceProvider)
		{
			return _binding;
		}
	}
}
