using System.Threading.Tasks;
using Xamarin.Forms;

namespace XFTest;

public class FocusManager
{
	public static readonly BindableProperty FocusedProperty =
		BindableProperty.CreateAttached("Focused", typeof(bool), typeof(FocusManager), false);

	private static VisualElement? _elementToFocus;

	public static bool GetFocused(BindableObject target)
	{
		return (target as VisualElement)?.IsFocused == true;
	}

	public static async void SetFocused(BindableObject target, bool value)
	{
		if (value && target is VisualElement visual && _elementToFocus != visual)
		{
			_elementToFocus = visual;
			while (true)
			{
				for (int i = 0; i < 10; i++)
				{
					if (_elementToFocus != visual)
					{
						return;
					}
					if (visual.Focus())
					{
						_elementToFocus = null;
						return;
					}
					await Task.Yield();
				}
				await Task.Delay(100);
			}
		}
	}
}