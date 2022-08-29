using XFTest.ViewModels;

#pragma warning disable 0067

namespace XFTest.Views
{
	partial class Page2 : INotifyPropertyChanged
	{
		private readonly Page2ViewModel _viewModel = new();

		public event PropertyChangedEventHandler? PropertyChanged;

		public void OnClicked(object sender, EventArgs e)
		{
		}
	}
}

namespace XFTest.Extensions
{
	public static class TestExtensions
	{
		public static string TrimNumber(this string value)
		{
			return null!;
		}
	}
}
