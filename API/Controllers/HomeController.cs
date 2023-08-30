using API.Helpers;
using API.Models;
using API.Repos.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

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

        [HttpPost("GetLottoWinnerEasy")]
        public async Task<ActionResult<DrawItemsToReturn>> GetEasyDrawItemsForPastDays(AuthDto authDto)
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
                var newList = new List<DrawItemsToReturn>();
                DateTime yesterday = IndianTimeHelper.GetIndianLocalTime().AddDays(-1);
                var items = await _lotteryContext.Tbldrawhistories.Where(x => x.LotteryId == 2 && x.DrawDate >= yesterday).ToListAsync();

                foreach (var item in items)
                {
                    var newItemToAdd = new DrawItemsToReturn
                    {
                        AddOn = item.DrawDate,
                        RaffleName = "Easy Draw",
                        TicketNo = item.Sequence,
                        Username = item.UniqueLotteryId
                    };

                    newList.Add(newItemToAdd);
                }

                if (items == null)
                {
                    return BadRequest("Error while finding Raffles");
                }

                return Ok(newList);
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

        [HttpPost("GetLottoWinnerMega")]
        public async Task<ActionResult<DrawItemsToReturn>> GetDrawItemsForPastDays(AuthDto authDto)
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
                var newList = new List<DrawItemsToReturn>();
                DateTime yesterday = IndianTimeHelper.GetIndianLocalTime().AddDays(-1);
                var items = await _lotteryContext.Tbldrawhistories.Where(x => x.LotteryId == 1 && x.DrawDate >= yesterday).ToListAsync();

                foreach (var item in items)
                {
                    var newItemToAdd = new DrawItemsToReturn
                    {
                        AddOn = item.DrawDate,
                        RaffleName = "Mega Draw",
                        TicketNo = item.Sequence,
                        Username = item.UniqueLotteryId
                    };

                    newList.Add(newItemToAdd);
                }

                if (items == null)
                {
                    return BadRequest("Error while finding Raffles");
                }

                return Ok(newList);
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

        [HttpPost("GetPastLottos")]
        public async Task<ActionResult<DrawItemsToReturn>> GetPastLottoDrawsBoughts(AuthDto authDto)
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
                var newList = new List<DrawItemsToReturn>();
                DateTime yesterday = IndianTimeHelper.GetIndianLocalTime().AddMinutes(-15);
                var items = await _lotteryContext.Tbllottoorderhistories
                    .Where(order => order.AddOn >= yesterday)
                    .OrderByDescending(x => x.AddOn)
                    .ToListAsync();

                foreach (var item in items)
                {
                    var newItemToAdd = new DrawItemsToReturn
                    {
                        AddOn = item.AddOn,
                        RaffleName = item.ReferenceId,
                        TicketNo = item.LottoNumbers,
                        Username = _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == item.UserId).Result.CustName
                    };

                    newList.Add(newItemToAdd);
                }

                if (items == null)
                {
                    return BadRequest("Error while finding Raffles");
                }

                return Ok(newList);
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

        [HttpPost("GetPastEasyDraws")]
        public async Task<ActionResult<DrawItemsToReturn>> GetPastEasyDayDrawsBoughts(AuthDto authDto)
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
                var newList = new List<DrawItemsToReturn>();
                DateTime yesterday = IndianTimeHelper.GetIndianLocalTime().AddDays(-1);
                var items = await _lotteryContext.Tblorderhistories
                    .Where(order => order.AddOn >= yesterday && order.RaffleId == 2)
                    .ToListAsync();

                foreach (var item in items)
                {
                    var newItemToAdd = new DrawItemsToReturn
                    {
                        AddOn = item.AddOn,
                        RaffleName = _lotteryContext.Tblraffles.FirstOrDefaultAsync(x => x.Id == item.RaffleId).Result.RaffleName,
                        TicketNo = item.TicketNo,
                        Username = _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == item.UserId).Result.CustName
                    };

                    newList.Add(newItemToAdd);
                }

                if (items == null)
                {
                    return BadRequest("Error while finding Raffles");
                }

                return Ok(newList);
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

        public class DrawItemsToReturn
        {
            public string RaffleName { get; set; }
            public string TicketNo { get; set; }
            public string Username { get; set; }
            public DateTime? AddOn { get; set; }
        }

        [HttpPost("GetPastMegaDraws")]
        public async Task<ActionResult<DrawItemsToReturn>> GetPastMegaDayDrawsBoughts(AuthDto authDto)
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
                var newList = new List<DrawItemsToReturn>();
                DateTime yesterday = IndianTimeHelper.GetIndianLocalTime().AddDays(-1);
                var items = await _lotteryContext.Tblorderhistories
                    .Where(order => order.AddOn >= yesterday && order.RaffleId == 1)
                    .ToListAsync();

                foreach (var item in items)
                {
                    var newItemToAdd = new DrawItemsToReturn
                    {
                        AddOn = item.AddOn,
                        RaffleName = _lotteryContext.Tblraffles.FirstOrDefaultAsync(x => x.Id == item.RaffleId).Result.RaffleName,
                        TicketNo = item.TicketNo,
                        Username = _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == item.UserId).Result.CustName
                    };

                    newList.Add(newItemToAdd);
                }

                if (items == null)
                {
                    return BadRequest("Error while finding Raffles");
                }

                return Ok(newList);
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
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
