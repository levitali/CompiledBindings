using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using WPFTest.ViewModels;

namespace WPFTest.Views;

internal partial class Page1
{
	private readonly Page1ViewModel _viewModel = new Page1ViewModel();

	private readonly ListView listView = new ListView();

	public string? Title { get; set; }

	public Task<ImageSource> LoadImageAsync()
	{
		return null!;
	}
}

internal static partial class UIElementExtensions
{
	public static void SetVisible(this UIElement element, bool isVisible) { }
}

public class InverseBooleanConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}

public static class XamlUtils
{
	public static Visibility TrueToVisible(bool? value, bool hide = false)
	{
		return default;
	}
}

