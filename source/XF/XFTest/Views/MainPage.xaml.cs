using Xamarin.Forms;
using XFTest.ViewModels;

namespace XFTest;

public partial class MainPage : ContentPage
{
	private readonly Page1ViewModel _viewModel;

	public MainPage()
	{
		InitializeComponent();

		BindingContext = _viewModel = new Page1ViewModel();
	}
}
