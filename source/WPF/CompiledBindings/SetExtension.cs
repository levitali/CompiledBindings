using System;
using System.Windows;
using System.Windows.Markup;

namespace CompiledBindings.Markup;

public class SetExtension : MarkupExtension
{
	public SetExtension(string expression)
	{
	}

	public SetExtension(string arg1, string arg2)
	{
	}

	public SetExtension(string arg1, string arg2, string arg3)
	{
	}

	public SetExtension(string arg1, string arg2, string arg3, string arg4)
	{
	}

	public SetExtension(string arg1, string arg2, string arg3, string arg4, string arg5)
	{
	}

	public SetExtension(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
	{
	}

	public SetExtension(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
	{
	}

	public SetExtension(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8)
	{
	}

	public SetExtension(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9)
	{
	}

	public SetExtension(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10)
	{
	}

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

