using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Enums;
using Minecraft_Realms_Emulator.Helpers.Config;
using Minecraft_Realms_Emulator.Responses;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [RequireMinecraftCookie]
    public class McoController : ControllerBase
    {
        [HttpGet("available")]
        public async Task<ActionResult<bool>> GetAvailable()
        {
            if (ConfigHelper.GetSetting(nameof(SettingsEnum.OnlineMode)))
            {
                var cookie = Request.Headers.Cookie.ToString();
                var playerUuid = cookie.Split(";")[0].Split(":")[2];

                try
                {
                    await new HttpClient().GetFromJsonAsync<MinecraftPlayerInfo>($"https://sessionserver.mojang.com/session/minecraft/profile/{playerUuid}");
                }
                catch
                {
                    return Unauthorized();
                }
            }

            return Ok(true);
        }

        [HttpGet("client/compatible")]
        public ActionResult<string> GetCompatible()
        {
            return Ok(nameof(VersionCompatibilityEnum.COMPATIBLE));
        }

        [HttpGet("v1/news")]
        public ActionResult<NewsResponse> GetNews()
        {
            var newsLink = ConfigHelper.GetSetting(nameof(SettingsEnum.NewsLink));

            var news = new NewsResponse
            {
                NewsLink = newsLink
            };

            return Ok(news);
        }
    }
}