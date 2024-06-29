using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using VisGitCore.Messages;

namespace VisGitCore.ViewModels
{
    public partial class MilestonesViewModel : ViewModelBase
    {
        #region Properties =========================================================================================

        [ObservableProperty]
        private MainViewModel _mainViewModel;

        [ObservableProperty]
        private MilestoneViewModel _selectedMilestoneViewModel;

        #endregion End: Properties

        #region Commands =========================================================================================

        [RelayCommand]
        private void NavigateHome()
        {
            Debug.WriteLine(MainViewModel.RepositoryMilestonesVMs.Count.ToString());
            WeakReferenceMessenger.Default.Send(new ChangeViewMessage(Enums.ViewRequest.Home));
        }

        private void Initialise(MilestoneViewModel viewModel)
        {
        }

        #endregion End: Commands

        public MilestonesViewModel(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
        }

        public MilestonesViewModel()
        {
        }
    }
}