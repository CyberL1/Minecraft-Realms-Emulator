using Microsoft.AspNetCore.Mvc;
using Minecraft_Realms_Emulator.Attributes;
using Minecraft_Realms_Emulator.Enums;
using Minecraft_Realms_Emulator.Objects;
using Minecraft_Realms_Emulator.Responses;

namespace Minecraft_Realms_Emulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [RequireMinecraftCookie]
    public class RegionsController : ControllerBase
    {
        [HttpGet("ping/stat")]
        public ActionResult GetRegionsStat()
        {
            return NoContent();
        }

        [HttpGet("preferredRegions")]
        public ActionResult<string> GetPreferredRegions()
        {
            var response = new RegionDataListResponse();

            foreach (var region in Enum.GetNames<RegionEnum>())
            {
                response.RegionDataList.Add(new RegionObject
                {
                    RegionName = region,
                    ServiceQuality = RegionServiceQualityEnum.Great
                });
            }
            
            return Ok(response);
        }
    }
}