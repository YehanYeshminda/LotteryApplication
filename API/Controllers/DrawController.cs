﻿using API.Helpers;
using API.Models;
using API.Repos.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Twilio.Http;
using static API.Repos.Dtos.DrawDto;

namespace API.Controllers
{
    public class DrawController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;
        private readonly Generators _generators;
        private ResponseDto _response;
        public DrawController(LotteryContext lotteryContext, Generators generators)
        {
            _lotteryContext = lotteryContext;
            _generators = generators;
            _response = new ResponseDto();
        }

        [HttpPost("Draw")]
        public async Task<ActionResult> CreateDraw(CreateEasyDrawDto createEasyDrawDto)
        {
            if (createEasyDrawDto == null)
            {
                return BadRequest("Invalid data!");
            }

            var existingDrawName = await _lotteryContext.Tblraffles.FirstOrDefaultAsync(x => x.RaffleName == createEasyDrawDto.RaffleName);

            if (existingDrawName != null) 
            {
                return BadRequest("Draw name already exists!");
            }

            if (createEasyDrawDto.AuthDto.Hash == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(createEasyDrawDto.AuthDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == createEasyDrawDto.AuthDto.Hash);

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
                    string ticketNo = createEasyDrawDto.TicketNo.ToString();
                    bool hasDuplicates = false;

                    for (int i = 0; i < ticketNo.Length - 1; i++)
                    {
                        if (ticketNo[i] == ticketNo[i + 1])
                        {
                            hasDuplicates = true;
                            break;
                        }
                    }

                    if (hasDuplicates)
                    {
                        return BadRequest("TicketNo contains duplicate values!");
                    }

                    var draw = new Tblraffle
                    {
                        RaffleDate = createEasyDrawDto.RaffleDate,
                        StartOn = createEasyDrawDto.StartOn,
                        EndOn = createEasyDrawDto.EndOn,
                        CustStatus = createEasyDrawDto.CustStatus,
                        TicketNo = createEasyDrawDto.TicketNo.ToString(),
                        RaffleName = createEasyDrawDto.RaffleName,
                        UniqueRaffleId = _generators.GenerateRandomString(6),
                        WinAmount = createEasyDrawDto.WinAmount ?? 0
                    };

                    await _lotteryContext.Tblraffles.AddAsync(draw);
                    await _lotteryContext.SaveChangesAsync();
                    return Ok(draw);
                }
                catch (Exception ex)
                {
                    return BadRequest("Error occured while creating Draw!" + ex.Message);
                }

            } else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

        [HttpPost("GetEasyDrawInfo")]
        public async Task<IActionResult> GetEasyDrawInfo(AuthDto authDto)
        {
            if (authDto == null)
            {
                return BadRequest("Invalid data!");
            }

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
                try
                {
                    var draw = await _lotteryContext.Tblraffles.FirstOrDefaultAsync(x => x.Id == 2);
                    return Ok(draw);
                }
                catch (Exception ex)
                {
                    return BadRequest("Error occured while creating Draw!" + ex.Message);
                }

            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

        [HttpPost("GetMegaDrawInfo")]
        public async Task<IActionResult> GetMegaDrawInfo(AuthDto authDto)
        {
            if (authDto == null)
            {
                return BadRequest("Invalid data!");
            }

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
                try
                {
                    var draw = await _lotteryContext.Tblraffles.FirstOrDefaultAsync(x => x.Id == 1);
                    return Ok(draw);
                }
                catch (Exception ex)
                {
                    return BadRequest("Error occured while creating Draw!" + ex.Message);
                }

            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

        public class OldRafflesReponse
        {
            public int Id { get; set; }
            public int? LotteryId { get; set; }
            public string? DrawDate { get; set; }
            public string? Sequence { get; set; }
            public int WinnerCount { get; set; }
        }

        public class DrawHistoryWithWinnerCountDto
        {
            public Tbldrawhistory DrawHistory { get; set; }
            public int WinnerCount { get; set; }
        }


        [HttpPost("GetAllDrawHistories")]
        public async Task<ActionResult<List<OldRafflesReponse>>> GetAllDrawHistory([FromBody] AuthDto authDto)
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
                    var drawHistories = await _lotteryContext.Tbldrawhistories.ToListAsync();
                    var lotteryWinners = await _lotteryContext.Tbllotterywinners.ToListAsync();

                    if (lotteryWinners.Count == 0 || lotteryWinners == null)
                    {
                        var defaultLotteryWinner = new Tbllotterywinner
                        {
                            AddOn = DateTime.UtcNow,
                            DrawDate = currentDate,
                            Matches = 0,
                            RaffleId = 0,
                            RaffleUniqueId = "",
                            TicketNo = "",
                            UserId = 0,
                            Id = 0
                        };

                        lotteryWinners.Add(defaultLotteryWinner);
                    }

                    var drawHistoryWithWinnerCountList = new List<DrawHistoryWithWinnerCountDto>();

                    foreach (var drawHistory in drawHistories)
                    {
                        var winnerCount = lotteryWinners.Count(w => w.RaffleUniqueId == drawHistory.UniqueLotteryId);
                        var drawHistoryWithWinnerCount = new DrawHistoryWithWinnerCountDto
                        {
                            DrawHistory = drawHistory,
                            WinnerCount = winnerCount
                        };
                        drawHistoryWithWinnerCountList.Add(drawHistoryWithWinnerCount);
                    }

                    var oldRafflesResponseList = drawHistoryWithWinnerCountList.Select(drawHistoryWithWinnerCount => new OldRafflesReponse
                    {
                        Id = drawHistoryWithWinnerCount.DrawHistory.Id,
                        LotteryId = drawHistoryWithWinnerCount.DrawHistory.LotteryId,
                        DrawDate = drawHistoryWithWinnerCount.DrawHistory.DrawDate?.ToString("yyyy-MM-ddTHH:mm:ss"),
                        Sequence = drawHistoryWithWinnerCount.DrawHistory.Sequence,
                        WinnerCount = drawHistoryWithWinnerCount.WinnerCount
                    }).ToList();

                    return Ok(oldRafflesResponseList);
                }
                catch (Exception ex)
                {
                    return BadRequest("Error occurred while getting Draw Histories!" + ex.Message);
                }
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

        public class GetAllDrawsDto
        {
            public int Id { get; set; }
            public DateTime? RaffleDate { get; set; }
            public DateTime? EndOn { get; set; }
            public string? RaffleName { get; set; }
            public int? DrawCount { get; set; }
            public decimal? WinAmount { get; set; }
            public int? RafflePrice { get; set; }
        }

        [HttpPost("GetAllRaffles")]
        public async Task<ActionResult<IEnumerable<GetAllDrawsDto>>> GetAllRaffles(AuthDto authDto)
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
                    var allRaffles = await _lotteryContext.Tblraffles
                        .Select(item => new GetAllDrawsDto
                        {
                            Id = (int)item.Id,
                            DrawCount = item.DrawCount,
                            EndOn = item.EndOn,
                            RaffleDate = item.RaffleDate,
                            RaffleName = item.RaffleName,
                            WinAmount = item.WinAmount,
                            RafflePrice = (int)item.RafflePrice
                        })
                        .ToListAsync();

                    return Ok(allRaffles);

                }
                catch (Exception ex)
                {
                    return BadRequest("Error occurred while getting Draw Histories!" + ex.Message);
                }
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

