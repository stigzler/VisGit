using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using VisGitCore.Data.Models;
using VisGitCore.Messages;

namespace VisGitCore.ViewModels
{
    public partial class MilestonesViewModel : ViewModelBase
    {
        #region Properties =========================================================================================

        //[ObservableProperty]
        //private MainViewModel _mainViewModel;

        [ObservableProperty]
        private ObservableCollection<MilestoneViewModel> _repositoryMilestonesVMs = new ObservableCollection<MilestoneViewModel>();

        [ObservableProperty]
        private ICollectionView _repositoryMilestonesView;

        [ObservableProperty]
        private MilestoneViewModel _selectedMilestoneViewModel;

        #endregion End: Properties

        #region OnPropertyChanged Methods =========================================================================================

        partial void OnRepositoryMilestonesVMsChanged(ObservableCollection<MilestoneViewModel> oldValue, ObservableCollection<MilestoneViewModel> newValue)
        {
            RepositoryMilestonesView = CollectionViewSource.GetDefaultView(newValue);
        }

        #endregion End: OnPropertyChanged Methods

        #region Commands =========================================================================================

        [RelayCommand]
        private void NavigateHome()
        {
            //Debug.WriteLine(MainViewModel.RepositoryMilestonesVMs.Count.ToString());
            WeakReferenceMessenger.Default.Send(new ChangeViewMessage(Enums.ViewRequest.Home));
        }

        [RelayCommand]
        private async Task DeleteMilestoneAsync()
        {
            await SelectedMilestoneViewModel.DeleteMilestoneAsync();
            RepositoryMilestonesVMs.Remove(SelectedMilestoneViewModel);
            if (RepositoryMilestonesVMs.Count > 0) SelectedMilestoneViewModel = RepositoryMilestonesVMs[0];
        }

        [RelayCommand]
        private async Task RunTestAsync()
        {
            RepositoryMilestonesView.SortDescriptions.Clear();
            RepositoryMilestonesView.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
        }

        #endregion End: Commands

        #region Public Methods =========================================================================================

        // Constructors ==============================================================================================
        public MilestonesViewModel(MainViewModel mainViewModel)
        {
            //MainViewModel = mainViewModel;
        }

        public MilestonesViewModel()
        {
            RepositoryMilestonesView = CollectionViewSource.GetDefaultView(RepositoryMilestonesVMs);
        }

        // Other ==============================================================================================

        public void SortMilestones(SortType sortType, ListSortDirection sortDirection)
        {
            RepositoryMilestonesView.SortDescriptions.Clear();

            if (sortType == SortType.Alphabetially)
                RepositoryMilestonesView.SortDescriptions.Add(new SortDescription("Title", sortDirection));
            if (sortType == SortType.DueDate)
                RepositoryMilestonesView.SortDescriptions.Add(new SortDescription("DueOn", sortDirection));
            //if (sortType == SortType.Open)
            //    RepositoryMilestonesView.SortDescriptions.Add(new SortDescription("State", sortDirection));
            if (sortType == SortType.RecentlyUpdated)
                RepositoryMilestonesView.SortDescriptions.Add(new SortDescription("UpdatedAt", sortDirection));
            if (sortType == SortType.OpenIssues)
                RepositoryMilestonesView.SortDescriptions.Add(new SortDescription("OpenIssues", sortDirection));
        }

        public void FilterMilestones(FilterType filterType)
        {
            if (filterType == FilterType.Closed)
                RepositoryMilestonesView.Filter = new Predicate<object>(x => ((MilestoneViewModel)x).Open == false);
            else if (filterType == FilterType.Open)
                RepositoryMilestonesView.Filter = new Predicate<object>(x => ((MilestoneViewModel)x).Open == true);
            else if (filterType == FilterType.None)
                RepositoryMilestonesView.Filter = null;
        }

        #endregion End: Public Methods
    }
}