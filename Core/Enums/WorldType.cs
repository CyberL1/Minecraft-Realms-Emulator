using System.Diagnostics.CodeAnalysis;

namespace Core.Enums;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum WorldType
{
    NORMAL,
    MINIGAME,
    ADVENTUREMAP,
    EXPERIENCE,
    INSPIRATION,
    UNKNOWN
}
