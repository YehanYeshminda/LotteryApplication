using API.Helpers;
using API.Repos;
using API.Repos.Dtos;
using API.Repos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class DrawController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;
        private readonly Generators _generators;

        public DrawController(LotteryContext lotteryContext, Generators generators)
        {
            _lotteryContext = lotteryContext;
            _generators = generators;
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

                    // Create a list to hold DrawHistoryWithWinnerCountDto objects
                    var drawHistoryWithWinnerCountList = new List<DrawHistoryWithWinnerCountDto>();

                    // Loop through each draw history record and count winners for each UniqueLotteryId
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

                    // Create a list to hold OldRafflesReponse objects
                    var oldRafflesResponseList = drawHistoryWithWinnerCountList.Select(drawHistoryWithWinnerCount => new OldRafflesReponse
                    {
                        Id = drawHistoryWithWinnerCount.DrawHistory.Id,
                        LotteryId = drawHistoryWithWinnerCount.DrawHistory.LotteryId,
                        DrawDate = drawHistoryWithWinnerCount.DrawHistory.DrawDate?.ToString("yyyy-MM-ddTHH:mm:ss"),
                        Sequence = drawHistoryWithWinnerCount.DrawHistory.Sequence,
                        WinnerCount = drawHistoryWithWinnerCount.WinnerCount
                    }).ToList();

                    // Return the list of OldRafflesReponse
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

    }
}
