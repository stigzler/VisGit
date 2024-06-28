using System.Windows;
using System.Windows.Controls;
using VisGitCore.Controllers;
using VisGitCore.Data;
using VisGitCore.ViewModels;

namespace VisGit.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            // Fixes
            CouldNotLoadFileOrAssemblyHack();

            InitializeComponent();
        }

        private void CouldNotLoadFileOrAssemblyHack()
        {
            // this solves MS bug:
            // https://stackoverflow.com/questions/78644729/visual-studio-extension-using-external-library-produces-could-not-load-file-or
            var assemblyLoadFix = new VisGitCore.ViewModels.MainViewModel();
        }

        private void VisGit_Initialized(object sender, EventArgs e)
        {
            // App functions
            MainViewModel mainViewModel = (MainViewModel)this.DataContext;
        }
    }
}