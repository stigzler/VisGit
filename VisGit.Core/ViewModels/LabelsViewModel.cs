// Ignore Spelling: Repo

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.VisualStudio.PlatformUI;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using VisGitCore.Controllers;
using VisGitCore.Data.Models;
using VisGitCore.Helpers;
using VisGitCore.Messages;

namespace VisGitCore.ViewModels
{
    public partial class LabelsViewModel : ViewModelBase,
        IRecipient<LabelDeletedMessage>,
        IRecipient<LabelNameChangingMessage>
    {
        #region Properties =========================================================================================

        // View Related
        [ObservableProperty]
        private ObservableCollection<LabelViewModel> _repositoryLabelsVMs = new ObservableCollection<LabelViewModel>();

        [ObservableProperty]
        private ICollectionView _repositoryLabelsView;

        [ObservableProperty]
        private LabelViewModel _selectedLabelViewModel;

        #endregion End: Properties ---------------------------------------------------------------------------------

        #region Operational Vars =========================================================================================

        public RepositoryViewModel gitRepositoryVm;
        private GitController gitController;

        private FilterType lastFilter = FilterType.None;
        private SortType lastSort = SortType.None;
        private ListSortDirection lastSortDirection = ListSortDirection.Ascending;

        #endregion End: Operational Vars ---------------------------------------------------------------------------------

        #region Messages =========================================================================================

        void IRecipient<LabelDeletedMessage>.Receive(LabelDeletedMessage message)
        {
            if (RepositoryLabelsVMs.Contains(message.Value))
                RepositoryLabelsVMs.Remove(message.Value);
            WeakReferenceMessenger.Default.Send(new UpdateUserMessage("Label Deleted"));
        }

        void IRecipient<LabelNameChangingMessage>.Receive(LabelNameChangingMessage message)
        {
            // Checks changed Name against other local names and remote names (excluding itself) to check for duplicates
            if (RepositoryLabelsVMs.Where(l => l.Name == message.NewName.Trim()).Count() > 0 ||
                RepositoryLabelsVMs.Where(l => (l.GitLabel.Name == message.NewName.Trim()) && message.Value.GitLabel.Id != l.GitLabel.Id).Count() > 0)
                message.Value.NameUnique = false;
            else
                message.Value.NameUnique = true;
        }

        #endregion End: Messages ---------------------------------------------------------------------------------

        #region Commands =========================================================================================

        [RelayCommand]
        private void SelectColor()
        {
            if (SelectedLabelViewModel == null) return;

            string r = SelectedLabelViewModel.Color.ToString().Substring(0, 2);
            string g = SelectedLabelViewModel.Color.ToString().Substring(2, 2);
            string b = SelectedLabelViewModel.Color.ToString().Substring(4, 2);

            if (SelectedLabelViewModel == null) return;

            var dialog = new Dsafa.WpfColorPicker.ColorPickerDialog(Color.FromArgb(255,
                byte.Parse(r, System.Globalization.NumberStyles.HexNumber), byte.Parse(g, System.Globalization.NumberStyles.HexNumber),
                byte.Parse(b, System.Globalization.NumberStyles.HexNumber)));
            //dialog.Owner = this;

            dialog.Background = new SolidColorBrush(VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowBackgroundColorKey).ToMediaColor());
            dialog.Foreground = new SolidColorBrush(VSColorTheme.GetThemedColor(EnvironmentColors.ToolWindowTextColorKey).ToMediaColor());

            var res = dialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                SelectedLabelViewModel.Color = dialog.Color.R.ToString("X2") + dialog.Color.G.ToString("X2") +
                    dialog.Color.B.ToString("X2");
            }
        }

        #endregion End: Commands ---------------------------------------------------------------------------------

        #region Public Methods =========================================================================================

        // Constructor ==============================================================================================
        internal LabelsViewModel(GitController gitController, RepositoryViewModel gitRepositoryVm)
        {
            this.gitController = gitController;
            this.gitRepositoryVm = gitRepositoryVm;

            RepositoryLabelsView = CollectionViewSource.GetDefaultView(RepositoryLabelsVMs);

            WeakReferenceMessenger.Default.Register<LabelDeletedMessage>(this);
            WeakReferenceMessenger.Default.Register<LabelNameChangingMessage>(this);
        }

        public async Task GetAllLabelsForRepoAsync()
        {
            RepositoryLabelsVMs.Clear();
            RepositoryLabelsVMs = await gitController.GetAllLabelsForRepoAsync(gitRepositoryVm.GitRepository.Id);
            RepositoryLabelsView = CollectionViewSource.GetDefaultView(RepositoryLabelsVMs);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>bool - whether new label created or not</returns>
        [RelayCommand]
        public async Task<bool> CreateNewLabelAsync()
        {
            Random rnd = new Random();
            string color = Services.Math.RandomNumber(rnd, 50, 255).ToString("X2") + Services.Math.RandomNumber(rnd, 50, 255).ToString("X2") +
                Services.Math.RandomNumber(rnd, 50, 255).ToString("X2");

            // Construct unique name
            int count = 1;
            while (RepositoryLabelsVMs.Any(l => l.Name == "label" + count)) count += 1;
            string name = "label" + count;

            Label newLabel = await gitController.CreateNewLabelAsync(gitRepositoryVm.GitRepository.Id, name, color);

            if (newLabel == null) return false; // Exception will have happened.

            LabelViewModel newLabelViewModel = new LabelViewModel(gitController, newLabel, gitRepositoryVm.GitRepository.Id);
            RepositoryLabelsVMs.Add(newLabelViewModel);
            return true;
        }

        #endregion End: Public Methods ---------------------------------------------------------------------------------
    }
}