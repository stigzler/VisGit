using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using VisGitCore.Controllers;
using CommunityToolkit.Mvvm.Input;

namespace VisGitCore.ViewModels
{
    public partial class MilestoneViewModel : ViewModelBase
    {
        #region Properties =========================================================================================

        // Model related ----------------------------------------------------------------
        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private DateTimeOffset? _dueOn;

        [ObservableProperty]
        private bool _open;

        // View Related ----------------------------------------------------------------
        public string IssuesStatus
        {
            get => $"{Services.Math.Percentage(GitMilestone.OpenIssues, GitMilestone.OpenIssues + GitMilestone.ClosedIssues).ToString()}% complete " +
                $"{GitMilestone.OpenIssues} open {GitMilestone.ClosedIssues} closed";
        }

        public int Number { get => GitMilestone.Number; }

        // Operational ----------------------------------------------------------------

        [ObservableProperty]
        private Milestone _gitMilestone;

        [ObservableProperty]
        private bool _hasChanges;

        public long RepositoryId { get; set; }

        #endregion End: Properties

        #region Operational Vars =========================================================================================

        internal GitController gitController;

        #endregion End: Operational Vars

        #region Property Events =========================================================================================

        partial void OnTitleChanged(string oldValue, string newValue)
        {
            if (String.IsNullOrWhiteSpace(newValue)) { newValue = oldValue; }
            HasChanges = HasDifferences();
        }

        partial void OnDescriptionChanged(string oldValue, string newValue)
        {
            HasChanges = HasDifferences();
        }

        partial void OnDueOnChanged(DateTimeOffset? oldValue, DateTimeOffset? newValue)
        {
            HasChanges = HasDifferences();
        }

        partial void OnOpenChanged(bool oldValue, bool newValue)
        {
            HasChanges = HasDifferences();
        }

        //partial void OnHasChangesChanged(bool oldValue, bool newValue)
        //{
        //    if (newValue == true) ChangesDescription = ChangesInText();
        //}

        #endregion End: Property Events ----------------------------------------------------------------------------------

        #region Commands =========================================================================================

        [RelayCommand]
        private async Task SaveMilestoneAsync()
        {
            Milestone milestone = await gitController.SaveMilestoneAsync(this);
            if (milestone != null)
            {
                UpdateViewmodelProperties(milestone);
                HasChanges = HasDifferences();
            }
        }

        #endregion End: Commands

        public MilestoneViewModel()
        {
        }

        internal MilestoneViewModel(GitController gitController, Milestone milestone, long repositoryId)
        {
            this.gitController = gitController;
            RepositoryId = repositoryId;
            UpdateViewmodelProperties(milestone);
        }

        #region Public Methods =========================================================================================

        internal async Task DeleteMilestoneAsync()
        {
            await gitController.DeleteMilestoneAsync(this);
        }

        #endregion End: Public Methods

        #region Private Methods =========================================================================================

        private void UpdateViewmodelProperties(Milestone milestone)
        {
            GitMilestone = milestone;
            // ignore the advisory against settings fields directly - needed to avoid firing the associated On[Property]Changed method
            _title = milestone.Title;
            _description = milestone.Description;
            _dueOn = milestone.DueOn;
            if (milestone.State == ItemState.Open) _open = true;
            else _open = false;
        }

        // Checks for any differences between the GitMilestone Model and the relevant (possibly updated) viewmodel properties.
        private bool HasDifferences()
        {
            bool hasDifferences = false;

            if (Title != GitMilestone.Title) { hasDifferences = true; }
            if (Description != GitMilestone.Description) { hasDifferences = true; }
            if (DueOn != GitMilestone.DueOn) { hasDifferences = true; }
            if (Open && GitMilestone.State == ItemState.Closed ||
                !Open && GitMilestone.State == ItemState.Open) { hasDifferences = true; }

            return hasDifferences;
        }

        #endregion End: Private Methods
    }
}