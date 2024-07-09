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
    public partial class Filter : ObservableObject
    {
        #region Properties =========================================================================================

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private ImageMoniker _icon;

        [ObservableProperty]
        private FilterType _filterType;

        #endregion End: Properties

        public Filter(string name, ImageMoniker icon, FilterType filterType)
        {
            Name = name;
            Icon = icon;
            FilterType = filterType;
        }

        public static ObservableCollection<Filter> MilestoneFilters = new ObservableCollection<Filter>()
        {   new Filter("Open", KnownMonikers.Visible, FilterType.Open),
            new Filter("Closed", KnownMonikers.CloakOrHide, FilterType.Closed),
            new Filter("None", KnownMonikers.DeleteFilter, FilterType.None)
        };

        public static ObservableCollection<Filter> LabelFilters = new ObservableCollection<Filter>()
        {
            new Filter("None", KnownMonikers.DeleteFilter, FilterType.None)
        };

        public static ObservableCollection<Filter> IssueFilters = new ObservableCollection<Filter>()
        {   new Filter("Open", KnownMonikers.Visible, FilterType.Open),
            new Filter("Closed", KnownMonikers.CloakOrHide, FilterType.Closed),
            new Filter("None", KnownMonikers.DeleteFilter, FilterType.None)
        };
    }
}