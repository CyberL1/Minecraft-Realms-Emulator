namespace Panel.Classes
{
    public class World
    {
        public int Id { get; set; }
        public string? Owner { get; set; }
        public string? OwnerUUID { get; set; }
        public string? Name { get; set; }
        public string? Motd { get; set; }
        public string State { get; set; } = null!;
        public string WorldType { get; set; } = null!;
        public int MaxPlayers { get; set; }
        public string? MinigameName { get; set; }
        public int? MinigameId { get; set; }
        public string? MinigameImage { get; set; }
        public int ActiveSlot { get; set; }
        public bool Member { get; set; }
    }
}
