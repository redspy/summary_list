using System.Windows.Controls;
using summary_list.ViewModels;
using System.Windows;

namespace summary_list.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            SizeChanged += MainView_SizeChanged;
        }

        private void MainView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.ControlWidth = ActualWidth;
                viewModel.ControlHeight = ActualHeight;
            }
        }
    }
} 