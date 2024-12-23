﻿namespace Minecraft_Realms_Emulator.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CheckActiveSubscription : Attribute
    {
        public bool IsSubscriptionActive(DateTime subscriptionStartDate)
        {
            return ((DateTimeOffset)subscriptionStartDate.AddDays(30) - DateTime.Today).Days > 0;
        }
    }
}
