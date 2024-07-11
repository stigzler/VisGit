using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisGitCore.Controllers;

namespace VisGitCore.ViewModels
{
    public partial class IssueCommentsViewModel : ViewModelBase
    {
        #region Properties =========================================================================================

        [ObservableProperty]
        private ObservableCollection<IssueCommentViewModel> _repositoryIssueCommentsVMs = new ObservableCollection<IssueCommentViewModel>();

        [ObservableProperty]
        private ICollectionView _repositoryIssueCommentsView;

        [ObservableProperty]
        private IssueViewModel _selectedIssueCommentViewModel;

        #endregion End: Properties ---------------------------------------------------------------------------------

        #region Operational Variables =========================================================================================

        internal GitController gitController;
        internal RepositoryViewModel repositoryVm;

        #endregion End: Operational Variables ---------------------------------------------------------------------------------

        internal IssueCommentsViewModel(GitController gitController, RepositoryViewModel gitRepositoryVm)
        {
            this.gitController = gitController;
            this.repositoryVm = gitRepositoryVm;
        }
    }
}