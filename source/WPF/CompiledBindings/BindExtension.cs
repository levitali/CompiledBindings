using System;
using System.Windows;
using System.Windows.Markup;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml", "CompiledBindings.Markup")]

namespace CompiledBindings.Markup;

public class BindExtension : MarkupExtension
{
	public BindExtension()
	{
	}

	public BindExtension(string arg1)
	{
	}

	public BindExtension(string arg1, string arg2)
	{
	}

	public BindExtension(string arg1, string arg2, string arg3)
	{
	}

	public BindExtension(string arg1, string arg2, string arg3, string arg4)
	{
	}

	public BindExtension(string arg1, string arg2, string arg3, string arg4, string arg5)
	{
	}

	public BindExtension(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
	{
	}

	public BindExtension(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
	{
	}

	public BindExtension(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8)
	{
	}

	public BindExtension(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9)
	{
	}

	public BindExtension(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10)
	{
	}

	public Type? DataType { get; set; }

	public string? Path { get; set; }

	public string? BindBack { get; set; }

	public BindingMode Mode { get; set; }

	public object? Converter { get; set; }

	public object? ConverterParameter { get; set; }

	public object? FallbackValue { get; set; }

	public object? TargetNullValue { get; set; }

	public bool IsItemsSource { get; set; }

	public object? UpdateSourceTrigger { get; set; }

	public override object? ProvideValue(IServiceProvider serviceProvider)
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

public enum BindingMode
{
	OneTime = 0x00,
	OneWay = 0x01,
	OneWayToSource = 0x10,
	TwoWay = 0x11,
}


