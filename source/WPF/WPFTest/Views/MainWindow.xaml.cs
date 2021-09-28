using System.Windows;
using System.Windows.Controls;
using WPFTest.ViewModels;

namespace WPFTest.Views
{
	public partial class MainWindow : Window
	{
		MainViewModel _viewModel;

		public MainWindow()
		{
			InitializeComponent();

			DataContext = _viewModel = new MainViewModel();
		}
	}
}

namespace WPFTest
{
	static partial class UIElementExtensions
	{
		public static void SetVisible(this UIElement element, bool isVisible) { }
	}
}