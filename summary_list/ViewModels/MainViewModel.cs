using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using summary_list.Models;
using summary_list.ViewModels;

namespace summary_list.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private const int ItemWidth = 200;
        private const int ItemHeight = 40;
        private const int ItemsPerPage = 20;

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private ObservableCollection<SummaryItem> _allItems;
        private ObservableCollection<SummaryItem> _currentPageItems;
        public ObservableCollection<SummaryItem> CurrentPageItems
        {
            get { return _currentPageItems; }
            set { SetProperty(ref _currentPageItems, value); }
        }

        private int _currentPage;
        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    UpdateCurrentPage();
                    OnPropertyChanged(nameof(PageInfo));
                }
            }
        }

        private int _totalPages;
        public int TotalPages
        {
            get { return _totalPages; }
            set
            {
                if (SetProperty(ref _totalPages, value))
                {
                    OnPropertyChanged(nameof(PageInfo));
                }
            }
        }

        public string PageInfo => $"{CurrentPage + 1}/{TotalPages}";

        public ICommand MoveToNextPageCommand { get; }
        public ICommand MoveToPreviousPageCommand { get; }

        public MainViewModel()
        {
            Title = "Summary List";
            _allItems = new ObservableCollection<SummaryItem>();
            CurrentPageItems = new ObservableCollection<SummaryItem>();
            
            MoveToNextPageCommand = new RelayCommand(MoveToNextPage);
            MoveToPreviousPageCommand = new RelayCommand(MoveToPreviousPage);
            
            // Sample data with simple 3-4 word phrases
            var dummyTexts = new[]
            {
                "Red Apple Tree", "Blue Sky View", "Green Grass Field", "Yellow Sun Light",
                "Black Night Sky", "White Snow Fall", "Purple Flower Garden", "Orange Sunset View",
                "Brown Wood Table", "Gray Cloud Day", "Pink Rose Petal", "Silver Moon Light",
                "Gold Star Night", "Rainbow Color Show", "Ocean Wave Sound", "Mountain Top View",
                "River Flow Path", "Forest Tree Line", "Desert Sand Dune", "Beach Sand Castle",
                "Spring Morning Dew", "Summer Beach Day", "Autumn Leaf Fall", "Winter Snow Storm",
                "Morning Coffee Cup", "Evening Tea Time", "Night Star Light", "Day Sun Shine",
                "City Street View", "Country Road Path", "Mountain Peak Top", "Valley River Flow",
                "Garden Flower Bed", "Park Bench Seat", "Lake Water Ripple", "Sea Wave Crash",
                "Forest Tree Shade", "Desert Sand Dune", "Beach Shell Collect", "Mountain Rock Climb",
                "River Fish Swim", "Lake Duck Float", "Ocean Whale Swim", "Sea Turtle Dive",
                "Sky Bird Fly", "Cloud Shape Form", "Rain Drop Fall", "Snow Flake Drift",
                "Wind Leaf Blow", "Storm Cloud Gather", "Sun Ray Shine", "Moon Light Glow",
                "Star Twinkle Night", "Planet Orbit Path", "Galaxy Star Cluster", "Universe Space Time",
                "Earth Planet Home", "Mars Red Planet", "Jupiter Gas Giant", "Saturn Ring World",
                "Mercury Hot Planet", "Venus Cloud World", "Uranus Ice Giant", "Neptune Blue World",
                "Pluto Dwarf Planet", "Sun Star Center", "Moon Earth Satellite", "Comet Tail Trail",
                "Asteroid Belt Ring", "Meteor Shower Rain", "Black Hole Gravity", "White Dwarf Star",
                "Red Giant Star", "Blue Supergiant Star", "Neutron Star Core", "Pulsar Radio Wave",
                "Quasar Bright Core", "Supernova Explosion", "Nebula Gas Cloud", "Galaxy Spiral Arm",
                "Milky Way Home", "Andromeda Galaxy", "Triangulum Galaxy", "Large Magellanic Cloud",
                "Small Magellanic Cloud", "Sombrero Galaxy", "Whirlpool Galaxy", "Pinwheel Galaxy",
                "Cartwheel Galaxy", "Ring Galaxy", "Barred Spiral Galaxy", "Elliptical Galaxy",
                "Irregular Galaxy", "Dwarf Galaxy", "Globular Cluster", "Open Cluster",
                "Star Cluster", "Planetary Nebula", "Dark Nebula", "Emission Nebula",
                "Reflection Nebula", "Supernova Remnant", "Pulsar Wind Nebula", "Molecular Cloud"
            };

            var random = new System.Random();
            foreach (var text in dummyTexts)
            {
                _allItems.Add(new SummaryItem
                {
                    Text = text,
                    IsChecked = random.Next(2) == 1 // Randomly set checked status
                });
            }

            TotalPages = (_allItems.Count + ItemsPerPage - 1) / ItemsPerPage;
            CurrentPage = 0;
            UpdateCurrentPage();
        }

        private void UpdateCurrentPage()
        {
            CurrentPageItems.Clear();
            var startIndex = CurrentPage * ItemsPerPage;
            var items = _allItems.Skip(startIndex).Take(ItemsPerPage);
            foreach (var item in items)
            {
                CurrentPageItems.Add(item);
            }
        }

        private void MoveToNextPage()
        {
            if (CurrentPage < TotalPages - 1)
            {
                CurrentPage++;
            }
        }

        private void MoveToPreviousPage()
        {
            if (CurrentPage > 0)
            {
                CurrentPage--;
            }
        }
    }
} 