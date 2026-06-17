namespace Core.Models;

public class MinecraftPublicKeys
{
    public required List<PublicKeyWrapper> ProfilePropertyKeys { get; set; }
    public required List<PublicKeyWrapper> PlayerCertificateKeys { get; set; }
    public required List<PublicKeyWrapper> AuthenticationKeys { get; set; }
}

public class PublicKeyWrapper
{
    public required string PublicKey { get; set; }
}