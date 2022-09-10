using System.Windows;

namespace UI;

public class FocusManager
{
	public static bool GetFocused(FrameworkElement element)
	{
		return element.IsFocused;
	}

	public static void SetFocused(FrameworkElement target, bool value)
	{

	}
}
