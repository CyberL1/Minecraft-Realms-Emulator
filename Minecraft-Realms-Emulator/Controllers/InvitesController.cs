using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;
using Minecraft_Realms_Emulator.Requests;
using Minecraft_Realms_Emulator.Responses;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [RequireMinecraftCookie]
    public class InvitesController(DataContext context) : ControllerBase
    {
        [HttpGet("pending")]
        public async Task<ActionResult<InviteList>> GetInvites()
        {
            string cookie = Request.Headers.Cookie;
            string playerUUID = cookie.Split(";")[0].Split(":")[2];

            var invites = await context.Invites.Where(i => i.RecipeintUUID == playerUUID).Include(i => i.World).ToListAsync();

            List<InviteResponse> invitesList = [];

            foreach (var invite in invites)
            {
                InviteResponse inv = new()
                {
                    InvitationId = invite.InvitationId,
                    WorldName = invite.World.Name,
                    WorldOwnerName = invite.World.Owner,
                    WorldOwnerUuid = invite.World.OwnerUUID,
                    Date = ((DateTimeOffset)invite.Date).ToUnixTimeMilliseconds(),
                };

                invitesList.Add(inv);
            }

            InviteList inviteListRespone = new()
            {
                Invites = invitesList
            };

            return Ok(inviteListRespone);
        }
        [HttpPut("accept/{id}")]
        public ActionResult<bool> AcceptInvite(string id)
        {
            string cookie = Request.Headers.Cookie;
            string playerUUID = cookie.Split(";")[0].Split(":")[2];

            var invite = context.Invites.Include(i => i.World).FirstOrDefault(i => i.InvitationId == id);

            if (invite == null) return NotFound("Invite not found");

            var player = context.Players.Where(p => p.World.Id == invite.World.Id).FirstOrDefault(p => p.Uuid == playerUUID);

            player.Accepted = true;

            context.Invites.Remove(invite);

            context.SaveChanges();

            return Ok(true);
        }

        [HttpPut("reject/{id}")]
        public ActionResult<bool> RejectInvite(string id)
        {
            var invite = context.Invites.Include(i => i.World).FirstOrDefault(i => i.InvitationId == id);

            if (invite == null) return NotFound("Invite not found");

            context.Invites.Remove(invite);

            string cookie = Request.Headers.Cookie;
            string playerUUID = cookie.Split(";")[0].Split(":")[2];

            var player = context.Players.Where(p => p.World.Id == invite.World.Id).FirstOrDefault(p => p.Uuid == playerUUID);

            context.Players.Remove(player);

            context.SaveChanges();

            return Ok(true);
        }

        [HttpPost("{wId}")]
        [CheckForWorld]
        [CheckRealmOwner]
        [CheckActiveSubscription]
        public async Task<ActionResult<World>> InvitePlayer(int wId, PlayerRequest body)
        {
            string cookie = Request.Headers.Cookie;
            string playerName = cookie.Split(";")[1].Split("=")[1];

            if (string.Equals(body.Name, playerName, StringComparison.CurrentCultureIgnoreCase))
            {
                var response = new ErrorResponse
                {
                    ErrorCode = 500,
                    ErrorMsg = "You cannot invite yourself"
                };

                return StatusCode(500, response);
            }

            var world = await context.Worlds.Include(w => w.Players).FirstOrDefaultAsync(w => w.Id == wId);

            if (world == null) return NotFound("World not found");

            // Get player UUID
            var playerInfo = await new HttpClient().GetFromJsonAsync<MinecraftPlayerInfo>($"https://api.mojang.com/users/profiles/minecraft/{body.Name}");

            var playerInDB = await context.Players.Where(p => p.World.Id == wId).FirstOrDefaultAsync(p => p.Uuid == playerInfo.Id);

            if (playerInDB?.Uuid == playerInfo.Id) return BadRequest("Player already invited");

            Player player = new()
            {
                Name = body.Name,
                Uuid = playerInfo.Id,
                World = world
            };

            context.Players.Add(player);

            Invite invite = new()
            {
                InvitationId = Guid.NewGuid().ToString(),
                World = world,
                RecipeintUUID = playerInfo.Id,
                Date = DateTime.UtcNow,
            };

            context.Invites.Add(invite);

            context.SaveChanges();

            return Ok(world);
        }

        [HttpDelete("{wId}/invite/{uuid}")]
        [CheckForWorld]
        [CheckRealmOwner]
        [CheckActiveSubscription]
        public async Task<ActionResult<bool>> DeleteInvite(int wId, string uuid)
        {
            var world = await context.Worlds.FirstOrDefaultAsync(w => w.Id == wId);

            if (world == null) return NotFound("World not found");

            var player = context.Players.Where(p => p.World.Id == wId).FirstOrDefault(p => p.Uuid == uuid);

            context.Players.Remove(player);

            var invite = await context.Invites.FirstOrDefaultAsync(i => i.RecipeintUUID == uuid);

            if (invite != null) context.Invites.Remove(invite);

            context.SaveChanges();

            return Ok(true);
        }

        [HttpDelete("{wId}")]
        [CheckForWorld]
        public async Task<ActionResult<bool>> LeaveWorld(int wId)
        {
            string cookie = Request.Headers.Cookie;
            string playerUUID = cookie.Split(";")[0].Split(":")[2];

            var world = await context.Worlds.FirstOrDefaultAsync(w => w.Id == wId);

            if (world == null) return NotFound("World not found");

            var player = context.Players.Where(p => p.World.Id == wId).FirstOrDefault(p => p.Uuid == playerUUID);

            context.Players.Remove(player);

            context.SaveChanges();

            return Ok(true);
        }
    }
}
