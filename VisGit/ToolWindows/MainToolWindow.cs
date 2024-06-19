using Microsoft.VisualStudio.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace VisGit
{
    public class MainToolWindow : BaseToolWindow<MainToolWindow>
    {
        public override string GetTitle(int toolWindowId) => "VisGit";

        public override Type PaneType => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            return Task.FromResult<FrameworkElement>(new Views.MainView());
        }

        [Guid("ae60ce0f-b778-40f7-8e88-3da09e36fb5c")]
        internal class Pane : ToolkitToolWindowPane
        {
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.ToolWindow;
            }
        }
    }
}