using CommunityToolkit.Mvvm.Messaging;
using Dsafa.WpfColorPicker;
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
using VisGitCore.Messages;

namespace VisGit.Views
{
    /// <summary>
    /// Interaction logic for MilestonesView.xaml
    /// </summary>
    public partial class LabelsView : UserControl
    {
        public LabelsView()
        {
            IncludeAssemblies();
            InitializeComponent();
        }

        private void IncludeAssemblies()
        {
            ColorPicker colorPicker = new ColorPicker();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ColorBT_Click(object sender, RoutedEventArgs e)
        {
            //string r = SelectedLabelViewModel.Color.ToString().Substring(0, 2);
            //string g = SelectedLabelViewModel.Color.ToString().Substring(2, 2);
            //string b = SelectedLabelViewModel.Color.ToString().Substring(4, 2);

            var dialog = new Dsafa.WpfColorPicker.ColorPickerDialog(Colors.Red);
            //dialog.Owner = this;

            //dialog.Background = VS.Services.GetFontAndColorStorageAsync().
            //dialog.Foreground = new SolidColorBrush(Colors.Gainsboro);

            dialog.Background = new SolidColorBrush(Color.FromArgb(255, 30, 30, 30));
            dialog.Foreground = new SolidColorBrush(Colors.Gainsboro);

            var res = dialog.ShowDialog();
            //if (res.HasValue && res.Value)
            //{
            //    Color = dialog.Color;
            //}
        }
    }
}