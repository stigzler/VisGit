using CommunityToolkit.Mvvm.ComponentModel;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisGitCore.Controllers;
using VisGitCore.Helpers;

namespace VisGitCore.ViewModels
{
    public partial class IssueViewModel : ViewModelBase
    {
        #region Properties =========================================================================================

        // Model related ==============================================================================================

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private bool _open;

        [ObservableProperty]
        private User _closedBy;

        [ObservableProperty]
        private DateTimeOffset? _closedAt;

        // Operational ==============================================================================================

        [ObservableProperty]
        private bool _hasChanges;

        public Issue GitIssue;

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
        }

        #endregion End: Property Events ---------------------------------------------------------------------------------

        #region Public Methods =========================================================================================

        // Constructors ==============================================================================================

        internal IssueViewModel(GitController gitController, Issue issue, long repositoryId)
        {
            this.gitController = gitController;
            RepositoryId = repositoryId;
            UpdateViewmodelProperties(issue);
        }

        #endregion End: Public Methods ---------------------------------------------------------------------------------

        #region Private Methods =========================================================================================

        private void UpdateViewmodelProperties(Issue issue)
        {
            GitIssue = issue;

            _title = GitIssue.Title;
            _closedBy = GitIssue.ClosedBy;
            _closedAt = GitIssue.ClosedAt;

            if (GitIssue.State == ItemState.Open) _open = true;
            else _open = false;
        }

        #endregion End: Private Methods ---------------------------------------------------------------------------------
    }
}