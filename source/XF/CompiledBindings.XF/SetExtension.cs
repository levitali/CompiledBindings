using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;



namespace CompiledBindings.Markup;

[ContentProperty("Expression")]
public class SetExtension : IMarkupExtension
{
	public string? Expression { get; set; }

	public object? ProvideValue(IServiceProvider serviceProvider)
	{
		return null;
	}
}
