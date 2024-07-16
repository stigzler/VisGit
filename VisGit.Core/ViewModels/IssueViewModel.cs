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
using VisGitCore.Comparers;
using VisGitCore.Controllers;
using VisGitCore.Helpers;

namespace VisGitCore.ViewModels
{
    public partial class IssueViewModel : ViewModelBase
    {
        #region Properties =========================================================================================

        // Model related ==============================================================================================

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasChanges))]
        private string _title;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasChanges))]
        private bool _open;

        [ObservableProperty]
        private User _closedBy;

        [ObservableProperty]
        private DateTimeOffset? _closedAt;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasChanges))]
        private string _body;

        [ObservableProperty]
        private ObservableCollection<Label> _labels = new ObservableCollection<Label>();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasChanges))]
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

        //[ObservableProperty]
        public bool HasChanges => ChangesMade();

        //public bool HasChanges
        //{
        //    get { return _hasChanges; }
        //    set { _hasChanges = ChangesMade(); }
        //}

        public Issue GitIssue;

        #endregion End: Properties ---------------------------------------------------------------------------------

        #region Operational Vars =========================================================================================

        internal GitController gitController;
        internal long RepositoryId;

        #endregion End: Operational Vars ---------------------------------------------------------------------------------

        #region Property Events =========================================================================================

        //partial void OnTitleChanged(string oldValue, string newValue)
        //{
        //    HasChanges = ChangesMade();
        //}

        //partial void OnBodyChanged(string oldValue, string newValue)
        //{
        //    HasChanges = ChangesMade();
        //}

        //partial void OnMilestoneChanged(Milestone oldValue, Milestone newValue)
        //{
        //    HasChanges = ChangesMade();
        //}

        //partial void OnLabelsChanged(ObservableCollection<Label> oldValue, ObservableCollection<Label> newValue)
        //{
        //    HasChanges = ChangesMade();
        //}

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

            //HasChanges = ChangesMade();

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
            UpdateViewmodelProperties(issue);

            Labels.CollectionChanged += (s, e) => OnPropertyChanged(nameof(HasChanges));
            Assignees.CollectionChanged += (s, e) => OnPropertyChanged(nameof(HasChanges));
        }

        private void Assignees_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion End: Public Methods ---------------------------------------------------------------------------------

        #region Private Methods =========================================================================================

        private void UpdateViewmodelProperties(Issue issue)
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

        private bool ChangesMade()
        {
            bool hasChanges = false;

            if (Body != GitIssue.Body) hasChanges = true;
            if (Title.Trim() != GitIssue.Title) hasChanges = true;
            if (Milestone != null && Milestone.Id != GitIssue.Milestone.Id) hasChanges = true;

            if (Open && GitIssue.State == ItemState.Closed) hasChanges = true;
            if (!Open && GitIssue.State == ItemState.Open) hasChanges = true;

            // get any labels additional to those in GitIssue.Labels
            var additonalLabels = Labels.Except(GitIssue.Labels, new LabelIdComparer()).ToList();
            // set changes to true if any additional labels or label counts don't match (this detects removed labels from GitIssue.Labels)
            if (additonalLabels.Count > 0 || GitIssue.Labels.Count != Labels.Count) hasChanges = true;

            // same approach as above for Assignees
            var additonalAssignees = Assignees.Except(GitIssue.Assignees, new AssigneeIdComparer()).ToList();
            if (additonalAssignees.Count > 0 || GitIssue.Assignees.Count != Assignees.Count) hasChanges = true;

            return hasChanges;
        }
    }

    #endregion End: Private Methods ---------------------------------------------------------------------------------
}