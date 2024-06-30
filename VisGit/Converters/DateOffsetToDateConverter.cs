using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VisGit.Converters
{
    internal class DateOffsetToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) { return DateTime.Now; }

            DateTime convertedDateTime = ((DateTimeOffset)value).DateTime;
            return convertedDateTime;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTimeOffset? convertedDateTime = DateTime.SpecifyKind((DateTime)value, DateTimeKind.Local);

            return convertedDateTime;
        }
    }
}