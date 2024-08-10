// Ignore Spelling: Repo

using Community.VisualStudio.Toolkit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using VisGitCore.Controllers;
using VisGitCore.Data.Models;
using VisGitCore.Messages;

namespace VisGitCore.ViewModels
{
    public partial class MilestonesViewModel : ViewModelBase,
        IRecipient<MilestoneTitleChangingMessage>
    {
        #region Properties =========================================================================================

        // Obs Props ==============================================================================================

        [ObservableProperty]
        private ObservableCollection<MilestoneViewModel> _repositoryMilestonesVMs = new ObservableCollection<MilestoneViewModel>();

        [ObservableProperty]
        private ICollectionView _repositoryMilestonesCollectionView;

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
            RepositoryMilestonesCollectionView = CollectionViewSource.GetDefaultView(newValue);
            FilterMilestones(lastFilter);
            SortMilestones(lastSort, lastSortDirection);
            if (RepositoryMilestonesVMs.Count > 0)
            {
                RepositoryMilestonesCollectionView.MoveCurrentToFirst();
                SelectedMilestoneViewModel = (MilestoneViewModel)RepositoryMilestonesCollectionView.CurrentItem;
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

        // TODO: This should be moved into MilestoneViewModel
        [RelayCommand]
        private async Task DeleteMilestoneAsync()
        {
            var result = await VS.MessageBox.ShowAsync("Are you sure you wish to delete this milestone?", "",
                 Microsoft.VisualStudio.Shell.Interop.OLEMSGICON.OLEMSGICON_WARNING,
                 Microsoft.VisualStudio.Shell.Interop.OLEMSGBUTTON.OLEMSGBUTTON_YESNO);

            if (result == Microsoft.VisualStudio.VSConstants.MessageBoxResult.IDNO) return;

            await SelectedMilestoneViewModel.DeleteMilestoneAsync();
            RepositoryMilestonesVMs.Remove(SelectedMilestoneViewModel);
            if (RepositoryMilestonesVMs.Count > 0) SelectedMilestoneViewModel = RepositoryMilestonesVMs[0];
            WeakReferenceMessenger.Default.Send(new UpdateUserMessage("Milestone deleted"));
        }

        [RelayCommand]
        private async Task RunTestAsync()
        {
            RepositoryMilestonesCollectionView.SortDescriptions.Clear();
            RepositoryMilestonesCollectionView.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
        }

        #endregion End: Commands

        #region Public Methods =========================================================================================

        // Constructors ==============================================================================================

        internal MilestonesViewModel(GitController gitController, RepositoryViewModel gitRepositoryVm)
        {
            this.gitController = gitController;
            this.gitRepositoryVm = gitRepositoryVm;

            RepositoryMilestonesCollectionView = CollectionViewSource.GetDefaultView(RepositoryMilestonesVMs);

            WeakReferenceMessenger.Default.Register<MilestoneTitleChangingMessage>(this);
        }

        // Sort and Filter ==============================================================================================
        public void SortMilestones(SortType sortType, ListSortDirection sortDirection)
        {
            lastSort = sortType;
            lastSortDirection = sortDirection;

            RepositoryMilestonesCollectionView.SortDescriptions.Clear();

            if (sortType == SortType.Alphabetially)
                RepositoryMilestonesCollectionView.SortDescriptions.Add(new SortDescription("Title", sortDirection));
            if (sortType == SortType.DueDate)
                RepositoryMilestonesCollectionView.SortDescriptions.Add(new SortDescription("DueOn", sortDirection));
            //if (sortType == SortType.Open)
            //    RepositoryMilestonesView.SortDescriptions.Add(new SortDescription("State", sortDirection));
            if (sortType == SortType.RecentlyUpdated)
                RepositoryMilestonesCollectionView.SortDescriptions.Add(new SortDescription("UpdatedAt", sortDirection));
            if (sortType == SortType.OpenIssues)
                RepositoryMilestonesCollectionView.SortDescriptions.Add(new SortDescription("OpenIssues", sortDirection));
        }

        public void FilterMilestones(FilterType filterType)
        {
            lastFilter = filterType;

            if (filterType == FilterType.Closed)
                RepositoryMilestonesCollectionView.Filter = new Predicate<object>(x => ((MilestoneViewModel)x).Open == false);
            else if (filterType == FilterType.Open)
                RepositoryMilestonesCollectionView.Filter = new Predicate<object>(x => ((MilestoneViewModel)x).Open == true);
            else if (filterType == FilterType.None)
            {
                RepositoryMilestonesCollectionView.Filter = null;
            }
        }

        // Milestone Operations ==============================================================================================

        public async Task<bool> CreateNewMilestoneAsync()
        {
            // string title = "New Milestone created " + DateTime.Now.ToShortDateString();

            // Construct unique name
            int count = 1;
            while (RepositoryMilestonesVMs.Any(l => l.Title == "New Milestone " + count)) count += 1;
            string title = "New Milestone " + count;

            Milestone newMilestone = await gitController.CreateNewMilestoneAsync(gitRepositoryVm.GitRepository.Id, title);
            if (newMilestone == null) return false;
            MilestoneViewModel newMilestoneViewModel = new MilestoneViewModel(gitController, newMilestone, gitRepositoryVm.GitRepository.Id);
            RepositoryMilestonesVMs.Add(newMilestoneViewModel);
            return true;
        }

        public async Task GetAllMilestonesForRepoAsync()
        {
            RepositoryMilestonesVMs.Clear();
            RepositoryMilestonesVMs = await gitController.GetAllMilestonesForRepoAsync(gitRepositoryVm.GitRepository.Id);
            RepositoryMilestonesCollectionView = CollectionViewSource.GetDefaultView(RepositoryMilestonesVMs);
            //milestonesViewModel.RepositoryMilestonesVMs = milestonesViewModel.RepositoryMilestonesVMs;
        }

        void IRecipient<MilestoneTitleChangingMessage>.Receive(MilestoneTitleChangingMessage message)
        {
            // Checks changed Title against other local names and remote names (excluding itself) to check for duplicates
            if (RepositoryMilestonesVMs.Where(m => m.Title == message.NewTitle.Trim()).Count() > 0 ||
                RepositoryMilestonesVMs.Where(l => (l.GitMilestone.Title == message.NewTitle.Trim()) && message.Value.GitMilestone.Id != l.GitMilestone.Id).Count() > 0)
                message.Value.TitleUnique = false;
            else
                message.Value.TitleUnique = true;
        }

        #endregion End: Public Methods
    }
}