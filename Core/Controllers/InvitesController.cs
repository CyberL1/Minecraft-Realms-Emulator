using Core.Data;
using Core.Models;
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
        var pendingInvitesListDb = await context.PendingInvites.Include(pendingInvite => pendingInvite.Realm)
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

        context.Players.Add(new Player
        {
            Uuid = playerData.Uuid,
            Name = playerData.Name,
            Operator = false,
            Accepted = true,
            RealmId = invite.RealmId
        });

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
}
