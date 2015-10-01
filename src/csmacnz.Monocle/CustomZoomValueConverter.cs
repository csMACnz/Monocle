using System;
using System.Globalization;
using System.Windows.Data;

namespace csmacnz.Monocle
{
    public class CustomZoomValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var scaleValue = Math.Pow(2, (double)value);
            return $"{scaleValue:P1}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
