namespace XFTest.Views
{
	using XFTest.Extensions;

#nullable disable

	[global::System.CodeDom.Compiler.GeneratedCode("CompiledBindings", null)]
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
				Update0_FuncProp(value);
				Update0_CurrentItem(value);
				Update0_CurrentItem2(value);
				Update0_Group(value);
				Update0_StringProp(value);
				Update0_CalculateString(value);
			}

			private void Update0_CurrentItem(global::XFTest.ViewModels.Page2ViewModel value)
			{
#line (21, 16) - (21, 62) 21 "Page2.xml"
				var value1 = value.CurrentItem;
#line default
				Update1_GuidProp(value1);
				_bindingsTrackings.SetPropertyChangedEventHandler1(value1);
			}

			private void Update0_CurrentItem2(global::XFTest.ViewModels.Page2ViewModel value)
			{
#line (22, 16) - (22, 60) 22 "Page2.xml"
				var value1 = value.CurrentItem2;
#line default
				Update2_Prop1(value1);
				_bindingsTrackings.SetPropertyChangedEventHandler2(value1);
			}

			private void Update0_StringProp(global::XFTest.ViewModels.Page2ViewModel value)
			{
#line (23, 16) - (23, 141) 23 "Page2.xml"
				_targetRoot.label3.Text = ((global::System.String)_targetRoot.someConverter.Convert(value.StringProp, typeof(global::System.String), value.DecimalProp + 1, null));
#line default
			}

			private void Update0_DecimalProp(global::XFTest.ViewModels.Page2ViewModel value)
			{
#line (23, 16) - (23, 141) 23 "Page2.xml"
				_targetRoot.label3.Text = ((global::System.String)_targetRoot.someConverter.Convert(value.StringProp, typeof(global::System.String), value.DecimalProp + 1, null));
#line default
			}

			private void Update0_CalculateString(global::XFTest.ViewModels.Page2ViewModel value)
			{
#line (24, 16) - (24, 73) 24 "Page2.xml"
				_targetRoot.label4.Text = value.CalculateString().TrimNumber();
#line default
			}

			private void Update0_FuncProp(global::XFTest.ViewModels.Page2ViewModel value)
			{
#line (25, 16) - (25, 67) 25 "Page2.xml"
				var value1 = value.FuncProp?.Invoke("test");
#line default
				Update3_GuidProp(value1);
				_bindingsTrackings.SetPropertyChangedEventHandler3(value1);
			}

			private void Update0_Group(global::XFTest.ViewModels.Page2ViewModel value)
			{
#line (27, 16) - (27, 59) 27 "Page2.xml"
				var value1 = value.Group?[0];
#line default
				Update4_GuidProp(value1);
				_bindingsTrackings.SetPropertyChangedEventHandler4(value1);
			}

			private void Update1_GuidProp(global::XFTest.ViewModels.ItemViewModel value)
			{
#line (21, 16) - (21, 62) 21 "Page2.xml"
				_targetRoot.label1.Text = value?.GuidProp.ToString();
#line default
			}

			private void Update2_Prop1(global::XFTest.ViewModels.Item2ViewModel value)
			{
#line (22, 16) - (22, 60) 22 "Page2.xml"
				_targetRoot.label2.Text = value?.Prop1;
#line default
			}

			private void Update3_GuidProp(global::XFTest.ViewModels.ItemViewModel value)
			{
#line (25, 16) - (25, 67) 25 "Page2.xml"
				var value1 = value?.GuidProp.ToString();
#line (25, 16) - (25, 67) 25 "Page2.xml"
				_targetRoot.label5.Text = value1;
#line (26, 16) - (26, 72) 26 "Page2.xml"
				_targetRoot.label6.Text = value1;
#line default
			}

			private void Update4_GuidProp(global::XFTest.ViewModels.ItemViewModel value)
			{
#line (27, 16) - (27, 59) 27 "Page2.xml"
				_targetRoot.label7.Text = value?.GuidProp.ToString();
#line default
			}

			class Page2_BindingsTrackings_this
			{
				global::System.WeakReference _bindingsWeakRef;
				global::System.ComponentModel.INotifyPropertyChanged _propertyChangeSource0;
				global::System.ComponentModel.INotifyPropertyChanged _propertyChangeSource1;
				global::System.ComponentModel.INotifyPropertyChanged _propertyChangeSource2;
				global::System.ComponentModel.INotifyPropertyChanged _propertyChangeSource3;
				global::System.ComponentModel.INotifyPropertyChanged _propertyChangeSource4;

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
					global::CompiledBindings.XF.CompiledBindingsHelper.SetPropertyChangedEventHandler(ref _propertyChangeSource0, value, OnPropertyChanged0);
				}

				public void SetPropertyChangedEventHandler1(global::XFTest.ViewModels.ItemViewModel value)
				{
					global::CompiledBindings.XF.CompiledBindingsHelper.SetPropertyChangedEventHandler(ref _propertyChangeSource1, value, OnPropertyChanged1);
				}

				public void SetPropertyChangedEventHandler2(global::XFTest.ViewModels.Item2ViewModel value)
				{
					global::CompiledBindings.XF.CompiledBindingsHelper.SetPropertyChangedEventHandler(ref _propertyChangeSource2, value, OnPropertyChanged2);
				}

				public void SetPropertyChangedEventHandler3(global::XFTest.ViewModels.ItemViewModel value)
				{
					global::CompiledBindings.XF.CompiledBindingsHelper.SetPropertyChangedEventHandler(ref _propertyChangeSource3, value, OnPropertyChanged3);
				}

				public void SetPropertyChangedEventHandler4(global::XFTest.ViewModels.ItemViewModel value)
				{
					global::CompiledBindings.XF.CompiledBindingsHelper.SetPropertyChangedEventHandler(ref _propertyChangeSource4, value, OnPropertyChanged4);
				}

				private void OnPropertyChanged0(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = global::CompiledBindings.XF.CompiledBindingsHelper.TryGetBindings<Page2_Bindings_this>(ref _bindingsWeakRef, Cleanup);
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
							bindings.Update0_CurrentItem(typedSender);
							break;
						case "CurrentItem2":
							bindings.Update0_CurrentItem2(typedSender);
							break;
						case "StringProp":
							bindings.Update0_StringProp(typedSender);
							break;
						case "DecimalProp":
							bindings.Update0_DecimalProp(typedSender);
							break;
						case "CalculateString":
							bindings.Update0_CalculateString(typedSender);
							break;
						case "FuncProp":
							bindings.Update0_FuncProp(typedSender);
							break;
						case "Group":
							bindings.Update0_Group(typedSender);
							break;
					}
				}

				private void OnPropertyChanged1(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = global::CompiledBindings.XF.CompiledBindingsHelper.TryGetBindings<Page2_Bindings_this>(ref _bindingsWeakRef, Cleanup);
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::XFTest.ViewModels.ItemViewModel)sender;
					if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "GuidProp")
					{
						bindings.Update1_GuidProp(typedSender);
					}
				}

				private void OnPropertyChanged2(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = global::CompiledBindings.XF.CompiledBindingsHelper.TryGetBindings<Page2_Bindings_this>(ref _bindingsWeakRef, Cleanup);
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::XFTest.ViewModels.Item2ViewModel)sender;
					if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "Prop1")
					{
						bindings.Update2_Prop1(typedSender);
					}
				}

				private void OnPropertyChanged3(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = global::CompiledBindings.XF.CompiledBindingsHelper.TryGetBindings<Page2_Bindings_this>(ref _bindingsWeakRef, Cleanup);
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::XFTest.ViewModels.ItemViewModel)sender;
					if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "GuidProp")
					{
						bindings.Update3_GuidProp(typedSender);
					}
				}

				private void OnPropertyChanged4(object sender, global::System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = global::CompiledBindings.XF.CompiledBindingsHelper.TryGetBindings<Page2_Bindings_this>(ref _bindingsWeakRef, Cleanup);
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::XFTest.ViewModels.ItemViewModel)sender;
					if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "GuidProp")
					{
						bindings.Update4_GuidProp(typedSender);
					}
				}
			}
		}
	}
}
