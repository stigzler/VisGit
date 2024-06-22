using Community.VisualStudio.Toolkit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using VisGit.Core.Controllers;
using VisGit.Core.Services;
using System.Diagnostics;

namespace VisGit.Core.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        #region Properties =========================================================================================

        [ObservableProperty]
        private bool _userAuthenicated = false;

        [ObservableProperty]
        private string _visualStudioStatusText = string.Empty;

        #endregion End: Properties

        #region Operational Vars =========================================================================================

        private UserSettings userSettings;
        private GitService gitClient;
        private GitController gitController;

        #endregion End: Operational Vars

        #region Property Changed Methods =========================================================================================

        partial void OnVisualStudioStatusTextChanged(string value)
        {
            _ = VS.StatusBar.ShowMessageAsync("VisGit: " + value);
        }

        #endregion End: Property Changed Methods

        #region Commands =========================================================================================

        [RelayCommand]
        private async Task InitialiseViewAsync()
        {
            userSettings = await UserSettings.CreateAsync();
            gitClient = new GitService(userSettings);
            gitController = new GitController(gitClient);

            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        [RelayCommand]
        private async Task AuthenticateUserAsync()
        {
            UserAuthenicated = await gitController.AuthenticateUserAsync();
            if (UserAuthenicated) UpdateVsStatusText("Login Successful");
            else UpdateVsStatusText("Login error. Check connection and PAT.");
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

        #endregion End: Private Methods

        public MainViewModel()
        {
        }
    }
}