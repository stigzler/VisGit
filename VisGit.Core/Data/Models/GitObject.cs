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
    public partial class GitObject : ObservableObject
    {
        #region Properties =========================================================================================

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private ImageMoniker _icon;

        [ObservableProperty]
        private GitObjectType _type;

        #endregion End: Properties

        public GitObject(string name, ImageMoniker icon, GitObjectType gitObjectType)
        {
            Name = name;
            Icon = icon;
            Type = gitObjectType;
        }

        public static ObservableCollection<GitObject> GitObjects = new ObservableCollection<GitObject>()
        {
            new GitObject("Issue", KnownMonikers.DisableAllBreakpoints, GitObjectType.Issue),
            new GitObject("Label", KnownMonikers.SmartTag, GitObjectType.Label),
            new GitObject("Milestone", KnownMonikers.SendSignalAction, GitObjectType.Milestone),
        };
    }
}