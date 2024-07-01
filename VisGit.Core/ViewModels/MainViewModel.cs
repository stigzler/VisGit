using Community.VisualStudio.Toolkit;
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
using System.Windows.Data;
using System.Windows.Navigation;
using VisGitCore.Controllers;
using VisGitCore.Data.Models;
using VisGitCore.Enums;
using VisGitCore.Messages;
using VisGitCore.Services;

namespace VisGitCore.ViewModels
{
    public partial class MainViewModel : ViewModelBase, IRecipient<ChangeViewMessage>
    {
        #region Properties =========================================================================================

        // UI Properties -------------------------------------------------------------
        [ObservableProperty]
        private bool _userAuthenicated = false;

        [ObservableProperty]
        private string _visualStudioStatusText = string.Empty;

        [ObservableProperty]
        private bool _operationInProgress = false;

        [ObservableProperty]
        public ViewModelBase _currentViewModel;

        [ObservableProperty]
        private ObservableCollection<GitObject> _gitObjects = GitObject.GitObjects;

        [ObservableProperty]
        private GitObject _selectedGitObject;

        [ObservableProperty]
        public int _repositoryDropDownWidth;

        // Repository objects -------------------------------------------------------------
        [ObservableProperty]
        private ObservableCollection<RepositoryViewModel> _userRepositoryVMs;

        [ObservableProperty]
        private RepositoryViewModel _selectedRespositoryVM;

        // Milestone objects -------------------------------------------------------------
        [ObservableProperty]
        private ObservableCollection<MilestoneViewModel> _repositoryMilestonesVMs = new ObservableCollection<MilestoneViewModel>();

        [ObservableProperty]
        private ICollectionView _repositoryMilestonesView;

        // Filter

        [ObservableProperty]
        internal ObservableCollection<Filter> _filters;

        [ObservableProperty]
        private Filter _selectedFilter;

        // Sort

        [ObservableProperty]
        private ObservableCollection<Data.Models.Sort> _sorts;

        [ObservableProperty]
        private Data.Models.Sort _selectedSort;

        [ObservableProperty]
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;

        #endregion End: Properties ---------------------------------------------------------------------------------------

        #region Operational Vars =========================================================================================

        private UserSettings userSettings;

        private GitService gitClient = new GitService();

        private GitController gitController;

        // DataViews

        // View Models

        private HomeViewModel homeViewModel;
        private MilestonesViewModel milestonesViewModel;

        #endregion End: Operational Vars

        #region Property Changed Methods =========================================================================================

        partial void OnVisualStudioStatusTextChanged(string value)
        {
            _ = VS.StatusBar.ShowMessageAsync(value);
        }

        partial void OnSelectedRespositoryVMChanged(RepositoryViewModel value)
        {
            _ = GetAllMilestonesForRepoAsync(value.GitRepository.Id);
        }

        partial void OnSelectedGitObjectChanged(GitObject gitObject)
        {
            switch (gitObject.Type)
            {
                case GitObjectType.Milestone:
                    CurrentViewModel = milestonesViewModel;
                    Filters = Filter.MilestoneFilters;
                    Sorts = Data.Models.Sort.MilestoneSorts;
                    if (RepositoryMilestonesVMs.Count > 0)
                    {
                        milestonesViewModel.SelectedMilestoneViewModel = RepositoryMilestonesVMs[0];
                        SelectedFilter = Filters.First();
                        SelectedSort = Sorts.First();
                    }
                    break;

                case GitObjectType.Label:
                    Filters = Filter.LabelFilters;
                    Sorts = Data.Models.Sort.LabelSorts;
                    break;
            }
            if (Filters.Count > 0) SelectedFilter = Filters[0];
        }

        partial void OnRepositoryMilestonesVMsChanged(ObservableCollection<MilestoneViewModel> oldValue, ObservableCollection<MilestoneViewModel> newValue)
        {
            if (RepositoryMilestonesVMs.Count > 0) milestonesViewModel.SelectedMilestoneViewModel = RepositoryMilestonesVMs.First();
        }

        partial void OnSelectedFilterChanged(Filter oldValue, Filter newValue)
        {
            if (newValue == null) return;
            FilterGitObjectViews();
        }

        partial void OnSortDirectionChanged(ListSortDirection oldValue, ListSortDirection newValue)
        {
            SortGitObjectViews();
        }

        partial void OnSelectedSortChanged(Data.Models.Sort oldValue, Data.Models.Sort newValue)
        {
            SortGitObjectViews();
        }

