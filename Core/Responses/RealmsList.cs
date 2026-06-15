using Core.Models;

namespace Core.Responses;

public class RealmsList
{
    public required List<Realm> Servers { get; set; }
}
