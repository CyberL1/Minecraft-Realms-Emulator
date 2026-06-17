using System.Text.Json;
using Core.Attributes;
using Core.Data;
using Core.Enums;
using Core.Models;
using Core.Requests;
using Core.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppConfig = Core.Models.AppConfig;
using Player = Core.Entities.Player;
using Realm = Core.Entities.Realm;
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
            .Include(realm => realm.RegionSelectionPreference)
            .ToListAsync();

        var servers = new RealmsList { Servers = [] };

        if (realms.All(realm => realm.Players.First().Uuid != playerData.Uuid))
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
                Accepted = false
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
                SlotName = "Slot #1",
                Version = playerData.Version
            };

            context.SlotOptions.Add(primarySlotOptions);
            await context.SaveChangesAsync();

            var regionSelectionPreference = new RealmRegionSelectionPreference
            {
                RegionSelectionPreference = RegionSelectionPreference.AutomaticOwner,
                PreferredRegion = nameof(Region.WestEurope)
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
                RegionSelectionPreference = regionSelectionPreference
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
            var server = new Responses.Realm
            {
                Id = realm.Id,
                RemoteSubscriptionId = realm.Subscription.SubscriptionId.Replace("-", ""),
                Name = realm.Name,
                Motd = realm.Description,
                State = realm.State,
                Owner = realm.Players.First().Name,
                OwnerUUID = realm.Players.First().Uuid.Replace("-", ""),
                Expired = realm.Subscription.Ended,
                ExpiredTrial = false,
                DaysLeft = realm.Subscription.Ended ? -1 : 0,
                WorldType = realm.WorldType,
                IsHardcore = realm.ActiveSlot.Settings.Contains("hardcore"),
                GameMode = realm.ActiveSlot.Options.Gamemode,
                ActiveSlot = -1,
                ActiveVersion = realm.ActiveSlot.Options.Version,
                Compatibility = nameof(RealmCompatibility.UNVERIFIABLE)
            };

            if (playerData.Uuid == server.OwnerUUID.Replace("-", ""))
            {
                server.ExpiredTrial = AppConfig.Trial && realm.Subscription.Ended;
                server.DaysLeft = realm.Subscription.DaysLeft;
                server.ActiveSlot = realm.ActiveSlot.SlotId;
            }

            // TODO: Improve this
            server.Compatibility = playerData.Version == realm.ActiveSlot.Options.Version
                ? nameof(RealmCompatibility.COMPATIBLE)
                : nameof(RealmCompatibility.INCOMPATIBLE);

            servers.Servers.Add(server);
        }

        return Ok(servers);
    }

    [HttpPost("{realmId:int}/initialize")]
    [HasRealmAccess(true)]
    public async Task<ActionResult<Realm>> PostInitialize(int realmId, RealmInitialize body)
    {
        var realm = await context.Realms.Include(realm => realm.Subscription).FirstAsync(realm => realm.Id == realmId);

        if (realm.State != nameof(RealmState.UNINITIALIZED)) return StatusCode(409, ApiError.WorldAlreadyInitialized);

        realm.Name = body.Name;

        if (body.Description != null && body.Description.Trim() != string.Empty)
            realm.Description = body.Description;

        realm.Subscription.StartDate = DateTime.UtcNow;

        realm.State = nameof(RealmState.OPEN);

        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("{realmId:int}")]
    [HasRealmAccess(true)]
    public async Task<ActionResult<Realm>> GetOwnRealm(int realmId)
    {
        var realm = await context.Realms.Include(realm => realm.Players).Include(realm => realm.Subscription)
            .Include(realm => realm.RegionSelectionPreference).Include(realm => realm.Slots)
            .ThenInclude(slot => slot.Options).Include(realm => realm.ActiveSlot).ThenInclude(slot => slot.Options)
            .FirstAsync(realm => realm.Id == realmId);

        var realmResponse = new Responses.Realm
        {
            Id = realm.Id,
            RemoteSubscriptionId = realm.Subscription.SubscriptionId.Replace("-", ""),
            Name = realm.Name,
            Motd = realm.Description,
            State = realm.State,
            Owner = realm.Players.First().Name,
            OwnerUUID = realm.Players.First().Uuid.Replace("-", ""),
            Players = realm.Players.FindAll(player => player.Uuid != realm.Players.First().Uuid)
                .SelectMany<Player, Responses.Player>(player =>
                [
                    new Responses.Player
                    {
                        Uuid = player.Uuid.Replace("-", ""),
                        Name = player.Name,
                        Operator = player.Operator,
                        Accepted = player.Accepted
                    }
                ]),
            Slots = realm.Slots.SelectMany<Slot, Responses.Slot>(slot =>
            [
                new Responses.Slot
                {
                    SlotId = slot.SlotId,
                    Options = JsonSerializer.Serialize(new Responses.SlotOptions
                    {
                        SpawnProtection = slot.Options.SpawnProtection,
                        ForceGeeMode = slot.Options.ForceGamemode,
                        Difficulty = slot.Options.Difficulty,
                        GameMode = slot.Options.Gamemode,
                        SlotName = slot.Options.SlotName,
                        Version = slot.Options.Version,
                        Compatibility = playerData.Version == slot.Options.Version
                            ? nameof(RealmCompatibility.COMPATIBLE)
                            : nameof(RealmCompatibility.INCOMPATIBLE)
                    }),
                    Settings = slot.Settings
                }
            ]),
            Expired = realm.Subscription.Ended,
            ExpiredTrial = AppConfig.Trial && realm.Subscription.Ended,
            DaysLeft = realm.Subscription.DaysLeft,
            WorldType = realm.WorldType,
            IsHardcore = realm.ActiveSlot.Settings.Contains("hardcore"),
            GameMode = realm.ActiveSlot.Options.Gamemode,
            ActiveSlot = realm.ActiveSlot.SlotId,
            ActiveVersion = realm.ActiveSlot.Options.Version,
            Compatibility = nameof(RealmCompatibility.UNVERIFIABLE),
            RegionSelectionPreference = new Models.RealmRegionSelectionPreference
            {
                RegionSelectionPreference = realm.RegionSelectionPreference.RegionSelectionPreference,
                PreferredRegion = realm.RegionSelectionPreference.PreferredRegion
            }
        };

        // TODO: Improve this
        realmResponse.Compatibility = playerData.Version == realm.ActiveSlot.Options.Version
            ? nameof(RealmCompatibility.COMPATIBLE)
            : nameof(RealmCompatibility.INCOMPATIBLE);

        return Ok(realmResponse);
    }
}
