using Core.Attributes;
using Core.Data;
using Core.Models;
using Core.Models.Requests;
using Core.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Player = Core.Entities.Player;

namespace Core.Controllers;

[ApiController]
[Route("[controller]")]
public class InvitesController(DataContext context, CookiePlayerData playerData) : ControllerBase
{
    [HttpGet("pending")]
    public async Task<ActionResult<PendingInvitesList>> GetPendingInvites()
    {
        var pendingInvitesListDb = await context.PendingInvites.Where(invite => invite.Player.Uuid == playerData.Uuid)
            .Include(pendingInvite => pendingInvite.Realm)
            .ThenInclude(realm => realm.Players).ToListAsync();

        var pendingInvitesListResponse = new PendingInvitesList { Invites = [] };

        foreach (var invite in pendingInvitesListDb)
            pendingInvitesListResponse.Invites.Add(new PendingInvite
            {
                InvitationId = invite.InvitationId,
                WorldName = invite.Realm.Name,
                WorldOwnerUuid = invite.Realm.Players[0].Uuid,
                Date = ((DateTimeOffset)invite.Date).ToUnixTimeMilliseconds()
            });

        return Ok(pendingInvitesListResponse);
    }

    [HttpPut("accept/{inviteId}")]
    public async Task<ActionResult<bool>> AcceptInvitation(string inviteId)
    {
        var invite = await context.PendingInvites.FirstOrDefaultAsync(invite => invite.InvitationId == inviteId);

        if (invite == null) return StatusCode(404, ApiError.InviteNotFound);

        await context.Players.Where(player => player.Id == invite.PlayerId)
            .ExecuteUpdateAsync(s => s.SetProperty(player => player.Accepted, true));

        context.PendingInvites.Remove(invite);
        await context.SaveChangesAsync();

        return Ok(true);
    }

    [HttpPut("reject/{inviteId}")]
    public async Task<ActionResult<bool>> RejectInvitation(string inviteId)
    {
        var invite = await context.PendingInvites.FirstOrDefaultAsync(invite => invite.InvitationId == inviteId);

        if (invite == null) return StatusCode(404, ApiError.InviteNotFound);

        context.PendingInvites.Remove(invite);
        await context.SaveChangesAsync();

        return Ok(true);
    }

    [HttpDelete("{realmId:int}")]
    [HasRealmAccess]
    public async Task<ActionResult<bool>> LeaveRealm(int realmId)
    {
        var realm = await context.Realms.FirstOrDefaultAsync(realm => realm.Id == realmId);

        if (realm == null) return StatusCode(404, ApiError.WorldNotFound);

        var player = await context.Players.Where(player => player.RealmId == realmId)
            .FirstOrDefaultAsync(player => player.Uuid == playerData.Uuid);

        if (player == null) return StatusCode(403, ApiError.NotAWorldMember);

        context.Players.Remove(player);
        await context.SaveChangesAsync();

        return Ok(true);
    }

    [HttpPost("{realmId:int}")]
    [HasRealmAccess(true)]
    public async Task<ActionResult<bool>> InvitePlayer(int realmId, RealmInvite body)
    {
        if (string.Equals(body.Name, playerData.Name, StringComparison.CurrentCultureIgnoreCase))
            return StatusCode(500, ApiError.CannotInviteYourself);

        var realm = await context.Realms.Include(w => w.Players).FirstOrDefaultAsync(w => w.Id == realmId);

        if (realm == null) return StatusCode(404, ApiError.WorldNotFound);

        var playerInfo =
            await new HttpClient().GetFromJsonAsync<MinecraftPlayerInfo>(
                $"https://api.mojang.com/users/profiles/minecraft/{body.Name}");

        if (playerInfo == null) return StatusCode(500, ApiError.FailedToGetPlayerData);

        var playerInDb = await context.Players.Where(player => player.RealmId == realmId)
            .FirstOrDefaultAsync(p => p.Uuid == playerInfo.Id);

        if (playerInDb?.Uuid == playerInfo.Id) return StatusCode(500, ApiError.PlayerAlreadyInvited);

        Player player = new()
        {
            Uuid = playerInfo.Id,
            Name = playerInfo.Name,
            Operator = false,
            Accepted = false,
            Realm = realm
        };

        context.Players.Add(player);

        Entities.PendingInvite invite = new()
        {
            InvitationId = Guid.NewGuid().ToString(),
            Date = DateTime.UtcNow,
            Realm = realm,
            Player = player
        };

        context.PendingInvites.Add(invite);
        await context.SaveChangesAsync();

        realm.Players.RemoveAt(0);
        return Ok(realm);
    }

    [HttpDelete("{realmId:int}/invite/{playerUuid}")]
    [HasRealmAccess(true)]
    public async Task<ActionResult<bool>> RemovePlayer(int realmId, string playerUuid)
    {
        var realm = await context.Realms.FirstOrDefaultAsync(realm => realm.Id == realmId);

        if (realm == null) return StatusCode(404, ApiError.WorldNotFound);

        var player = await context.Players.Where(player => player.RealmId == realmId)
            .FirstOrDefaultAsync(player => player.Uuid == playerUuid);

        if (player == null) return StatusCode(500, ApiError.PlayerNotInvited);
        if (player.Uuid == playerData.Uuid) return StatusCode(500, ApiError.CannotUnInviteYourself);

        context.Players.Remove(player);
        await context.SaveChangesAsync();

        return Ok(true);
    }
}
