using System.Windows;
using PreformanceCompareApp.ViewModels;

namespace PreformanceCompareApp;

public partial class BindingWindow : Window
{
    public BindingWindow(StatisticsViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}
