using API.Models;
using API.Repos.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class RegionController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;

        public RegionController(LotteryContext lotteryContext)
        {
            _lotteryContext = lotteryContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRegions()
        {
            var regions = await _lotteryContext.Tblregions.ToListAsync();

            if (regions == null)
            {
                return BadRequest("Unable to find any regions");
            }

            return Ok(regions);
        }

        [HttpPost]
        public async Task<IActionResult> AddRegion(AddNewRegionDto region)
        {
            if (region == null)
            {
                return BadRequest("Region is null");
            }

            if (region.RegionName == null)
            {
                return BadRequest("Region name is null");
            }

            if (region.Code == null)
            {
                return BadRequest("Region code is null");
            }

            if (region.Country == null)
            {
                return BadRequest("Region country is null");
            }

            var newItem = new Tblregion
            {
                Code = region.Code,
                Country = region.Country,
                RegionName = region.RegionName
            };

            await _lotteryContext.Tblregions.AddAsync(newItem);
            await _lotteryContext.SaveChangesAsync();

            return Ok(region);
        }
    }
}
