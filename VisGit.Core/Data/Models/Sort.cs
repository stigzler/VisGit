using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisGitCore.Data.Models
{
    public partial class Sort : ObservableObject
    {
        #region Properties =========================================================================================

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private ImageMoniker _icon;

        [ObservableProperty]
        private SortType _sortType;

        #endregion End: Properties

        public Sort(string name, ImageMoniker icon, SortType sortType)
        {
            Name = name;
            Icon = icon;
            SortType = sortType;
        }

        public static ObservableCollection<Sort> MilestoneSorts = new ObservableCollection<Sort>()
        {
            new Sort("Due By", KnownMonikers.Calendar, SortType.DueDate),
            new Sort("Open", KnownMonikers.Visible, SortType.Open),
            new Sort("Alphabetically", KnownMonikers.FilterAlphabetically, SortType.None),
            new Sort("Recently Updated", KnownMonikers.Time, SortType.RecentlyUpdated),
            new Sort("Open Issues", KnownMonikers.DisableAllBreakpoints, SortType.OpenIssues),
            new Sort("None", KnownMonikers.ClearSort, SortType.None),
        };

        public static ObservableCollection<Sort> LabelSorts = new ObservableCollection<Sort>()
        {
            new Sort("Alphabetically", KnownMonikers.FilterAlphabetically, SortType.None),
            new Sort("Open Issues", KnownMonikers.DisableAllBreakpoints, SortType.OpenIssues),
            new Sort("None", KnownMonikers.ClearSort, SortType.None)
        };
    }
}