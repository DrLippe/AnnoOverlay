using System.Collections.Generic;
using System.ComponentModel;

namespace AnnoOverlay
{
    public class ViewModel : BaseViewModel
    {
        #region public properties

        public bool OverlayIsVisible { get; set; } = false;

        public bool ColorConverterEnabled { get; set; } = false;

        public int ActiveIslandId { get; set; }

        public bool UseFullHouse { get; set; } = false;

        public Dictionary<int, int> IslandLinks { get; set; }
        public Dictionary<int, int>[] IslandLinksByZone { get; set; }

        public Island[] Islands { get; set; }

        public Parameters Parameters { get; set; }

        public Settings Settings { get; set; }

        #endregion

        public ViewModel()
        {

        }
    }

    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}