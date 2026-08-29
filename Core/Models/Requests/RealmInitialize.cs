namespace Core.Models.Requests;

public class RealmInitialize
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
