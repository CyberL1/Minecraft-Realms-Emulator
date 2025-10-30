using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Enums;
using Minecraft_Realms_Emulator.Helpers;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Responses;
using Newtonsoft.Json;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [RequireMinecraftCookie]
    public class ActivitiesController(DataContext context) : ControllerBase
    {
        [HttpGet("liveplayerlist")]
        public ActionResult<LivePlayerListsResponse> GetLivePlayerList()
        {
            string cookie = Request.Headers.Cookie;
            string playerUUID = cookie.Split(";")[0].Split(":")[2];

            List<LivePlayerList> lists = [];

            var worlds = context.Worlds.Where(w => w.State == nameof(StateEnum.OPEN) && w.OwnerUUID == playerUUID || w.State == nameof(StateEnum.OPEN) && w.Players.Any(p => p.Uuid == playerUUID && p.Accepted)).ToList();

            foreach (var world in worlds)
            {
                var connection = context.Connections.Where(c => c.World.Id == world.Id).FirstOrDefault();
                var query = new MinecraftServerQuery().Query(connection.Address);

                if (query == null) continue;

                List<object> players = [];

                if (query.Players.Sample == null) continue;

                foreach (var player in query.Players.Sample)
                {
                    players.Add(new
                    {
                        playerId = player.Id.Replace("-", ""),
                    });
                }

                LivePlayerList list = new()
                {
                    ServerId = world.Id,
                    PlayerList = JsonConvert.SerializeObject(players),
                };

                lists.Add(list);
            };

            LivePlayerListsResponse response = new()
            {
                Lists = lists
            };

            return Ok(response);
        }
    }
}
