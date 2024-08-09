using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VisGit.Views
{
    /// <summary>
    /// Interaction logic for MilestonesView.xaml
    /// </summary>
    public partial class IssuesView : UserControl
    {
        public IssuesView()
        {
            InitializeComponent();
        }

        private void MiliestonesLV_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }

        private void Calendar_SelectedDatesChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //ContextPopup.IsOpen = false;
        }

        private void AssignedUsersLV_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var dave = e.AddedItems;
        }

        private void MarkdownViewer_Click(object sender, RoutedEventArgs e)
        {
        }

        private void MarkdownViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        private void MarkdownViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);

            eventArg.RoutedEvent = UIElement.MouseWheelEvent;

            eventArg.Source = sender;

            var parent = ((Control)sender).Parent as UIElement;

            parent.RaiseEvent(eventArg);
        }

        private void TextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);

            eventArg.RoutedEvent = UIElement.MouseWheelEvent;

            eventArg.Source = CommentsLV;

            CommentsLV.RaiseEvent(eventArg);

            Debug.WriteLine("Scrolled");

            //var parent = ((Control)sender).Parent as UIElement;

            //parent.RaiseEvent(eventArg);
        }

        private void CloseIssueLV_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (OpenCloseIssueLV.SelectedItem == null) return;
            OpenCloseIssuePopup.IsOpen = false;
            //OpenCloseIssueLV.SelectedItem = null;
        }

        private void LockIssueLV_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (LockIssueLV.SelectedItem == null) return;
            LockIssuePopup.IsOpen = false;
            //LockIssueLV.SelectedItem = null;
        }

        private void LockIssuePopup_Closed(object sender, EventArgs e)
        {
            LockIssueLV.SelectedItem = null;
        }

        private void OpenCloseIssuePopup_Closed(object sender, EventArgs e)
        {
            OpenCloseIssueLV.SelectedItem = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void AddCommentBT_Click(object sender, RoutedEventArgs e)
        {
            //CommentsLV.SelectedIndex = Math.Max(CommentsLV.Items.Count - 1, 0);
            //CommentsLV.Items.MoveCurrentToLast();
            //CommentsLV.ScrollIntoView(CommentsLV.Items.CurrentItem);
            CommentsLV.Items.MoveCurrentToLast();
            CommentsLV.ScrollIntoView(CommentsLV.Items.CurrentItem);
        }

        private void MarkdownViewer_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //try { Process.Start(e.Uri.ToString()); }
            //catch { }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try { Process.Start(e.Parameter.ToString()); }
            catch { }
        }

        private void OpenHyperlink(object sender, ExecutedRoutedEventArgs e)
        {
            try { Process.Start(e.Parameter.ToString()); }
            catch { }
        }
    }
}