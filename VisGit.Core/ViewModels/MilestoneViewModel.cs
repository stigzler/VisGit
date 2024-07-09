using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;
using VisGitCore.Controllers;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using VisGitCore.Messages;
using System.ComponentModel.DataAnnotations;

namespace VisGitCore.ViewModels
{
    public partial class MilestoneViewModel : ViewModelBase
    {
        #region Properties =========================================================================================

        // Model related ----------------------------------------------------------------
        [ObservableProperty]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Title cannot be empty")]
        [NotifyDataErrorInfo]
        [CustomValidation(typeof(MilestoneViewModel), nameof(ValidateTitleUnique))]
        private string _title;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private DateTimeOffset? _dueOn;

        [ObservableProperty]
        private bool _open;

        public int Number { get => GitMilestone.Number; }
        public Octokit.StringEnum<ItemState> State { get => GitMilestone.State; }
        public DateTimeOffset? UpdatedAt { get => GitMilestone.UpdatedAt; }
        public int OpenIssues { get => GitMilestone.OpenIssues; }

        // View Related ----------------------------------------------------------------
        public string IssuesStatus
        {
            get
            {
                double percentageComplete = PercentageComplete;
                return $"{percentageComplete.ToString()}% complete " + $"{GitMilestone.OpenIssues} open {GitMilestone.ClosedIssues} closed";
            }
        }

        public double PercentageComplete
        {
            get
            {
                return Services.Math.Percentage(GitMilestone.ClosedIssues, GitMilestone.OpenIssues + GitMilestone.ClosedIssues);
            }
        }

        // Operational ----------------------------------------------------------------

        [ObservableProperty]
        private Milestone _gitMilestone;

        [ObservableProperty]
        private bool _hasChanges;

        [ObservableProperty]
        private bool _titleUnique;

        public long RepositoryId { get; set; }

        #endregion End: Properties

        #region Operational Vars =========================================================================================

        internal GitController gitController;

        #endregion End: Operational Vars

        #region Property Events =========================================================================================

        partial void OnTitleChanging(string value)
        {
            WeakReferenceMessenger.Default.Send(new MilestoneTitleChangingMessage(this) { NewTitle = value });
        }

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

        #region Public Methods =========================================================================================

        public MilestoneViewModel()
        {
        }

        internal MilestoneViewModel(GitController gitController, Milestone milestone, long repositoryId)
        {
            this.gitController = gitController;
            RepositoryId = repositoryId;
            UpdateViewmodelProperties(milestone);
        }

        internal async Task DeleteMilestoneAsync()
        {
            await gitController.DeleteMilestoneAsync(this);
        }

        public static ValidationResult ValidateTitleUnique(string name, ValidationContext context)
        {
            MilestoneViewModel milestoneViewModel = (MilestoneViewModel)context.ObjectInstance;

            if (milestoneViewModel.TitleUnique) return ValidationResult.Success;
            return new("Title must be unique. Conflicts with another local or remote (git) title.");
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