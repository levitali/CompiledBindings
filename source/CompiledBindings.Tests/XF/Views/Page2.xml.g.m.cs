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
			button1 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Button>(this, "button1");

			someConverter = (global::Xamarin.Forms.IValueConverter)(this.Resources.ContainsKey("someConverter") == true ? this.Resources["someConverter"] : global::Xamarin.Forms.Application.Current.Resources["someConverter"]);

#line (27, 16) - (27, 80) 27 "Page2.xml"
			label7.Text = Test.BarcodeKeys.HU + "," + Test.BarcodeKeys.Batch;
#line (32, 17) - (32, 43) 32 "Page2.xml"
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
				if (_targetRoot != null)
					throw new System.InvalidOperationException();
				if (dataRoot == null)
					throw new System.ArgumentNullException(nameof(dataRoot));

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
				if (_targetRoot == null)
				{
					throw new System.InvalidOperationException();
				}

				var targetRoot = _targetRoot;
				var dataRoot = _targetRoot;
#line (24, 16) - (24, 67) 24 "Page2.xml"
				var value1 = dataRoot._viewModel.FuncProp?.Invoke("test");
#line (21, 16) - (21, 62) 21 "Page2.xml"
				var value2 = dataRoot._viewModel.CurrentItem;
#line (29, 13) - (29, 65) 29 "Page2.xml"
				var value3 = dataRoot._viewModel.GetIcon();
#line (26, 16) - (26, 59) 26 "Page2.xml"
				var value4 = dataRoot._viewModel.Group?[0];
#line (24, 16) - (24, 67) 24 "Page2.xml"
				var value5 = value1?.GuidProp.ToString();
#line (21, 16) - (21, 62) 21 "Page2.xml"
				targetRoot.label1.Text = value2?.GuidProp.ToString();
#line (22, 16) - (22, 141) 22 "Page2.xml"
				targetRoot.label2.Text = ((global::System.String)targetRoot.someConverter.Convert(dataRoot._viewModel.StringProp, typeof(global::System.String), dataRoot._viewModel.DecimalProp + 1, null));
#line (23, 16) - (23, 72) 23 "Page2.xml"
				targetRoot.label3.Text = dataRoot._viewModel.CalculateString().TrimNumber();
#line (24, 16) - (24, 67) 24 "Page2.xml"
				targetRoot.label4.Text = value5;
#line (25, 16) - (25, 72) 25 "Page2.xml"
				targetRoot.label5.Text = value5;
#line (26, 16) - (26, 59) 26 "Page2.xml"
				targetRoot.label6.Text = value4?.GuidProp.ToString();
#line (29, 13) - (29, 65) 29 "Page2.xml"
				targetRoot.label8.FontFamily = value3.Item1;
#line (30, 13) - (30, 54) 30 "Page2.xml"
				targetRoot.label8.Text = value3.Item2;
#line default

				_bindingsTrackings.SetPropertyChangedEventHandler0(dataRoot._viewModel);
				_bindingsTrackings.SetPropertyChangedEventHandler1(value2);
				_bindingsTrackings.SetPropertyChangedEventHandler2(value1);
				_bindingsTrackings.SetPropertyChangedEventHandler3(value4);
			}

			private void Update0_CurrentItem(global::XFTest.ViewModels.ItemViewModel value)
			{
				var dataRoot = _targetRoot;
#line default
				Update1_GuidProp(value?.GuidProp ?? default);
				_bindingsTrackings.SetPropertyChangedEventHandler1(value);
			}

			private void Update0_StringProp(global::System.Decimal value)
			{
				var dataRoot = _targetRoot;
#line (22, 16) - (22, 141) 22 "Page2.xml"
				_targetRoot.label2.Text = ((global::System.String)targetRoot.someConverter.Convert(value, typeof(global::System.String), dataRoot._viewModel.DecimalProp + 1, null));
#line default
			}

			private void Update0_DecimalProp(global::System.Decimal value)
			{
				var dataRoot = _targetRoot;
#line (22, 16) - (22, 141) 22 "Page2.xml"
				_targetRoot.label2.Text = ((global::System.String)targetRoot.someConverter.Convert(dataRoot._viewModel.StringProp, typeof(global::System.String), value + 1, null));
#line default
			}

			private void Update0_FuncProp(global::System.Func<global::System.String, global::XFTest.ViewModels.ItemViewModel> value)
			{
				var dataRoot = _targetRoot;
#line default
				Update2_GuidProp(value?.Invoke("test")?.GuidProp ?? default);
				_bindingsTrackings.SetPropertyChangedEventHandler2(value?.Invoke("test"));
			}

			private void Update0_Group(global::XFTest.ViewModels.GroupViewModel value)
			{
				var dataRoot = _targetRoot;
#line default
				Update3_GuidProp(value?[0]?.GuidProp ?? default);
				_bindingsTrackings.SetPropertyChangedEventHandler3(value?[0]);
			}

			private void Update1_GuidProp(global::System.Guid value)
			{
				var dataRoot = _targetRoot;
#line (21, 16) - (21, 62) 21 "Page2.xml"
				_targetRoot.label1.Text = value.ToString();
#line default
			}

			private void Update2_GuidProp(global::System.Guid value)
			{
				var dataRoot = _targetRoot;
#line (24, 16) - (24, 67) 24 "Page2.xml"
				var value1 = value.ToString();
#line (24, 16) - (24, 67) 24 "Page2.xml"
				_targetRoot.label4.Text = value1;
#line (25, 16) - (25, 72) 25 "Page2.xml"
				_targetRoot.label5.Text = value1;
#line default
			}

			private void Update3_GuidProp(global::System.Guid value)
			{
				var dataRoot = _targetRoot;
#line (26, 16) - (26, 59) 26 "Page2.xml"
				_targetRoot.label6.Text = value.ToString();
#line default
			}

			class Page2_BindingsTrackings_this
			{
				global::System.WeakReference _bindingsWeakRef;
				global::XFTest.ViewModels.Page2ViewModel _propertyChangeSource0;
				global::XFTest.ViewModels.ItemViewModel _propertyChangeSource1;
				global::XFTest.ViewModels.ItemViewModel _propertyChangeSource2;
				global::XFTest.ViewModels.ItemViewModel _propertyChangeSource3;

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

				public void SetPropertyChangedEventHandler2(global::XFTest.ViewModels.ItemViewModel value)
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

				private void OnPropertyChanged0(object sender, System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::XFTest.ViewModels.Page2ViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "CurrentItem")
					{
						bindings.Update0_CurrentItem(typedSender.CurrentItem);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "StringProp")
					{
						bindings.Update0_StringProp(typedSender.StringProp);
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
					if (notifyAll || e.PropertyName == "FuncProp")
					{
						bindings.Update0_FuncProp(typedSender.FuncProp);
						if (!notifyAll)
						{
							return;
						}
					}
					if (notifyAll || e.PropertyName == "Group")
					{
						bindings.Update0_Group(typedSender.Group);
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
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "GuidProp")
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

					var typedSender = (global::XFTest.ViewModels.ItemViewModel)sender;
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "GuidProp")
					{
						bindings.Update2_GuidProp(typedSender.GuidProp);
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
					var notifyAll = string.IsNullOrEmpty(e.PropertyName);

					if (notifyAll || e.PropertyName == "GuidProp")
					{
						bindings.Update3_GuidProp(typedSender.GuidProp);
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
