using System.Windows;

namespace WPFTest;

internal class TestExtensions
{
	public static int GetMyProperty(DependencyObject obj)
	{
		return (int)obj.GetValue(MyPropertyProperty);
	}

	public static void SetMyProperty(DependencyObject obj, int value)
	{
		obj.SetValue(MyPropertyProperty, value);
	}

	// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty MyPropertyProperty =
		DependencyProperty.RegisterAttached("MyProperty", typeof(int), typeof(TestExtensions), new PropertyMetadata(0));
}
