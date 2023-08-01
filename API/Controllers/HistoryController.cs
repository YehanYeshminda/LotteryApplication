using API.Helpers;
using API.Models;
using API.Repos.Dtos;
using API.Repos.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class HistoryController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;
        private readonly IHistoryService _historyService;

        public HistoryController(LotteryContext lotteryContext, IHistoryService historyService)
        {
            _lotteryContext = lotteryContext;
            _historyService = historyService;
        }

        [HttpPost("User-History")]
        public async Task<ActionResult<PagedList<GetHistoryDto>>> GetUserHistoryController([FromBody] UserParams userParams)
        {
            if (userParams.authDto == null)
            {
                return BadRequest("Invalid data!");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(userParams.authDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == userParams.authDto.Hash);

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
                    var query = _lotteryContext.Tblorderhistories
                        .Where(x => x.UserId == decodedValues.UserId)
                        .Select(item => new GetHistoryDto
                        {
                            OrderedOn = item.AddOn,
                            RaffleId = item.RaffleId,
                            ReferenceId = item.LotteryReferenceId,
                            TicketNumber = item.TicketNo,
                            UniqueRaffleId = item.RaffleUniqueId,
                            IsWin = false
                        });

                    var pagedHistories = await PagedList<GetHistoryDto>.CreateAsync(
                        query, userParams.PageNumber, userParams.PageSize);

                    return Ok(pagedHistories);
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

        [HttpPost("User-History-Winnings")]
        public async Task<ActionResult<IEnumerable<GetHistoryDto>>> GetUserWinnings(AuthDto authDto)
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
                    var data = await _lotteryContext.Tblorderhistories.Where(x => x.UserId == _user.Id).ToListAsync();

                    var dataToReturn = data.Select(item => new GetHistoryDto
                    {
                        OrderedOn = item.AddOn,
                        RaffleId = item.RaffleId,
                        ReferenceId = item.LotteryReferenceId,
                        TicketNumber = item.TicketNo,
                        UniqueRaffleId = item.RaffleUniqueId,
                        IsWin = _lotteryContext.Tbllotterywinners.Any(x => x.TicketNo == item.TicketNo)
                    });

                    return Ok(dataToReturn.Where(x => x.IsWin).ToList());

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

        [HttpPost("User-History-Winnings-BasedOnDate")]
        public async Task<ActionResult<IEnumerable<GetHistoryDto>>> GetUserWinningsBasedOnDate([FromBody] AuthDto authDto)
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
                    var lastSevenDaysDate = DateTime.UtcNow.Date.AddDays(-3);

                    var data = await _lotteryContext.Tblorderhistories
                        .Where(item => item.AddOn >= lastSevenDaysDate && item.AddOn <= DateTime.UtcNow && item.UserId == _user.Id)
                        .ToListAsync();

                    var dataToReturn = data.Select(item => new GetHistoryDto
                    {
                        OrderedOn = item.AddOn,
                        RaffleId = item.RaffleId,
                        ReferenceId = item.LotteryReferenceId,
                        TicketNumber = item.TicketNo,
                        UniqueRaffleId = item.RaffleUniqueId,
                        IsWin = _lotteryContext.Tbllotterywinners.Any(x => x.TicketNo == item.TicketNo)
                    }).OrderByDescending(x => x.OrderedOn);

                    return Ok(dataToReturn);

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

        [HttpPost("Search-Based-OnHistory")]
        public async Task<ActionResult<IEnumerable<GetHistoryDto>>> GetAllHistoryBasedOnUnqueRaffle([FromBody] SearchBasedOnHistory searchBasedOnHistory)
        {
            if (searchBasedOnHistory.AuthDto == null)
            {
                return BadRequest("Invalid data!");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(searchBasedOnHistory.AuthDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == searchBasedOnHistory.AuthDto.Hash);

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
                    var data = await _historyService.GetUserHistoryWinningsBasedOnUniqueRaffleId(decodedValues.UserId, searchBasedOnHistory.RaffleUniqueId);
                    return Ok(data);
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
