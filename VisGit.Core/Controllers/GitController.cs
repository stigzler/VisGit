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

        internal async Task<ObservableCollection<MilestoneViewModel>> GetAllMilestonesForRepoAsync(long repositoryId)
        {
            ObservableCollection<MilestoneViewModel> milestoneViewModels = new ObservableCollection<MilestoneViewModel>();

            IReadOnlyList<Milestone> repositoryMilestones = await gitService.GetAllMilestonesForRepositoryAsync(repositoryId);

            foreach (Milestone milestone in repositoryMilestones)
            {
                milestoneViewModels.Add(new MilestoneViewModel(milestone));
            }

            return milestoneViewModels;
        }
    }
}