using System;
using System.Globalization;
using System.Windows.Data;

namespace summary_list.Converters
{
    public class WidthToColumnsConverter : IValueConverter
    {
        private const double ItemMinWidth = 200; // Minimum width for each item

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double width)
            {
                // Calculate how many items can fit in the available width
                int columns = Math.Max(1, (int)(width / ItemMinWidth));
                return columns;
            }
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 