using System.Windows;
using System.Windows.Controls;
using VisGitCore.ViewModels;

namespace VisGit.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
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
        }
    }
}