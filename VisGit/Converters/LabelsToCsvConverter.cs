using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Octokit;

namespace VisGit.Converters
{
    internal class LabelsToCsvConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string csv = "";

            List<Label> labels = ((ObservableCollection<Label>)value).ToList();
            foreach (Label label in labels)
            {
                csv += label.Name + " | ";
            }

            if (csv.Length > 1) csv = csv.Substring(0, csv.Length - 2);

            return csv;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}