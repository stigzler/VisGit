using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisGitCore.Controllers;
using Octokit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

        // Operational ==============================================================================================

        [ObservableProperty]
        private bool _hasChanges;

        public IssueComment GitIssueComment { get; set; }

        #endregion End: Properties ---------------------------------------------------------------------------------

        #region Operational Vars =========================================================================================

        internal GitController gitController;
        internal long RepositoryId;

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

        internal IssueCommentViewModel(GitController gitController, IssueComment comment, long repositoryId)
        {
            this.gitController = gitController;
            this.RepositoryId = repositoryId;
            UpdateViewmodelProperties(comment);
        }

        #endregion End: Public Methods ---------------------------------------------------------------------------------

        [RelayCommand]
        private async Task SaveCommentAsync()
        {
            IssueComment comment = await gitController.SaveIssueCommentAsync(RepositoryId, GitIssueComment.Id, Body);
            if (comment != null)
            {
                UpdateViewmodelProperties(comment);
                HasChanges = false;
            }
        }

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

        #endregion End: Private Methods ---------------------------------------------------------------------------------

        private bool ChangesMade()
        {
            bool hasChanges = false;

            if (Body != GitIssueComment.Body) hasChanges = true;
            if (Reactions != GitIssueComment.Reactions) hasChanges = true;

            return hasChanges;
        }
    }
}