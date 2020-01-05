using System.Collections.Generic;
using System.ComponentModel;

namespace AnnoOverlay
{
    public class ViewModel : BaseViewModel
    {
        #region public properties

        public bool OverlayIsVisible { get; set; } = false;

        public bool ColorConverterEnabled { get; set; } = Properties.Settings.Default.CompactOverlay_ColorCoded;

        public int ActiveIslandId { get; set; }

        public bool UseFullHouse { get; set; } = false;

        public Dictionary<int, int> IslandLinks { get; set; }

        public Dictionary<int, int>[] IslandLinksByZone { get; set; }

        public Dictionary<int, Dictionary<int, int>> IslandFactoryCount { get; set; }

        public Island[] Islands { get; set; }

        public Parameters Parameters { get; set; }

        public Settings Settings { get; set; }

        public string Hotkey { get; set; }
        #endregion

        public ViewModel()
        {

        }
    }

    public class BaseViewModel : INotifyPropertyChanged
    {
#pragma warning disable CS0067 // Das Ereignis "BaseViewModel.PropertyChanged" wird nie verwendet.
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067 // Das Ereignis "BaseViewModel.PropertyChanged" wird nie verwendet.
    }
}