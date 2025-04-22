using System.Collections.ObjectModel;
using summary_list.Models;
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

        private ObservableCollection<SummaryItem> _items;
        public ObservableCollection<SummaryItem> Items
        {
            get { return _items; }
            set { SetProperty(ref _items, value); }
        }

        public MainViewModel()
        {
            Title = "Summary List";
            Items = new ObservableCollection<SummaryItem>();
            
            // Sample data with simple 3-4 word phrases
            var dummyTexts = new[]
            {
                "Red Apple Tree",
                "Blue Sky View",
                "Green Grass Field",
                "Yellow Sun Light",
                "Black Night Sky",
                "White Snow Fall",
                "Purple Flower Garden",
                "Orange Sunset View",
                "Brown Wood Table",
                "Gray Cloud Day",
                "Pink Rose Petal",
                "Silver Moon Light",
                "Gold Star Night",
                "Rainbow Color Show",
                "Ocean Wave Sound",
                "Mountain Top View",
                "River Flow Path",
                "Forest Tree Line",
                "Desert Sand Dune",
                "Beach Sand Castle"
            };

            var random = new System.Random();
            foreach (var text in dummyTexts)
            {
                Items.Add(new SummaryItem
                {
                    Text = text,
                    IsChecked = random.Next(2) == 1 // Randomly set checked status
                });
            }
        }
    }
} 