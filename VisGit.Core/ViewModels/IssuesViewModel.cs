using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.Design.WebControls;
using System.Windows.Data;
using VisGitCore.Controllers;
using VisGitCore.Messages;

namespace VisGitCore.ViewModels
{
    public partial class IssuesViewModel : ViewModelBase
    {
        #region Properties =========================================================================================

        [ObservableProperty]
        private ObservableCollection<IssueViewModel> _repositoryIssuesVMs = new ObservableCollection<IssueViewModel>();

        [ObservableProperty]
        private ICollectionView _repositoryIssuesView;

        [ObservableProperty]
        private IssueViewModel _selectedIssueViewModel;

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

        #endregion End: Properties ---------------------------------------------------------------------------------

        #region Operational Vars =========================================================================================

        internal GitController gitController;
        internal RepositoryViewModel gitRepositoryVm;

        #endregion End: Operational Vars ---------------------------------------------------------------------------------

        #region Property Changed Events =========================================================================================

        partial void OnSelectedNewLabelChanged(LabelViewModel value)
        {
            if (!SelectedIssueViewModel.Labels.Any(l => l.Name == SelectedNewLabel.GitLabel.Name))
                SelectedIssueViewModel.Labels.Add(SelectedNewLabel.GitLabel);
        }

        partial void OnSelectedMilestoneVMChanged(MilestoneViewModel value)
        {
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
            IssueCommentsVM.RepositoryIssueCommentsVMs = await gitController.GetAllCommentsForIssueAsync(gitRepositoryVm.GitRepository.Id,
                issueViewModel.GitIssue.Number);
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

        #endregion End: Commands ---------------------------------------------------------------------------------

        #region Public Methods =========================================================================================

        internal IssuesViewModel(GitController gitController, RepositoryViewModel gitRepositoryVm,
            ObservableCollection<LabelViewModel> repositoryLabels)
        {
            this.gitController = gitController;
            this.gitRepositoryVm = gitRepositoryVm;
            _repositoryLabels = repositoryLabels;
            IssueCommentsVM = new IssueCommentsViewModel(gitController, gitRepositoryVm);
        }

        public async Task GetAllIssuesForRepoAsync()
        {
            RepositoryIssuesVMs.Clear();
            RepositoryIssuesVMs = await gitController.GetAllIssuesForRepoAsync(gitRepositoryVm.GitRepository.Id);
            RepositoryIssuesView = CollectionViewSource.GetDefaultView(RepositoryIssuesVMs);
        }

        #endregion End: Public Methods ---------------------------------------------------------------------------------
    }
}