﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Shared.Attributes;
using Minecraft_Realms_Emulator.Shared.Data;
using Minecraft_Realms_Emulator.Shared.Responses;

namespace Minecraft_Realms_Emulator.Modes.External
{
    [Route("modes/external/[controller]")]
    [ApiController]
    [RequireMinecraftCookie]
    public class SubscriptionsController : ControllerBase
    {
        private readonly DataContext _context;

        public SubscriptionsController(DataContext context)
        {
            _context = context;
        }
        [HttpGet("{wId}")]
        [CheckForWorld]
        [CheckRealmOwner]
        public async Task<ActionResult<SubscriptionResponse>> Get(int wId)
        {
            var world = await _context.Worlds.Include(w => w.Subscription).Include(w => w.ParentWorld.Subscription).FirstOrDefaultAsync(w => w.Id == wId);

            if (world.ParentWorld != null)
            {
                world.Subscription = world.ParentWorld.Subscription;
            }

            if (world?.Subscription == null) return NotFound("Subscription not found");

            var sub = new SubscriptionResponse
            {
                StartDate = ((DateTimeOffset)world.Subscription.StartDate).ToUnixTimeMilliseconds(),
                DaysLeft =  ((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days,
                SubscriptionType = world.Subscription.SubscriptionType
            };

            return Ok(sub);
        }
    }
}
