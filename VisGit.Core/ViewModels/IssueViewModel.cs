using Community.VisualStudio.Toolkit;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.VCProjectEngine;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using VisGitCore.Controllers;
using VisGitCore.Helpers;

namespace VisGitCore.ViewModels
{
    public partial class IssueViewModel : ViewModelBase
    {
        #region Properties =========================================================================================

        // Model related ==============================================================================================

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private bool _open;

        [ObservableProperty]
        private User _closedBy;

        [ObservableProperty]
        private DateTimeOffset? _closedAt;

        [ObservableProperty]
        private string _body;

        [ObservableProperty]
        private ObservableCollection<Label> _labels = new ObservableCollection<Label>();

        [ObservableProperty]
        private Milestone _milestone;

        [ObservableProperty]
        private User _author;

        [ObservableProperty]
        private ObservableCollection<User> _assignees = new ObservableCollection<User>();

        // View Related ==============================================================================================

        public string IssuesStatus
        {
            get
            {
                if (Milestone != null)
                {
                    double percentageComplete = PercentageComplete;
                    return $"{percentageComplete.ToString()}% complete " + $"{Milestone.OpenIssues} open {Milestone.ClosedIssues} closed";
                }
                return null;
            }
        }

        public double PercentageComplete
        {
            get
            {
                if (Milestone != null)
                    return Services.Math.Percentage(Milestone.ClosedIssues, Milestone.OpenIssues + Milestone.ClosedIssues);
                return 0;
            }
        }

        public string MilestoneTitle
        {
            get
            {
                if (GitIssue.Milestone != null) return GitIssue.Milestone.Title;
                return null;
            }
        }

        // Operational ==============================================================================================

        [ObservableProperty]
        private bool _hasChanges;

        public Issue GitIssue;

        #endregion End: Properties ---------------------------------------------------------------------------------

        #region Operational Vars =========================================================================================

        internal GitController gitController;
        internal long RepositoryId;

        #endregion End: Operational Vars ---------------------------------------------------------------------------------

        #region Property Events =========================================================================================

        partial void OnOpenChanged(bool value)
        {
            if (!Open)
            {
                ClosedBy = gitController.User;
                ClosedAt = DateTime.Now.ToDateTimeOffset();
            }
            else
            {
                ClosedBy = null;
                ClosedAt = null;
            }
            // ToDo: Do I need to change any respective milestone? i.e. -1 from open issues and +1 to closed issues?
            // May have to refactor MilestoneVM as these numbers are gained form the GitMilestone, not an individ property
        }

        #endregion End: Property Events ---------------------------------------------------------------------------------

        #region Public Methods =========================================================================================

        // Constructors ==============================================================================================

        internal IssueViewModel(GitController gitController, Issue issue, long repositoryId)
        {
            this.gitController = gitController;
            RepositoryId = repositoryId;
            _ = UpdateViewmodelPropertiesAsync(issue);
        }

        #endregion End: Public Methods ---------------------------------------------------------------------------------

        #region Private Methods =========================================================================================

        private async Task UpdateViewmodelPropertiesAsync(Issue issue)
        {
            GitIssue = issue;

            _title = GitIssue.Title;
            _closedBy = GitIssue.ClosedBy;
            _closedAt = GitIssue.ClosedAt;
            _body = GitIssue.Body;
            _milestone = GitIssue.Milestone;
            _author = GitIssue.User;
            _assignees = new ObservableCollection<User>(GitIssue.Assignees);

            _labels.Clear();
            foreach (var label in GitIssue.Labels)
                _labels.Add(label);

            if (GitIssue.State == ItemState.Open) _open = true;
            else _open = false;
        }
    }

    #endregion End: Private Methods ---------------------------------------------------------------------------------
}