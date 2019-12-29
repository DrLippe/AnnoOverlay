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
using System.Windows.Shapes;

namespace AnnoOverlay
{
    /// <summary>
    /// Interaktionslogik für MainOverlay.xaml
    /// </summary>
    public partial class MainOverlay : Window
    {
        private CancellationTokenSource tokenSource;

        public MainOverlay()
        {
            InitializeComponent();
            InitializeGUI();
        }

        private void InitializeGUI()
        {
            // Set the position of the access button
            Height = Scale.ToScreenHeight(665);
            Width = Scale.ToScreenHeight(340);

            Top = SystemParameters.PrimaryScreenHeight - Height - 4;
            Left = SystemParameters.PrimaryScreenWidth - Width - 11;

            // Enable Developer mode
            string[] args = Environment.GetCommandLineArgs();
            if (args.Contains("/dev"))
            {
                Button_DevTools.IsEnabled = true;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            switch (Grid_Settings.IsVisible)
            {
                case true:
                    Grid_Settings.Visibility = Visibility.Hidden;
                    ItemsControl_Content.Visibility = Visibility.Visible;
                    break;
                case false:
                    ItemsControl_Content.Visibility = Visibility.Hidden;
                    Grid_Settings.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            MainWindow.viewModel.OverlayIsVisible = false;
        }

        private void Button_FullHouse_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.viewModel.UseFullHouse = !MainWindow.viewModel.UseFullHouse;
            Button_FullHouse.Content = MainWindow.viewModel.UseFullHouse ? "Aktuelle Bevölkerung" : "Maximalbevölkerung";
            Label_FullHouse.Content = MainWindow.viewModel.UseFullHouse ? "Maximalbevölkerung" : "Aktuelle Bevölkerung";
        }

        private void Button_RelinkIslands_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.islandLinker != null && MainWindow.islandLinker.ScanThread.IsAlive)
            {
                Button_RelinkIslands.Content = "Neu zuordnen";
                ListBox_Islands.Visibility = Visibility.Collapsed;
                StackPanel_HowTo.Visibility = Visibility.Visible;
                tokenSource.Cancel();
            }
            else
            {
                tokenSource = new CancellationTokenSource();

                MainWindow.islandLinker = new IslandLinker();
                MainWindow.islandLinker.ScanThread.Start(tokenSource.Token);

                Button_RelinkIslands.Content = "Abbrechen";
                StackPanel_HowTo.Visibility = Visibility.Collapsed;
                ListBox_Islands.Visibility = Visibility.Visible;
            }
        }

        private void ListBox_Islands_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get selected island in list and early opt out
            if ((ListBox_Islands.SelectedItems.Count != 1 && MainWindow.viewModel.ActiveIslandId == 0) || ListBox_Islands.Items.Count < 1)
                return;

            Button_RelinkIslands.Content = "Neu zuordnen";
            ListBox_Islands.Visibility = Visibility.Collapsed;
            StackPanel_HowTo.Visibility = Visibility.Visible;
            tokenSource.Cancel();

            MainWindow.settings.MapIslands(((Island)((ListBox)e.Source).Items[0]).Offset);
        }

        private void Button_CompactOverlay_Click(object sender, RoutedEventArgs e)
        {
            switch (MainWindow.compactOverlay.IsVisible)
            {
                case true:
                    MainWindow.compactOverlay.Hide();
                    Button_ColorConverter.IsEnabled = false;
                    break;
                case false:
                    MainWindow.compactOverlay.Show();
                    Button_ColorConverter.IsEnabled = true;
                    break;
            }
        }

        private void Button_ColorConverter_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.viewModel.ColorConverterEnabled = !MainWindow.viewModel.ColorConverterEnabled;
        }

        private void Button_DevTools_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.devTools.Show();
        }
    }
}
