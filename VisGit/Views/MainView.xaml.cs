using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using VisGitCore.Controllers;
using VisGitCore.Data;
using VisGitCore.ViewModels;
using System.IO;

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
            // Don't ask - bloody shoddy VS - couldn't use other fix below because Annotations has no members
            // https://www.reddit.com/r/esapi/comments/1bq696k/could_not_load_file_or_assembly_with_mvvm/
            string dllFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string dllPath = Path.GetDirectoryName(dllFilePath);
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                if (args.Name.Contains("System.ComponentModel.Annotations"))
                {
                    // Specify the path to the assembly
                    string assemblyPath = Path.Combine(dllPath, @"System.ComponentModel.Annotations.dll");
                    return Assembly.LoadFrom(assemblyPath);
                }
                return null; // or handle other assemblies if necessary
            };

            var fix1 = new Markdig.Helpers.NewLine();
            var fix2 = new Markdig.Wpf.MarkdownViewer();

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