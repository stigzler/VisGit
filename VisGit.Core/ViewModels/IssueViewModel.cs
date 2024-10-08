﻿using Community.VisualStudio.Toolkit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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
using System.Xml.Linq;
using VisGitCore.Comparers;
using VisGitCore.Controllers;
using VisGitCore.Helpers;
using VisGitCore.Messages;

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
        private DateTimeOffset _dateCreated;

        [ObservableProperty]
        private DateTimeOffset? _dateUpdated;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasChanges))]
        private StringEnum<ItemStateReason>? _itemStateReason = null;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(LockReason))]
        [NotifyPropertyChangedFor(nameof(HasChanges))]
        private bool _locked;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasChanges))]
        private StringEnum<LockReason>? _lockReason;

        // TODO: The below is possibly redundant as gitHubClient.Issue.GetAllForRepository seems
        // to return null for every issue.ClosedBy (should be a user). Test repositoryIssues
        // at GitController.GetAllIssuesForRepoAsync
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
        [NotifyPropertyChangedFor(nameof(HasChanges))]
        private ObservableCollection<User> _assignees = new ObservableCollection<User>();

        // View Related ==============================================================================================

        public int IssueNumber { get => GitIssue.Number; }

        public string IssueUrl { get => GitIssue.HtmlUrl; }

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

        private bool _hasChanges = false;
        public bool HasChanges { get => ChangesMade(); set => SetProperty(ref _hasChanges, value); }

        public Issue GitIssue;

        public bool DoNotMonitorChanges = false;

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

            //HasChanges = ChangesMade();

            // ToDo: Do I need to change any respective milestone? i.e. -1 from open issues and +1 to closed issues?
            // May have to refactor MilestoneVM as these numbers are gained form the GitMilestone, not an individ property
        }

        #endregion End: Property Events ---------------------------------------------------------------------------------

        #region Commands =========================================================================================

        [RelayCommand]
        private async Task SaveIssueAsync()
        {
            Issue issue = await gitController.SaveIssueAsync(RepositoryId, this);
            if (issue != null)
            {
                UpdateViewmodelProperties(issue);
                WeakReferenceMessenger.Default.Send(new UpdateUserMessage("Issue saved successfully."));
            }
        }

        [RelayCommand]
        private async Task DeleteIssueAsync()
        {
            var result = await VS.MessageBox.ShowAsync("Are you sure you wish to delete this issue:", Title,
                                 Microsoft.VisualStudio.Shell.Interop.OLEMSGICON.OLEMSGICON_WARNING,
                                 Microsoft.VisualStudio.Shell.Interop.OLEMSGBUTTON.OLEMSGBUTTON_YESNO);

            if (result == Microsoft.VisualStudio.VSConstants.MessageBoxResult.IDNO) return;
        }

        #endregion End: Commands ---------------------------------------------------------------------------------

        #region Public Methods =========================================================================================

        // Constructors ==============================================================================================

        internal IssueViewModel(GitController gitController, Issue issue, long repositoryId)
        {
            this.gitController = gitController;
            RepositoryId = repositoryId;
            UpdateViewmodelProperties(issue);

            Labels.CollectionChanged += (s, e) => OnPropertyChanged(nameof(HasChanges));
            Assignees.CollectionChanged += (s, e) => AssigneeCollectionChanged();
        }

        private void AssigneeCollectionChanged()
        {
            HasChanges = ChangesMade();
            OnPropertyChanged(nameof(HasChanges));
            OnPropertyChanged(nameof(Assignees));
        }

        public void TryAddAssignee(User user)
        {
            if (!Assignees.Any(a => a.Id == user.Id))
                Assignees.Add(user);
        }

        public void TryRemoveAssignee(User user)
        {
            if (Assignees.Any(a => a.Id == user.Id))
                Assignees.Remove(user);
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

            _dateCreated = GitIssue.CreatedAt;
            _dateUpdated = GitIssue.UpdatedAt;

            if (GitIssue.State == ItemState.Open) _open = true;
            else _open = false;
            _itemStateReason = GitIssue.StateReason;

            _locked = GitIssue.Locked;
            _lockReason = GitIssue.ActiveLockReason;

            _labels.Clear();
            foreach (var label in GitIssue.Labels)
                _labels.Add(label);
        }

        private bool ChangesMade()
        {
            if (DoNotMonitorChanges) return false;

            bool hasChanges = false;

            if (Body != GitIssue.Body) hasChanges = true;
            if (Title.Trim() != GitIssue.Title) hasChanges = true;
            if ((GitIssue.Milestone == null && Milestone != null) || Milestone != null && Milestone.Id != GitIssue.Milestone.Id) hasChanges = true;

            if (Open && GitIssue.State == ItemState.Closed) hasChanges = true;
            if (!Open && GitIssue.State == ItemState.Open) hasChanges = true;

            if (ItemStateReason != GitIssue.StateReason) hasChanges = true;

            if (Locked != GitIssue.Locked) hasChanges = true;
            if (LockReason != GitIssue.ActiveLockReason) hasChanges = true;

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