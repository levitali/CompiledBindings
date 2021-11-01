using System;
using System.Windows;
using System.Windows.Markup;

namespace CompiledBindings.Markup
{
	public class SetExtension : MarkupExtension
	{
		public SetExtension(string expression)
		{
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var provideValueTarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
			var propertyType = (provideValueTarget.TargetProperty as DependencyProperty)?.PropertyType;
			if (propertyType?.IsValueType == true)
			{
				return Activator.CreateInstance(propertyType);
			}
			else
			{
				return null;
			}
		}
	}
}
