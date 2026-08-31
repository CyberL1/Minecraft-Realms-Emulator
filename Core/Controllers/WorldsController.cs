using System.Text.Json;
using Core.Attributes;
using Core.Data;
using Core.Enums;
using Core.Models;
using Core.Models.Requests;
using Core.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppConfig = Core.Models.AppConfig;
using Player = Core.Entities.Player;
using Realm = Core.Entities.Realm;
using RealmCompatibility = Core.Helpers.RealmCompatibility;
using RealmConnection = Core.Entities.RealmConnection;
using RealmRegionSelectionPreference = Core.Entities.RealmRegionSelectionPreference;
using Region = Core.Enums.Region;
using Slot = Core.Entities.Slot;
using SlotOptions = Core.Entities.SlotOptions;
using Subscription = Core.Entities.Subscription;

namespace Core.Controllers;

[ApiController]
[Route("[controller]")]
public class WorldsController(DataContext context, CookiePlayerData playerData) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<RealmsList>> GetRealms()
    {
        var realms = await context.Realms.Include(realm => realm.Subscription).Include(realm => realm.ActiveSlot)
            .ThenInclude(slot => slot.Options).Include(realm => realm.Players)
            .Include(realm => realm.RegionSelectionPreference).ToListAsync();

        var servers = new RealmsList { Servers = [] };

        if (realms.All(realm => realm.Owner.Uuid != playerData.Uuid))
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            var subscription = new Subscription
            {
                SubscriptionId = Guid.NewGuid().ToString(),
                StartDate = DateTime.UtcNow,
                Type = nameof(SubscriptionType.NORMAL)
            };

            context.Subscriptions.Add(subscription);
            await context.SaveChangesAsync();

            var owner = new Player
            {
                Uuid = playerData.Uuid,
                Name = playerData.Name,
                Operator = false,
                Accepted = false,
                Owner = true
            };

            context.Players.Add(owner);
            await context.SaveChangesAsync();

            var primarySlot = new Slot
            {
                SlotId = 1
            };

            context.Slots.Add(primarySlot);
            await context.SaveChangesAsync();

            var primarySlotOptions = new SlotOptions
            {
                SlotId = primarySlot.Id,
                SpawnProtection = 16,
                Difficulty = SlotOptionsDifficulty.Normal,
                GameMode = SlotOptionsGameMode.Survival,
                Version = playerData.Version
            };

            context.SlotOptions.Add(primarySlotOptions);
            await context.SaveChangesAsync();

            var regionSelectionPreference = new RealmRegionSelectionPreference
            {
                RegionSelectionPreference = RegionSelectionPreference.AutomaticOwner,
                PreferredRegion = nameof(Region.WestEurope)
            };

            var connection = new RealmConnection
            {
                Address = "127.0.0.1"
            };

            var realm = new Realm
            {
                Name = "",
                Description = "",
                State = nameof(RealmState.UNINITIALIZED),
                Players = [owner],
                Slots = [primarySlot],
                WorldType = nameof(WorldType.NORMAL),
                ActiveSlotId = primarySlot.Id,
                RegionSelectionPreference = regionSelectionPreference,
                Connection = connection
            };

            context.Realms.Add(realm);
            await context.SaveChangesAsync();

            subscription.RealmId = realm.Id;

            context.Subscriptions.Update(subscription);
            await context.SaveChangesAsync();

            await transaction.CommitAsync();
            realms.Add(realm);
        }

        foreach (var realm in realms)
        {
            var server = new Models.Responses.Realm
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
                WorldType = realm.WorldType,
                IsHardcore = realm.ActiveSlot.Settings.Contains("hardcore"),
                GameMode = (int)realm.ActiveSlot.Options.GameMode,
                ActiveSlot = -1,
                ActiveVersion = realm.ActiveSlot.Options.Version,
                Compatibility =
                    RealmCompatibility.CheckRealmCompatibility(playerData.Version, realm.ActiveSlot.Options.Version)
            };

            if (playerData.Uuid == server.OwnerUUID.Replace("-", ""))
            {
                server.ExpiredTrial = AppConfig.Trial && realm.Subscription.Ended;
                server.DaysLeft = realm.Subscription.DaysLeft;
                server.ActiveSlot = realm.ActiveSlot.SlotId;
            }

            if (realm.Players.Any(player => player.Uuid == playerData.Uuid)) servers.Servers.Add(server);
        }

        return Ok(servers);
    }

    [HttpPost("{realmId:int}/initialize")]
    [HasRealmAccess(true)]
    public async Task<ActionResult<Realm>> PostInitialize(int realmId, RealmInitialize body)
    {
        var realm = await context.Realms.Include(realm => realm.Subscription).Include(realm => realm.ActiveSlot)
            .ThenInclude(slot => slot.Options).FirstAsync(realm => realm.Id == realmId);

        if (realm.State != nameof(RealmState.UNINITIALIZED)) return StatusCode(409, ApiError.WorldAlreadyInitialized);

        realm.Name = body.Name.Trim();

        if (!string.IsNullOrEmpty(body.Description))
            realm.Description = body.Description.Trim();

        realm.Subscription.StartDate = DateTime.UtcNow;

        realm.State = nameof(RealmState.OPEN);
        realm.ActiveSlot.Options.Version = playerData.Version;

        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("v1/{realmId:int}/join/pc")]
    [HasRealmAccess]
    public async Task<ActionResult<Realm>> JoinRealm(int realmId)
    {
        var realm = await context.Realms.Include(realm => realm.Connection)
            .Include(realm => realm.RegionSelectionPreference).FirstAsync(realm => realm.Id == realmId);

        var realmJoinResponse = new Models.Responses.RealmConnection
        {
            Address = realm.Connection.Address,
            ResourcePackUrl = realm.Connection.ResourcePackUrl,
            ResourcePackHash = realm.Connection.ResourcePackHash,
            SessionRegionData = new Models.Region
            {
                RegionName = realm.RegionSelectionPreference.PreferredRegion,
                ServiceQuality = ServiceQuality.Great
            }
        };

        return Ok(realmJoinResponse);
    }

    [HttpGet("{realmId:int}")]
    [HasRealmAccess(true)]
    public async Task<ActionResult<Realm>> GetOwnRealm(int realmId)
    {
        var realm = await context.Realms.Include(realm => realm.Players).Include(realm => realm.Subscription)
            .Include(realm => realm.RegionSelectionPreference).Include(realm => realm.Slots)
            .ThenInclude(slot => slot.Options).Include(realm => realm.ActiveSlot).ThenInclude(slot => slot.Options)
            .FirstAsync(realm => realm.Id == realmId);

        var realmResponse = new Models.Responses.Realm
        {
            Id = realm.Id,
            RemoteSubscriptionId = realm.Subscription.SubscriptionId.Replace("-", ""),
            Name = realm.Name,
            Motd = realm.Description,
            State = realm.State,
            Owner = realm.Owner.Name,
            OwnerUUID = realm.Owner.Uuid.Replace("-", ""),
            Players = realm.Players.FindAll(player => player.Uuid != realm.Owner.Uuid)
                .SelectMany<Player, Models.Responses.Player>(player =>
                [
                    new Models.Responses.Player
                    {
                        Uuid = player.Uuid.Replace("-", ""),
                        Name = player.Name,
                        Operator = player.Operator,
                        Accepted = player.Accepted
                    }
                ]),
            Slots = realm.Slots.SelectMany<Slot, Models.Responses.Slot>(slot =>
            [
                new Models.Responses.Slot
                {
                    SlotId = slot.SlotId,
                    Options = JsonSerializer.Serialize(new Models.Responses.SlotOptions
                    {
                        SpawnProtection = slot.Options.SpawnProtection,
                        ForceGeeMode = slot.Options.ForceGameMode,
                        Difficulty = (int)slot.Options.Difficulty,
                        GameMode = (int)slot.Options.GameMode,
                        SlotName = slot.Options.SlotName,
                        Version = slot.Options.Version,
                        Compatibility = RealmCompatibility.CheckRealmCompatibility(playerData.Version,
                            realm.ActiveSlot.Options.Version)
                    }),
                    Settings = slot.Settings
                }
            ]),
            Expired = realm.Subscription.Ended,
            ExpiredTrial = AppConfig.Trial && realm.Subscription.Ended,
            DaysLeft = realm.Subscription.DaysLeft,
            WorldType = realm.WorldType,
            IsHardcore = realm.ActiveSlot.Settings.Contains("hardcore"),
            GameMode = (int)realm.ActiveSlot.Options.GameMode,
            ActiveSlot = realm.ActiveSlot.SlotId,
            ActiveVersion = realm.ActiveSlot.Options.Version,
            Compatibility =
                RealmCompatibility.CheckRealmCompatibility(playerData.Version, realm.ActiveSlot.Options.Version),
            RegionSelectionPreference = new Models.RealmRegionSelectionPreference
            {
                RegionSelectionPreference = realm.RegionSelectionPreference.RegionSelectionPreference,
                PreferredRegion = realm.RegionSelectionPreference.PreferredRegion
            }
        };

        return Ok(realmResponse);
    }

    [HttpPost("{realmId:int}/configuration")]
    [HasRealmAccess(true)]
    public async Task<ActionResult<Realm>> ConfigureRealm(int realmId, RealmConfiguration body)
    {
        var realm = await context.Realms.Include(realm => realm.RegionSelectionPreference)
            .FirstOrDefaultAsync(realm => realm.Id == realmId);

        if (realm == null) return StatusCode(404, ApiError.WorldNotFound);

        if (body.Description != null)
        {
            if (!string.IsNullOrEmpty(body.Description.Name)) realm.Name = body.Description.Name.Trim();

            if (body.Description.Description != null)
                realm.Description = body.Description.Description.Trim();
        }

        if (body.RegionSelectionPreference != null)
        {
            realm.RegionSelectionPreference.RegionSelectionPreference =
                body.RegionSelectionPreference.RegionSelectionPreference;

            if (!string.IsNullOrEmpty(body.RegionSelectionPreference.PreferredRegion))
                realm.RegionSelectionPreference.PreferredRegion = body.RegionSelectionPreference.PreferredRegion;
        }

        await context.SaveChangesAsync();
        return Ok(realm);
    }


    [HttpPut("{realmId:int}/open")]
    [HasRealmAccess(true)]
    public async Task<ActionResult<bool>> OpenRealm(int realmId)
    {
        var realm = await context.Realms.FirstOrDefaultAsync(realm => realm.Id == realmId);

        if (realm == null) return StatusCode(404, ApiError.WorldNotFound);

        realm.State = nameof(RealmState.OPEN);

        await context.SaveChangesAsync();
        return Ok(true);
    }

    [HttpPut("{realmId:int}/close")]
    [HasRealmAccess(true)]
    public async Task<ActionResult<bool>> CloseRealm(int realmId)
    {
        var realm = await context.Realms.FirstOrDefaultAsync(realm => realm.Id == realmId);

        if (realm == null) return StatusCode(404, ApiError.WorldNotFound);

        realm.State = nameof(RealmState.CLOSED);

        await context.SaveChangesAsync();
        return Ok(true);
    }

    [HttpDelete("{realmId:int}")]
    [HasRealmAccess(true)]
    public async Task<ActionResult<bool>> DeleteRealm(int realmId)
    {
        var realm = await context.Realms.FirstOrDefaultAsync(realm => realm.Id == realmId);

        if (realm == null) return StatusCode(404, ApiError.WorldNotFound);

        context.Realms.Remove(realm);

        await context.SaveChangesAsync();
        return Ok(true);
    }
}
