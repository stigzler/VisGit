using CommunityToolkit.Mvvm.Messaging;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using VisGitCore.Messages;
using VisGitCore.Services;
using VisGitCore.ViewModels;

namespace VisGitCore.Controllers
{
    internal class GitController
    {
        public User User { get; set; }

        private GitService gitService;

        public GitController(GitService gitService)
        {
            this.gitService = gitService;
        }

        internal void SendOperationErrorMessage(string message, Exception exception = null, object associatedObject = null)
        {
            WeakReferenceMessenger.Default.Send(new UpdateUserMessage(message)
            { Exception = exception, AssociatedObject = associatedObject });
        }

        #region Users =========================================================================================

        internal async Task<bool> AuthenticateUserAsync(string personalAccessToken)
        {
            try
            {
                if (await gitService.AuthenticateUserAsync(personalAccessToken) == null)
                {
                    User = await gitService.GetAuthenticatedUserAsync();

                    return true;
                }
                else return false;
            }
            catch (Exception ex) { SendOperationErrorMessage("Error authenticating PAT", ex); }
            return false;
        }

        internal async Task<SearchUsersResult> SearchUsersAsync(string loginname)
        {
            try
            {
                return await gitService.SearchUsersAsync(loginname);
            }
            catch (Exception ex) { SendOperationErrorMessage("Error searching users", ex); }
            return null;
        }

        #endregion End: Users ---------------------------------------------------------------------------------

        internal async Task<ObservableCollection<RepositoryViewModel>> GetAllRepositoriesAsync()
        {
            ObservableCollection<RepositoryViewModel> repositoryViewModels = new ObservableCollection<RepositoryViewModel>();

            try
            {
                IReadOnlyList<Repository> userRepositories = await gitService.GetAllUserRepositoriesAsync();

                foreach (Repository repository in userRepositories)
                {
                    repositoryViewModels.Add(new RepositoryViewModel(repository));
                }

                return repositoryViewModels;
            }
            catch (Exception ex)
            {
                SendOperationErrorMessage("Error loading repositories", ex);
            }

            return null;
        }

        #region Milestones =========================================================================================

        internal async Task<ObservableCollection<MilestoneViewModel>> GetAllMilestonesForRepoAsync(long repositoryId)
        {
            ObservableCollection<MilestoneViewModel> milestoneViewModels = new ObservableCollection<MilestoneViewModel>();

            try
            {
                IReadOnlyList<Milestone> repositoryMilestones = await gitService.GetAllMilestonesForRepositoryAsync(repositoryId);

                foreach (Milestone milestone in repositoryMilestones)
                {
                    milestoneViewModels.Add(new MilestoneViewModel(this, milestone, repositoryId));
                }

                return milestoneViewModels;
            }
            catch (Exception ex)
            {
                SendOperationErrorMessage("Error loading Milestones", ex);
            }
            return null;
        }

        internal async Task<Milestone> SaveMilestoneAsync(MilestoneViewModel milestoneViewModel)
        {
            try
            {
                return await gitService.SaveMilestoneAsync(milestoneViewModel.RepositoryId, milestoneViewModel.GitMilestone.Number,
                     milestoneViewModel.Title, milestoneViewModel.Description, milestoneViewModel.DueOn, milestoneViewModel.Open);
            }
            catch (Exception ex)
            {
                SendOperationErrorMessage("Error saving Milestone", ex);
            }
            return null;
        }

        internal async Task DeleteMilestoneAsync(MilestoneViewModel milestoneViewModel)
        {
            try
            {
                await gitService.DeleteMilestoneAsync(milestoneViewModel.RepositoryId, milestoneViewModel.GitMilestone.Number);
            }
            catch (Exception ex)
            {
                SendOperationErrorMessage("Error deleting Milestone", ex);
            }
        }

        internal async Task<Milestone> CreateNewMilestoneAsync(long repositoryId, string title)
        {
            try
            {
                return await gitService.CreateNewMilestoneAsync(repositoryId, title);
            }
            catch (Exception ex)
            {
                SendOperationErrorMessage("Error creating new Milestone", ex);
            }
            return null;
        }

        #endregion End: Milestones ---------------------------------------------------------------------------------

        #region Labels =========================================================================================

        internal async Task<ObservableCollection<LabelViewModel>> GetAllLabelsForRepoAsync(long repositoryId)
        {
            ObservableCollection<LabelViewModel> labelViewModels = new ObservableCollection<LabelViewModel>();

            try
            {
                IReadOnlyList<Label> repositoryLabels = await gitService.GetAllLabelsForRepositoryAsync(repositoryId);

                foreach (Label label in repositoryLabels)
                {
                    labelViewModels.Add(new LabelViewModel(this, label, repositoryId));
                }

                return labelViewModels;
            }
            catch (Exception ex)
            {
                SendOperationErrorMessage("Error loading Labels", ex);
            }
            return null;
        }

        internal async Task<Label> SaveLabelAsync(LabelViewModel labelViewModel)
        {
            try
            {
                return await gitService.SaveLabelAsync(labelViewModel.RepositoryId, labelViewModel.GitLabel.Name, labelViewModel.Name,
                    labelViewModel.Description, labelViewModel.Color);
            }
            catch (Exception ex) { SendOperationErrorMessage("Error saving Label", ex); }
            return null;
        }

        internal async Task DeleteLabelAsync(LabelViewModel labelViewModel)
        {
            try
            {
                await gitService.DeleteLabelAsync(labelViewModel.RepositoryId, labelViewModel.GitLabel.Name);
            }
            catch (Exception ex) { SendOperationErrorMessage("Error deleting Label", ex); }
        }

        internal async Task<Label> CreateNewLabelAsync(long repositoryId, string title, string color)
        {
            try { return await gitService.CreateNewLabelAsync(repositoryId, title, color); }
            catch (Exception ex) { SendOperationErrorMessage("Error creating new Label", ex); }
            return null;
        }

        #endregion End: Labels ---------------------------------------------------------------------------------

        #region Issues =========================================================================================

        internal async Task<ObservableCollection<IssueViewModel>> GetAllIssuesForRepoAsync(long repositoryId)
        {
            ObservableCollection<IssueViewModel> issueViewModels = new ObservableCollection<IssueViewModel>();

            try
            {
                IReadOnlyList<Issue> repositoryIssues = await gitService.GetAllIssuesForRepositoryAsync(repositoryId);

                foreach (Issue issue in repositoryIssues)
                {
                    issueViewModels.Add(new IssueViewModel(this, issue, repositoryId));
                }

                return issueViewModels;
            }
            catch (Exception ex) { SendOperationErrorMessage("Error loading Issues", ex); }
            return null;
        }

        internal async Task<ObservableCollection<IssueCommentViewModel>> GetAllCommentsForIssueAsync(long repositoryId, int issueNumber)
        {
            ObservableCollection<IssueCommentViewModel> issueCommentViewModels = new ObservableCollection<IssueCommentViewModel>();

            try
            {
                IReadOnlyList<IssueComment> repositoryIssueComments = await gitService.GetAllCommentsForIssueAsync(repositoryId, issueNumber);
                foreach (IssueComment issueComment in repositoryIssueComments)
                {
                    issueCommentViewModels.Add(new IssueCommentViewModel(this, issueComment, repositoryId));
                }
                return issueCommentViewModels;
            }
            catch (Exception ex) { SendOperationErrorMessage("Error loading Issue Comments", ex); }
            return null;
        }

        #endregion End: Issues ---------------------------------------------------------------------------------
    }
}