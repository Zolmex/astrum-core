namespace Common.Resources.World
{
    /// <summary>
    /// Contains important information about a specific world type
    /// </summary>
    public struct WorldConfig
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int Difficulty { get; set; }
        public int MaxPlayers { get; set; }
        public int Blocksight { get; set; }
        public int Background { get; set; }
        public bool AllowTeleport { get; set; }
        public bool ShowDisplays { get; set; }
        public string[] Maps { get; set; }
        public bool LongLasting { get; set; }
    }
}
