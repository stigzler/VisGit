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

        private UserSettings userSettings;

        public GitService(UserSettings userSettings)
        {
            this.userSettings = userSettings;
        }

        internal async Task<Exception> AuthenticateUserAsync()
        {
            var tokenAuth = new Credentials(userSettings.PersonalAccessToken);
            gitHubClient.Credentials = tokenAuth;

            try
            {
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