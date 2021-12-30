using System.Windows;
using PreformanceCompareApp.ViewModels;

namespace PreformanceCompareApp
{

	public partial class MainWindow : Window
	{
		MainViewModel _viewModel = new MainViewModel();

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Binding_Click(object sender, RoutedEventArgs e)
		{
			var wnd = new BindingWindow(new StatisticsViewModel(_viewModel.TransferOrders));
			wnd.Show();
		}

		private void xBind_Click(object sender, RoutedEventArgs e)
		{
			var wnd = new xBindWindow(new StatisticsViewModel(_viewModel.TransferOrders));
			wnd.Show();
		}
	}
}