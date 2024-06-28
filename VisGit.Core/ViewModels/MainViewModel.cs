using Community.VisualStudio.Toolkit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using VisGitCore.Controllers;
using VisGitCore.Services;

namespace VisGitCore.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        #region Properties =========================================================================================

        [ObservableProperty]
        private bool _userAuthenicated = false;

        [ObservableProperty]
        private string _visualStudioStatusText = string.Empty;

        // Repository objects
        [ObservableProperty]
        private ObservableCollection<RepositoryViewModel> _userRepositoryVMs;

        [ObservableProperty]
        private RepositoryViewModel _selectedRespositoryVM;

        // Milestone objects
        [ObservableProperty]
        private ObservableCollection<MilestoneViewModel> _repositoryMilestonesVMs = new ObservableCollection<MilestoneViewModel>();

        #endregion End: Properties

        #region Property Changed Methods =========================================================================================

        partial void OnVisualStudioStatusTextChanged(string value)
        {
            _ = VS.StatusBar.ShowMessageAsync(value);
        }

        partial void OnSelectedRespositoryVMChanged(RepositoryViewModel value)
        {
            _ = GetAllMilestonesForRepoAsync(value.GitRepository.Id);
        }

        #endregion End: Property Changed Methods

        #region Operational Vars =========================================================================================

        //public IUserSettings UserSettings;

        public string personalAccessToken = string.Empty;

        private GitService gitClient = new GitService();
        private GitController gitController;

        #endregion End: Operational Vars

        #region Commands =========================================================================================

        [RelayCommand]
        private void InitialiseView()
        {
            gitController = new GitController(gitClient);
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            UserRepositoryVMs = new ObservableCollection<RepositoryViewModel>();
        }

        [RelayCommand]
        private async Task AuthenticateUserAsync()
        {
            UserAuthenicated = await gitController.AuthenticateUserAsync(
                Encryption.DpapiToInsecureString(
                    Encryption.DpapiDecryptString(Properties.Settings.Default.PersonalAccessTokenEncrypted)));

            if (!UserAuthenicated)
            {
                UpdateVsStatusText("User authentication error. Check connection and PAT.");
                return;
            }

            UpdateVsStatusText("User authentication successful.");

            // Now get all repos for user
            await GetUserRespositoriesAsync();
        }

        #endregion End: Commands

        #region Private Methods =========================================================================================

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

        #endregion End: Private Methods

        public MainViewModel()
        {
        }
    }
}