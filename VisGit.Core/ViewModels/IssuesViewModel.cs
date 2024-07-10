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

        [ObservableProperty]
        private Label _selectedExistingLabel;

        [ObservableProperty]
        private LabelViewModel _selectedNewLabel;

        [ObservableProperty]
        private ObservableCollection<LabelViewModel> _repositoryLabels = new ObservableCollection<LabelViewModel>();

        #endregion End: Properties ---------------------------------------------------------------------------------

        #region Operational Vars =========================================================================================

        internal GitController gitController;
        internal RepositoryViewModel gitRepositoryVm;

        #endregion End: Operational Vars ---------------------------------------------------------------------------------

        #region Property Changed Events =========================================================================================

        partial void OnSelectedNewLabelChanged(LabelViewModel value)
        {
            RepositoryLabels.Count();
        }

        #endregion End: Property Changed Events ---------------------------------------------------------------------------------

        #region Commands =========================================================================================

        [RelayCommand]
        private void RemoveLabel()
        {
            if (SelectedIssueViewModel == null) return;
            SelectedIssueViewModel.Labels.Remove(SelectedExistingLabel);
        }

        #endregion End: Commands ---------------------------------------------------------------------------------

        #region Public Methods =========================================================================================

        internal IssuesViewModel(GitController gitController, RepositoryViewModel gitRepositoryVm,
            ObservableCollection<LabelViewModel> repositoryLabels)
        {
            this.gitController = gitController;
            this.gitRepositoryVm = gitRepositoryVm;
            _repositoryLabels = repositoryLabels;
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