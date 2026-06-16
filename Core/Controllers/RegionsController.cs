using Core.Enums;
using Core.Responses;
using Microsoft.AspNetCore.Mvc;
using Region = Core.Enums.Region;

namespace Core.Controllers;

[ApiController]
[Route("[controller]")]
public class RegionsController : ControllerBase
{
    [HttpGet("ping/stat")]
    public ActionResult GetPing()
    {
        return Ok();
    }

    [HttpGet("preferredRegions")]
    public ActionResult<RegionDataArray> GetPreferredRegions()
    {
        var regionDataList = new RegionDataArray { RegionDataList = [] };

        foreach (var region in Enum.GetNames<Region>())
            regionDataList.RegionDataList.Add(new Models.Region
            {
                RegionName = region,
                ServiceQuality = ServiceQuality.Great
            });

        return Ok(regionDataList);
    }
}
