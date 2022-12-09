using System;
using Microsoft.UI.Xaml.Data;

namespace WinUITest.Converters;

public class TestConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language)
	{
		return value;
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		return value;
	}
}
