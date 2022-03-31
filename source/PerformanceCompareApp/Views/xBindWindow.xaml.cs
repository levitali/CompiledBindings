using System.Windows;
using PreformanceCompareApp.ViewModels;

namespace PreformanceCompareApp;

public partial class xBindWindow : Window
{
    public xBindWindow(StatisticsViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}
