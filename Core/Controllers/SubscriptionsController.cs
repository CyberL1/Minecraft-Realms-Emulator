using Core.Attributes;
using Core.Data;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core.Controllers;

[ApiController]
[Route("[controller]")]
public class SubscriptionsController(DataContext context) : ControllerBase
{
    [HttpGet("{wId}")]
    [HasRealmAccess(true)]
    public async Task<ActionResult<Subscription>> Get(int wId)
    {
        var realm = await context.Realms.Include(realm => realm.Subscription).FirstAsync(w => w.Id == wId);

        var subscriptionResponse = new Models.Responses.Subscription
        {
            StartDate = ((DateTimeOffset)realm.Subscription.StartDate).ToUnixTimeMilliseconds(),
            DaysLeft = realm.Subscription.DaysLeft,
            SubscriptionType = realm.Subscription.Type
        };

        return Ok(subscriptionResponse);
    }
}
