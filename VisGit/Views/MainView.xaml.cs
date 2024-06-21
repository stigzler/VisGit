using System.Windows;
using System.Windows.Controls;
using VisGit.Core.ViewModels;

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
            var assemblyLoadFix = new VisGit.Core.ViewModels.MainViewModel();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            VS.MessageBox.Show("VisGit", "Button clicked");
        }

        private void VisGit_Initialized(object sender, EventArgs e)
        {
        }
    }
}