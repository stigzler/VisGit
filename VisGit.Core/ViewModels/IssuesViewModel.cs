using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using EnvDTE;
using Microsoft.VisualStudio.TaskRunnerExplorer;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.Design.WebControls;
using System.Windows.Controls;
using System.Windows.Data;
using VisGitCore.Controllers;
using VisGitCore.Data.Models;
using VisGitCore.Messages;
using Label = Octokit.Label;

namespace VisGitCore.ViewModels
{
    public partial class IssuesViewModel : ViewModelBase
    {
        #region Properties =========================================================================================

        [ObservableProperty]
        private ObservableCollection<IssueViewModel> _repositoryIssuesVMs = new ObservableCollection<IssueViewModel>();

        [ObservableProperty]
        private ICollectionView _repositoryIssuesCollectionView;

        [ObservableProperty]
        private IssueViewModel _selectedIssueViewModel;

        [ObservableProperty]
        private List<ItemStateReasonMap> _itemStateReasons = ItemStateReasonMap.ItemStateReasons();

        [ObservableProperty]
        private List<LockReasonMap> _lockStateReasons = LockReasonMap.LockReasonItems();

        // Labels ==============================================================================================

        [ObservableProperty]
        private ObservableCollection<LabelViewModel> _repositoryLabels = new ObservableCollection<LabelViewModel>();

        [ObservableProperty]
        private Label _selectedExistingLabel;

        [ObservableProperty]
        private LabelViewModel _selectedNewLabel;

        // Milestones ==============================================================================================

