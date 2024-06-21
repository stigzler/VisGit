using Community.VisualStudio.Toolkit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

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

        #region Property Changed Methods =========================================================================================

        partial void OnVisualStudioStatusTextChanged(string value)
        {
            _ = VS.StatusBar.ShowMessageAsync("VisGit: " + value);
        }

        #endregion End: Property Changed Methods

        #region Commands =========================================================================================

        [RelayCommand]
        private void InitialiseView()
        {
            //VisualStudioStatusText = "VisGit Initialised";
        }

        #endregion End: Commands

        public MainViewModel()
        {
        }
    }
}