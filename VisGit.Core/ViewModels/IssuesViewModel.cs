using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
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

        #endregion End: Properties ---------------------------------------------------------------------------------

        #region Operational Vars =========================================================================================

        internal GitController gitController;
        internal RepositoryViewModel gitRepositoryVm;

        #endregion End: Operational Vars ---------------------------------------------------------------------------------

        #region Public Methods =========================================================================================

        public async Task GetAllLabelsForRepoAsync()
        {
            RepositoryIssuesVMs.Clear();
            RepositoryIssuesVMs = await gitController.GetAllIssuesForRepoAsync(gitRepositoryVm.GitRepository.Id);
            RepositoryIssuesView = CollectionViewSource.GetDefaultView(RepositoryIssuesVMs);
        }

        #endregion End: Public Methods ---------------------------------------------------------------------------------

        internal IssuesViewModel(GitController gitController, RepositoryViewModel gitRepositoryVm)
        {
            this.gitController = gitController;
            this.gitRepositoryVm = gitRepositoryVm;

            //RepositoryLabelsView = CollectionViewSource.GetDefaultView(RepositoryLabelsVMs);

            //WeakReferenceMessenger.Default.Register<LabelDeletedMessage>(this);
            //WeakReferenceMessenger.Default.Register<LabelNameChangingMessage>(this);
        }
    }
}