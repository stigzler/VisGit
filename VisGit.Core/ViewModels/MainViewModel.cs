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
using System.Text;
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
    public partial class MainViewModel : ViewModelBase, IRecipient<ChangeViewMessage>, IRecipient<UpdateUserMessage>
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

        // Filter -------------------------------------------------------------

        [ObservableProperty]
        internal ObservableCollection<Filter> _filters;

        [ObservableProperty]
        private Filter _selectedFilter;

        // Sort -------------------------------------------------------------

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

        private HomeViewModel homeViewModel;

        private MilestonesViewModel milestonesViewModel;

        private LabelsViewModel labelsViewModel;

        #endregion End: Operational Vars

        #region Property Changed Methods =========================================================================================

        partial void OnVisualStudioStatusTextChanged(string value)
        {
            _ = VS.StatusBar.ShowMessageAsync(value);
        }

        partial void OnSelectedRespositoryVMChanged(RepositoryViewModel value)
        {
            //if (SelectedGitObject.Type == GitObjectType.Milestone)
            //{
            //    milestonesViewModel.gitRepositoryVm = value;
            //    _ = milestonesViewModel.GetAllMilestonesForRepoAsync();
            //}
            //else if (SelectedGitObject.Type == GitObjectType.Label)
            //{
            //    labelsViewModel.gitRepositoryVm = value;
            //    _ = labelsViewModel.GetAllLabelsForRepoAsync();
            //}

            milestonesViewModel.gitRepositoryVm = value;
            _ = milestonesViewModel.GetAllMilestonesForRepoAsync();
            labelsViewModel.gitRepositoryVm = value;
            _ = labelsViewModel.GetAllLabelsForRepoAsync();
        }

        partial void OnSelectedGitObjectChanged(GitObject gitObject)
        {
            switch (gitObject.Type)
            {
                case GitObjectType.Milestone:
                    CurrentViewModel = milestonesViewModel;
                    Filters = Filter.MilestoneFilters;
                    Sorts = Data.Models.Sort.MilestoneSorts;
                    break;

                case GitObjectType.Label:
                    CurrentViewModel = labelsViewModel;
                    Filters = Filter.LabelFilters;
                    Sorts = Data.Models.Sort.LabelSorts;
                    break;
            }
            if (Filters.Count > 0) SelectedFilter = Filters[0];
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

            UserRepositoryVMs = new ObservableCollection<RepositoryViewModel>(); // can i do this in the def?

            WeakReferenceMessenger.Default.Register<ChangeViewMessage>(this);
            WeakReferenceMessenger.Default.Register<UpdateUserMessage>(this);

            milestonesViewModel = new MilestonesViewModel(gitController, SelectedRespositoryVM);
            labelsViewModel = new LabelsViewModel(gitController, SelectedRespositoryVM);

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
            bool success;
            switch (CurrentViewModel)
            {
                case MilestonesViewModel:
                    StartOperation("Creating new Milestone");
                    success = await milestonesViewModel.CreateNewMilestoneAsync();
                    if (success) FinishOperation("New Milestone Created");
                    break;

                case LabelsViewModel:
                    StartOperation("Creating new Label");
                    success = await labelsViewModel.CreateNewLabelAsync();
                    if (success) FinishOperation("New Label Created");
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

        private async Task GetUserRespositoriesAsync()
        {
            UserRepositoryVMs.Clear();
            UserRepositoryVMs = await gitController.GetAllRepositoriesAsync();
            //milestonesViewModel.RepositoryMilestonesVMs = milestonesViewModel.RepositoryMilestonesVMs;
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
            if (SelectedSort == null) return;
            if (SelectedGitObject.Type == GitObjectType.Milestone)
                milestonesViewModel.SortMilestones(SelectedSort.SortType, SortDirection);
        }

        private void FilterGitObjectViews()
        {
            if (SelectedGitObject.Type == GitObjectType.Milestone)
                milestonesViewModel.FilterMilestones(SelectedFilter.FilterType);
        }

        void IRecipient<UpdateUserMessage>.Receive(UpdateUserMessage message)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(message.Value);
            if (message.Exception != null) sb.Append($": {message.Exception.Message}");
            FinishOperation(sb.ToString());
        }

        #endregion End: Private Methods

        #region Public Methods =========================================================================================

        // Constructor ==============================================================================================
        public MainViewModel()
        {
        }

        #endregion End: Public Methods ---------------------------------------------------------------------------------
    }
}