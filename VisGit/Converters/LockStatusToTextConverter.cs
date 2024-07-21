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
    internal class LockStatusToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            StringEnum<LockReason>? stringEnum = (StringEnum<LockReason>?)value;
            if (stringEnum == null) return "Not Locked";
            else if (stringEnum.Value == LockReason.OffTopic) return "Off Topic";
            else if (stringEnum.Value == LockReason.Resolved) return "Resolved";
            else if (stringEnum.Value == LockReason.Spam) return "Spam";
            else if (stringEnum.Value == LockReason.TooHeated) return "Too Heated";
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}