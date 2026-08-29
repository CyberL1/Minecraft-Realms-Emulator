namespace Core.Models.Responses;

public class Player
{
    public required string Uuid { get; set; }
    public required string Name { get; set; }
    public required bool Operator { get; set; }
    public required bool Accepted { get; set; }
}
