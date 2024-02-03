using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Entities;

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

            if (world == null) return NotFound("Subscription njot found");

            var subscription = subscriptions.Find(s => s.RemoteId == world.RemoteSubscriptionId);

            return Ok(subscription);
        }
    }
}
