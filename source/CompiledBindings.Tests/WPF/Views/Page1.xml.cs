using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using WPFTest.ViewModels;

#nullable enable

namespace WPFTest.Views;

partial class Page1
{
	private Page1ViewModel _viewModel = new Page1ViewModel();

	private ListView listView = new ListView();

	public string? Title { get; set; }

	public Task<ImageSource> LoadImageAsync() => null!;
}

static partial class UIElementExtensions
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
