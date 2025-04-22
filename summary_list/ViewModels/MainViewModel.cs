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
using System.Xml.Serialization;

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

        private ObservableCollection<ProtocolViewModel> _protocols;
        public ObservableCollection<ProtocolViewModel> Protocols
        {
            get { return _protocols; }
            set { SetProperty(ref _protocols, value); }
        }

        private ObservableCollection<SummaryItem> _allItems;

        private ProtocolViewModel _currentProtocol;
        public ProtocolViewModel CurrentProtocol
        {
            get { return _currentProtocol; }
            set
            {
                if (SetProperty(ref _currentProtocol, value))
                {
                    UpdateCurrentPage();
                }
            }
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
            Protocols = new ObservableCollection<ProtocolViewModel>();
            _allItems = new ObservableCollection<SummaryItem>();
            
            MoveToNextPageCommand = new RelayCommand(MoveToNextPage);
            MoveToPreviousPageCommand = new RelayCommand(MoveToPreviousPage);
            SaveCurrentPageCommand = new RelayCommand(SaveCurrentPage);
            SaveAllPagesCommand = new RelayCommand(SaveAllPages);
            
            LoadProtocolData();
        }

        private void LoadProtocolData()
        {
            try
            {
                string dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "dummy_data.xml");
                
                if (!File.Exists(dataPath))
                {
                    MessageBox.Show($"Data file not found at: {dataPath}\nPlease ensure the file exists in the correct location.", 
                                  "Error", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Error);
                    return;
                }

                var serializer = new XmlSerializer(typeof(Protocols));
                using (var reader = new StreamReader(dataPath))
                {
                    var protocols = (Protocols)serializer.Deserialize(reader);
                    
                    if (protocols == null || protocols.ProtocolList == null || protocols.ProtocolList.Count == 0)
                    {
                        MessageBox.Show("Invalid protocol data format. Please check the XML file structure.", 
                                      "Error", 
                                      MessageBoxButton.OK, 
                                      MessageBoxImage.Error);
                        return;
                    }

                    Protocols.Clear();

                    foreach (var protocol in protocols.ProtocolList)
                    {
                        var protocolViewModel = new ProtocolViewModel
                        {
                            Name = protocol.Name,
                            Groups = new ObservableCollection<GroupViewModel>()
                        };

                        foreach (var group in protocol.Groups)
                        {
                            if (group.Items == null || group.Items.Count == 0)
                            {
                                continue;
                            }

                            var groupViewModel = new GroupViewModel(group.Name)
                            {
                                Items = new ObservableCollection<SummaryItem>(group.Items.OrderBy(item => item.Text))
                            };

                            protocolViewModel.Groups.Add(groupViewModel);
                        }

                        Protocols.Add(protocolViewModel);
                    }
                }

                if (Protocols.Count == 0)
                {
                    MessageBox.Show("No valid protocols found in the data.", 
                                  "Warning", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Warning);
                    return;
                }

                CurrentProtocol = Protocols.First();
                TotalPages = Protocols.Count;
                CurrentPage = 0;
                UpdateCurrentPage();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("The data file could not be found. Please ensure the file exists in the correct location.", 
                              "Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Error deserializing XML data: {ex.Message}\nPlease check the XML file format.", 
                              "Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred while loading protocol data: {ex.Message}", 
                              "Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
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
            
            // Update total pages based on current protocol's items
            if (CurrentProtocol != null && CurrentProtocol.Groups != null)
            {
                int totalItems = CurrentProtocol.Groups.Sum(g => g.Items.Count);
                TotalPages = (totalItems + _itemsPerPage - 1) / _itemsPerPage;
            }
            
            // Update current page to ensure it's within bounds
            if (CurrentPage >= TotalPages)
            {
                CurrentPage = Math.Max(0, TotalPages - 1);
            }
            
            UpdateCurrentPage();
        }

        private void UpdateCurrentPage()
        {
            if (CurrentProtocol == null || CurrentProtocol.Groups == null)
            {
                return;
            }

            CurrentProtocol = Protocols[CurrentPage];
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
            double currentY = 0;
            RenderTargetBitmap bitmap = null;
            try
            {
                if (CurrentProtocol == null)
                {
                    return;
                }

                var visual = new DrawingVisual();
                using (var drawingContext = visual.RenderOpen())
                {
                    // Draw protocol title
                    var titleText = new FormattedText(
                        CurrentProtocol.Name,
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"),
                        24,
                        Brushes.Black,
                        96);
                    drawingContext.DrawText(titleText, new Point(10, 10));

                    currentY = 50; // Start after title
                    foreach (var group in CurrentProtocol.Groups)
                    {
                        // Draw group separator
                        var separatorText = new FormattedText(
                            group.Name,
                            System.Globalization.CultureInfo.CurrentCulture,
                            FlowDirection.LeftToRight,
                            new Typeface("Arial"),
                            18,
                            Brushes.Black,
                            96);
                        drawingContext.DrawText(separatorText, new Point(10, currentY));
                        currentY += 30;

                        // Draw items
                        double x = 10;
                        foreach (var item in group.Items)
                        {
                            if (x + ItemWidth > 800) // Page width
                            {
                                x = 10;
                                currentY += ItemHeight + ItemMargin;
                            }

                            // Draw item background
                            drawingContext.DrawRectangle(
                                new SolidColorBrush(item.IsChecked ? Colors.LightGreen : Colors.White),
                                new Pen(Brushes.Black, 1),
                                new Rect(x, currentY, ItemWidth, ItemHeight));

                            // Draw check symbol
                            var symbolText = new FormattedText(
                                item.CheckSymbol,
                                System.Globalization.CultureInfo.CurrentCulture,
                                FlowDirection.LeftToRight,
                                new Typeface("Arial"),
                                20,
                                Brushes.Black,
                                96);
                            drawingContext.DrawText(symbolText, new Point(x + 5, currentY + 5));

                            // Draw item text
                            var itemText = new FormattedText(
                                item.Text,
                                System.Globalization.CultureInfo.CurrentCulture,
                                FlowDirection.LeftToRight,
                                new Typeface("Arial"),
                                12,
                                Brushes.Black,
                                96);
                            drawingContext.DrawText(itemText, new Point(x + 30, currentY + 10));

                            x += ItemWidth + ItemMargin;
                        }
                        currentY += ItemHeight + ItemMargin + 20; // Add extra space after group
                    }
                }

                // Create bitmap
                bitmap = new RenderTargetBitmap(800, (int)currentY + 20, 96, 96, PixelFormats.Pbgra32);
                bitmap.Render(visual);

                // Save bitmap
                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                var fileName = $"{CurrentProtocol.Name.Replace(" ", "_")}.bmp";
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
            finally
            {
                if (bitmap != null)
                {
                    bitmap.Freeze();
                }
            }
        }

        private void SaveAllPages()
        {
            try
            {
                foreach (var protocol in Protocols)
                {
                    CurrentProtocol = protocol;
                    SaveCurrentPage();
                }
                MessageBox.Show("All pages saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving files: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 