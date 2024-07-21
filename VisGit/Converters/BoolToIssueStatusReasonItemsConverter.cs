using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VisGit.Converters
{
    internal class BoolToIssueStatusReasonItemsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value) return new ObservableCollection<ItemStateReason?>() { null, ItemStateReason.Completed, ItemStateReason.NotPlanned };
            else return new ObservableCollection<ItemStateReason?>() { null, ItemStateReason.Reopened };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}