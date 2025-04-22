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
            CurrentPageItems = new ObservableCollection<SummaryItem>();
            
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
            foreach (var text in dummyTexts)
            {
                _allItems.Add(new SummaryItem
                {
                    Text = text,
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
            int availableHeight = (int)ControlHeight - 100; // 100 for title and bottom controls
            int rowsPerPage = Math.Max(1, availableHeight / (ItemHeight + ItemMargin * 2));
            
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
            CurrentPageItems.Clear();
            var startIndex = CurrentPage * _itemsPerPage;
            var items = _allItems.Skip(startIndex).Take(_itemsPerPage);
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

        private void SaveCurrentPage()
        {
            try
            {
                // Calculate how many items can fit in a row
                int itemsPerRow = Math.Max(1, (int)((ControlWidth - 20) / (ItemWidth + ItemMargin * 2)));
                
                // Calculate total height needed
                int totalRows = (int)Math.Ceiling((double)CurrentPageItems.Count / itemsPerRow);
                int totalHeight = totalRows * (ItemHeight + ItemMargin * 2) + 50; // 50 for title and bottom margin

                // Ensure the height is at least the control height
                totalHeight = Math.Max(totalHeight, (int)ControlHeight);

                // Create a visual for the current page
                var visual = new DrawingVisual();
                using (var drawingContext = visual.RenderOpen())
                {
                    // Create a dark background
                    drawingContext.DrawRectangle(new SolidColorBrush(Color.FromRgb(30, 30, 30)), null, new Rect(0, 0, ControlWidth, totalHeight));

                    // Draw title
                    var titleText = new FormattedText(
                        $"Page {CurrentPage + 1}",
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"),
                        16,
                        Brushes.White,
                        96);
                    drawingContext.DrawText(titleText, new Point(10, 10));

                    // Draw each item
                    int currentRow = 0;
                    int currentColumn = 0;
                    foreach (var item in CurrentPageItems)
                    {
                        double x = currentColumn * (ItemWidth + ItemMargin * 2) + ItemMargin;
                        double y = currentRow * (ItemHeight + ItemMargin * 2) + 40; // 40 for title and top margin

                        // Draw item background with dark theme color
                        drawingContext.DrawRectangle(
                            new SolidColorBrush(Color.FromRgb(45, 45, 45)),
                            new Pen(new SolidColorBrush(Color.FromRgb(62, 62, 62)), 1),
                            new Rect(x, y, ItemWidth, ItemHeight));

                        // Draw check symbol and text
                        var symbolText = new FormattedText(
                            item.CheckSymbol,
                            System.Globalization.CultureInfo.CurrentCulture,
                            FlowDirection.LeftToRight,
                            new Typeface("Arial"),
                            20,
                            item.IsChecked ? Brushes.Green : Brushes.Red,
                            96);
                        drawingContext.DrawText(symbolText, new Point(x + ItemPadding, y + ItemPadding));

                        var itemText = new FormattedText(
                            item.Text,
                            System.Globalization.CultureInfo.CurrentCulture,
                            FlowDirection.LeftToRight,
                            new Typeface("Arial"),
                            12,
                            Brushes.White,
                            96);
                        drawingContext.DrawText(itemText, new Point(x + ItemPadding + 30, y + ItemPadding + 4));

                        currentColumn++;
                        if (currentColumn >= itemsPerRow)
                        {
                            currentColumn = 0;
                            currentRow++;
                        }
                    }
                }

                // Create a bitmap
                var bitmap = new RenderTargetBitmap((int)ControlWidth, totalHeight, 96, 96, PixelFormats.Pbgra32);
                bitmap.Render(visual);

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

                // Save each page
                for (int i = 0; i < TotalPages; i++)
                {
                    CurrentPage = i;
                    UpdateCurrentPage();

                    // Calculate how many items can fit in a row
                    int itemsPerRow = Math.Max(1, (int)((ControlWidth - 20) / (ItemWidth + ItemMargin * 2)));
                    
                    // Calculate total height needed
                    int totalRows = (int)Math.Ceiling((double)CurrentPageItems.Count / itemsPerRow);
                    int totalHeight = totalRows * (ItemHeight + ItemMargin * 2) + 50; // 50 for title and bottom margin

                    // Ensure the height is at least the control height
                    totalHeight = Math.Max(totalHeight, (int)ControlHeight);

                    // Create a visual for the current page
                    var visual = new DrawingVisual();
                    using (var drawingContext = visual.RenderOpen())
                    {
                        // Create a dark background
                        drawingContext.DrawRectangle(new SolidColorBrush(Color.FromRgb(30, 30, 30)), null, new Rect(0, 0, ControlWidth, totalHeight));

                        // Draw title
                        var titleText = new FormattedText(
                            $"Page {CurrentPage + 1} of {TotalPages}",
                            System.Globalization.CultureInfo.CurrentCulture,
                            FlowDirection.LeftToRight,
                            new Typeface("Arial"),
                            16,
                            Brushes.White,
                            96);
                        drawingContext.DrawText(titleText, new Point(10, 10));

                        // Draw each item
                        int currentRow = 0;
                        int currentColumn = 0;
                        foreach (var item in CurrentPageItems)
                        {
                            double x = currentColumn * (ItemWidth + ItemMargin * 2) + ItemMargin;
                            double y = currentRow * (ItemHeight + ItemMargin * 2) + 40; // 40 for title and top margin

                            // Draw item background with dark theme color
                            drawingContext.DrawRectangle(
                                new SolidColorBrush(Color.FromRgb(45, 45, 45)),
                                new Pen(new SolidColorBrush(Color.FromRgb(62, 62, 62)), 1),
                                new Rect(x, y, ItemWidth, ItemHeight));

                            // Draw check symbol and text
                            var symbolText = new FormattedText(
                                item.CheckSymbol,
                                System.Globalization.CultureInfo.CurrentCulture,
                                FlowDirection.LeftToRight,
                                new Typeface("Arial"),
                                20,
                                item.IsChecked ? Brushes.Green : Brushes.Red,
                                96);
                            drawingContext.DrawText(symbolText, new Point(x + ItemPadding, y + ItemPadding));

                            var itemText = new FormattedText(
                                item.Text,
                                System.Globalization.CultureInfo.CurrentCulture,
                                FlowDirection.LeftToRight,
                                new Typeface("Arial"),
                                12,
                                Brushes.White,
                                96);
                            drawingContext.DrawText(itemText, new Point(x + ItemPadding + 30, y + ItemPadding + 4));

                            currentColumn++;
                            if (currentColumn >= itemsPerRow)
                            {
                                currentColumn = 0;
                                currentRow++;
                            }
                        }
                    }

                    // Create a bitmap
                    var bitmap = new RenderTargetBitmap((int)ControlWidth, totalHeight, 96, 96, PixelFormats.Pbgra32);
                    bitmap.Render(visual);

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
    }
} 