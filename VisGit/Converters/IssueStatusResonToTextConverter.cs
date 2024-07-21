using Octokit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VisGit.Converters
{
    internal class IssueStatusReasonToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StringEnum<ItemStateReason>? stringEnum = (StringEnum<ItemStateReason>?)value;
            if (stringEnum == null) return "Open";
            else if (stringEnum.Value == ItemStateReason.NotPlanned) return "Not Planned";
            else if (stringEnum.Value == ItemStateReason.Completed) return "Completed";
            else if (stringEnum.Value == ItemStateReason.Reopened) return "Reopened";
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}