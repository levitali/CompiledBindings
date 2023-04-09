using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;

namespace XFTest.ViewModels;

public class SynchronizedObservableObject : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	public static bool IsUiThread =>
#if WINDOWS_UWP
		global::Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher?.HasThreadAccess == true;
#elif __ANDROID__
		Looper.MyLooper() == Looper.MainLooper;
#elif __IOS__
		NSThread.Current.IsMainThread;
#elif WPF
		System.Windows.Application.Current?.Dispatcher?.CheckAccess() == true;
#elif WIN_UI
		DispatcherQueue.GetForCurrentThread()?.HasThreadAccess == true;
#elif XAMARIN_ESSENTIALS
		MainThread.IsMainThread;
#endif

	public static bool RunOnUiThread(Action action)
	{
#if __IOS__
		UIApplication.SharedApplication.InvokeOnMainThread(action);
		return true;
#elif WINDOWS_UWP
		var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
		if (dispatcher != null)
		{
			_ = dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
			return true;
		}
		return false;
#elif __ANDROID__
		var activity = CurrentActivity;
		if (activity != null)
		{
			activity.RunOnUiThread(action);
			return true;
		}
		return false;
#elif WPF
		var dispatcher = System.Windows.Application.Current?.Dispatcher;
		if (dispatcher != null)
		{
			dispatcher.BeginInvoke(action);
			return true;
		}
		return false;
#elif WIN_UI
		var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
		if (dispatcherQueue != null)
		{
			dispatcherQueue.TryEnqueue(() => action());
			return true;
		}
		return false;
#elif XAMARIN_ESSENTIALS
		MainThread.BeginInvokeOnMainThread(action);
		return true;
#endif
	}

	protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
	{
		if (object.Equals(storage, value))
		{
			return false;
		}

		storage = value;
		OnPropertyChanged(propertyName);
		return true;
	}

	protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		var handler = PropertyChanged;
		if (handler != null)
		{
			PropertyChangedEventArgs e = new(propertyName);
			if (!IsUiThread)
			{
				_ = RunOnUiThread(() => handler(this, e));
			}
			else
			{
				handler(this, e);
			}
		}
	}
}
