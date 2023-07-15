using API.Helpers;
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
        public async Task<ActionResult<IEnumerable<GetRaffleDto>>> GetRaffleInformation([FromBody] AuthDto authDto)
        {
            if (authDto.Hash == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(authDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == authDto.Hash);

            if (_user == null)
            {
                return Unauthorized("Invalid Authentication Details");
            }

            var decryptedDateWithOffset = decodedValues.Date.AddDays(1);
            var currentDate = DateTime.UtcNow.Date;

            if (currentDate < decryptedDateWithOffset.Date)
            {
                var items = await _lotteryContext.Tblraffles.ToListAsync();

                if (items == null)
                {
                    return BadRequest("Error while finding Raffles");
                }

                return Ok(items);
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }
    }
}
