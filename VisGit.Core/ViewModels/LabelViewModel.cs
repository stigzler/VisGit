﻿using Community.VisualStudio.Toolkit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft;
using Octokit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisGitCore.Controllers;
using VisGitCore.Messages;

namespace VisGitCore.ViewModels
{
    public partial class LabelViewModel : ViewModelBase
    {
        #region Properties =========================================================================================

        // Model related ----------------------------------------------------------------
        [ObservableProperty]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name cannot be empty")]
        [NotifyDataErrorInfo]
        [CustomValidation(typeof(LabelViewModel), nameof(ValidateNameUnique), ErrorMessage = "Name must be unique")]
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

        [ObservableProperty]
        private bool _nameUnique = true;

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

        partial void OnNameChanging(string value)
        {
            WeakReferenceMessenger.Default.Send(new LabelNameChangingMessage(this) { NewName = value });
        }

        #endregion End: Property Events ---------------------------------------------------------------------------------

        #region Operational Vars =========================================================================================

        internal GitController gitController;

        #endregion End: Operational Vars ---------------------------------------------------------------------------------

        #region Commands =========================================================================================

        [RelayCommand]
        private async Task SaveLabelAsync()
        {
            Label returnedLabel = await gitController.SaveLabelAsync(this);

            if (returnedLabel != null)
            {
                UpdateViewmodelProperties(returnedLabel);
                HasChanges = HasDifferences();
            }
        }

        [RelayCommand]
        private async Task DeleteLabelAsync()
        {
            var result = await VS.MessageBox.ShowAsync("Are you sure you wish to delete this label:", Name,
                 Microsoft.VisualStudio.Shell.Interop.OLEMSGICON.OLEMSGICON_WARNING,
                 Microsoft.VisualStudio.Shell.Interop.OLEMSGBUTTON.OLEMSGBUTTON_YESNO);

            if (result == Microsoft.VisualStudio.VSConstants.MessageBoxResult.IDNO) return;

            await gitController.DeleteLabelAsync(this); // reinstate once messaging sorted

            WeakReferenceMessenger.Default.Send(new LabelDeletedMessage(this));
        }

        #endregion End: Commands ---------------------------------------------------------------------------------

        #region Public Methods =========================================================================================

        // Constructor ==============================================================================================
        internal LabelViewModel(GitController gitController, Label label, long repositoryId)
        {
            this.gitController = gitController;
            RepositoryId = repositoryId;
            UpdateViewmodelProperties(label);
        }

        #endregion End: Public Methods ---------------------------------------------------------------------------------

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

        public static ValidationResult ValidateNameUnique(string name, ValidationContext context)
        {
            //RegistrationForm instance = (RegistrationForm)context.ObjectInstance;
            //bool isValid = instance.service.Validate(name);

            //if (isValid)
            //{
            //    return ValidationResult.Success;
            //}

            LabelViewModel labelViewModel = (LabelViewModel)context.ObjectInstance;

            if (labelViewModel.NameUnique) return ValidationResult.Success;
            return new("Name must be unique");
        }

        //void IRecipient<QueryNameUniqueMessage>.Receive(QueryNameUniqueMessage message)
        //{
        //    if (message.Name == Name)
        //        NameUnique = true;
        //    else
        //        NameUnique = false;
        //}

        #endregion End: Private Methods ---------------------------------------------------------------------------------
    }
}