using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisGitCore.Controllers;
using Octokit;
using CommunityToolkit.Mvvm.ComponentModel;

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

        public IssueComment GitIssueComment { get; set; }

        #endregion End: Properties ---------------------------------------------------------------------------------

        #region Operational Vars =========================================================================================

        internal GitController gitController;
        internal long RepositoryId;

        #endregion End: Operational Vars ---------------------------------------------------------------------------------

        internal IssueCommentViewModel(GitController gitController, IssueComment comment, long repositoryId)
        {
            this.gitController = gitController;
            this.RepositoryId = repositoryId;
            _ = UpdateViewmodelProperties(comment);
        }

        #region Private Methods =========================================================================================

        private async Task UpdateViewmodelProperties(IssueComment comment)
        {
            GitIssueComment = comment;

            _body = comment.Body;
            _author = comment.User;
            _createdAt = comment.CreatedAt;
            _updatedAt = comment.UpdatedAt;
            _reactions = comment.Reactions;

            #endregion End: Private Methods ---------------------------------------------------------------------------------
        }
    }
}