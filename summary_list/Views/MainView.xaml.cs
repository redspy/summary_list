using System.Windows.Controls;
using summary_list.ViewModels;

namespace summary_list.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
} 