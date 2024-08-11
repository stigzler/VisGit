using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using VisGitCore.Controllers;
using VisGitCore.Messages;

namespace VisGitCore.ViewModels
{
    public partial class IssueCommentsViewModel : ViewModelBase, IRecipient<CommentDeletedMessage>
    {
        #region Properties =========================================================================================

        [ObservableProperty]
        private ObservableCollection<IssueCommentViewModel> _repositoryIssueCommentsVMs = new ObservableCollection<IssueCommentViewModel>();

        [ObservableProperty]
        private ICollectionView _repositoryIssueCommentsView;

        [ObservableProperty]
        private IssueViewModel _selectedIssueCommentViewModel;

        public int CommentsCount { get => RepositoryIssueCommentsVMs.Count; }

        #endregion End: Properties ---------------------------------------------------------------------------------

        #region Operational Variables =========================================================================================

        internal GitController gitController;
        internal RepositoryViewModel repositoryVm;

        #endregion End: Operational Variables ---------------------------------------------------------------------------------

        internal IssueCommentsViewModel(GitController gitController, RepositoryViewModel gitRepositoryVm)
        {
            this.gitController = gitController;
            this.repositoryVm = gitRepositoryVm;

            WeakReferenceMessenger.Default.Register<CommentDeletedMessage>(this);
        }

        void IRecipient<CommentDeletedMessage>.Receive(CommentDeletedMessage message)
        {
            RepositoryIssueCommentsVMs.Remove(message.Value);
            WeakReferenceMessenger.Default.Send(new UpdateUserMessage("Comment deleted successfully."));
        }
    }
}