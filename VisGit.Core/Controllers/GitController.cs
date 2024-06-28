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

        internal async Task<bool> AuthenticateUserAsync()
        {
            if (await gitService.AuthenticateUserAsync() == null) return true;
            else return false;
        }

        internal async Task<ObservableCollection<RepositoryViewModel>> GetRepositoriesAsync()
        {
            ObservableCollection<RepositoryViewModel> repositoryViewModels = new ObservableCollection<RepositoryViewModel>();

            IReadOnlyList<Repository> userRepositories = await gitService.GetAllUserRepositoriesAsync();

            foreach (Repository repository in userRepositories)
            {
                repositoryViewModels.Add(new RepositoryViewModel(repository));
            }

            return repositoryViewModels;
        }
    }
}