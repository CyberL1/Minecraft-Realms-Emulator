using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Enums;
using Minecraft_Realms_Emulator.Helpers;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Helpers.Config;
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
        public async Task<ActionResult<LivePlayerListsResponse>> GetLivePlayerList()
        {
            string cookie = Request.Headers.Cookie.ToString();
            string playerUuid = cookie.Split(";")[0].Split(":")[2];

            List<LivePlayerList> lists = [];

            var worlds = context.Worlds.Where(w =>
                w.OwnerUUID == playerUuid || w.Players.Any(p => p.Uuid == playerUuid && p.Accepted)).ToList();
            var defaultServerAddress = ConfigHelper.GetSetting(nameof(SettingsEnum.DefaultServerAddress));

            foreach (var world in worlds)
            {
                if (!await new DockerHelper(world.Id).IsRunning())
                {
                    continue;
                }

                var port = await new DockerHelper(world.Id).GetServerPort();
                var query = new MinecraftServerQuery().Query($"{defaultServerAddress}:{port}");

                if (query == null) continue;

                List<object> players = [];

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
            }

            LivePlayerListsResponse response = new()
            {
                Lists = lists
            };

            return Ok(response);
        }
    }
}