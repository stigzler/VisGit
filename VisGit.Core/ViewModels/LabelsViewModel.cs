using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using VisGitCore.Controllers;
using VisGitCore.Data.Models;

namespace VisGitCore.ViewModels
{
    public partial class LabelsViewModel : ViewModelBase
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

        #region Commands =========================================================================================

        [RelayCommand]
        private void SelectColor()
        {
            string r = SelectedLabelViewModel.Color.ToString().Substring(0, 2);
            string g = SelectedLabelViewModel.Color.ToString().Substring(2, 2);
            string b = SelectedLabelViewModel.Color.ToString().Substring(4, 2);

            if (SelectedLabelViewModel == null) return;

            var dialog = new Dsafa.WpfColorPicker.ColorPickerDialog(Color.FromArgb(255,
                byte.Parse(r, System.Globalization.NumberStyles.HexNumber), byte.Parse(g, System.Globalization.NumberStyles.HexNumber),
                byte.Parse(b, System.Globalization.NumberStyles.HexNumber)));
            //dialog.Owner = this;

            dialog.Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30));
            dialog.Foreground = new SolidColorBrush(Colors.Gainsboro);

            var res = dialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                SelectedLabelViewModel.Color = dialog.Color.R.ToString("X2") + dialog.Color.G.ToString("X2") +
                    dialog.Color.B.ToString("X2");
            }
        }

        [RelayCommand]
        private void SaveLabelAsync()
        {
        }

        #endregion End: Commands ---------------------------------------------------------------------------------

        #region Public Methods =========================================================================================

        // Constructor ==============================================================================================

        internal LabelsViewModel(GitController gitController, RepositoryViewModel gitRepositoryVm)
        {
            this.gitController = gitController;
            this.gitRepositoryVm = gitRepositoryVm;

            RepositoryLabelsView = CollectionViewSource.GetDefaultView(RepositoryLabelsVMs);
        }

        public async Task GetAllLabelsForRepoAsync()
        {
            RepositoryLabelsVMs.Clear();
            RepositoryLabelsVMs = await gitController.GetAllLabelsForRepoAsync(gitRepositoryVm.GitRepository.Id);
            RepositoryLabelsView = CollectionViewSource.GetDefaultView(RepositoryLabelsVMs);
        }

        #endregion End: Public Methods ---------------------------------------------------------------------------------
    }
}