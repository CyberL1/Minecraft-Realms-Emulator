using System.Text.RegularExpressions;

namespace Minecraft_Realms_Emulator.Shared.Helpers
{
    public class MinecraftVersionParser
    {
        public class MinecraftVersion : IComparable<MinecraftVersion>
        {
            public int Major { get; private set; }
            public int Minor { get; private set; }
            public int Patch { get; private set; }
            public string PreRelease { get; private set; }
            public string Snapshot { get; private set; }
            
            private static readonly Regex VersionRegex = new(@"^(\d+)\.(\d+)(\.(\d+))?(-[a-zA-Z0-9\-]+)?$|^(\d{2})w(\d{2})([a-z])$");

            public MinecraftVersion(string version)
            {
                var match = VersionRegex.Match(version);
                if (!match.Success)
                {
                    throw new ArgumentException("Invalid version format", nameof(version));
                }

                if (match.Groups[1].Success)
                {
                    Major = int.Parse(match.Groups[1].Value);
                    Minor = int.Parse(match.Groups[2].Value);
                    Patch = match.Groups[4].Success ? int.Parse(match.Groups[4].Value) : 0;
                    PreRelease = match.Groups[5].Success ? match.Groups[5].Value.Substring(1) : null;
                }
                else if (match.Groups[6].Success)
                {
                    Major = 0;
                    Minor = int.Parse(match.Groups[6].Value);
                    Patch = int.Parse(match.Groups[7].Value);
                    Snapshot = match.Groups[8].Value;
                }
            }

            public int CompareTo(MinecraftVersion other)
            {
                if (other == null) return 1;

                if (Snapshot != null && other.Snapshot != null)
                {
                    int minorComparisonS = Minor.CompareTo(other.Minor);
                    if (minorComparisonS != 0) return minorComparisonS;

                    int patchComparisonS = Patch.CompareTo(other.Patch);
                    if (patchComparisonS != 0) return patchComparisonS;

                    return string.Compare(Snapshot, other.Snapshot, StringComparison.Ordinal);
                }

                if (Snapshot != null) return -1;
                if (other.Snapshot != null) return 1;

                int majorComparison = Major.CompareTo(other.Major);
                if (majorComparison != 0) return majorComparison;

                int minorComparison = Minor.CompareTo(other.Minor);
                if (minorComparison != 0) return minorComparison;

                int patchComparison = Patch.CompareTo(other.Patch);
                if (patchComparison != 0) return patchComparison;

                if (PreRelease == null && other.PreRelease == null) return 0;
                if (PreRelease == null) return 1;
                if (other.PreRelease == null) return -1;

                return string.Compare(PreRelease, other.PreRelease, StringComparison.Ordinal);
            }
        }
    }
}