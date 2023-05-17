using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;

namespace CompiledBindings.Markup;

public class BindExtension : MarkupExtension
{
	public Type? DataType { get; set; }

	public string? Path { get; set; }

	public string? BindBack { get; set; }

	public BindingMode Mode { get; set; }

	public object? Converter { get; set; }

	public object? ConverterParameter { get; set; }

	public object? FallbackValue { get; set; }

	public object? TargetNullValue { get; set; }

	public bool IsItemsSource { get; set; }

	public object? UpdateSourceEventNames { get; set; }

	public string? StringFormat { get; set; }

	protected override object? ProvideValue(IXamlServiceProvider serviceProvider)
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


