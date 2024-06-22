using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;
using VisGit.Core.Data;
using System;

namespace VisGit.Core.Services
{
    internal class GitService
    {
        private GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue(Constants.GitProductHeaderValue));

        public GitService()
        {
        }

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

        internal async Task<IReadOnlyList<Repository>> GetAllUserRepositoriesAsync()
        {
            return await gitHubClient.Repository.GetAllForCurrent();
        }

        internal void Logout()
        {
            gitHubClient.Credentials = new Credentials("NULLED - FORCE LOGOUT ON ANY MORE REQUESTS");
        }
    }
}