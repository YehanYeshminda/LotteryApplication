using API.Helpers;
using API.Models;
using API.Repos.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static API.Controllers.CartController;

namespace API.Controllers
{
    public class WinnerController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;

        public WinnerController(LotteryContext lotteryContext)
        {
            _lotteryContext = lotteryContext;
        }

        public class GetAllWinnersDto
        {
            public int Id { get; set; }
            public string UserName { get; set; }
            public string Price { get; set; }
        }

        [HttpPost("Get10Winners")]
        public async Task<ActionResult<IEnumerable<GetAllWinnersDto>>> GetAllWinners(AuthDto authDto)
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
                try
                {
                    var allPackages = await _lotteryContext.Tblpackageorderhistories.Take(10).ToListAsync();

                    if (allPackages == null)
                    {
                        return BadRequest("packages does not exist!");
                    }

                    var newListToReturn = new List<GetAllWinnersDto>();

                    foreach (var item in allPackages)
                    {
                        var newItem = new GetAllWinnersDto
                        {
                            Id = (int)item.Id,
                            Price = item.PackagePrice,
                            UserName = _lotteryContext.Tblregisters.FirstOrDefault(x => x.Id == item.UserId).CustName,
                        };

                        newListToReturn.Add(newItem);
                    }

                    return Ok(newListToReturn);
                }
                catch (Exception ex)
                {
                    return BadRequest("Error occured while deleting cart item! " + ex.Message);
                }
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }
    }
}
