﻿using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Enums;
using Minecraft_Realms_Emulator.Helpers;
using Minecraft_Realms_Emulator.Responses;

namespace Minecraft_Realms_Emulator.Modes.External.Controllers
{
    [Route("modes/external/[controller]")]
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
        public ActionResult<bool> GetAvailable()
        {
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