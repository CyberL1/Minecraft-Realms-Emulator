using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Responses;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [RequireMinecraftCookie]
    public class OpsController : ControllerBase
    {
        private readonly DataContext _context;

        public OpsController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("{wId}/{uuid}")]
        public ActionResult<OpsResponse> OpPlayer(int wId, string uuid)
        {
            var ops = _context.Players.Where(p => p.World.Id == wId && p.Operator == true).ToList();
            var player = _context.Players.Where(p => p.World.Id == wId).FirstOrDefault(p => p.Uuid == uuid);

            List<string> opNames = [];

            foreach (var op in ops)
            {
                opNames.Add(op.Name);
            }

            player.Permission = "OPERATOR";
            player.Operator = true;

            _context.SaveChanges();

            opNames.Add(player.Name);

            var opsResponse = new OpsResponse
            {
                Ops = opNames
            };

            return Ok(opsResponse);
        }

        [HttpDelete("{wId}/{uuid}")]
        public ActionResult<OpsResponse> DeopPlayer(int wId, string uuid)
        {
            var ops = _context.Players.Where(p => p.World.Id == wId && p.Operator == true).ToList();
            var player = _context.Players.Where(p => p.World.Id == wId).FirstOrDefault(p => p.Uuid == uuid);

            List<string> opNames = [];

            foreach (var op in ops)
            {
                opNames.Add(op.Name);
            }

            player.Permission = "MEMBER";
            player.Operator = false;

            _context.SaveChanges();

            opNames.Remove(player.Name);

            var opsResponse = new OpsResponse
            {
                Ops = opNames
            };

            return Ok(opsResponse);
        }
    }
}
