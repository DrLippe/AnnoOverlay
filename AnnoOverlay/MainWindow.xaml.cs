using AnnoOverlay.Helpers;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace AnnoOverlay
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        // Windows
        public static MainOverlay mainOverlay = new MainOverlay();
        public static CompactOverlay compactOverlay = new CompactOverlay();
        public static ClosingWindow closingWindow = new ClosingWindow();
        public static DevTools devTools = new DevTools();

        // Hotkey
        private readonly LowLevelKeyboardListener keyboardListener;
        public static bool waitHotkey = false;

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

            // Hotkey
            viewModel.Hotkey = Properties.Settings.Default.Hotkey;
            keyboardListener = new LowLevelKeyboardListener();
            keyboardListener.OnKeyPressed += KeyboardListener_OnKeyPressed;
            keyboardListener.HookKeyboard();

        }

        private void KeyboardListener_OnKeyPressed(object sender, KeyPressedArgs e)
        {
            if (waitHotkey)
            {
                waitHotkey = false;
                viewModel.Hotkey = $"Hotkey: {e.KeyPressed}";
                Properties.Settings.Default.Hotkey = viewModel.Hotkey.ToString();
                return;
            }

            if (e.KeyPressed == (Key)Enum.Parse(typeof(Key), viewModel.Hotkey.Remove(0, 8)))
                Button_MainWindow_OverlaySwitch_Click(new object(), new RoutedEventArgs());
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
            switch (mainOverlay.IsVisible)
            {
                case true:
                    mainOverlay.Hide();
                    viewModel.OverlayIsVisible = false;
                    Properties.Settings.Default.MainOverlay_Visible = false;
                    break;
                case false:
                    mainOverlay.Show();
                    viewModel.OverlayIsVisible = true;
                    Properties.Settings.Default.MainOverlay_Visible = true;
                    break;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            keyboardListener.UnHookKeyboard();
            tokenSource.Cancel();
            Application.Current.Shutdown();
        }

        public static void CloseApplication()
        {
            Application.Current.Shutdown();
        }

        public void Dispose()
        {
            tokenSource.Dispose();
        }
    }
}
