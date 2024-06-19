using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace VisGit.ViewModels
{
    internal partial class MainViewModel : ObservableObject
    {
        #region PROPERTIES =========================================================================================

        [ObservableProperty]
        private bool _userAuthenicated = false;

        #endregion End: Properties

        #region COMMANDS =========================================================================================

        [RelayCommand]
        private void InitialiseView()
        {
            //UserAuthenicated = true;
        }

        #endregion End: Commands

        public MainViewModel()
        {
        }
    }
}