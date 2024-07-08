// Ignore Spelling: Repo

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using VisGitCore.Controllers;
using VisGitCore.Data.Models;
using VisGitCore.Messages;

namespace VisGitCore.ViewModels
{
    public partial class MilestonesViewModel : ViewModelBase
    {
        #region Properties =========================================================================================

        // Obs Props ==============================================================================================

        [ObservableProperty]
        private ObservableCollection<MilestoneViewModel> _repositoryMilestonesVMs = new ObservableCollection<MilestoneViewModel>();

        [ObservableProperty]
        private ICollectionView _repositoryMilestonesView;

        [ObservableProperty]
        private MilestoneViewModel _selectedMilestoneViewModel;

        public RepositoryViewModel gitRepositoryVm;

        #endregion End: Properties

        #region Private Vars =========================================================================================

        // Operational ==============================================================================================

        private GitController gitController;

        private FilterType lastFilter = FilterType.None;
        private SortType lastSort = SortType.None;
        private ListSortDirection lastSortDirection = ListSortDirection.Ascending;

        #endregion End: Private Vars ---------------------------------------------------------------------------------

        #region OnPropertyChanged Methods =========================================================================================

        partial void OnRepositoryMilestonesVMsChanged(ObservableCollection<MilestoneViewModel> oldValue, ObservableCollection<MilestoneViewModel> newValue)
        {
            RepositoryMilestonesView = CollectionViewSource.GetDefaultView(newValue);
            FilterMilestones(lastFilter);
            SortMilestones(lastSort, lastSortDirection);
            if (RepositoryMilestonesVMs.Count > 0)
            {
                RepositoryMilestonesView.MoveCurrentToFirst();
                SelectedMilestoneViewModel = (MilestoneViewModel)RepositoryMilestonesView.CurrentItem;
            }
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

        internal MilestonesViewModel(GitController gitController, RepositoryViewModel gitRepositoryVm)
        {
            this.gitController = gitController;
            this.gitRepositoryVm = gitRepositoryVm;

            RepositoryMilestonesView = CollectionViewSource.GetDefaultView(RepositoryMilestonesVMs);
        }

        // Sort and Filter ==============================================================================================
        public void SortMilestones(SortType sortType, ListSortDirection sortDirection)
        {
            lastSort = sortType;
            lastSortDirection = sortDirection;

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
            lastFilter = filterType;

            if (filterType == FilterType.Closed)
                RepositoryMilestonesView.Filter = new Predicate<object>(x => ((MilestoneViewModel)x).Open == false);
            else if (filterType == FilterType.Open)
                RepositoryMilestonesView.Filter = new Predicate<object>(x => ((MilestoneViewModel)x).Open == true);
            else if (filterType == FilterType.None)
                RepositoryMilestonesView.Filter = null;
        }

        // Milestone Operations ==============================================================================================

        public async Task CreateNewMilestoneAsync()
        {
            string title = "New Milestone created " + DateTime.Now.ToShortDateString();
            Milestone newMilestone = await gitController.CreateNewMilestoneAsync(gitRepositoryVm.GitRepository.Id, title);
            MilestoneViewModel newMilestoneViewModel = new MilestoneViewModel(gitController, newMilestone, gitRepositoryVm.GitRepository.Id);
            RepositoryMilestonesVMs.Add(newMilestoneViewModel);
        }

        public async Task GetAllMilestonesForRepoAsync()
        {
            RepositoryMilestonesVMs.Clear();
            RepositoryMilestonesVMs = await gitController.GetAllMilestonesForRepoAsync(gitRepositoryVm.GitRepository.Id);
            RepositoryMilestonesView = CollectionViewSource.GetDefaultView(RepositoryMilestonesVMs);
            //milestonesViewModel.RepositoryMilestonesVMs = milestonesViewModel.RepositoryMilestonesVMs;
        }

        #endregion End: Public Methods
    }
}