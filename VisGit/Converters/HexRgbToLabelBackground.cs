using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace VisGit.Converters
{
    internal class HexRgbToLabelBackground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //string r = value.ToString().Substring(0, 2);
            //string g = value.ToString().Substring(2, 2);
            //string b = value.ToString().Substring(4, 2);

            string colorString = "#40" + value;
            Color color = (Color)ColorConverter.ConvertFromString(colorString);

            SolidColorBrush brush = new SolidColorBrush(color);

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}