using Microsoft.UI.Xaml.Controls;
using WinUITest.ViewModels;

namespace WinUITest.Views;

public sealed partial class MainPage : Page
{
	public MainPage()
	{
		InitializeComponent();

		DataContext = new MainViewModel();
	}
}
