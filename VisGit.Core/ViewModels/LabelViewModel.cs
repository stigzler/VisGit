using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisGitCore.Controllers;

namespace VisGitCore.ViewModels
{
    public partial class LabelViewModel : ViewModelBase
    {
        #region Properties =========================================================================================

        // Model related ----------------------------------------------------------------
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private string _color;

        // Operational ----------------------------------------------------------------------
        [ObservableProperty]
        private bool _hasChanges;

        [ObservableProperty]
        private Label _gitLabel;

        public long RepositoryId { get; set; }

        #endregion End: Properties ---------------------------------------------------------------------------------

        #region Property Events =========================================================================================

        partial void OnColorChanged(string oldValue, string newValue)
        {
            HasChanges = HasDifferences();
        }

        partial void OnDescriptionChanged(string oldValue, string newValue)
        {
            HasChanges = HasDifferences();
        }

        partial void OnNameChanged(string oldValue, string newValue)
        {
            HasChanges = HasDifferences();
        }

        #endregion End: Property Events ---------------------------------------------------------------------------------

        #region Operational Vars =========================================================================================

        internal GitController gitController;

        #endregion End: Operational Vars ---------------------------------------------------------------------------------

        #region Commands =========================================================================================

        [RelayCommand]
        private async Task SaveLabelAsync()
        {
            // TODO: Implement below
            //Milestone milestone = await gitController.SaveMilestoneAsync(this);
            //if (milestone != null)
            //{
            //    UpdateViewmodelProperties(milestone);
            //    HasChanges = HasDifferences();
            //}
        }

        #endregion End: Commands ---------------------------------------------------------------------------------

        internal LabelViewModel(GitController gitController, Label label, long repositoryId)
        {
            this.gitController = gitController;
            RepositoryId = repositoryId;
            UpdateViewmodelProperties(label);
        }

        #region Private Methods =========================================================================================

        private void UpdateViewmodelProperties(Label label)
        {
            GitLabel = label;

            _name = label.Name;
            _description = label.Description;
            _color = label.Color;
        }

        // Checks for any differences between the GitMilestone Model and the relevant (possibly updated) viewmodel properties.
        private bool HasDifferences()
        {
            bool hasDifferences = false;

            if (Name != GitLabel.Name) { hasDifferences = true; }
            if (Description != GitLabel.Description) { hasDifferences = true; }
            if (Color != GitLabel.Color) { hasDifferences = true; }
            return hasDifferences;
        }

        #endregion End: Private Methods ---------------------------------------------------------------------------------
    }
}