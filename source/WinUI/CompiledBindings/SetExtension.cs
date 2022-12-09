#nullable enable

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;

namespace CompiledBindings.Markup;

public class SetExtension : MarkupExtension
{
	public string? Path { get; set; }

	protected override object? ProvideValue(IXamlServiceProvider serviceProvider)
	{
		return null;
	}
}
