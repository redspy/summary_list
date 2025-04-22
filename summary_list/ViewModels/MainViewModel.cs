using System.Collections.ObjectModel;
using summary_list.ViewModels;

namespace summary_list.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private ObservableCollection<string> _items;
        public ObservableCollection<string> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public MainViewModel()
        {
            Title = "Summary List";
            Items = new ObservableCollection<string>();
            
            // Sample data
            Items.Add("Item 1");
            Items.Add("Item 2");
            Items.Add("Item 3");
        }
    }
} 