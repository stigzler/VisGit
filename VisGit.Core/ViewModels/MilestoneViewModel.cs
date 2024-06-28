using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace VisGitCore.ViewModels
{
    public partial class MilestoneViewModel : ViewModelBase
    {
        #region PROPERTIES =========================================================================================

        // Fields
        [ObservableProperty]
        private Milestone _gitMilestone;

        // Obs Props
        [ObservableProperty]
        private string _title;

        #endregion End: PROPERTIES

        public MilestoneViewModel()
        {
        }

        public MilestoneViewModel(Milestone milestone)
        {
            UpdateProperties(milestone);
        }

        private void UpdateProperties(Milestone milestone)
        {
            Title = milestone.Title;
            GitMilestone = milestone;
        }
    }
}