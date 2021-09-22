using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2009/xaml", "CompiledBindings.Markup")]

#nullable enable

namespace CompiledBindings.Markup
{
	[ContentProperty("Expression")]
	public class BindExtension : IMarkupExtension
	{
		public string? Expression { get; set; }

		public BindingMode Mode { get; set; }

		public string? BindBack { get; set; }

		public object? Converter { get; set; }

		public object? ConverterParameter { get; set; }

		public object? ProvideValue(IServiceProvider serviceProvider)
		{
			return null;
		}
	}

	public enum BindingMode
	{
		OneTime = 0x00,
		OneWay = 0x01,
		OneWayToSource = 0x10,
		TwoWay = 0x11,
	}
}