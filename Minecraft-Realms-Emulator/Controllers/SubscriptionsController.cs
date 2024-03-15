using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Responses;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly DataContext _context;

        public SubscriptionsController(DataContext context)
        {
            _context = context;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Subscription>> Get(int id)
        {
            var world = await _context.Worlds.FindAsync(id);
            var subscriptions = await _context.Subscriptions.ToListAsync();

            if (world == null) return NotFound("Subscription not found");

            var subscription = subscriptions.Find(s => s.World.RemoteSubscriptionId == world.RemoteSubscriptionId);

            var sub = new Subscription
            {
                StartDate = ((DateTimeOffset) subscription.StartDate).ToUnixTimeMilliseconds(),
                DaysLeft = subscription.World.DaysLeft,
                SubscriptionType = subscription.SubscriptionType
            };

            return Ok(sub);
        }
    }
}
