using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AnnoOverlay
{
    /// <summary>
    /// Interaktionslogik für MainOverlay.xaml
    /// </summary>
    public partial class MainOverlay : Window, IDisposable
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

            Top = Properties.Settings.Default.MainOverlay_Top;
            Left = Properties.Settings.Default.MainOverlay_Left;

            if (Properties.Settings.Default.MainOverlay_Visible)
                Show();

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
            Properties.Settings.Default.MainOverlay_Visible = false;
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
            var listBox = (ListBox)sender;

            // Get selected island in list and early opt out
            if (e.AddedItems.Count != 1 || MainWindow.viewModel.ActiveIslandId == 0 || listBox.Items.Count < 1)
                return;

            Button_RelinkIslands.Content = "Neu zuordnen";
            ListBox_Islands.Visibility = Visibility.Collapsed;
            StackPanel_HowTo.Visibility = Visibility.Visible;
            tokenSource.Cancel();

            var selectedItem = e.AddedItems[0] as Island;

            // MainWindow.settings.MapIslands(((Island)((ListBox)e.Source).Items[0]).Offset);
            MainWindow.settings.MapIslands(selectedItem.Offset, MainWindow.viewModel.ActiveIslandId);
        }

        private void Button_CompactOverlay_Click(object sender, RoutedEventArgs e)
        {
            switch (MainWindow.compactOverlay.IsVisible)
            {
                case true:
                    MainWindow.compactOverlay.Hide();
                    Properties.Settings.Default.CompactOverlay_Visible = false;
                    Button_ColorConverter.IsEnabled = false;
                    break;
                case false:
                    MainWindow.compactOverlay.Show();
                    Button_ColorConverter.IsEnabled = true;
                    Properties.Settings.Default.CompactOverlay_Visible = true;
                    break;
            }
        }

        private void Button_ColorConverter_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.viewModel.ColorConverterEnabled = !MainWindow.viewModel.ColorConverterEnabled;
            Properties.Settings.Default.CompactOverlay_ColorCoded = MainWindow.viewModel.ColorConverterEnabled;
        }

        private void Button_DevTools_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.devTools.Show();
        }

        private void Button_Hotkey_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.viewModel.Hotkey = "Warte auf Eingabe";
            MainWindow.waitHotkey = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // This is under development!
            if (!Environment.GetCommandLineArgs().Contains("/dev"))
                return;

            int guid = (int)((Button)sender).Tag;

            if (!MainWindow.viewModel.IslandFactoryCount.ContainsKey(MainWindow.viewModel.ActiveIslandId))
                MainWindow.viewModel.IslandFactoryCount.Add(MainWindow.viewModel.ActiveIslandId, new Dictionary<int, int>());
            if (!MainWindow.viewModel.IslandFactoryCount[MainWindow.viewModel.ActiveIslandId].ContainsKey(guid))
                MainWindow.viewModel.IslandFactoryCount[MainWindow.viewModel.ActiveIslandId].Add(guid, 0);
            MainWindow.viewModel.IslandFactoryCount[MainWindow.viewModel.ActiveIslandId][guid] += 1;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.MainOverlay_Top = Top;
            Properties.Settings.Default.MainOverlay_Left = Left;

            Properties.Settings.Default.Save();
        }

        public void Dispose()
        {
            tokenSource.Cancel();
            tokenSource.Dispose();
        }
    }
}
