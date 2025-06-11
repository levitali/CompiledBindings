using System.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using WinUITest.ViewModels;

namespace WinUITest.Views;

public sealed partial class MainPage : Page, INotifyPropertyChanged
{
	public MainPage()
	{
		InitializeComponent();

		DataContext = new MainViewModel();
	}

	public int PageProp => 0;

	public event PropertyChangedEventHandler? PropertyChanged;
}
