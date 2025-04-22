using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using summary_list.Models;
using summary_list.ViewModels;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Threading;

namespace summary_list.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private const int ItemWidth = 200;
        private const int ItemHeight = 40;
        private const int ItemMargin = 5;
        private const int ItemPadding = 10;
        private int _itemsPerPage = 20;

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private ObservableCollection<SummaryItem> _allItems;
        private CollectionViewSource _currentPageItemsView;
        public CollectionViewSource CurrentPageItemsView
        {
            get { return _currentPageItemsView; }
            set { SetProperty(ref _currentPageItemsView, value); }
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
        public ICommand SaveCurrentPageCommand { get; }
        public ICommand SaveAllPagesCommand { get; }

        private double _controlWidth = 800;
        public double ControlWidth
        {
            get { return _controlWidth; }
            set 
            { 
                if (SetProperty(ref _controlWidth, value))
                {
                    UpdateItemsPerPage();
                }
            }
        }

        private double _controlHeight = 450;
        public double ControlHeight
        {
            get { return _controlHeight; }
            set 
            { 
                if (SetProperty(ref _controlHeight, value))
                {
                    UpdateItemsPerPage();
                }
            }
        }

        public MainViewModel()
        {
            Title = "Summary List";
            _allItems = new ObservableCollection<SummaryItem>();
            _currentPageItemsView = new CollectionViewSource();
            _currentPageItemsView.Source = new ObservableCollection<SummaryItem>();
            _currentPageItemsView.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
            
            MoveToNextPageCommand = new RelayCommand(MoveToNextPage);
            MoveToPreviousPageCommand = new RelayCommand(MoveToPreviousPage);
            SaveCurrentPageCommand = new RelayCommand(SaveCurrentPage);
            SaveAllPagesCommand = new RelayCommand(SaveAllPages);
            
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
            for (int i = 0; i < dummyTexts.Length; i++)
            {
                _allItems.Add(new SummaryItem
                {
                    Text = dummyTexts[i],
                    Group = i < dummyTexts.Length / 2 ? "Group One" : "Group Two",
                    IsChecked = random.Next(2) == 1 // Randomly set checked status
                });
            }

            TotalPages = (_allItems.Count + _itemsPerPage - 1) / _itemsPerPage;
            CurrentPage = 0;
            UpdateCurrentPage();
        }

        private void UpdateItemsPerPage()
        {
            if (ControlWidth <= 0 || ControlHeight <= 0) return;

            // Calculate how many items can fit in a row
            int itemsPerRow = Math.Max(1, (int)((ControlWidth - 20) / (ItemWidth + ItemMargin * 2)));
            
            // Calculate how many rows can fit in the available height
            // Account for title (24px + 10px margin), bottom controls (30px + 10px margin), and group headers (20px + 25px margin)
            int availableHeight = (int)ControlHeight - 100; // 100 for title and bottom controls
            int rowsPerPage = Math.Max(1, (availableHeight - 45) / (ItemHeight + ItemMargin * 2)); // 45 for group header height and margins
            
            _itemsPerPage = itemsPerRow * rowsPerPage;
            TotalPages = (_allItems.Count + _itemsPerPage - 1) / _itemsPerPage;
            
            // Update current page to ensure it's within bounds
            if (CurrentPage >= TotalPages)
            {
                CurrentPage = Math.Max(0, TotalPages - 1);
            }
            
            UpdateCurrentPage();
        }

        private void UpdateCurrentPage()
        {
            var currentPageItems = new ObservableCollection<SummaryItem>();
            var startIndex = CurrentPage * _itemsPerPage;
            var items = _allItems.Skip(startIndex).Take(_itemsPerPage);
            foreach (var item in items)
            {
                currentPageItems.Add(item);
            }
            _currentPageItemsView.Source = currentPageItems;
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

        private void SaveCurrentPage()
        {
            try
            {
                // Get the main window
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow == null) return;

                // Find the MainView control
                var mainView = FindVisualChild<Views.MainView>(mainWindow);
                if (mainView == null) return;

                // Find the ItemsControl in MainView
                var mainItemsControl = FindVisualChild<ItemsControl>(mainView);
                if (mainItemsControl == null) return;

                // Create a bitmap of the control
                var bitmap = new RenderTargetBitmap(
                    (int)mainItemsControl.ActualWidth,
                    (int)mainItemsControl.ActualHeight,
                    96,
                    96,
                    PixelFormats.Pbgra32);

                // Render the control directly
                bitmap.Render(mainItemsControl);

                // Save the bitmap to a file
                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                var fileName = $"{Guid.NewGuid()}.bmp";
                using (var stream = File.Create(fileName))
                {
                    encoder.Save(stream);
                }

                MessageBox.Show($"File saved as: {fileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveAllPages()
        {
            try
            {
                int originalPage = CurrentPage;
                var savedFiles = new List<string>();

                // Get the main window
                var mainWindow = Application.Current.MainWindow;
                if (mainWindow == null) return;

                // Find the MainView control
                var mainView = FindVisualChild<Views.MainView>(mainWindow);
                if (mainView == null) return;

                // Find the ItemsControl in MainView
                var mainItemsControl = FindVisualChild<ItemsControl>(mainView);
                if (mainItemsControl == null) return;

                // Save each page
                for (int i = 0; i < TotalPages; i++)
                {
                    // Navigate to the page
                    CurrentPage = i;
                    UpdateCurrentPage();

                    // Wait for the UI to update
                    Application.Current.Dispatcher.Invoke(() => { }, DispatcherPriority.Render);

                    // Create a bitmap of the control
                    var bitmap = new RenderTargetBitmap(
                        (int)mainItemsControl.ActualWidth,
                        (int)mainItemsControl.ActualHeight,
                        96,
                        96,
                        PixelFormats.Pbgra32);

                    // Render the control directly
                    bitmap.Render(mainItemsControl);

                    // Save the bitmap to a file
                    var encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));

                    var fileName = $"Page_{i + 1}_{Guid.NewGuid()}.bmp";
                    using (var stream = File.Create(fileName))
                    {
                        encoder.Save(stream);
                    }
                    savedFiles.Add(fileName);
                }

                // Restore original page
                CurrentPage = originalPage;
                UpdateCurrentPage();

                MessageBox.Show($"Saved {TotalPages} files:\n{string.Join("\n", savedFiles)}", 
                              "Success", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving files: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                {
                    return typedChild;
                }

                var result = FindVisualChild<T>(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
} 