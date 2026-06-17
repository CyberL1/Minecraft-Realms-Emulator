using Core.Models;

namespace Core.Responses;

public class RealmConnection
{
    public required string Address { get; set; }
    public string? ResourcePackUrl { get; set; }
    public string? ResourcePackHash { get; set; }
    public required Region SessionRegionData { get; set; }
}