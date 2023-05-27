using Windows.UI.Xaml.Markup;

namespace CompiledBindings.Markup;

public class SetExtension : MarkupExtension
{
	public string? Path { get; set; }

	protected override object? ProvideValue()
	{
		return null;
	}
}
