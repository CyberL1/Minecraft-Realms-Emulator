using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Responses;

namespace Minecraft_Realms_Emulator.Modes.Realms.Controllers
{
    [Route("modes/realms/[controller]")]
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
        [CheckRealmOwner]
        public async Task<ActionResult<SubscriptionResponse>> Get(int wId)
        {
            var world = await _context.Worlds.Include(w => w.Subscription).FirstOrDefaultAsync(w => w.Id == wId);

            if (world?.Subscription == null) return NotFound("Subscription not found");

            var sub = new SubscriptionResponse
            {
                StartDate = ((DateTimeOffset)world.Subscription.StartDate).ToUnixTimeMilliseconds(),
                DaysLeft = ((DateTimeOffset)world.Subscription.StartDate.AddDays(30) - DateTime.Today).Days,
                SubscriptionType = world.Subscription.SubscriptionType
            };

            return Ok(sub);
        }
    }
}
