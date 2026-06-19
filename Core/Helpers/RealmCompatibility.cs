namespace Core.Helpers;

public class RealmCompatibility
{
    public static string CheckRealmCompatibility(string playerVersion, string realmVersion)
    {
        var playerVersionParsed = new MinecraftVersion(playerVersion);
        var realmVersionParsed = new MinecraftVersion(realmVersion);
        
        return playerVersionParsed.CompareTo(realmVersionParsed) switch
        {
            < 0 => nameof(Enums.RealmCompatibility.NEEDS_UPGRADE),
            > 0 => nameof(Enums.RealmCompatibility.NEEDS_DOWNGRADE),
            0 => nameof(Enums.RealmCompatibility.COMPATIBLE)
        };
    }
}
