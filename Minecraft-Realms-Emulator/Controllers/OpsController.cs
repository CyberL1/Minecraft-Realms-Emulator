using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Responses;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [RequireMinecraftCookie]
    public class OpsController(DataContext context) : ControllerBase
    {
        [HttpPost("{wId}/{uuid}")]
        [CheckForWorld]
        [CheckRealmOwner]
        [CheckActiveSubscription]
        public ActionResult<OpsResponse> OpPlayer(int wId, string uuid)
        {
            var ops = context.Players.Where(p => p.World.Id == wId && p.Operator == true).ToList();
            var player = context.Players.Where(p => p.World.Id == wId).FirstOrDefault(p => p.Uuid == uuid);

            List<string> opNames = [];

            foreach (var op in ops)
            {
                opNames.Add(op.Name);
            }

            player.Permission = "OPERATOR";
            player.Operator = true;

            context.SaveChanges();

            opNames.Add(player.Name);

            var opsResponse = new OpsResponse
            {
                Ops = opNames
            };

            return Ok(opsResponse);
        }

        [HttpDelete("{wId}/{uuid}")]
        [CheckForWorld]
        [CheckRealmOwner]
        [CheckActiveSubscription]
        public ActionResult<OpsResponse> DeopPlayer(int wId, string uuid)
        {
            var ops = context.Players.Where(p => p.World.Id == wId && p.Operator == true).ToList();
            var player = context.Players.Where(p => p.World.Id == wId).FirstOrDefault(p => p.Uuid == uuid);

            List<string> opNames = [];

            foreach (var op in ops)
            {
                opNames.Add(op.Name);
            }

            player.Permission = "MEMBER";
            player.Operator = false;

            context.SaveChanges();

            opNames.Remove(player.Name);

            var opsResponse = new OpsResponse
            {
                Ops = opNames
            };

            return Ok(opsResponse);
        }
    }
}
