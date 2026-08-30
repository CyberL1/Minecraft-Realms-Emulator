using Core.Attributes;
using Core.Data;
using Core.Models;
using Core.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core.Controllers;

[ApiController]
[Route("[controller]")]
public class OpsController(DataContext context) : ControllerBase
{
    [HttpPost("{realmId:int}/{playerUuid}")]
    [HasRealmAccess(true)]
    public async Task<ActionResult<OperatorList>> OpPlayer(int realmId, string playerUuid)
    {
        var player = await context.Players.Where(player => player.RealmId == realmId)
            .FirstOrDefaultAsync(p => p.Uuid == playerUuid);

        if (player == null) return StatusCode(500, ApiError.PlayerNotInvited);


        player.Operator = true;
        await context.SaveChangesAsync();

        var ops = await context.Players.Where(player => player.RealmId == realmId && player.Operator).ToListAsync();

        List<string> opNames = [];

        opNames.AddRange(ops.Select(op => op.Name));

        var operatorList = new OperatorList { Ops = opNames };
        return Ok(operatorList);
    }

    [HttpDelete("{realmId:int}/{playerUuid}")]
    [HasRealmAccess(true)]
    public async Task<ActionResult<OperatorList>> DeopPlayer(int realmId, string playerUuid)
    {
        var player = context.Players.Where(player => player.RealmId == realmId)
            .FirstOrDefault(p => p.Uuid == playerUuid);

        if (player == null) return StatusCode(500, ApiError.PlayerNotInvited);

        player.Operator = false;
        await context.SaveChangesAsync();

        var ops = context.Players.Where(player => player.RealmId == realmId && player.Operator).ToList();

        List<string> opNames = [];
        opNames.AddRange(ops.Select(op => op.Name));

        var operatorList = new OperatorList { Ops = opNames };
        return Ok(operatorList);
    }
}
