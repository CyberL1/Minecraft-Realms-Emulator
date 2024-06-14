namespace Minecraft_Realms_Emulator.Responses
{
    public class LivePlayerListsResponse
    {
        public List<LivePlayerList> Lists { get; set; } = [];
    }

    public class LivePlayerList
    {
        public int ServerId { get; set; }
        public string PlayerList { get; set; } = null!;
    }
}
