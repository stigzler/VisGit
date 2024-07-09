using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;
using VisGitCore.Data;
using System;
using VisGitCore.ViewModels;

namespace VisGitCore.Services
{
    internal class GitService
    {
        private GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue(Constants.GitProductHeaderValue));

        public GitService()
        {
        }

        // User ==============================================================================================

        internal async Task<Exception> AuthenticateUserAsync(string personalAccessToken)
        {
            try
            {
                var tokenAuth = new Credentials(personalAccessToken);
                gitHubClient.Credentials = tokenAuth;
                var authTest = await gitHubClient.RateLimit.GetRateLimits(); // have to be authorised to access this. Thus, provides a test.
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        internal void Logout()
        {
            gitHubClient.Credentials = new Credentials("NULLED - FORCE LOGOUT ON ANY MORE REQUESTS");
        }

        // Repositories ==============================================================================================

        internal async Task<IReadOnlyList<Repository>> GetAllUserRepositoriesAsync()
        {
            return await gitHubClient.Repository.GetAllForCurrent();
        }

        // Milestones ==============================================================================================

        internal async Task<IReadOnlyList<Milestone>> GetAllMilestonesForRepositoryAsync(long repositoryId)
        {
            return await gitHubClient.Issue.Milestone.GetAllForRepository(repositoryId, new MilestoneRequest { State = ItemStateFilter.All });
        }

        internal async Task<Milestone> SaveMilestoneAsync(long repositoryId, int milestoneNumber, string title, string description,
            DateTimeOffset? dueOn, bool open)
        {
            MilestoneUpdate milestoneUpdate = new MilestoneUpdate();
            milestoneUpdate.Title = title;
            milestoneUpdate.Description = description;

            if (dueOn != null)
            {
                DateTimeOffset resolvedDueOn = (DateTimeOffset)dueOn;
                resolvedDueOn = resolvedDueOn.AddDays(1); // hack: controls for date off by one further up tree
                milestoneUpdate.DueOn = resolvedDueOn;
            }

            if (open) milestoneUpdate.State = ItemState.Open;
            else milestoneUpdate.State = ItemState.Closed;

            return await gitHubClient.Issue.Milestone.Update(repositoryId, milestoneNumber, milestoneUpdate);
        }

        internal async Task DeleteMilestoneAsync(long repositoryId, int milestoneNumber)
        {
            await gitHubClient.Issue.Milestone.Delete(repositoryId, milestoneNumber);
        }

        internal async Task<Milestone> CreateNewMilestoneAsync(long repositoryId, string title)
        {
            NewMilestone newMilestone = new NewMilestone(title);
            return await gitHubClient.Issue.Milestone.Create(repositoryId, newMilestone);
        }

        // Labels ==============================================================================================

        internal async Task<IReadOnlyList<Label>> GetAllLabelsForRepositoryAsync(long repositoryId)
        {
            return await gitHubClient.Issue.Labels.GetAllForRepository(repositoryId);
        }

        internal async Task<Label> SaveLabelAsync(long repositoryId, string gitName, string newName, string description, string color)
        {
            LabelUpdate labelUpdate = new LabelUpdate(gitName, color);
            labelUpdate.Name = newName;
            labelUpdate.Description = description;

            return await gitHubClient.Issue.Labels.Update(repositoryId, gitName, labelUpdate);
        }

        internal async Task DeleteLabelAsync(long repositoryId, string gitName)
        {
            await gitHubClient.Issue.Labels.Delete(repositoryId, gitName);
        }

        internal async Task<Label> CreateNewLabelAsync(long repositoryId, string title, string color)
        {
            NewLabel newLabel = new NewLabel(title, color);
            return await gitHubClient.Issue.Labels.Create(repositoryId, newLabel);
        }
    }
}