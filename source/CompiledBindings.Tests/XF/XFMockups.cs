using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using Xamarin.Forms;

[assembly: XmlnsDefinition("http://xamarin.com/schemas/2014/forms", "Xamarin.Forms")]

namespace Xamarin.Forms
{
	public class VisualElement : INotifyPropertyChanged
	{
		public bool IsVisible { get; set; }
		public bool IsEnabled { get; set; }

		public event EventHandler Focused;
		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class Page : VisualElement
	{
		public string Title { get; set; }
	}

	public class Label : VisualElement
	{
		public string Text { get; set; }
		public string FontFamily { get; set; }
		public Color TextColor { get; set; }
	}

	public class Entry : VisualElement
	{
		public string Text { get; set; }

		public bool IsFocused { get; }
	}

	public class Button : VisualElement
	{
		public event EventHandler? Clicked;
	}

	public class StackLayout : VisualElement
	{
	}

	public class CollectionView : VisualElement
	{
		public IEnumerable ItemsSource { get; set; }
		public DataTemplate ItemTemplate { get; set; }
		public object SelectedItem { get; set; }
	}

	public sealed class XmlnsDefinitionAttribute : Attribute
	{
		public XmlnsDefinitionAttribute(string xmlNamespace, string clrNamespace)
		{
			XmlNamespace = xmlNamespace;
			ClrNamespace = clrNamespace;
		}

		public string XmlNamespace { get; }
		public string ClrNamespace { get; }
		public string AssemblyName { get; set; }
	}

	public class DataTemplate
	{
	}

	public interface IValueConverter
	{
		object Convert(object value, Type targetType, object parameter, CultureInfo culture);
		object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
	}

	public struct Color
	{
		public static Color Red { get; } = new Color();
		public static Color Green { get; } = new Color();
	}
}

namespace UI
{
	public static class Extensions
	{
		public static void SetFocused(this VisualElement control, bool isFocused)
		{
		}
	}
}


