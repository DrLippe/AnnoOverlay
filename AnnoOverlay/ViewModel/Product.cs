namespace AnnoOverlay
{
    public class Product : BaseViewModel
    {
        public string Name { get; set; }
        public int Guid { get; set; }
        public string Icon { get; set; }
        public int[] Producers { get; set; }
        public double Amount { get; set; }
    }
}
