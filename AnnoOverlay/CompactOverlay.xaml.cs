using System.Windows;
using System.Windows.Input;

namespace AnnoOverlay
{
    /// <summary>
    /// Interaktionslogik für CompactOverlay.xaml
    /// </summary>
    public partial class CompactOverlay : Window
    {
        public CompactOverlay()
        {
            InitializeComponent();
            InitializeGUI();
        }

        private void InitializeGUI()
        {
            // Set the position of the access button
            Height = Scale.ToScreenHeight(200);
            Width = Scale.ToScreenHeight(220);

            Top = Properties.Settings.Default.CompactOverlay_Top;
            Left = Properties.Settings.Default.CompactOverlay_Left;

            if (Properties.Settings.Default.CompactOverlay_Visible)
            {
                Show();
                MainWindow.mainOverlay.Button_ColorConverter.IsEnabled = true;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.CompactOverlay_Top = Top;
            Properties.Settings.Default.CompactOverlay_Left = Left;

            Properties.Settings.Default.Save();
        }
    }
}
