using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace summary_list.Converters
{
    public class CheckToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked)
            {
                return isChecked ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightPink);
            }
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 