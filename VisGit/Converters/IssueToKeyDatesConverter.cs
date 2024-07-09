using Microsoft.VisualStudio.Shell.FindResults;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using VisGitCore.ViewModels;

namespace VisGit.Converters
{
    internal class IssueToKeyDatesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null; // Todo: check this once Issues fully authored - may be redundant

            IssueViewModel issueVm = (IssueViewModel)value;
            DateTimeOffset opened = issueVm.GitIssue.CreatedAt;
            DateTimeOffset? updated = issueVm.GitIssue.UpdatedAt;
            DateTimeOffset? closed = issueVm.ClosedAt;

            string dates = null;
            dates += $"Opened: {opened.ToString("MMMM dd, yyyy")}";

            if (closed != null)
            {
                dates += $"  Closed: {((DateTimeOffset)closed).ToString("MMMM dd, yyyy")} by {issueVm.ClosedBy.Login}";
            }
            else if (updated != null)
            {
                dates += $"  Updated: {((DateTimeOffset)updated).ToString("MMMM dd, yyyy")}";
            }

            return dates;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}