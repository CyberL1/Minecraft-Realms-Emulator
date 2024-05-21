﻿namespace Minecraft_Realms_Emulator.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CheckRealmOwnerAttribute : Attribute
    {
        public bool IsRealmOwner(string playerUUID, string ownerUUID)
        {
            return playerUUID == ownerUUID;
        }
    }
}
