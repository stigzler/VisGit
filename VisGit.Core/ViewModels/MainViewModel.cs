using Community.VisualStudio.Toolkit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
        private object _selectedGitObject;

        [ObservableProperty]
        private ObservableCollection<GitObject> _gitObjects = GitObject.GitObjects;

        [ObservableProperty]
        internal ObservableCollection<Filter> _milestonesFilters = Filter.MilestoneFilters;

        // Repository objects -------------------------------------------------------------
        [ObservableProperty]
        private ObservableCollection<RepositoryViewModel> _userRepositoryVMs;

        [ObservableProperty]
        private RepositoryViewModel _selectedRespositoryVM;

        // Milestone objects -------------------------------------------------------------
        [ObservableProperty]
        private ObservableCollection<MilestoneViewModel> _repositoryMilestonesVMs = new ObservableCollection<MilestoneViewModel>();

        #endregion End: Properties

        #region Operational Vars =========================================================================================

        private UserSettings userSettings;

        private GitService gitClient = new GitService();

        private GitController gitController;

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

        partial void OnSelectedGitObjectChanged(object value)
        {
            GitObject item = (GitObject)value;
            switch (item.Type)
            {
                case GitObjectType.Milestone:
                    CurrentViewModel = milestonesViewModel;
                    if (RepositoryMilestonesVMs.Count > 0) milestonesViewModel.SelectedMilestoneViewModel = RepositoryMilestonesVMs[0];
                    break;
            }

            Debug.WriteLine($"Git Object changed: {item.Name}");
        }

        partial void OnRepositoryMilestonesVMsChanged(ObservableCollection<MilestoneViewModel> oldValue, ObservableCollection<MilestoneViewModel> newValue)
        {
            if (RepositoryMilestonesVMs.Count > 0) milestonesViewModel.SelectedMilestoneViewModel = RepositoryMilestonesVMs.First();
        }

        #endregion End: Property Changed Methods

        #region Commands =========================================================================================

        [RelayCommand]
        private void RunTest()
        {
            //ViewModelsController.CurrentViewModel = new MilestonesViewModel(ViewModelsController);
            CurrentViewModel = new MilestonesViewModel(this);
        }

        [RelayCommand]
        private async Task InitialiseViewAsync()
        {
            userSettings = await UserSettings.GetLiveInstanceAsync();

            gitController = new GitController(gitClient);

            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            UserRepositoryVMs = new ObservableCollection<RepositoryViewModel>(); // can i do this in the def?

            WeakReferenceMessenger.Default.Register<ChangeViewMessage>(this);

            milestonesViewModel = new MilestonesViewModel(this);
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
        }

        private async Task GetAllMilestonesForRepoAsync(long repositoryId)
        {
            RepositoryMilestonesVMs.Clear();
            RepositoryMilestonesVMs = await gitController.GetAllMilestonesForRepoAsync(repositoryId);
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

        #endregion End: Private Methods

        public MainViewModel()
        {
        }
    }
}