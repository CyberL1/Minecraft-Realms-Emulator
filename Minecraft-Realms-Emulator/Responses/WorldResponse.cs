﻿using Minecraft_Realms_Emulator.Responses;

namespace Minecraft_Realms_Emulator.Entities
{
    public class WorldResponse : World
    {
        public string RemoteSubscriptionId { get; set; } = Guid.NewGuid().ToString();
        public int DaysLeft { get; set; } = 30;
        public bool Expired { get; set; } = false;
        public bool ExpiredTrial { get; set; } = false;
        public string Compatibility { get; set; } = null!;
        public List<SlotResponse> Slots { get; set; } = null!;
        public string ActiveVersion { get; set; } = null!;
    }
}