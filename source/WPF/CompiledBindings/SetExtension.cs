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

		//public object DesignValue { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			//if (DesignValue != null)
			//{
			//	if (propertyType.IsEnum && DesignValue is string str)
			//	{
			//		return Enum.Parse(propertyType, str);
			//	}
			//	return Convert.ChangeType(DesignValue, propertyType);
			//}

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
