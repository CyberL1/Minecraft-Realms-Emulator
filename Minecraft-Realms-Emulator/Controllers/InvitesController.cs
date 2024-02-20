using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;
using Minecraft_Realms_Emulator.Responses;
using System;
using System.Text.Json;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
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
            var invite = _context.Invites.FirstOrDefault(i => i.InvitationId == id);

            if (invite == null) return NotFound("Invite not found");
            _context.Invites.Remove(invite);

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpPut("reject/{id}")]
        public async Task<ActionResult<bool>> RejectInvite(string id)
        {
            var invite = _context.Invites.Include(i => i.World).FirstOrDefault(i => i.InvitationId == id);

            if (invite == null) return NotFound("Invite not found");
            
            _context.Invites.Remove(invite);

            string cookie = Request.Headers.Cookie;
            string playerUUID = cookie.Split(";")[0].Split(":")[2];
            
            World world = await _context.Worlds.FindAsync(invite.World.Id);
            var playerIndex = world.Players.FindIndex(p => p.RootElement.GetProperty("uuid").ToString() == playerUUID);

            world.Players.RemoveAt(playerIndex);

            _context.SaveChanges();

            return Ok(true);
        }

        [HttpPost("{wId}")]
        public async Task<ActionResult<World>> InvitePlayer(int wId, Player body)
        {
            string cookie = Request.Headers.Cookie;
            string playerName = cookie.Split(";")[1].Split("=")[1];

            if (body.Name == playerName) return Forbid("You cannot invite yourself");

            var world = await _context.Worlds.FirstOrDefaultAsync(w => w.Id == wId);

            if (world == null) return NotFound("World not found");

            if (world.Players.Exists(p => p.RootElement.GetProperty("name").ToString() == body.Name)) return NotFound("Player already invited");

            // Get player UUID
            var playerInfo = await new HttpClient().GetFromJsonAsync<MinecraftPlayerInfo>($"https://api.mojang.com/users/profiles/minecraft/{body.Name}");
            body.Uuid = playerInfo.Id;

            JsonDocument player = JsonDocument.Parse(JsonSerializer.Serialize(body));
            world.Players.Add(player);

            _context.Worlds.Update(world);

            Invite invite = new()
            {
                InvitationId = Guid.NewGuid().ToString(),
                World = world,
                RecipeintUUID = body.Uuid,
                Date = DateTime.UtcNow,
            };

            _context.Invites.Add(invite);

            _context.SaveChanges();

            return Ok(world);
        }

        [HttpDelete("{wId}/invite/{uuid}")]
        public async Task<ActionResult<bool>> DeleteInvite(int wId, string uuid)
        {
            Console.WriteLine($"{wId} - {uuid}");

            var world = await _context.Worlds.FirstOrDefaultAsync(w => w.Id == wId);

            if (world == null) return NotFound("World not found");

            var players = world.Players.ToList();

            var playerIndex = world.Players.FindIndex(p => p.RootElement.GetProperty("uuid").ToString() == uuid);

            world.Players.RemoveAt(playerIndex);

            var invite = await _context.Invites.FirstOrDefaultAsync(i => i.RecipeintUUID == uuid);

            if (invite != null) _context.Invites.Remove(invite);

            _context.SaveChanges();

            return Ok(true);
        }
    }
}
