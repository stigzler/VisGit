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

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private DateTimeOffset? _dueOn;

        public string IssuesStatus
        {
            get => $"{Services.Math.Percentage(GitMilestone.OpenIssues, GitMilestone.OpenIssues + GitMilestone.ClosedIssues).ToString()}% complete " +
                $"{GitMilestone.OpenIssues} open {GitMilestone.ClosedIssues} closed";
        }

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
            Description = milestone.Description;
            DueOn = milestone.DueOn;

            GitMilestone = milestone;
        }
    }
}