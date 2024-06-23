using System.Windows;
using System.Windows.Controls;
using VisGit.Core.Data;
using VisGit.Core.ViewModels;

namespace VisGit.Views
{
    public partial class MainView : UserControl
    {
        private MainViewModel viewModel;

        public MainView()
        {
            CouldNotLoadFileOrAssemblyHack();

            InitializeComponent();

            viewModel = (MainViewModel)this.DataContext;

            //var dave = OptionsPage.Instance;

            //viewModel.UserSettings = new UserSettings();
            //viewModel.UserSettings.PersonalAccessToken = dave.PersonalAccessToken;
        }

        private void CouldNotLoadFileOrAssemblyHack()
        {
            // this solves MS bug:
            // https://stackoverflow.com/questions/78644729/visual-studio-extension-using-external-library-produces-could-not-load-file-or
            var assemblyLoadFix = new VisGit.Core.ViewModels.MainViewModel();
        }

        private void VisGit_Initialized(object sender, EventArgs e)
        {
        }
    }
}