        [ObservableProperty]
        private ObservableCollection<MilestoneViewModel> _repositoryMilestonesVMs = new ObservableCollection<MilestoneViewModel>();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedIssueViewModel))]
        private MilestoneViewModel _selectedMilestoneVM;

        // Assignees ==============================================================================================

        [ObservableProperty]
        private User _selectedAssignee;

        [ObservableProperty]
        private SearchUsersResult _searchUserResult;

        [ObservableProperty]
        private IssueCommentsViewModel _issueCommentsVM;

        // View Related =============================================================================================

        [ObservableProperty]
        private bool _collapseAllComments;

        [ObservableProperty]
        private ObservableCollection<ItemStateReason?> _openedReasons = new ObservableCollection<ItemStateReason?>() { null, ItemStateReason.Reopened };

        #endregion End: Properties ---------------------------------------------------------------------------------

        #region Operational Vars =========================================================================================

        internal GitController gitController;
        internal RepositoryViewModel gitRepositoryVm;

        public ObservableCollection<ItemStateReason?> ClosedReasons => new ObservableCollection<ItemStateReason?>() { null, ItemStateReason.Completed, ItemStateReason.NotPlanned };

        #endregion End: Operational Vars ---------------------------------------------------------------------------------

        #region Property Changed Events =========================================================================================

        partial void OnSelectedNewLabelChanged(LabelViewModel value)
        {
            if (SelectedNewLabel == null) return;

            if (!SelectedIssueViewModel.Labels.Any(l => l.Name == SelectedNewLabel.GitLabel.Name))
                SelectedIssueViewModel.Labels.Add(SelectedNewLabel.GitLabel);
        }

        partial void OnSelectedMilestoneVMChanged(MilestoneViewModel value)
        {
            if (value == null) return;

            SelectedIssueViewModel.Milestone = value.GitMilestone;
            //TODO: This will throw out the open/closed numbers for this specific milestone. Not sure how to manage yet. Ideas:
            // Refresh entire collection - would need ot also save this
            // Subtract 1 from open issues
        }

        partial void OnSelectedIssueViewModelChanged(IssueViewModel oldValue, IssueViewModel newValue)
        {
            _ = UpdateIssueCommentsAsync(newValue);
        }

        private async Task UpdateIssueCommentsAsync(IssueViewModel issueViewModel)
        {
            if (issueViewModel == null) return;

            IssueCommentsVM.RepositoryIssueCommentsVMs = await gitController.GetAllCommentsForIssueAsync(gitRepositoryVm.GitRepository.Id,
                issueViewModel.GitIssue.Number, CollapseAllComments);
        }

        #endregion End: Property Changed Events ---------------------------------------------------------------------------------

        #region Commands =========================================================================================

        [RelayCommand]
        private void RemoveLabel()
        {
            if (SelectedIssueViewModel == null) return;
            SelectedIssueViewModel.Labels.Remove(SelectedExistingLabel);
        }

        [RelayCommand]
        private async Task SearchUsersAsync(string loginName)
        {
            SearchUserResult = await gitController.SearchUsersAsync(loginName);
        }

        [RelayCommand]
        private void AssignUser(User user)
        {
            if (SelectedIssueViewModel.Assignees.Count() < 10
                && !SelectedIssueViewModel.Assignees.Contains(user)
                && user != null)
            {
                SelectedIssueViewModel.Assignees.Add(user);
            }
        }

        [RelayCommand]
        private void UnassignUser()
        {
            SelectedIssueViewModel.Assignees.Remove(SelectedAssignee);
        }

        [RelayCommand]
        private void CloseIssue(ItemStateReasonMap itemStateReasonMap)
        {
            SelectedIssueViewModel.ItemStateReason = itemStateReasonMap.ItemStateReason;

            if (itemStateReasonMap.ItemStateReason == ItemStateReason.Completed) SelectedIssueViewModel.Open = false;
            else if (itemStateReasonMap.ItemStateReason == ItemStateReason.NotPlanned) SelectedIssueViewModel.Open = false;
            else if (itemStateReasonMap.ItemStateReason == ItemStateReason.Reopened) SelectedIssueViewModel.Open = true;
        }

        [RelayCommand]
        private void LockIssue(LockReasonMap lockReasonMap)
        {
            //SelectedIssueViewModel.DoNotMonitorChanges = true; // this stops HasChanges check firing twice due to setting two properties in this method

            if (lockReasonMap.LockReason == LockReason.OffTopic) SelectedIssueViewModel.Locked = true;
            else if (lockReasonMap.LockReason == LockReason.Resolved) SelectedIssueViewModel.Locked = true;
            else if (lockReasonMap.LockReason == LockReason.Spam) SelectedIssueViewModel.Locked = true;
            else if (lockReasonMap.LockReason == LockReason.TooHeated) SelectedIssueViewModel.Locked = true;
            // This maps to "Unlock" in menu
            else if (lockReasonMap.LockReason == null) SelectedIssueViewModel.Locked = false;

            // SelectedIssueViewModel.DoNotMonitorChanges = false;

            SelectedIssueViewModel.LockReason = lockReasonMap.LockReason;
        }

        [RelayCommand]
        private void GoToIssueLink(string url)
        {
            System.Diagnostics.Process.Start(url);
        }

        [RelayCommand]
        private async Task CreateNewIssueCommentAsync()
        {
            IssueComment newIssueComment = await gitController.CreateNewIssueCommentAsync(gitRepositoryVm.GitRepository.Id,
                SelectedIssueViewModel, $"New comment");

            if (newIssueComment == null) return;

            IssueCommentViewModel newIssueCommentViewModel = new IssueCommentViewModel(gitController, newIssueComment, gitRepositoryVm.GitRepository.Id, CollapseAllComments);
            IssueCommentsVM.RepositoryIssueCommentsVMs.Add(newIssueCommentViewModel);

            WeakReferenceMessenger.Default.Send(new UpdateUserMessage("New Comment created successfully."));

            return;
        }

        [RelayCommand]
        private void CollapseComments()
        {
            foreach (IssueCommentViewModel issueCommentViewModel in IssueCommentsVM.RepositoryIssueCommentsVMs)
            {
                issueCommentViewModel.Collapsed = CollapseAllComments;
            }
        }

        #endregion End: Commands ---------------------------------------------------------------------------------

        #region Public Methods =========================================================================================

        internal IssuesViewModel(GitController gitController, RepositoryViewModel gitRepositoryVm,
            ObservableCollection<LabelViewModel> repositoryLabels)
        {
            this.gitController = gitController;
            this.gitRepositoryVm = gitRepositoryVm;
            _repositoryLabels = repositoryLabels;

            RepositoryIssuesCollectionView = CollectionViewSource.GetDefaultView(RepositoryIssuesVMs);
            IssueCommentsVM = new IssueCommentsViewModel(gitController, gitRepositoryVm);
        }

        // Sort and Filter ==============================================================================================
        public void SortIssues(SortType sortType, ListSortDirection sortDirection)
        {
            //lastSort = sortType;
            //lastSortDirection = sortDirection;

            RepositoryIssuesCollectionView.SortDescriptions.Clear();

            if (sortType == SortType.Alphabetially)
                RepositoryIssuesCollectionView.SortDescriptions.Add(new SortDescription("Title", sortDirection));
            if (sortType == SortType.DateCreated)
                RepositoryIssuesCollectionView.SortDescriptions.Add(new SortDescription("DateCreated", sortDirection));
            if (sortType == SortType.UpdatedOrClosed)
            {
                RepositoryIssuesCollectionView.SortDescriptions.Add(new SortDescription("ClosedAt", sortDirection));
                RepositoryIssuesCollectionView.SortDescriptions.Add(new SortDescription("DateUpdated", sortDirection));
            }
        }

        public void FilterIssues(FilterType filterType)
        {
            if (filterType == FilterType.Closed)
                RepositoryIssuesCollectionView.Filter = new Predicate<object>(x => ((IssueViewModel)x).Open == false);
            //((labelFilter != null && ((IssueViewModel)x).Labels.Any(l => l.Id == labelFilter.GitLabel.Id))));
            if (filterType == FilterType.Open)
                RepositoryIssuesCollectionView.Filter = new Predicate<object>(x => ((IssueViewModel)x).Open == true);
            if (filterType == FilterType.Locked)
                RepositoryIssuesCollectionView.Filter = new Predicate<object>(x => ((IssueViewModel)x).Locked == true);
            if (filterType == FilterType.NotLocked)
                RepositoryIssuesCollectionView.Filter = new Predicate<object>(x => ((IssueViewModel)x).Locked == false);
            if (filterType == FilterType.MyIssues)
                RepositoryIssuesCollectionView.Filter = new Predicate<object>(x => ((IssueViewModel)x).GitIssue.User.Login == gitController.User.Login);
            if (filterType == FilterType.OtherIssues)
                RepositoryIssuesCollectionView.Filter = new Predicate<object>
                    (x => ((IssueViewModel)x).GitIssue.User.Login != gitController.User.Login);
            else if (filterType == FilterType.None)
                RepositoryIssuesCollectionView.Filter = null;

            if (!RepositoryIssuesCollectionView.IsEmpty)
            {
                RepositoryIssuesCollectionView.MoveCurrentToFirst();
                SelectedIssueViewModel = (IssueViewModel)RepositoryIssuesCollectionView.CurrentItem;
            }
        }

        // ToDo: Spent 3 hours trying to merge this into one method with the above. Fuck it - life's too short.
        public void FilterIssuesByTypeAndLabel(FilterType filterType, LabelViewModel labelFilter = null)
        {
            if (labelFilter == null) return;

            if (filterType == FilterType.Closed)
                RepositoryIssuesCollectionView.Filter = new Predicate<object>(x => ((IssueViewModel)x).Open == false
                    && ((IssueViewModel)x).Labels.Any(l => l.Id == labelFilter.GitLabel.Id));
            if (filterType == FilterType.Open)
                RepositoryIssuesCollectionView.Filter = new Predicate<object>(x => ((IssueViewModel)x).Open == true && ((IssueViewModel)x).Labels.Any(l => l.Id == labelFilter.GitLabel.Id));
            if (filterType == FilterType.Locked)
                RepositoryIssuesCollectionView.Filter = new Predicate<object>(x => ((IssueViewModel)x).Locked == true && ((IssueViewModel)x).Labels.Any(l => l.Id == labelFilter.GitLabel.Id));
            if (filterType == FilterType.NotLocked)
                RepositoryIssuesCollectionView.Filter = new Predicate<object>(x => ((IssueViewModel)x).Locked == false && ((IssueViewModel)x).Labels.Any(l => l.Id == labelFilter.GitLabel.Id));
            if (filterType == FilterType.MyIssues)
                RepositoryIssuesCollectionView.Filter = new Predicate<object>(x => ((IssueViewModel)x).GitIssue.User.Login == gitController.User.Login && ((IssueViewModel)x).Labels.Any(l => l.Id == labelFilter.GitLabel.Id));
            if (filterType == FilterType.OtherIssues)
                RepositoryIssuesCollectionView.Filter = new Predicate<object>(x => ((IssueViewModel)x).GitIssue.User.Login != gitController.User.Login && ((IssueViewModel)x).Labels.Any(l => l.Id == labelFilter.GitLabel.Id));
            else if (filterType == FilterType.None)
                RepositoryIssuesCollectionView.Filter = new Predicate<object>(x => ((IssueViewModel)x).Labels.Any(l => l.Id == labelFilter.GitLabel.Id));

            if (!RepositoryIssuesCollectionView.IsEmpty)
            {
                RepositoryIssuesCollectionView.MoveCurrentToFirst();
                SelectedIssueViewModel = (IssueViewModel)RepositoryIssuesCollectionView.CurrentItem;
            }
        }

        public async Task GetAllIssuesForRepoAsync()
        {
            RepositoryIssuesVMs.Clear();
            RepositoryIssuesVMs = await gitController.GetAllIssuesForRepoAsync(gitRepositoryVm.GitRepository.Id);
            RepositoryIssuesCollectionView = CollectionViewSource.GetDefaultView(RepositoryIssuesVMs);
        }

        internal async Task<bool> CreateNewIssueAsync()
        {
            // Construct unique name
            int count = 1;
            while (RepositoryIssuesVMs.Any(l => l.Title == "Issue " + count)) count += 1;
            string title = "Issue " + count;

            Issue newIssue = await gitController.CreateNewIssueAsync(gitRepositoryVm.GitRepository.Id, title);

            if (newIssue == null) return false;

            IssueViewModel newIssueViewModel = new IssueViewModel(gitController, newIssue, gitRepositoryVm.GitRepository.Id);
            RepositoryIssuesVMs.Add(newIssueViewModel);
            return true;
        }

        #endregion End: Public Methods ---------------------------------------------------------------------------------

        #region Private Methods =========================================================================================

        private void FilterItemStateReasons(object sender, FilterEventArgs e)
        {
            ItemStateReasonMap itemStateReasonMap = e.Item as ItemStateReasonMap;
            if (itemStateReasonMap != null)
            {
                if (SelectedIssueViewModel.Open) e.Accepted = true;
                else e.Accepted = false;
            }
        }

        #endregion End: Private Methods ---------------------------------------------------------------------------------
    }
}