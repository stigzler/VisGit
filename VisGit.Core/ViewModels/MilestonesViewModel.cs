using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
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

        [RelayCommand]
        private async Task DeleteMilestoneAsync()
        {
            await SelectedMilestoneViewModel.DeleteMilestoneAsync();
            MainViewModel.RepositoryMilestonesVMs.Remove(SelectedMilestoneViewModel);
            if (MainViewModel.RepositoryMilestonesVMs.Count > 0) SelectedMilestoneViewModel = MainViewModel.RepositoryMilestonesVMs[0];
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