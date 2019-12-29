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
using System.Windows.Shapes;

namespace AnnoOverlay
{
    /// <summary>
    /// Interaktionslogik für ClosingWindow.xaml
    /// </summary>
    public partial class ClosingWindow : Window
    {
        public ClosingWindow()
        {
            InitializeComponent();
            InitializeGUI();
        }

        private void InitializeGUI()
        {
            // Set the position of the access button
            Height = Scale.ToScreenHeight(241);
            Width = Scale.ToScreenHeight(340);
        }
        private void Button_Yes_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
            Application.Current.Shutdown();
        }

        private void Button_No_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
