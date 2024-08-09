using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;
using VisGitCore.Data;
using System;
using VisGitCore.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.IO;

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

        internal async Task<User> GetAuthenticatedUserAsync()
        {
            return await gitHubClient.User.Current();
        }

        internal void Logout()
        {
            gitHubClient.Credentials = new Credentials("NULLED - FORCE LOGOUT ON ANY MORE REQUESTS");
        }

        internal async Task<SearchUsersResult> SearchUsersAsync(string loginName)
        {
            SearchUsersRequest searchUsersRequest = new SearchUsersRequest(loginName);
            searchUsersRequest.In = new List<UserInQualifier>() { UserInQualifier.Username };
            return await gitHubClient.Search.SearchUsers(searchUsersRequest);
        }

        // Repositories ==============================================================================================

        internal async Task<IReadOnlyList<Repository>> GetAllUserRepositoriesAsync()
        {
            return await gitHubClient.Repository.GetAllForCurrent();
        }

        #region Milestones =========================================================================================

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

        #endregion End: Milestones ---------------------------------------------------------------------------------

        #region Labels =========================================================================================

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

        #endregion End: Labels ---------------------------------------------------------------------------------

        #region Issues =========================================================================================

        internal async Task<Issue> CreateNewIssueAsync(long repositoryId, string title)
        {
            NewIssue newIssue = new NewIssue(title);
            return await gitHubClient.Issue.Create(repositoryId, newIssue);
        }

        internal async Task<IReadOnlyList<Issue>> GetAllIssuesForRepositoryAsync(long repositoryId)
        {
            return await gitHubClient.Issue.GetAllForRepository(repositoryId);
        }

        internal async Task<IReadOnlyList<IssueComment>> GetAllCommentsForIssueAsync(long repositoryId, int issueNumber)
        {
            return await gitHubClient.Issue.Comment.GetAllForIssue(repositoryId, issueNumber);
        }

        internal async Task<Issue> SaveIssueAsync(long repositoryId, int issueNumber, string title, string body,
            bool open, StringEnum<ItemStateReason>? itemStateReason,
            bool locked, StringEnum<LockReason>? lockReason,
            Milestone milestone, ObservableCollection<Label> labels, ObservableCollection<User> assignees)
        {
            IssueUpdate issueUpdate = new IssueUpdate();

            issueUpdate.ClearAssignees();
            issueUpdate.ClearLabels();

            issueUpdate.Title = title;
            issueUpdate.Body = body;

            if (milestone != null) issueUpdate.Milestone = milestone.Number;

            if (open) issueUpdate.State = ItemState.Open;
            else issueUpdate.State = ItemState.Closed;

            if (itemStateReason != null)
            {
                ItemStateReason stateReason;
                itemStateReason.Value.TryParse(out stateReason);
                issueUpdate.StateReason = stateReason;
            }

            foreach (Label label in labels) issueUpdate.AddLabel(label.Name);

            foreach (User assignee in assignees) issueUpdate.AddAssignee(assignee.Login);

            // Process any lock
            ILockUnlockClient lockUnlockClient = gitHubClient.Issue.LockUnlock;

            if (locked && lockReason != null)
            {
                LockReason lockReasonEnum;
                lockReason.Value.TryParse(out lockReasonEnum);
                await lockUnlockClient.Lock(repositoryId, issueNumber, lockReasonEnum);
            }
            else if (!locked)
            {
                await lockUnlockClient.Unlock(repositoryId, issueNumber);
            }

            // Update issue and return new model data

            var returnedIssue = await gitHubClient.Issue.Update(repositoryId, issueNumber, issueUpdate);

            return returnedIssue;
        }

        internal async Task DeleteTaskAsync()
        {
            Issue issue = new Issue();
        }

        // Comments ==============================================================================================

        internal async Task<IssueComment> SaveIssueCommentAsync(long repositoryId, long commentId, string body)
        {
            IIssueCommentsClient issueCommentsClient = gitHubClient.Issue.Comment;
            return await gitHubClient.Issue.Comment.Update(repositoryId, commentId, body);
        }

        internal async Task<IssueComment> CreateNewCommentAsync(long repositoryId, int issueNumber, string body)
        {
            return await gitHubClient.Issue.Comment.Create(repositoryId, issueNumber, body);
        }

        internal async Task DeleteIssueCommentAsync(long repositoryId, long commentId)
        {
            await gitHubClient.Issue.Comment.Delete(repositoryId, commentId);
        }

        // Attachments ==============================================================================================

        internal async Task UploadFileAsync(long repositoryId, string endPath, string filePath)
        {
            //string base64String = GetImageBase64String(filePath);

            //CreateFileRequest createFileRequest = new CreateFileRequest("Attachment", base64String);
            //var result = await gitHubClient.Repository.Content.CreateFile(repositoryId, endPath, createFileRequest);
            //Debug.WriteLine(result.ToString());

            //using (var fileContents = File.OpenRead(filePath))
            //{
            //    var assetUpload = new ReleaseAssetUpload()
            //    {
            //        FileName = endPath,
            //        ContentType = "image/png",
            //        RawData = fileContents
            //    };
            //    //var release = gitHubClient.Repository.Release.Get("octokit", "octokit.net", 1).Result;

            //    var dave = new Octokit.

            //    var release = await gitHubClient.Repository.Release.Get(repositoryId, 0);
            //    var asset = await gitHubClient.Repository.Release.UploadAsset(release, assetUpload);
            //}

            var owner = "stigzler"; // Replace with the repository owner
            var repoName = "VisGit.TestRepo"; // Replace with your repository name
            //var filePath = "path_to_your_image.jpg"; // Replace with the path to your image
            var commitMessage = "Adding an image";
            var branch = "master"; // Replace with the branch you want to commit to

            // Read the file content
            var fileContent = await File.ReadAllBytesAsync(filePath.Replace(@"\\", @"\"));
            var base64FileContent = Convert.ToBase64String(fileContent);

            // Get the reference of the branch
            var reference = await gitHubClient.Git.Reference.Get(owner, repoName, $"heads/{branch}");

            // Get the latest commit on the branch
            var latestCommit = await gitHubClient.Git.Commit.Get(owner, repoName, reference.Object.Sha);

            // Get the tree of the latest commit
            var baseTree = latestCommit.Tree.Sha;

            // Create a new blob
            var newBlob = new NewBlob
            {
                Content = base64FileContent,
                Encoding = EncodingType.Base64
            };
            var blobRef = await gitHubClient.Git.Blob.Create(owner, repoName, newBlob);

            // Create a new tree item
            var newTreeItem = new NewTreeItem
            {
                Path = Path.GetFileName(filePath),
                Mode = "100644",
                Type = TreeType.Blob,
                Sha = blobRef.Sha
            };

            // Create a new tree
            var newTree = new NewTree { BaseTree = baseTree };
            newTree.Tree.Add(newTreeItem);

            var newTreeResponse = await gitHubClient.Git.Tree.Create(owner, repoName, newTree);

            // Create a new commit
            var newCommit = new NewCommit(commitMessage, newTreeResponse.Sha, reference.Object.Sha);
            var commit = await gitHubClient.Git.Commit.Create(owner, repoName, newCommit);

            // Update the reference to point to the new commit
            var dave = await gitHubClient.Git.Reference.Update(owner, repoName, reference.Ref, new ReferenceUpdate(commit.Sha));

            //await gitHubClient.Git.Reference.Update(repositoryId, reference.Ref, new ReferenceUpdate(commit.Sha));

            Debug.WriteLine("Image uploaded successfully!");
        }

        static string GetImageBase64String(string imgPath)
        {
            byte[] imageBytes = System.IO.File.ReadAllBytes(imgPath);
            return Convert.ToBase64String(imageBytes);
        }

        #endregion End: Issues ---------------------------------------------------------------------------------
    }
}