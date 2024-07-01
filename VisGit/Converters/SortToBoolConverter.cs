using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VisGit.Converters
{
    internal class SortToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ListSortDirection direction = (ListSortDirection)value;
            if (direction == ListSortDirection.Ascending) { return true; }
            else return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool ascending = (bool)value;
            if (ascending) { return ListSortDirection.Ascending; }
            else return ListSortDirection.Descending;
        }
    }
}