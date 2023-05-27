using UWPTest.ViewModels;
using Windows.UI.Xaml.Controls;

namespace UWPTest;

public sealed partial class MainPage : Page
{
	public MainPage()
	{
		InitializeComponent();

		DataContext = new MainViewModel();
	}
}
