namespace XFTest.Views
{
	using System.Threading;
	using XFTest.Extensions;

#nullable disable

	[System.CodeDom.Compiler.GeneratedCode("CompiledBindings", null)]
	partial class Page2
	{
		private global::Xamarin.Forms.Label label1;
		private global::Xamarin.Forms.Label label2;
		private global::Xamarin.Forms.Label label3;
		private global::Xamarin.Forms.Label label4;
		private global::Xamarin.Forms.Label label5;
		private global::Xamarin.Forms.Label label6;
		private global::Xamarin.Forms.Label label7;
		private global::Xamarin.Forms.Label label8;
		private global::Xamarin.Forms.Label label9;
		private global::Xamarin.Forms.Button button1;
		global::Xamarin.Forms.IValueConverter someConverter;
		private bool _generatedCodeInitialized;

		private void InitializeAfterConstructor()
		{
			if (_generatedCodeInitialized)
				return;

			_generatedCodeInitialized = true;

			label1 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label1");
			label2 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label2");
			label3 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label3");
			label4 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label4");
			label5 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label5");
			label6 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label6");
			label7 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label7");
			label8 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label8");
			label9 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label9");
			button1 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Button>(this, "button1");

			someConverter = (global::Xamarin.Forms.IValueConverter)(this.Resources.ContainsKey("someConverter") == true ? this.Resources["someConverter"] : global::Xamarin.Forms.Application.Current.Resources["someConverter"]);

#line (28, 16) - (28, 80) 28 "Page2.xml"
			label8.Text = Test.BarcodeKeys.HU + "," + Test.BarcodeKeys.Batch;
#line (33, 17) - (33, 43) 33 "Page2.xml"
			button1.Clicked += this.OnClicked;
#line default

			this.BindingContextChanged += this_BindingContextChanged;
			if (this.BindingContext is global::XFTest.Views.Page2 dataRoot0)
			{
				Bindings_this.Initialize(this);
			}
		}

		~Page2()
		{
			if (Bindings_this != null)
			{
				Bindings_this.Cleanup();
			}
		}

		private void this_BindingContextChanged(object sender, global::System.EventArgs e)
		{
			Bindings_this.Cleanup();
			if (((global::Xamarin.Forms.Element)sender).BindingContext is global::XFTest.Views.Page2 dataRoot)
			{
				Bindings_this.Initialize(this);
			}
		}

		Page2_Bindings_this Bindings_this = new Page2_Bindings_this();

		class Page2_Bindings_this
		{
			Page2 _targetRoot;
			Page2_BindingsTrackings_this _bindingsTrackings;

			public void Initialize(Page2 dataRoot)
			{
				_targetRoot = dataRoot;
				_bindingsTrackings = new Page2_BindingsTrackings_this(this);

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
#line (30, 13) - (30, 65) 30 "Page2.xml"
				var value1 = dataRoot._viewModel.GetIcon();
#line (24, 16) - (24, 72) 24 "Page2.xml"
				_targetRoot.label4.Text = dataRoot._viewModel.CalculateString().TrimNumber();
#line (30, 13) - (30, 65) 30 "Page2.xml"
				_targetRoot.label9.FontFamily = value1.Item1;
#line (31, 13) - (31, 54) 31 "Page2.xml"
				_targetRoot.label9.Text = value1.Item2;
#line default
				Update0(dataRoot._viewModel);
				_bindingsTrackings.SetPropertyChangedEventHandler0(dataRoot._viewModel);
			}

			private void Update0(global::XFTest.ViewModels.Page2ViewModel value)
			{
				Update0_FuncProp(value.FuncProp);
				Update0_CurrentItem(value.CurrentItem);
				Update0_CurrentItem2(value.CurrentItem2);
				Update0_Group(value.Group);
				Update0_StringProp(value.StringProp);
			}

			private void Update0_CurrentItem(global::XFTest.ViewModels.ItemViewModel value)
			{
				Update1_GuidProp(value?.GuidProp ?? default);
				_bindingsTrackings.SetPropertyChangedEventHandler1(value);
			}

			private void Update0_CurrentItem2(global::XFTest.ViewModels.Item2ViewModel value)
			{
				Update2_Prop1(value?.Prop1);
				_bindingsTrackings.SetPropertyChangedEventHandler2(value);
			}

			private void Update0_StringProp(global::System.Decimal value)
			{
				var dataRoot = _targetRoot;
#line (23, 16) - (23, 141) 23 "Page2.xml"
				_targetRoot.label3.Text = ((global::System.String)_targetRoot.someConverter.Convert(value, typeof(global::System.String), dataRoot._viewModel.DecimalProp + 1, null));
#line default
			}

			private void Update0_DecimalProp(global::System.Decimal value)
			{
				var dataRoot = _targetRoot;
#line (23, 16) - (23, 141) 23 "Page2.xml"
				_targetRoot.label3.Text = ((global::System.String)_targetRoot.someConverter.Convert(dataRoot._viewModel.StringProp, typeof(global::System.String), value + 1, null));
#line default
			}

			private void Update0_FuncProp(global::System.Func<global::System.String, global::XFTest.ViewModels.ItemViewModel> value)
			{
#line (25, 16) - (25, 67) 25 "Page2.xml"
				var value1 = value?.Invoke("test");
#line default
				Update3_GuidProp(value1?.GuidProp ?? default);
				_bindingsTrackings.SetPropertyChangedEventHandler3(value1);
			}

			private void Update0_Group(global::XFTest.ViewModels.GroupViewModel value)
			{
#line (27, 16) - (27, 59) 27 "Page2.xml"
				var value1 = value?[0];
#line default
				Update4_GuidProp(value1?.GuidProp ?? default);
				_bindingsTrackings.SetPropertyChangedEventHandler4(value1);
			}

