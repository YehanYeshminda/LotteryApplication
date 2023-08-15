using API.Models;
using Microsoft.AspNetCore.Mvc;

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
            //var regions = await _lotteryContext.Tblregions.ToListAsync();
            return Ok();
        }
    }
}
