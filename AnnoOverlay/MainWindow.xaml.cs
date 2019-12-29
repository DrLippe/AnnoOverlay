using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using AnnoOverlay.Helpers;

namespace AnnoOverlay
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Windows
        public static MainOverlay mainOverlay = new MainOverlay();
        public static CompactOverlay compactOverlay = new CompactOverlay();
        public static ClosingWindow closingWindow = new ClosingWindow();
        public static DevTools devTools = new DevTools();

        // ViewModel
        public static ViewModel viewModel = new ViewModel();

        // Config
        public static Settings settings;
        public static int inhabitantOffset = 0x04;
        public static bool colorConverterStatus = false;

        // Threads
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        public static IslandReader islandReader = new IslandReader();
        public static IslandLinker islandLinker;

        public MainWindow()
        {
            // Setting data context for windows
            DataContext = viewModel;
            mainOverlay.DataContext = viewModel;
            compactOverlay.DataContext = viewModel;
            devTools.DataContext = viewModel;

            InitializeGUI();
            InitializeComponent();

            islandReader.ScanThread.Start(tokenSource.Token);
        }

        private void InitializeGUI()
        {
            // Set the position of the access button
            Top = Scale.ToScreenHeight(100);
            Left = 0;

            Height = Scale.ToScreenHeight(42);
            Width = Height;
        }

        private void Button_MainWindow_OverlaySwitch_Click(object sender, RoutedEventArgs e)
        {
            switch(mainOverlay.IsVisible)
            {
                case true:
                    mainOverlay.Hide();
                    viewModel.OverlayIsVisible = false;
                    break;
                case false:
                    mainOverlay.Show();
                    viewModel.OverlayIsVisible = true;
                    break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            tokenSource.Cancel();
            Application.Current.Shutdown();
        }

        public static void CloseApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