        #endregion End: Property Changed Methods

        #region Commands =========================================================================================

        [RelayCommand]
        private void RunTest()
        {
            //ViewModelsController.CurrentViewModel = new MilestonesViewModel(ViewModelsController);
            // CurrentViewModel = new MilestonesViewModel();
            //RepositoryMilestonesView.SortDescriptions.Add()
        }

        [RelayCommand]
        private async Task InitialiseViewAsync()
        {
            userSettings = await UserSettings.GetLiveInstanceAsync();

            gitController = new GitController(gitClient);

            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            UserRepositoryVMs = new ObservableCollection<RepositoryViewModel>(); // can i do this in the def?

            WeakReferenceMessenger.Default.Register<ChangeViewMessage>(this);

            milestonesViewModel = new MilestonesViewModel();

            RepositoryDropDownWidth = userSettings.RepositoryDropDownWidth;

            UserSettings.Saved += UserSettings_Saved;
        }

        private void UserSettings_Saved(UserSettings obj)
        {
            // Update any UIElements with related settings:
            RepositoryDropDownWidth = userSettings.RepositoryDropDownWidth;
        }

        [RelayCommand]
        private async Task AuthenticateUserAsync()
        {
            StartOperation("Authenticating User");

            UserAuthenicated = await gitController.AuthenticateUserAsync(userSettings.PersonalAccessToken);

            if (!UserAuthenicated)
            {
                FinishOperation("User authentication error. Check connection and PAT.");
                return;
            }

            UpdateVsStatusText("User authentication successful. Getting Repositories...");

            // Now get all repos for user
            await GetUserRespositoriesAsync();

            FinishOperation("User authenticated and Repositories populated.");
        }

        [RelayCommand]
        private async Task AddItemAsync()
        {
            switch (CurrentViewModel)
            {
                case MilestonesViewModel:
                    Debug.WriteLine("Add Milestone!");
                    string title = "New Milestone created " + DateTime.Now.ToShortDateString();
                    Milestone newMilestone = await gitController.CreateNewMilestoneAsync(SelectedRespositoryVM.GitRepository.Id, title);
                    MilestoneViewModel newMilestoneViewModel = new MilestoneViewModel(gitController, newMilestone, SelectedRespositoryVM.GitRepository.Id);
                    RepositoryMilestonesVMs.Add(newMilestoneViewModel);
                    break;
            }
        }

        #endregion End: Commands

        #region Private Methods =========================================================================================

        private void StartOperation(string statusMessage)
        {
            UpdateVsStatusText(statusMessage);
            OperationInProgress = true;
        }

        private void FinishOperation(string statusMessage)
        {
            UpdateVsStatusText(statusMessage);
            OperationInProgress = false;
        }

        private void UpdateVsStatusText(string message)
        {
            VisualStudioStatusText = $"VisGit: {message}";
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Debug.WriteLine($"TASK EXCEPTION: {e.Exception.Message}");
            //SingleLineFeedback = e.Exception.Message;
        }

        private async Task GetUserRespositoriesAsync()
        {
            UserRepositoryVMs.Clear();
            UserRepositoryVMs = await gitController.GetAllRepositoriesAsync();
            milestonesViewModel.RepositoryMilestonesVMs = RepositoryMilestonesVMs;
        }

        private async Task GetAllMilestonesForRepoAsync(long repositoryId)
        {
            RepositoryMilestonesVMs.Clear();
            RepositoryMilestonesVMs = await gitController.GetAllMilestonesForRepoAsync(repositoryId);
            RepositoryMilestonesView = CollectionViewSource.GetDefaultView(RepositoryMilestonesVMs);
            milestonesViewModel.RepositoryMilestonesVMs = RepositoryMilestonesVMs;
        }

        void IRecipient<ChangeViewMessage>.Receive(ChangeViewMessage message)
        {
            switch (message.Value)
            {
                case ViewRequest.Home:
                    CurrentViewModel = homeViewModel;
                    break;
            }
        }

        private void SortGitObjectViews()
        {
            if (SelectedGitObject.Type == GitObjectType.Milestone)
                milestonesViewModel.SortMilestones(SelectedSort.SortType, SortDirection);
        }

        private void FilterGitObjectViews()
        {
            if (SelectedGitObject.Type == GitObjectType.Milestone)
                milestonesViewModel.FilterMilestones(SelectedFilter.FilterType);
        }

        #endregion End: Private Methods

        public MainViewModel()
        {
        }
    }
}