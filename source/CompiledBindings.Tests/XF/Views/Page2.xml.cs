using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XFTest.ViewModels;

namespace XFTest.Views
{
	partial class Page2 : INotifyPropertyChanged
	{
		private readonly Page2ViewModel _viewModel = new Page2ViewModel();

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnClicked(object sender, EventArgs e)
		{ 
		}
	}
}

namespace XFTest.Extensions
{
	public static class TestExtensions
	{
		public static string TrimNumber(this string value) => null;
	}
}
