using API.Helpers;
using API.Repos;
using API.Repos.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class HistoryController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;

        public HistoryController(LotteryContext lotteryContext)
        {
            _lotteryContext = lotteryContext;
        }

        [HttpPost("User-History")]
        public async Task<ActionResult<IEnumerable<GetHistoryDto>>> GetUserHistoryController(AuthDto authDto)
        {
            if (authDto == null)
            {
                return BadRequest("Invalid data!");
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
                try
                {
                    var histories = new List<GetHistoryDto>();
                    var items = await _lotteryContext.Tblorderhistories.Where(x => x.UserId == decodedValues.UserId).ToListAsync();

                    foreach (var item in items)
                    {
                        var newItem = new GetHistoryDto
                        {
                            OrderedOn = item.AddOn,
                            RaffleId = item.RaffleId,
                            ReferenceId = item.LotteryReferenceId,
                            TicketNumber = item.TicketNo,
                            UniqueRaffleId = item.RaffleUniqueId
                        };

                        histories.Add(newItem);
                    }

                    return histories;
                }
                catch (Exception ex)
                {
                    return BadRequest("Error occurred while getting Draw Histories! " + ex.Message);
                }
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

    }
}
