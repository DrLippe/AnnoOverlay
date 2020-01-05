using System;
using System.Windows;

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
