using System.Windows;

namespace AnnoOverlay
{
    /// <summary>
    /// Interaktionslogik für DevTools.xaml
    /// </summary>
    public partial class DevTools : Window
    {
        public DevTools()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
