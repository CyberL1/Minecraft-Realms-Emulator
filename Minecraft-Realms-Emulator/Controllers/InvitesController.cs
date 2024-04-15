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
    public class InvitesController : ControllerBase
    {
        private readonly DataContext _context;

        public InvitesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("pending")]
        public async Task<ActionResult<InviteList>> GetInvites()
        {
            string cookie = Request.Headers.Cookie;
            string playerUUID = cookie.Split(";")[0].Split(":")[2];

            var invites = await _context.Invites.Where(i => i.RecipeintUUID == playerUUID).Include(i => i.World).ToListAsync();

            List<InviteResponse> invitesList = [];
            
            foreach (var invite in invites)
            {
                InviteResponse inv = new()
                {
                    InvitationId = invite.InvitationId,
                    WorldName = invite.World.Name,
                    WorldOwnerName = invite.World.Owner,
                    WorldOwnerUuid = invite.World.OwnerUUID,
                    Date = ((DateTimeOffset) invite.Date).ToUnixTimeMilliseconds(),
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

            var invite = _context.Invites.Include(i => i.World).FirstOrDefault(i => i.InvitationId == id);

            if (invite == null) return NotFound("Invite not found");

            var player = _context.Players.Where(p => p.World.Id == invite.World.Id).FirstOrDefault(p => p.Uuid == playerUUID);

            player.Accepted = true;

            _context.Invites.Remove(invite);

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpPut("reject/{id}")]
        public ActionResult<bool> RejectInvite(string id)
        {
            var invite = _context.Invites.Include(i => i.World).FirstOrDefault(i => i.InvitationId == id);

            if (invite == null) return NotFound("Invite not found");
            
            _context.Invites.Remove(invite);

            string cookie = Request.Headers.Cookie;
            string playerUUID = cookie.Split(";")[0].Split(":")[2];

            var player = _context.Players.Where(p => p.World.Id == invite.World.Id).FirstOrDefault(p => p.Uuid == playerUUID);

            _context.Players.Remove(player);

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpPost("{wId}")]
        public async Task<ActionResult<World>> InvitePlayer(int wId, PlayerRequest body)
        {
            string cookie = Request.Headers.Cookie;
            string playerName = cookie.Split(";")[1].Split("=")[1];

            if (body.Name == playerName) return Forbid("You cannot invite yourself");

            var world = await _context.Worlds.Include(w => w.Players).FirstOrDefaultAsync(w => w.Id == wId);

            if (world == null) return NotFound("World not found");

            // Get player UUID
            var playerInfo = await new HttpClient().GetFromJsonAsync<MinecraftPlayerInfo>($"https://api.mojang.com/users/profiles/minecraft/{body.Name}");

            var playerInDB = await _context.Players.Where(p => p.World.Id == wId).FirstOrDefaultAsync(p => p.Uuid == playerInfo.Id);

            if (playerInDB?.Uuid == playerInfo.Id) return BadRequest("Player already invited");

            Player player = new()
            {
                Name = body.Name,
                Uuid = playerInfo.Id,
                World = world
            };

            _context.Players.Add(player);

            Invite invite = new()
            {
                InvitationId = Guid.NewGuid().ToString(),
                World = world,
                RecipeintUUID = playerInfo.Id,
                Date = DateTime.UtcNow,
            };

            _context.Invites.Add(invite);

            _context.SaveChanges();

            return Ok(world);
        }

        [HttpDelete("{wId}/invite/{uuid}")]
        public async Task<ActionResult<bool>> DeleteInvite(int wId, string uuid)
        {
            var world = await _context.Worlds.FirstOrDefaultAsync(w => w.Id == wId);

            if (world == null) return NotFound("World not found");

            var player = _context.Players.Where(p => p.World.Id == wId).FirstOrDefault(p => p.Uuid == uuid);

            _context.Players.Remove(player);

            var invite = await _context.Invites.FirstOrDefaultAsync(i => i.RecipeintUUID == uuid);

            if (invite != null) _context.Invites.Remove(invite);

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpDelete("{wId}")]
        public async Task<ActionResult<bool>> LeaveWorld(int wId)
        {
            string cookie = Request.Headers.Cookie;
            string playerUUID = cookie.Split(";")[0].Split(":")[2];

            var world = await _context.Worlds.FirstOrDefaultAsync(w => w.Id == wId);

            if (world == null) return NotFound("World not found");

            var player = _context.Players.Where(p => p.World.Id == wId).FirstOrDefault(p => p.Uuid == playerUUID);

            _context.Players.Remove(player);

            _context.SaveChanges();

            return Ok(true);
        }
    }
}
