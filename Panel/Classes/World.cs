namespace Panel.Classes
{
    public class World
    {
        public int Id { get; set; }
        public string? Owner { get; set; }
        public string? OwnerUuid { get; set; }
        public string? Name { get; set; }
        public string? Motd { get; set; }
        public required string State { get; set; }
        public required string WorldType { get; set; }
        public int MaxPlayers { get; set; }
        public string? MinigameName { get; set; }
        public int? MinigameId { get; set; }
        public string? MinigameImage { get; set; }
        public int ActiveSlot { get; set; }
        public bool Member { get; set; }
    }
}
