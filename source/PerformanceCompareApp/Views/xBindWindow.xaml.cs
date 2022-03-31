using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PreformanceCompareApp.ViewModels;

namespace PreformanceCompareApp
{
    /// <summary>
    /// Interaction logic for xBindWindow.xaml
    /// </summary>
    public partial class xBindWindow : Window
    {
        public xBindWindow(StatisticsViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
