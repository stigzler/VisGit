using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisGitCore.Controllers;
using Octokit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VisGitCore.Messages;
using Community.VisualStudio.Toolkit;

namespace VisGitCore.ViewModels
{
    public partial class IssueCommentViewModel : ViewModelBase
    {
        #region Properties =========================================================================================

        // Model Related ==============================================================================================

        [ObservableProperty]
        private string _body;

        [ObservableProperty]
        private User _author;

        [ObservableProperty]
        private DateTimeOffset? _createdAt;

        [ObservableProperty]
        private DateTimeOffset? _updatedAt;

        [ObservableProperty]
        private ReactionSummary _reactions;

        // View Related =============================================================================================

        [ObservableProperty]
        private bool _hasChanges;

        [ObservableProperty]
        private bool _collapsed = false;

        public string BodyFirstLine { get => Body.Split(new[] { '\r', '\n' }).FirstOrDefault(); }

        #endregion End: Properties ---------------------------------------------------------------------------------

        #region Operational Vars =========================================================================================

        internal GitController gitController;
        internal long RepositoryId;
        public IssueComment GitIssueComment { get; set; }

        #endregion End: Operational Vars ---------------------------------------------------------------------------------

        #region Property Events =========================================================================================

        partial void OnBodyChanged(string oldValue, string newValue)
        {
            HasChanges = ChangesMade();
        }

        partial void OnReactionsChanged(ReactionSummary oldValue, ReactionSummary newValue)
        {
            HasChanges = ChangesMade();
        }

        #endregion End: Property Events ---------------------------------------------------------------------------------

        #region Public Methods =========================================================================================

        internal IssueCommentViewModel(GitController gitController, IssueComment comment, long repositoryId,
            bool startCollapsed = false)
        {
            this.gitController = gitController;
            this.RepositoryId = repositoryId;
            UpdateViewmodelProperties(comment);
            _collapsed = startCollapsed;
        }

        #endregion End: Public Methods ---------------------------------------------------------------------------------

        #region Commands =========================================================================================

        [RelayCommand]
        private async Task SaveCommentAsync()
        {
            IssueComment comment = await gitController.SaveIssueCommentAsync(RepositoryId, GitIssueComment.Id, Body);
            if (comment != null)
            {
                UpdateViewmodelProperties(comment);
                HasChanges = false;
                WeakReferenceMessenger.Default.Send(new UpdateUserMessage("Comment saved successfully."));
            }
        }

        [RelayCommand]
        private async Task DeleteCommentAsync()
        {
            var result = await VS.MessageBox.ShowAsync("Are you sure you wish to delete this comment?", "",
                 Microsoft.VisualStudio.Shell.Interop.OLEMSGICON.OLEMSGICON_WARNING,
                 Microsoft.VisualStudio.Shell.Interop.OLEMSGBUTTON.OLEMSGBUTTON_YESNO);

            if (result == Microsoft.VisualStudio.VSConstants.MessageBoxResult.IDNO) return;

            bool success = await gitController.DeleteIssueCommentAsync(RepositoryId, this);
            if (success)
            {
                WeakReferenceMessenger.Default.Send(new CommentDeletedMessage(this));
            }
        }

        #endregion End: Commands ---------------------------------------------------------------------------------

        #region Private Methods =========================================================================================

        private void UpdateViewmodelProperties(IssueComment comment)
        {
            GitIssueComment = comment;

            _body = comment.Body;
            _author = comment.User;
            _createdAt = comment.CreatedAt;
            _updatedAt = comment.UpdatedAt;
            _reactions = comment.Reactions;
        }

        private bool ChangesMade()
        {
            bool hasChanges = false;

            if (Body != GitIssueComment.Body) hasChanges = true;
            if (Reactions != GitIssueComment.Reactions) hasChanges = true;

            return hasChanges;
        }

        #endregion End: Private Methods ---------------------------------------------------------------------------------
    }
}