			private void Update1_GuidProp(global::System.Guid value)
			{
#line (21, 16) - (21, 62) 21 "Page2.xml"
				_targetRoot.label1.Text = value.ToString();
#line default
			}

			private void Update2_Prop1(global::System.String value)
			{
#line (22, 10) - (22, 54) 22 "Page2.xml"
				_targetRoot.label2.Text = value;
#line default
			}

			private void Update3_GuidProp(global::System.Guid value)
			{
#line (25, 16) - (25, 67) 25 "Page2.xml"
				var value1 = value.ToString();
#line (25, 16) - (25, 67) 25 "Page2.xml"
				_targetRoot.label5.Text = value1;
#line (26, 16) - (26, 72) 26 "Page2.xml"
				_targetRoot.label6.Text = value1;
#line default
			}

			private void Update4_GuidProp(global::System.Guid value)
			{
#line (27, 16) - (27, 59) 27 "Page2.xml"
				_targetRoot.label7.Text = value.ToString();
#line default
			}

			class Page2_BindingsTrackings_this
			{
				global::System.WeakReference _bindingsWeakRef;
				global::XFTest.ViewModels.Page2ViewModel _propertyChangeSource0;
				global::XFTest.ViewModels.ItemViewModel _propertyChangeSource1;
				global::XFTest.ViewModels.Item2ViewModel _propertyChangeSource2;
				global::XFTest.ViewModels.ItemViewModel _propertyChangeSource3;
				global::XFTest.ViewModels.ItemViewModel _propertyChangeSource4;

				public Page2_BindingsTrackings_this(Page2_Bindings_this bindings)
				{
					_bindingsWeakRef = new global::System.WeakReference(bindings);
				}

				public void Cleanup()
				{
					SetPropertyChangedEventHandler0(null);
					SetPropertyChangedEventHandler1(null);
					SetPropertyChangedEventHandler2(null);
					SetPropertyChangedEventHandler3(null);
					SetPropertyChangedEventHandler4(null);
				}

				public void SetPropertyChangedEventHandler0(global::XFTest.ViewModels.Page2ViewModel value)
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

				public void SetPropertyChangedEventHandler1(global::XFTest.ViewModels.ItemViewModel value)
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

				public void SetPropertyChangedEventHandler2(global::XFTest.ViewModels.Item2ViewModel value)
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

				public void SetPropertyChangedEventHandler3(global::XFTest.ViewModels.ItemViewModel value)
				{
					if (_propertyChangeSource3 != null && !object.ReferenceEquals(_propertyChangeSource3, value))
					{
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource3).PropertyChanged -= OnPropertyChanged3;
						_propertyChangeSource3 = null;
					}
					if (_propertyChangeSource3 == null && value != null)
					{
						_propertyChangeSource3 = value;
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource3).PropertyChanged += OnPropertyChanged3;
					}
				}

				public void SetPropertyChangedEventHandler4(global::XFTest.ViewModels.ItemViewModel value)
				{
					if (_propertyChangeSource4 != null && !object.ReferenceEquals(_propertyChangeSource4, value))
					{
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource4).PropertyChanged -= OnPropertyChanged4;
						_propertyChangeSource4 = null;
					}
					if (_propertyChangeSource4 == null && value != null)
					{
						_propertyChangeSource4 = value;
						((System.ComponentModel.INotifyPropertyChanged)_propertyChangeSource4).PropertyChanged += OnPropertyChanged4;
					}
				}

				private void OnPropertyChanged0(object sender, System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::XFTest.ViewModels.Page2ViewModel)sender;
					switch (e.PropertyName)
					{
						case null:
						case "":
							bindings.Update0(typedSender);
							break;
						case "CurrentItem":
							bindings.Update0_CurrentItem(typedSender.CurrentItem);
							break;
						case "CurrentItem2":
							bindings.Update0_CurrentItem2(typedSender.CurrentItem2);
							break;
						case "StringProp":
							bindings.Update0_StringProp(typedSender.StringProp);
							break;
						case "DecimalProp":
							bindings.Update0_DecimalProp(typedSender.DecimalProp);
							break;
						case "FuncProp":
							bindings.Update0_FuncProp(typedSender.FuncProp);
							break;
						case "Group":
							bindings.Update0_Group(typedSender.Group);
							break;
					}
				}

				private void OnPropertyChanged1(object sender, System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::XFTest.ViewModels.ItemViewModel)sender;
					if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "GuidProp")
					{
						bindings.Update1_GuidProp(typedSender.GuidProp);
					}
				}

				private void OnPropertyChanged2(object sender, System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::XFTest.ViewModels.Item2ViewModel)sender;
					if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "Prop1")
					{
						bindings.Update2_Prop1(typedSender.Prop1);
					}
				}

				private void OnPropertyChanged3(object sender, System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::XFTest.ViewModels.ItemViewModel)sender;
					if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "GuidProp")
					{
						bindings.Update3_GuidProp(typedSender.GuidProp);
					}
				}

				private void OnPropertyChanged4(object sender, System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::XFTest.ViewModels.ItemViewModel)sender;
					if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "GuidProp")
					{
						bindings.Update4_GuidProp(typedSender.GuidProp);
					}
				}

				Page2_Bindings_this TryGetBindings()
				{
					Page2_Bindings_this bindings = null;
					if (_bindingsWeakRef != null)
					{
						bindings = (Page2_Bindings_this)_bindingsWeakRef.Target;
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
