using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Octokit;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using VisGitCore.Services;
using VisGitCore.ViewModels;

namespace VisGitCore.Controllers
{
    internal class GitController
    {
        private GitService gitService;

        public GitController(GitService gitService)
        {
            this.gitService = gitService;
        }

        internal async Task<bool> AuthenticateUserAsync(string personalAccessToken)
        {
            if (await gitService.AuthenticateUserAsync(personalAccessToken) == null) return true;
            else return false;
        }

        internal async Task<ObservableCollection<RepositoryViewModel>> GetAllRepositoriesAsync()
        {
            ObservableCollection<RepositoryViewModel> repositoryViewModels = new ObservableCollection<RepositoryViewModel>();

            IReadOnlyList<Repository> userRepositories = await gitService.GetAllUserRepositoriesAsync();

            foreach (Repository repository in userRepositories)
            {
                repositoryViewModels.Add(new RepositoryViewModel(repository));
            }

            return repositoryViewModels;
        }

        // Milestones ==============================================================================================

        internal async Task<ObservableCollection<MilestoneViewModel>> GetAllMilestonesForRepoAsync(long repositoryId)
        {
            ObservableCollection<MilestoneViewModel> milestoneViewModels = new ObservableCollection<MilestoneViewModel>();

            IReadOnlyList<Milestone> repositoryMilestones = await gitService.GetAllMilestonesForRepositoryAsync(repositoryId);

            foreach (Milestone milestone in repositoryMilestones)
            {
                milestoneViewModels.Add(new MilestoneViewModel(this, milestone, repositoryId));
            }

            return milestoneViewModels;
        }

        internal async Task<Milestone> SaveMilestoneAsync(MilestoneViewModel milestoneViewModel)
        {
            return await gitService.SaveMilestoneAsync(milestoneViewModel.RepositoryId, milestoneViewModel.GitMilestone.Number,
                 milestoneViewModel.Title, milestoneViewModel.Description, milestoneViewModel.DueOn, milestoneViewModel.Open);
        }

        internal async Task DeleteMilestoneAsync(MilestoneViewModel milestoneViewModel)
        {
            await gitService.DeleteMilestoneAsync(milestoneViewModel.RepositoryId, milestoneViewModel.GitMilestone.Number);
        }

        internal async Task<Milestone> CreateNewMilestoneAsync(long repositoryId, string title)
        {
            return await gitService.CreateNewMilestoneAsync(repositoryId, title);
        }

        // Labels ==============================================================================================

        internal async Task<ObservableCollection<LabelViewModel>> GetAllLabelsForRepoAsync(long repositoryId)
        {
            ObservableCollection<LabelViewModel> labelViewModels = new ObservableCollection<LabelViewModel>();

            IReadOnlyList<Label> repositoryLabels = await gitService.GetAllLabelsForRepositoryAsync(repositoryId);

            foreach (Label label in repositoryLabels)
            {
                labelViewModels.Add(new LabelViewModel(this, label, repositoryId));
            }

            return labelViewModels;
        }
    }
}