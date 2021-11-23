namespace CompiledBindings
{
	using System.Windows;

	public class DataTemplateBindings
	{
		public static readonly DependencyProperty BindingsProperty =
			DependencyProperty.RegisterAttached("Bindings", typeof(IGeneratedDataTemplate), typeof(DataTemplateBindings), new PropertyMetadata(BindingsChanged));

		public static IGeneratedDataTemplate GetBindings(DependencyObject @object)
		{
			return (IGeneratedDataTemplate)@object.GetValue(BindingsProperty);
		}

		public static void SetBindings(DependencyObject @object, IGeneratedDataTemplate value)
		{
			@object.SetValue(BindingsProperty, value);
		}

		static void BindingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((IGeneratedDataTemplate)e.NewValue).Initialize((FrameworkElement)d);
		}

		public static readonly DependencyProperty RootProperty =
			DependencyProperty.RegisterAttached("Root", typeof(FrameworkElement), typeof(DataTemplateBindings), new PropertyMetadata(null));

		public static FrameworkElement GetRoot(DependencyObject @object)
		{
			return (FrameworkElement)@object.GetValue(RootProperty);
		}

		public static void SetRoot(DependencyObject @object, FrameworkElement value)
		{
			@object.SetValue(RootProperty, value);
		}
	}

	public interface IGeneratedDataTemplate
	{
		void Initialize(FrameworkElement rootElement);
	}
}