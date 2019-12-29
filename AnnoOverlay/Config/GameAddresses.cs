namespace AnnoOverlay
{
    /// <summary>
    /// Contains the game adresses
    /// </summary>
    public class GameAddresses
    {
        /// <summary>
        /// Contains the target Game Version
        /// </summary>
        public string GameVersion { get; set; }
        /// <summary>
        /// Contains the pointer path to island populations
        /// </summary>
        public int[] IslandPopulationPointer { get; set; }

        /// <summary>
        /// Contains the pointers to population guids
        /// </summary>
        public int[] IslandPopulationPosPtr { get; set; }

        /// <summary>
        /// Contains the pointer path to the selected island
        /// </summary>
        public int[] IslandIdPointer { get; set; }
    }
}