        public class BuyDrawsDto
        {
            public string RaffleId { get; set; }
            public string TicketNo { get; set; }
            public AuthDto AuthDto { get; set; }
        }

        [NonAction]
        public string GenerateUniqueOrderHistoryNumber()
        {
            string orderNo;
            do
            {
                orderNo = _generators.GenerateRandomNumericStringForTblOrderHistories(8);
            } while (!_generators.IsUniqueOrder(orderNo));

            return orderNo;
        }

        [HttpPost("BuyDraws")]
        public async Task<ResponseDto> BuyDraws([FromBody] BuyDrawsDto buyDrawsDto)
        {
            if (buyDrawsDto.AuthDto.Hash == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Missing Hash Information!";
                return _response;
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(buyDrawsDto.AuthDto.Hash);

            var _user = await _lotteryContext.Tblregisters
                .FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == buyDrawsDto.AuthDto.Hash);

            if (_user == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Invalid Authentication Details!";
                return _response;
            }

            var decryptedDateWithOffset = decodedValues.Date.AddDays(1);
            var currentDate = DateTime.UtcNow.Date;

            if (currentDate < decryptedDateWithOffset.Date)
            {
                try
                {
                    var existingRaffle = await _lotteryContext.Tblraffles
                        .FirstOrDefaultAsync(x => x.Id == Convert.ToInt32(buyDrawsDto.RaffleId));

                    if (existingRaffle == null)
                    {
                        _response.IsSuccess = false;
                        _response.Message = "Invalid Raffle Id!";
                        return _response;
                    }

                    var existingTicketNo = await _lotteryContext.Tblorderhistories.AnyAsync(x => x.TicketNo == buyDrawsDto.TicketNo);

                    if (existingTicketNo != false)
                    {
                        _response.IsSuccess = false;
                        _response.Message = "Duplicate Ticket No!";
                        return _response;
                    }

                    if (_user.AccountBalance < existingRaffle.RafflePrice)
                    {
                        _response.IsSuccess = false;
                        _response.Message = "Insufficient Account Balance!";
                        return _response;
                    }

                    var newOrder = new Tblorderhistory
                    {
                        AddOn = IndianTimeHelper.GetIndianLocalTime(),
                        LotteryReferenceId = GenerateUniqueOrderHistoryNumber(),
                        RaffleId = Convert.ToInt32(existingRaffle.Id),
                        RaffleUniqueId = existingRaffle.UniqueRaffleId,
                        TicketNo = buyDrawsDto.TicketNo,
                        UserId = _user.Id,
                        AssignedSupervisorId = _user.AssignedSupervisorId
                    };

                    _user.AccountBalance -= existingRaffle.RafflePrice;
                    await _lotteryContext.Tblorderhistories.AddAsync(newOrder);
                    await _lotteryContext.SaveChangesAsync();

                    _response.IsSuccess = true;
                    _response.Result = newOrder;
                    _response.Message = "Successfully Purchased!";
                    return _response;
                }
                catch (Exception ex)
                {
                    _response.IsSuccess = false;
                    _response.Message = ex.Message;

                    return _response;
                }
            }
            else
            {
                _response.IsSuccess = false;
                _response.Message = "Invalid Authentication Details";
                return _response;
            }
        }
    }
}
