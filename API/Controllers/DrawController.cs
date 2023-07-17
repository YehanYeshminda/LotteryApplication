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

        public DrawController(LotteryContext lotteryContext)
        {
            _lotteryContext = lotteryContext;
        }

        [HttpPost("EasyDraw")]
        public async Task<ActionResult> CreateEasyDraw(CreateEasyDrawDto createEasyDrawDto)
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
                    bool hasDuplicates = ticketNo.Distinct().Count() != ticketNo.Length;
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
                        RaffleName = createEasyDrawDto.RaffleName
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
    }
}
