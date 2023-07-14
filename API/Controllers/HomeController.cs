using API.Repos;
using API.Repos.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class HomeController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;

        public HomeController(LotteryContext lotteryContext)
        {
            _lotteryContext = lotteryContext;
        }

        public class GetRaffleDto
        {
            public AuthDto AuthDto { get; set; }
            public int Id { get; set; }
            public string DrawName { get; set; }
            public DateTime DrawDateStartDate { get; set; }
            public DateTime DrawDateEndDate { get; set; }
            public string SequenceNo { get; set; }
            public string RafflePrice { get; set; }
        }

        [HttpPost("GetHomeInfo")]
        public async Task<ActionResult<IEnumerable<GetRaffleDto>>> GetRaffleInformation()
        {
            var items = await _lotteryContext.Tblraffles.ToListAsync();

            if (items == null)
            {
                return BadRequest("Error while finding items");
            }

            return Ok(items);
        }
    }
}
