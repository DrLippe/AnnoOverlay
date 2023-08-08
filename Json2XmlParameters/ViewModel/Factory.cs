namespace AnnoOverlay
{
    public class Factory
    {
        public string Name { get; set; }
        public int Guid { get; set; }
        public double? Tpmin { get; set; }
        public Output[] Outputs { get; set; }
        public Maintenance[] Maintenances { get; set; }
    }
}
