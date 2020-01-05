using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

namespace AnnoOverlayUpdater
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ViewModel viewModel = new ViewModel();
        private Updater updater = new Updater();
        public MainWindow()
        {
            string LocaText_AskForUpdate = CultureInfo.CurrentCulture.ToString() == "de-DE" ? "Es ist eine neue Version verfügbar. Update ausführen?" : "There is a new version available. Do you like to update?";
            string LocaText_Question = "Anno Overlay Updater";

            InitializeComponent();
            DataContext = viewModel;

            if (!updater.TryCheckVersion())
            {
                if (MessageBox.Show(LocaText_AskForUpdate, LocaText_Question, MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    updater.TryUpdate();
                    Process.Start(System.IO.Path.Combine(Environment.CurrentDirectory, "AnnoOverlay.exe"));
                }
            }

            Application.Current.Shutdown();
        }
    }
}
