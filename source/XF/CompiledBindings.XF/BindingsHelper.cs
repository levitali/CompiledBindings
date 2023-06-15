using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace CompiledBindings.XF;

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

	public static readonly BindableProperty BindingsProperty =
		BindableProperty.CreateAttached("Bindings", typeof(IGeneratedDataTemplate), typeof(BindingsHelper), null, propertyChanged: BindingsChanged);

	public static IGeneratedDataTemplate GetBindings(BindableObject @object)
	{
		return (IGeneratedDataTemplate)@object.GetValue(BindingsProperty);
	}

	public static void SetBindings(BindableObject @object, IGeneratedDataTemplate value)
	{
		@object.SetValue(BindingsProperty, value);
	}

	static void BindingsChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (oldValue != null)
		{
			((IGeneratedDataTemplate)oldValue).Cleanup((Element)bindable);
		}
		if (newValue != null)
		{
			((IGeneratedDataTemplate)newValue).Initialize((Element)bindable);
		}
	}
}

public interface IGeneratedDataTemplate
{
	void Initialize(Element rootElement);
	void Cleanup(Element rootElement);
}