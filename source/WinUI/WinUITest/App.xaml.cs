using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using WinUITest.Views;

namespace WinUITest;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
	{
		var rootFrame = new Frame();

		var window = new Window
		{
			Content = rootFrame
		};
		window.Activate();

		rootFrame.Navigate(typeof(MainPage));
	}
}
