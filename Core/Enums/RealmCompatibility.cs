using System.Diagnostics.CodeAnalysis;

namespace Core.Enums;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum RealmCompatibility
{
    UNVERIFIABLE,
    INCOMPATIBLE,
    RELEASE_TYPE_INCOMPATIBLE,
    NEEDS_DOWNGRADE,
    NEEDS_UPGRADE,
    COMPATIBLE
}
