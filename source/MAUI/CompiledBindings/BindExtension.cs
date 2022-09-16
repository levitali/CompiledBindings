[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2009/xaml", "CompiledBindings.Markup")]

namespace CompiledBindings.Markup;

[ContentProperty("Path")]
public class BindExtension : IMarkupExtension
{
	public Type? DataType { get; set; }

	public string? Path { get; set; }

	public BindingMode Mode { get; set; }

	public string? BindBack { get; set; }

	public object? Converter { get; set; }

	public object? ConverterParameter { get; set; }

	public object? FallbackValue { get; set; }

	public object? TargetNullValue { get; set; }

	public bool IsItemsSource { get; set; }

	public object? UpdateSourceTrigger { get; set; }

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
