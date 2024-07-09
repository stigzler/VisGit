using System;
using System.Collections.Generic;
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
    }
}