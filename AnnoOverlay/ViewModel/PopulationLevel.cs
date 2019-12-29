namespace AnnoOverlay
{
    public class PopulationLevel : BaseViewModel
    {
        public string Name { get; set; }
        public int Guid { get; set; }
        public int Amount { get; set; }
        public int FullHouse { get; set; }
        public Need[] Needs { get; set; }
    }
}
