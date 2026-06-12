using System.Diagnostics.CodeAnalysis;

namespace Core.Enums;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum CompatibleVersionResponse
{
    COMPATIBLE,
    OUTDATED,
    OTHER
}
