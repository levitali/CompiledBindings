namespace WPFTest.Views
{
#nullable disable

	[global::System.CodeDom.Compiler.GeneratedCode("CompiledBindings", null)]
	partial class Page2
	{
		private bool _generatedCodeInitialized;

		private void InitializeAfterConstructor()
		{
			if (_generatedCodeInitialized)
				return;

			_generatedCodeInitialized = true;


			this.DataContextChanged += this_DataContextChanged;
			if (this.DataContext is global::WPFTest.ViewModels.Page1ViewModel dataRoot0)
			{
				Bindings_this.Initialize(this, dataRoot0);
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

		Page2_Bindings_this Bindings_this = new Page2_Bindings_this();

		class Page2_Bindings_this
		{
			Page2 _targetRoot;
			global::WPFTest.ViewModels.Page1ViewModel _dataRoot;

			public void Initialize(Page2 targetRoot, global::WPFTest.ViewModels.Page1ViewModel dataRoot)
			{
				_targetRoot = targetRoot;
				_dataRoot = dataRoot;

				Update();
			}

			public void Cleanup()
			{
				if (_targetRoot != null)
				{
					_dataRoot = null;
					_targetRoot = null;
				}
			}

			public void Update()
			{
				var dataRoot = _dataRoot;
#line (15, 19) - (15, 69) 15 "Page3.xml"
				_targetRoot.dataGrid1.ItemsSource = dataRoot.ListProp;
#line default
			}
		}
	}

	class Page2_dataGridTextColumn1_Binding : global::System.Windows.Markup.MarkupExtension, global::System.Windows.Data.IValueConverter
	{
		public override object ProvideValue(global::System.IServiceProvider serviceProvider)
		{
			return new global::System.Windows.Data.Binding
			{
				Converter = this,
				Mode = global::System.Windows.Data.BindingMode.OneTime
			};
		}

		public object Convert(object value, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
		{
			if (value is not WPFTest.ViewModels.EntityViewModel dataRoot)
			{
				return global::System.Windows.DependencyProperty.UnsetValue;
			}
			return dataRoot.Title;
		}

		public object ConvertBack(object value, global::System.Type targetType, object parameter, global::System.Globalization.CultureInfo culture)
		{
			throw new global::System.NotSupportedException();
		}
	}
}
