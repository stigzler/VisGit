using System.Windows;
using System.Windows.Controls;

namespace VisGit.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            VS.MessageBox.Show("VisGit", "Button clicked");
        }
    }
}