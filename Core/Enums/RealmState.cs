using System.Diagnostics.CodeAnalysis;

namespace Core.Enums;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum RealmState
{
    OPEN,
    CLOSED,
    UNINITIALIZED
}