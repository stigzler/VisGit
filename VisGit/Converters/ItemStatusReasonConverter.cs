using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using VisGitCore.Data;
using VisGitCore.Data.Models;

namespace VisGit.Converters
{
    internal class ItemStatusReasonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null) { return null; }
            return (((List<ItemStateReasonMap>)value).Where(i => i.Open == (bool)parameter)).ToList<ItemStateReasonMap>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}