using Semver;

namespace Core.Helpers;

public class MinecraftVersion
{
    public readonly SemVersion Version;

    public MinecraftVersion(string versionString)
    {
        Version = SemVersion.Parse(versionString, SemVersionStyles.OptionalPatch);
    }

    public int CompareTo(MinecraftVersion versionToCompare)
    {
        return SemVersion.ComparePrecedence(Version, versionToCompare.Version);
    }
}
