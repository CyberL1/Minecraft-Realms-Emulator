﻿using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Shared.Attributes;
using Minecraft_Realms_Emulator.Shared.Enums;
using Minecraft_Realms_Emulator.Shared.Helpers;
using Minecraft_Realms_Emulator.Shared.Data;
using Minecraft_Realms_Emulator.Shared.Responses;

namespace Minecraft_Realms_Emulator.Modes.Realms.Controllers
{
    [Route("modes/realms/[controller]")]
    [ApiController]
    [RequireMinecraftCookie]
    public class McoController : ControllerBase
    {
        private readonly DataContext _context;

        public McoController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("available")]
        public async Task<ActionResult<bool>> GetAvailable()
        {
            if (new ConfigHelper(_context).GetSetting(nameof(SettingsEnum.OnlineMode)).Value)
            {
                string cookie = Request.Headers.Cookie;
                string playerUUID = cookie.Split(";")[0].Split(":")[2];

                try
                {
                    await new HttpClient().GetFromJsonAsync<MinecraftPlayerInfo>($"https://sessionserver.mojang.com/session/minecraft/profile/{playerUUID}");
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
            var config = new ConfigHelper(_context);
            var newsLink = config.GetSetting(nameof(SettingsEnum.NewsLink));

            var news = new NewsResponse
            {
                NewsLink = newsLink.Value
            };

            return Ok(news);
        }
    }
}