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
        }

        private void CloseIssueLV_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (OpenCloseIssueLV.SelectedItem == null) return;
            OpenCloseIssuePopup.IsOpen = false;
        }

        private void LockIssueLV_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (LockIssueLV.SelectedItem == null) return;
            LockIssuePopup.IsOpen = false;
        }

        private void LockIssuePopup_Closed(object sender, EventArgs e)
        {
            LockIssueLV.SelectedItem = null;
        }

        private void OpenCloseIssuePopup_Closed(object sender, EventArgs e)
        {
            OpenCloseIssueLV.SelectedItem = null;
        }

        private void AddCommentBT_Click(object sender, RoutedEventArgs e)
        {
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

        /// <summary>
        /// Implements text zoom (CTRL+Mouse Wheel) in comment edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommentEditTB_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                TextBox tb = sender as TextBox;
                if (e.Delta > 0 && tb.FontSize < 100) tb.FontSize += 1;
                else if (e.Delta < 0 && tb.FontSize > 10) tb.FontSize -= 1;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Implements Text Zoom (CTRL+Mouse Wheel) in Issue Post Edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditIssuePostTB_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                TextBox tb = sender as TextBox;
                if (e.Delta > 0 && tb.FontSize < 100) tb.FontSize += 1;
                else if (e.Delta < 0 && tb.FontSize > 10) tb.FontSize -= 1;
                e.Handled = true;
            }
        }

        private void AssignUserLV_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
            AssignUserPopup.IsOpen = false;
        }

        //private void CommentEditTB_PreviewKeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Tab && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
        //    {
        //        Debug.WriteLine("Shift + Tab");
        //        e.Handled = true;
        //    }
        //}
    }
}