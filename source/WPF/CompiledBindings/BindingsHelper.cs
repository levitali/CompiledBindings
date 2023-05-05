using System;
using System.ComponentModel;
using System.Windows;

namespace CompiledBindings.WPF;

public class BindingsHelper
{
	public static void SetPropertyChangedEventHandler(ref INotifyPropertyChanged? cache, INotifyPropertyChanged? source, PropertyChangedEventHandler handler)
	{
		if (cache != null && !object.ReferenceEquals(cache, source))
		{
			cache.PropertyChanged -= handler;
			cache = null;
		}
		if (cache == null && source != null)
		{
			cache = source;
			cache.PropertyChanged += handler;
		}
	}

	public static void SetPropertyChangedEventHandler(ref INotifyPropertyChanged? cache, object? source, PropertyChangedEventHandler handler)
	{
		if (cache != null && !object.ReferenceEquals(cache, source))
		{
			cache.PropertyChanged -= handler;
			cache = null;
		}
		if (cache == null && source is INotifyPropertyChanged npc)
		{
			cache = npc;
			cache.PropertyChanged += handler;
		}
	}

	public static void SetPropertyChangedEventHandler(ref DependencyObject? cache, DependencyObject? source, DependencyProperty property, Type targetType, EventHandler handler)
	{
		if (cache != null && !object.ReferenceEquals(cache, source))
		{
			DependencyPropertyDescriptor.FromProperty(property, targetType).RemoveValueChanged(cache, handler);
			cache = null;
		}
		if (cache == null && source != null)
		{
			cache = source;
			DependencyPropertyDescriptor.FromProperty(property, targetType).AddValueChanged(cache, handler);
		}
	}

	public static T? TryGetBindings<T>(ref WeakReference? bindingsWeakReference, Action cleanup)
		where T : class
	{
		T? bindings = null;
		if (bindingsWeakReference != null)
		{
			bindings = (T?)bindingsWeakReference.Target;
			if (bindings == null)
			{
				bindingsWeakReference = null;
				cleanup();
			}
		}
		return bindings;
	}

	public static readonly DependencyProperty BindingsProperty =
		DependencyProperty.RegisterAttached("Bindings", typeof(IGeneratedDataTemplate), typeof(BindingsHelper), new PropertyMetadata(BindingsChanged));

	public static IGeneratedDataTemplate GetBindings(DependencyObject @object)
	{
		return (IGeneratedDataTemplate)@object.GetValue(BindingsProperty);
	}

	public static void SetBindings(DependencyObject @object, IGeneratedDataTemplate value)
	{
		@object.SetValue(BindingsProperty, value);
	}

	static void BindingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (e.OldValue != null)
		{
			((IGeneratedDataTemplate)e.OldValue).Cleanup((FrameworkElement)d);
		}
		if (e.NewValue != null)
		{
			((IGeneratedDataTemplate)e.NewValue).Initialize((FrameworkElement)d);
		}
	}
}

public interface IGeneratedDataTemplate
{
	void Initialize(FrameworkElement rootElement);
	void Cleanup(FrameworkElement rootElement);
}
