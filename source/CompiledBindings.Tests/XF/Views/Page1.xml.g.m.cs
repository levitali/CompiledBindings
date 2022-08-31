namespace XFTest.Views
{
	using System.Threading;
	using UI;

#nullable disable

	[System.CodeDom.Compiler.GeneratedCode("CompiledBindings", null)]
	partial class Page1
	{
		private global::Xamarin.Forms.Label label5;
		private global::Xamarin.Forms.Label label6;
		private global::Xamarin.Forms.Label label7;
		private global::Xamarin.Forms.Label label8;
		private global::Xamarin.Forms.Label label9;
		private global::Xamarin.Forms.Label label10;
		private global::Xamarin.Forms.Entry entry1;
		private global::Xamarin.Forms.Entry entry2;
		private global::UI.PickerEx pickerEx1;
		private global::Xamarin.Forms.Button button1;
		private global::Xamarin.Forms.Label label13;
		private bool _generatedCodeInitialized;

		private void InitializeAfterConstructor()
		{
			if (_generatedCodeInitialized)
				return;

			_generatedCodeInitialized = true;

			label5 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label5");
			label6 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label6");
			label7 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label7");
			label8 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label8");
			label9 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label9");
			label10 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label10");
			entry1 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Entry>(this, "entry1");
			entry2 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Entry>(this, "entry2");
			pickerEx1 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::UI.PickerEx>(this, "pickerEx1");
			button1 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Button>(this, "button1");
			label13 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(this, "label13");

#line (13, 5) - (13, 48) 13 "Page1.xml"
			var value1 = XFTest.Strings.Instance;
#line (13, 5) - (13, 48) 13 "Page1.xml"
			Title = value1.Title;
#line (40, 16) - (40, 60) 40 "Page1.xml"
			label5.Text = value1.Header1;
#line (71, 13) - (71, 34) 71 "Page1.xml"
			button1.Clicked += (p1, p2) => this.Save();
#line default

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

				var dataRoot = _targetRoot;
				Update1_SelectedItem(dataRoot.list.SelectedItem);

				_bindingsTrackings.SetPropertyChangedEventHandler1(dataRoot.list);
			}

			private void Update0_BooleanProp(global::System.Boolean value)
			{
#line (70, 13) - (70, 92) 70 "Page1.xml"
				_targetRoot.button1.IsEnabled = value;
#line default
			}

			private void Update1_SelectedItem(global::System.Object value)
			{
#line (70, 13) - (70, 92) 70 "Page1.xml"
				var value1 = (((global::XFTest.ViewModels.EntityViewModel)value));
#line default
				Update0_BooleanProp(value1?.BooleanProp ?? default);
				_bindingsTrackings.SetPropertyChangedEventHandler0(value1);
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

					var typedSender = (global::XFTest.ViewModels.EntityViewModel)sender;
					if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "BooleanProp")
					{
						bindings.Update0_BooleanProp(typedSender.BooleanProp);
					}
				}

				private void OnPropertyChanged1(object sender, System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::Xamarin.Forms.CollectionView)sender;
					if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "SelectedItem")
					{
						bindings.Update1_SelectedItem(typedSender.SelectedItem);
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
			bool _settingBinding7;
			bool _settingBinding9;
			bool _settingBinding10;

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
				_targetRoot.entry2.PropertyChanged += OnTargetChanged1;
				_targetRoot.pickerEx1.PropertyChanged += OnTargetChanged2;
			}

			public void Cleanup()
			{
				if (_targetRoot != null)
				{
					_targetRoot.entry1.PropertyChanged -= OnTargetChanged0;
					_targetRoot.entry2.PropertyChanged -= OnTargetChanged1;
					_targetRoot.pickerEx1.PropertyChanged -= OnTargetChanged2;
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

				var dataRoot = _dataRoot;
#line (57, 13) - (57, 63) 57 "Page1.xml"
				var value1 = dataRoot.ListProp;
#line (47, 16) - (47, 54) 47 "Page1.xml"
				var value2 = value1[0];
#line (57, 13) - (57, 63) 57 "Page1.xml"
				_targetRoot.list.ItemsSource = value1;
#line default
				Update0(dataRoot);
				Update1_DecimalProp(value2.DecimalProp);

				_bindingsTrackings.SetPropertyChangedEventHandler1(value2);
			}

			private void Update0(global::XFTest.ViewModels.Page1ViewModel value)
			{
#line (42, 13) - (42, 39) 42 "Page1.xml"
				var value1 = value.DecimalProp;
#line (42, 13) - (42, 39) 42 "Page1.xml"
				_targetRoot.label6.Text = value1.ToString();
#line (44, 16) - (44, 46) 44 "Page1.xml"
				_targetRoot.label7.Text = (value1 + 1).ToString();
#line (53, 22) - (53, 68) 53 "Page1.xml"
				var value2 = value.StringProp;
#line default
				if (!object.Equals(_targetRoot.pickerEx1.SelectedItem, value2))
				{
					_settingBinding10 = true;
					try
					{
						_targetRoot.pickerEx1.SelectedItem = value2;
					}
					finally
					{
						_settingBinding10 = false;
					}
				}
#line default
				Update0_BooleanProp(value.BooleanProp);
				Update0_NullableIntProp(value.NullableIntProp);
				Update0_FocusedField(value.FocusedField);
				Update0_ArrayProp(value.ArrayProp);
			}

			private void Update0_DecimalProp(global::System.Decimal value)
			{
				var dataRoot = _dataRoot;
#line (42, 13) - (42, 39) 42 "Page1.xml"
				_targetRoot.label6.Text = value.ToString();
#line (44, 16) - (44, 46) 44 "Page1.xml"
				_targetRoot.label7.Text = (value + 1).ToString();
#line (74, 13) - (74, 138) 74 "Page1.xml"
				_targetRoot.label13.Text = $"Decimal value: {value:0.###}, Boolean value: {dataRoot.BooleanProp}, String value: {dataRoot.StringProp?.TrimStart('0')}";
#line default
			}

			private void Update0_BooleanProp(global::System.Boolean value)
			{
				var dataRoot = _dataRoot;
#line (43, 13) - (43, 58) 43 "Page1.xml"
				_targetRoot.label6.TextColor = (value ? Xamarin.Forms.Color.Green : Xamarin.Forms.Color.Red);
#line (46, 16) - (46, 46) 46 "Page1.xml"
				_targetRoot.label9.Text = (!value).ToString();
#line (50, 13) - (50, 58) 50 "Page1.xml"
				var value1 = value.ToString();
#line default
				if (!object.Equals(_targetRoot.entry1.Text, value1))
				{
					_settingBinding7 = true;
					try
					{
						_targetRoot.entry1.Text = value1;
					}
					finally
					{
						_settingBinding7 = false;
					}
				}
#line (74, 13) - (74, 138) 74 "Page1.xml"
				_targetRoot.label13.Text = $"Decimal value: {dataRoot.DecimalProp:0.###}, Boolean value: {value}, String value: {dataRoot.StringProp?.TrimStart('0')}";
#line default
			}

			private void Update0_NullableIntProp(global::System.Nullable<global::System.Int32> value)
			{
#line (45, 16) - (45, 51) 45 "Page1.xml"
				_targetRoot.label8.Text = (value ?? 0).ToString();
#line (52, 16) - (52, 59) 52 "Page1.xml"
				var value1 = value?.ToString();
#line default
				if (!object.Equals(_targetRoot.entry2.Text, value1))
				{
					_settingBinding9 = true;
					try
					{
						_targetRoot.entry2.Text = value1;
					}
					finally
					{
						_settingBinding9 = false;
					}
				}
#line (73, 13) - (73, 52) 73 "Page1.xml"
				global::Xamarin.Forms.Grid.SetRow(_targetRoot.label13, value ?? 9);
#line default
			}

			private void Update0_FocusedField(global::UI.FocusState<global::XFTest.ViewModels.Page1ViewModel.Field> value)
			{
#line (51, 13) - (51, 56) 51 "Page1.xml"
				_targetRoot.entry1.SetFocused(value[XFTest.ViewModels.Page1ViewModel.Field.Field1]);
#line default
			}

			private void Update0_StringProp(global::System.String value)
			{
				var dataRoot = _dataRoot;
				if (!object.Equals(_targetRoot.pickerEx1.SelectedItem, value))
				{
					_settingBinding10 = true;
					try
					{
						_targetRoot.pickerEx1.SelectedItem = value;
					}
					finally
					{
						_settingBinding10 = false;
					}
				}
#line (74, 13) - (74, 138) 74 "Page1.xml"
				_targetRoot.label13.Text = $"Decimal value: {dataRoot.DecimalProp:0.###}, Boolean value: {dataRoot.BooleanProp}, String value: {value?.TrimStart('0')}";
#line default
			}

			private void Update0_ArrayProp(global::System.Int32[] value)
			{
#line (56, 13) - (56, 53) 56 "Page1.xml"
				_targetRoot.list.IsVisible = value?.Length > 0;
#line default
			}

			private void Update1_DecimalProp(global::System.Decimal value)
			{
#line (47, 16) - (47, 54) 47 "Page1.xml"
				_targetRoot.label10.Text = value.ToString();
#line default
			}

			private void OnTargetChanged0(global::System.Object p0, global::System.ComponentModel.PropertyChangedEventArgs p1)
			{
				var dataRoot = _dataRoot;
				switch (p1.PropertyName)
				{
					case "IsFocused":
						try
						{
#line (49, 13) - (49, 74) 49 "Page1.xml"
							dataRoot.FocusedField[XFTest.ViewModels.Page1ViewModel.Field.Field1] = _targetRoot.entry1.IsFocused;
#line default
						}
						catch
						{
						}
						break;
					case "Text":
						if (!_settingBinding7)
						{
							try
							{
#line (50, 13) - (50, 58) 50 "Page1.xml"
								dataRoot.SetValue(_targetRoot.entry1.Text);
#line default
							}
							catch
							{
							}
						}
						break;
				}
			}

			private void OnTargetChanged1(global::System.Object p0, global::System.ComponentModel.PropertyChangedEventArgs p1)
			{
				var dataRoot = _dataRoot;
				switch (p1.PropertyName)
				{
					case "Text":
						if (!_settingBinding9)
						{
							try
							{
#line (52, 16) - (52, 59) 52 "Page1.xml"
								dataRoot.NullableIntProp = _targetRoot.entry2.Text is var t9 && string.IsNullOrEmpty(t9) ? null : (global::System.Nullable<global::System.Int32>)global::System.Convert.ChangeType(t9, typeof(global::System.Int32), null);
#line default
							}
							catch
							{
							}
						}
						break;
				}
			}

			private void OnTargetChanged2(global::System.Object p0, global::System.ComponentModel.PropertyChangedEventArgs p1)
			{
				var dataRoot = _dataRoot;
				switch (p1.PropertyName)
				{
					case "SelectedItem":
						if (!_settingBinding10)
						{
							try
							{
#line (53, 22) - (53, 68) 53 "Page1.xml"
								dataRoot.StringProp = _targetRoot.pickerEx1.SelectedItem?.ToString();
#line default
							}
							catch
							{
							}
						}
						break;
				}
			}

			class Page1_BindingsTrackings_this
			{
				global::System.WeakReference _bindingsWeakRef;
				global::XFTest.ViewModels.Page1ViewModel _propertyChangeSource0;
				global::XFTest.ViewModels.EntityViewModel _propertyChangeSource1;

				public Page1_BindingsTrackings_this(Page1_Bindings_this bindings)
				{
					_bindingsWeakRef = new global::System.WeakReference(bindings);
				}

				public void Cleanup()
				{
					SetPropertyChangedEventHandler0(null);
					SetPropertyChangedEventHandler1(null);
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

				public void SetPropertyChangedEventHandler1(global::XFTest.ViewModels.EntityViewModel value)
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

					var typedSender = (global::XFTest.ViewModels.Page1ViewModel)sender;
					switch (e.PropertyName)
					{
						case null:
						case "":
							bindings.Update0(typedSender);
							break;
						case "DecimalProp":
							bindings.Update0_DecimalProp(typedSender.DecimalProp);
							break;
						case "BooleanProp":
							bindings.Update0_BooleanProp(typedSender.BooleanProp);
							break;
						case "NullableIntProp":
							bindings.Update0_NullableIntProp(typedSender.NullableIntProp);
							break;
						case "FocusedField":
							bindings.Update0_FocusedField(typedSender.FocusedField);
							break;
						case "StringProp":
							bindings.Update0_StringProp(typedSender.StringProp);
							break;
						case "ArrayProp":
							bindings.Update0_ArrayProp(typedSender.ArrayProp);
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

					var typedSender = (global::XFTest.ViewModels.EntityViewModel)sender;
					if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "DecimalProp")
					{
						bindings.Update1_DecimalProp(typedSender.DecimalProp);
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

				var dataRoot = _dataRoot;
#line (20, 24) - (20, 54) 20 "Page1.xml"
				var value1 = dataRoot.Model;
#line default
				Update0_BooleanProp(dataRoot.BooleanProp);
				Update1_SByteProp(value1?.SByteProp ?? default);

				_bindingsTrackings.SetPropertyChangedEventHandler1(value1);
			}

			private void Update0_BooleanProp(global::System.Boolean value)
			{
#line (21, 24) - (21, 50) 21 "Page1.xml"
				_targetRoot.label2.Text = value.ToString();
#line default
			}

			private void Update1_SByteProp(global::System.SByte value)
			{
#line (20, 24) - (20, 54) 20 "Page1.xml"
				_targetRoot.label1.Text = value.ToString();
#line default
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

					var typedSender = (global::XFTest.ViewModels.EntityViewModel)sender;
					if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "BooleanProp")
					{
						bindings.Update0_BooleanProp(typedSender.BooleanProp);
					}
				}

				private void OnPropertyChanged1(object sender, System.ComponentModel.PropertyChangedEventArgs e)
				{
					var bindings = TryGetBindings();
					if (bindings == null)
					{
						return;
					}

					var typedSender = (global::XFTest.ViewModels.EntityModel)sender;
					if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "SByteProp")
					{
						bindings.Update1_SByteProp(typedSender.SByteProp);
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
		private global::Xamarin.Forms.Label label4;

		public void Initialize(global::Xamarin.Forms.Element rootElement)
		{
			label3 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(rootElement, "label3");
			label4 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(rootElement, "label4");


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

				var dataRoot = _dataRoot;
#line (31, 36) - (31, 100) 31 "Page1.xml"
				_targetRoot.label4.Text = (dataRoot as global::XFTest.ViewModels.ExtEntityViewModel)?.ExtraProp;
#line default
				Update0_BooleanProp(dataRoot.BooleanProp);
			}

			private void Update0_BooleanProp(global::System.Boolean value)
			{
#line (30, 36) - (30, 62) 30 "Page1.xml"
				_targetRoot.label3.Text = value.ToString();
#line default
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

					var typedSender = (global::XFTest.ViewModels.EntityViewModel)sender;
					if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == "BooleanProp")
					{
						bindings.Update0_BooleanProp(typedSender.BooleanProp);
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
		private global::Xamarin.Forms.Label label11;
		private global::Xamarin.Forms.Label label12;

		public void Initialize(global::Xamarin.Forms.Element rootElement)
		{
			label11 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(rootElement, "label11");
			label12 = global::Xamarin.Forms.NameScopeExtensions.FindByName<global::Xamarin.Forms.Label>(rootElement, "label12");


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

				var dataRoot = _dataRoot;
				Update0(dataRoot);
			}

			private void Update0(global::XFTest.ViewModels.EntityViewModel value)
			{
				Update0_DecimalProp(value?.DecimalProp ?? default);
				Update0_StringProp(value?.StringProp);
			}

			private void Update0_DecimalProp(global::System.Decimal value)
			{
#line (62, 32) - (62, 58) 62 "Page1.xml"
				_targetRoot.label11.Text = value.ToString();
#line default
			}

			private void Update0_StringProp(global::System.String value)
			{
#line (63, 32) - (63, 72) 63 "Page1.xml"
				_targetRoot.label12.Text = value?.TrimStart('0');
#line default
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

					var typedSender = (global::XFTest.ViewModels.EntityViewModel)sender;
					switch (e.PropertyName)
					{
						case null:
						case "":
							bindings.Update0(typedSender);
							break;
						case "DecimalProp":
							bindings.Update0_DecimalProp(typedSender.DecimalProp);
							break;
						case "StringProp":
							bindings.Update0_StringProp(typedSender.StringProp);
							break;
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
