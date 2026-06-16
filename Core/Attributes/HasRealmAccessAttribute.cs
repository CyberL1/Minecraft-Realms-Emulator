namespace Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class HasRealmAccessAttribute(bool isOwner = false) : Attribute
{
    public bool IsOwner { get; } = isOwner;
}
