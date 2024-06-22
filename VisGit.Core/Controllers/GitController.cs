﻿using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Octokit;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using VisGit.Core.Services;
using VisGit.Core.ViewModels;

namespace VisGit.Core.Controllers
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