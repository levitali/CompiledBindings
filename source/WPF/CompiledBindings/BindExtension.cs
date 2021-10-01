using System;
using System.Windows;
using System.Windows.Markup;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml", "CompiledBindings.Markup")]

namespace CompiledBindings.Markup
{
	public class BindExtension : MarkupExtension
	{
		public BindExtension(string path)
		{
		}

		public string Path { get; set; }

		public string BindBack { get; set; }

		public BindingMode Mode { get; set; }

		public object Converter { get; set; }

		public object ConverterParameter { get; set; }

		//public object DesignValue { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			var provideValueTarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
			var propertyType = (provideValueTarget.TargetProperty as DependencyProperty)?.PropertyType;

			//if (DesignValue != null)
			//{
			//	if (propertyType.IsEnum && DesignValue is string str)
			//	{
			//		return Enum.Parse(propertyType, str);
			//	}
			//	return Convert.ChangeType(DesignValue, propertyType);
			//}

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

	public enum BindingMode
	{
		OneTime = 0x00,
		OneWay = 0x01,
		OneWayToSource = 0x10,
		TwoWay = 0x11,
	}
}

