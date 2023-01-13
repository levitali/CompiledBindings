namespace XFTest.Views
{
#nullable disable

	[global::System.CodeDom.Compiler.GeneratedCode("CompiledBindings", null)]
	partial class Page3
	{
		private global::Xamarin.Forms.Label label1;
		private global::Xamarin.Forms.Label label2;
		private global::Xamarin.Forms.Label label3;
		private global::Xamarin.Forms.Picker picker2;
		private bool _generatedCodeInitialized;

		private void InitializeAfterConstructor()
		{
			if (_generatedCodeInitialized)
				return;

			_generatedCodeInitialized = true;

			label1 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label1");
			label2 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label2");
			label3 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label3");
			picker2 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Picker>(this, "picker2");


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

		Page3_Bindings_this Bindings_this = new Page3_Bindings_this();

		class Page3_Bindings_this
		{
			Page3 _targetRoot;
			global::XFTest.ViewModels.Page3ViewModel _dataRoot;
			Page3_BindingsTrackings_this _bindingsTrackings;

			public void Initialize(Page3 targetRoot, global::XFTest.ViewModels.Page3ViewModel dataRoot)
			{
				_targetRoot = targetRoot;
				_dataRoot = dataRoot;
				_bindingsTrackings = new Page3_BindingsTrackings_this(this);

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
#line (30, 13) - (30, 46) 30 "Page3.xml"
				_targetRoot.picker2.ItemsSource = dataRoot.PickedItems;
#line default
				Update0(dataRoot);
			}

			private void Update0(global::XFTest.ViewModels.Page3ViewModel value)
			{
				Update0_Entity(value.Entity);
			}

			private void Update1(global::XFTest.ViewModels.EntityModel value)
			{
				Update1_SByteProp(value?.SByteProp);
				Update1_UShortProp(value?.UShortProp);
			}

			private void Update0_Entity(global::XFTest.ViewModels.EntityModel value)
			{
				var dataRoot = _dataRoot;
#line (25, 16) - (25, 64) 25 "Page3.xml"
				_targetRoot.label1.IsVisible = value != null && dataRoot.IsLoading;
#line default
				Update1(value);
				_bindingsTrackings.SetPropertyChangedEventHandler1(value);
			}

			private void Update0_IsLoading(global::System.Boolean value)
			{
				var dataRoot = _dataRoot;
#line (25, 16) - (25, 64) 25 "Page3.xml"
				_targetRoot.label1.IsVisible = dataRoot.Entity != null && value;
#line default
			}

			private void Update1_SByteProp(global::System.SByte? value)
			{
#line (26, 16) - (26, 47) 26 "Page3.xml"
				_targetRoot.label2.Text = value?.ToString();
#line default
			}

			private void Update1_UShortProp(global::System.UInt16? value)
			{
#line (27, 16) - (27, 48) 27 "Page3.xml"
				_targetRoot.label3.Text = value?.ToString();
#line default
			}

			class Page3_BindingsTrackings_this
			{
				global::System.WeakReference _bindingsWeakRef;
				global::XFTest.ViewModels.Page3ViewModel _propertyChangeSource0;
				global::XFTest.ViewModels.EntityModel _propertyChangeSource1;

				public Page3_BindingsTrackings_this(Page3_Bindings_this bindings)
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

				private void OnPropertyChanged0(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
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
							bindings.Update0_Entity(typedSender.Entity);
							break;
						case "IsLoading":
							bindings.Update0_IsLoading(typedSender.IsLoading);
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

					var typedSender = (global::XFTest.ViewModels.EntityModel)sender;
					switch (e.PropertyName)
					{
						case null:
						case "":
							bindings.Update1(typedSender);
							break;
						case "SByteProp":
							bindings.Update1_SByteProp(typedSender.SByteProp);
							break;
						case "UShortProp":
							bindings.Update1_UShortProp(typedSender.UShortProp);
							break;
					}
				}

				Page3_Bindings_this TryGetBindings()
				{
					Page3_Bindings_this bindings = null;
					if (_bindingsWeakRef != null)
					{
						bindings = (Page3_Bindings_this)_bindingsWeakRef.Target;
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

	class Page3_DataTemplate0_picker1_ItemDisplayBinding : global::Xamarin.Forms.Xaml.IMarkupExtension
	{
		global::Xamarin.Forms.Internals.TypedBindingBase _binding = new global::Xamarin.Forms.Internals.TypedBinding<global::XFTest.ViewModels.EntityModel, global::System.SByte>(
			dataRoot => dataRoot == null ? (default, false) : (
#line (20, 21) - (20, 92) 20 "Page3.xml"
				dataRoot.SByteProp,
#line default
				true),
			null,
			new[]
			{
				new global::System.Tuple<global::System.Func<global::XFTest.ViewModels.EntityModel, object>, string>(dataRoot =>
#line (20, 21) - (20, 92) 20 "Page3.xml"
					dataRoot,
#line default
					"SByteProp"),
			});

		public object ProvideValue(global::System.IServiceProvider serviceProvider)
		{
			return _binding;
		}
	}

	class Page3_picker2_ItemDisplayBinding : global::Xamarin.Forms.Xaml.IMarkupExtension
	{
		global::Xamarin.Forms.Internals.TypedBindingBase _binding = new global::Xamarin.Forms.Internals.TypedBinding<global::XFTest.ViewModels.PickItem, global::System.String>(
			dataRoot => dataRoot == null ? (default, false) : (
#line (29, 13) - (29, 83) 29 "Page3.xml"
				dataRoot.Description,
#line default
				true),
			null,
			new[]
			{
				new global::System.Tuple<global::System.Func<global::XFTest.ViewModels.PickItem, object>, string>(dataRoot =>
#line (29, 13) - (29, 83) 29 "Page3.xml"
					dataRoot,
#line default
					"Description"),
			});

		public object ProvideValue(global::System.IServiceProvider serviceProvider)
		{
			return _binding;
		}
	}
}
