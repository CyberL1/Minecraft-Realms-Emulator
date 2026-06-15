using Core.Data;
using Core.Enums;
using Core.Models;
using Core.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core.Controllers;

[ApiController]
[Route("[controller]")]
public class WorldsController(DataContext context, CookiePlayerData playerData) : ControllerBase
{
    [HttpGet]
    public ActionResult<RealmsList> GetReleasedRealms()
    {
        var realms = context.Realms.Include(realm => realm.Owner).Include(realm => realm.Subscription)
            .Include(realm => realm.ActiveSlot).ThenInclude(slot => slot.Settings)
            .Include(realm => realm.ActiveSlot).ThenInclude(slot => slot.Options).ToList();

        var servers = new RealmsList { Servers = [] };

        foreach (var realm in realms)
        {
            var server = new Realm
            {
                Id = realm.Id,
                RemoteSubscriptionId = realm.Subscription.SubscriptionId.Replace("-", ""),
                Name = realm.Name,
                Motd = realm.Description,
                State = realm.State,
                Owner = realm.Owner.Name,
                OwnerUUID = realm.Owner.Uuid.Replace("-", ""),
                Expired = realm.Subscription.Ended,
                ExpiredTrial = false,
                DaysLeft = realm.Subscription.Ended ? -1 : 0,
                IsHardcore = realm.ActiveSlot.Settings.Hardcore,
                GameMode = realm.ActiveSlot.Options.Gamemode,
                ActiveSlot = -1,
                ActiveVersion = realm.ActiveSlot.Options.Version,
                Compatibility = nameof(RealmCompatibility.UNVERIFIABLE)
            };

            if (playerData.Uuid == server.OwnerUUID.Replace("-", ""))
            {
                server.ExpiredTrial = AppConfig.Trial && realm.Subscription.Ended;
                server.DaysLeft = realm.Subscription.DaysLeft;
                server.ActiveSlot = realm.ActiveSlot.Id;
            }

            // TOOO: Improve this
            server.Compatibility = playerData.Version == realm.ActiveSlot.Options.Version
                ? nameof(RealmCompatibility.COMPATIBLE)
                : nameof(RealmCompatibility.INCOMPATIBLE);

            servers.Servers.Add(server);
        }

        return Ok(servers);
    }
}
