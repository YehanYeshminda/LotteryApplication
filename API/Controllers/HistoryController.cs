using API.Helpers;
using API.Models;
using API.Repos.Dtos;
using API.Repos.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static API.Controllers.UpiController;
using static API.Repos.Dtos.DrawDto;

namespace API.Controllers
{
    public class HistoryController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;
        private readonly IHistoryService _historyService;
        private readonly ResponseDto _response;

        public HistoryController(LotteryContext lotteryContext, IHistoryService historyService)
        {
            _lotteryContext = lotteryContext;
            _historyService = historyService;
            _response = new ResponseDto();
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
                        .OrderByDescending(x => x.AddOn)
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
                    var data = await _lotteryContext.Tblorderhistories.Where(x => x.UserId == _user.Id).OrderByDescending(x => x.AddOn).ToListAsync();

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

        public class WinnerList
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string TicketNo { get; set; }
            public DateTime DrawDate { get; set; }
            public string RaffleId { get; set; }
            public int WonAmount { get; set; }
        }

        // TOTAL WINNER HISTORY
        [HttpPost("GetUserTransactionHistory")]
        public async Task<ResponseDto> GetUserTransactionHistory(AuthDto authDto)
        {
            var _response = new ResponseDto();

            if (authDto == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Missing Authentication Details";
                return _response;
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(authDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == authDto.Hash);

            if (_user == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Missing Authentication Details";
                return _response;
            }

            var decryptedDateWithOffset = decodedValues.Date.AddDays(1);
            var currentDate = DateTime.UtcNow.Date;

            if (currentDate < decryptedDateWithOffset.Date)
            {
                try
                {
                    var newWinnerList = new List<WinnerList>(); // Change the type to match your model
                    var winnerList = await _lotteryContext.Tblorderhistories.ToListAsync();
                    var allLotteryWinners = await _lotteryContext.Tbllotterywinners.ToListAsync();

                    if (winnerList.Count == 0)
                    {
                        _response.Message = "Unable to get winners";
                        _response.IsSuccess = false;
                        return _response;
                    }

                    if (allLotteryWinners.Count == 0)
                    {
                        _response.Message = "Unable to get winners";
                        _response.IsSuccess = false;
                        return _response;
                    }

                    foreach (var winner in allLotteryWinners.Where(w => w.UserId == _user.Id))
                    {
                        var matchingOrder = winnerList.FirstOrDefault(order => order.RaffleUniqueId == winner.RaffleUniqueId);

                        var existingUser = _lotteryContext.Tblregisters.FirstOrDefault(x => x.Id == winner.UserId);

                        if (existingUser == null)
                        {
                            _response.Message = "Unable to get winners";
                            _response.IsSuccess = false;
                            return _response;
                        }

                        var newWinner = new WinnerList
                        {
                            Id = winner.Id,
                            DrawDate = winner.DrawDate ?? new DateTime(),
                            RaffleId = winner.RaffleUniqueId,
                            TicketNo = winner.TicketNo,
                            Username = _lotteryContext.Tblregisters.FirstOrDefault(x => x.Id == winner.UserId).CustName,
                            WonAmount = (int)_lotteryContext.Tblraffles.FirstOrDefault(x => x.Id == winner.RaffleId).RafflePrice,
                        };

                        if (matchingOrder != null)
                        {
                            newWinnerList.Add(newWinner);
                        }
                    }

                    _response.Message = "";
                    _response.IsSuccess = true;
                    _response.Result = newWinnerList;
                    return _response;
                }
                catch (Exception ex)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Error while fetching current user history! " + ex.Message;
                    return _response;
                }
            }
            else
            {
                _response.IsSuccess = false;
                _response.Message = "Missing Authentication Details";
                return _response;
            }
        }


        [HttpPost("GetUserTransactionHistoryWinningAmount")]
        public async Task<ResponseDto> GetUserTransactionHistoryWinningAmount(AuthDto authDto)
        {
            var _response = new ResponseDto();

            if (authDto == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Missing Authentication Details";
                return _response;
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(authDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == authDto.Hash);

            if (_user == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Missing Authentication Details";
                return _response;
            }

            var decryptedDateWithOffset = decodedValues.Date.AddDays(1);
            var currentDate = DateTime.UtcNow.Date;

            if (currentDate < decryptedDateWithOffset.Date)
            {
                try
                {
                    var winnerList = await _lotteryContext.Tblorderhistories.ToListAsync();
                    var allLotteryWinners = await _lotteryContext.Tbllotterywinners.ToListAsync();

                    var totalWinningAmount = 0; // Initialize the total winning amount counter

                    foreach (var winner in allLotteryWinners.Where(w => w.UserId == _user.Id))
                    {
                        var matchingOrder = winnerList.FirstOrDefault(order => order.RaffleUniqueId == winner.RaffleUniqueId);

                        if (matchingOrder != null)
                        {
                            var rafflePrice = _lotteryContext.Tblraffles.FirstOrDefault(x => x.Id == winner.RaffleId)?.RafflePrice;
                            totalWinningAmount += (int)rafflePrice;
                        }
                    }

                    _response.Message = "";
                    _response.IsSuccess = true;
                    _response.Result = totalWinningAmount; // Set the total winning amount in the response
                    return _response;
                }
                catch (Exception ex)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Error while fetching current user history! " + ex.Message;
                    return _response;
                }
            }
            else
            {
                _response.IsSuccess = false;
                _response.Message = "Missing Authentication Details";
                return _response;
            }
        }




        public class LoserList
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string TicketNo { get; set; }
            public DateTime? DrawDate { get; set; }
            public string RaffleId { get; set; }
            public int WonAmount { get; set; }
        }

        [HttpPost("GetUserTransactionLoserHistory")]
        public async Task<ResponseDto> GetUserTransactionLoserHistory(AuthDto authDto)
        {
            var _response = new ResponseDto();

            if (authDto == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Missing Authentication Details";
                return _response;
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(authDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == authDto.Hash);

            if (_user == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Missing Authentication Details";
                return _response;
            }

            var decryptedDateWithOffset = decodedValues.Date.AddDays(1);
            var currentDate = DateTime.UtcNow.Date;

            if (currentDate < decryptedDateWithOffset.Date)
            {
                try
                {
                    var loserList = new List<LoserList>();

                    var winnerList = await _lotteryContext.Tblorderhistories.ToListAsync();
                    var allLotteryWinners = await _lotteryContext.Tbllotterywinners.ToListAsync();

                    var raffleUniqueIds = allLotteryWinners
                        .Where(winner => winner.UserId == _user.Id)
                        .Select(winner => winner.RaffleUniqueId)
                        .ToList();

                    var loserHistory = winnerList
                        .Where(history => !raffleUniqueIds.Contains(history.RaffleUniqueId) && history.UserId == _user.Id)
                        .ToList();

                    foreach (var item in loserHistory)
                    {
                        var newLoserItem = new LoserList
                        {
                            DrawDate = item.AddOn,
                            Id = item.Id,
                            RaffleId = item.RaffleUniqueId,
                            TicketNo = item.TicketNo,
                            Username = item.UserId.ToString(),
                            WonAmount = (int)_lotteryContext.Tblraffles.FirstOrDefault(x => x.Id == item.RaffleId).RafflePrice,
                        };

                        loserList.Add(newLoserItem);
                    }

                    _response.Message = "";
                    _response.IsSuccess = true;
                    _response.Result = loserList;
                    return _response;
                }
                catch (Exception ex)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Error while fetching current user history! " + ex.Message;
                    return _response;
                }
            }
            else
            {
                _response.IsSuccess = false;
                _response.Message = "Missing Authentication Details";
                return _response;
            }
        }



        [HttpPost("GetUserTransactionLoserHistoryTotal")]
        public async Task<ResponseDto> GetUserTransactionLoserHistoryTotal(AuthDto authDto)
        {
            if (authDto == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Missing Authentication Details";
                return _response;
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(authDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == authDto.Hash);

            if (_user == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Missing Authentication Details";
                return _response;
            }

            var decryptedDateWithOffset = decodedValues.Date.AddDays(1);
            var currentDate = DateTime.UtcNow.Date;

            if (currentDate < decryptedDateWithOffset.Date)
            {
                try
                {
                    var winnerList = await _lotteryContext.Tblorderhistories.ToListAsync();
                    var allLotteryWinners = await _lotteryContext.Tbllotterywinners.ToListAsync();

                    var raffleUniqueIds = allLotteryWinners
                        .Where(winner => winner.UserId == _user.Id)
                        .Select(winner => winner.RaffleUniqueId)
                        .ToList();

                    var loserHistoryForUser = winnerList
                        .Where(history => !raffleUniqueIds.Contains(history.RaffleUniqueId) && history.UserId == _user.Id)
                        .ToList();

                    var totalLostAmount = 0;

                    foreach (var item in loserHistoryForUser)
                    {
                        var rafflePrice = _lotteryContext.Tblraffles.FirstOrDefault(x => x.Id == item.RaffleId)?.RafflePrice;
                        if (rafflePrice.HasValue)
                        {
                            totalLostAmount += (int)rafflePrice;
                        }
                    }

                    _response.Message = "";
                    _response.IsSuccess = true;
                    _response.Result = totalLostAmount;
                    return _response;
                }
                catch (Exception ex)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Error while fetching current user history! " + ex.Message;
                    return _response;
                }
            }
            else
            {
                _response.IsSuccess = false;
                _response.Message = "Missing Authentication Details";
                return _response;
            }
        }

    }
}
