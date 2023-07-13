using API.API.Repos.Models;
using API.Helpers;
using API.Repos;
using API.Repos.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static API.Controllers.CartController;

namespace API.Controllers
{
    public class CartController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;

        public CartController(LotteryContext lotteryContext)
        {
            _lotteryContext = lotteryContext;
        }

        public class AddToCartDto
        {
            public AuthDto AuthDto { get; set; }
            public List<int> CartNumbers { get; set; }
            public int RaffleId { get; set; }
        }


        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(AddToCartDto addToCartDto)
        {
            if (addToCartDto.AuthDto.Hash == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(addToCartDto.AuthDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == addToCartDto.AuthDto.Hash);

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
                    var existingRaffle = await _lotteryContext.Tblraffles.SingleOrDefaultAsync(x => x.Id == addToCartDto.RaffleId);

                    if (existingRaffle == null)
                    {
                        return BadRequest("Raffle does not exist!");
                    }

                    var convertedNumbers = String.Join("", addToCartDto.CartNumbers);
                    var existingLotteryValue = await _lotteryContext.Tbllotterynos.SingleOrDefaultAsync(x => x.LotteryNo == convertedNumbers && x.RaffleNo == existingRaffle.Id.ToString());

                    if (existingLotteryValue != null)
                    {
                        return BadRequest("Lottery Number Already Exists");
                    }

                    var lotteryNo = new Tbllotteryno
                    {
                        AddOn = DateTime.UtcNow,
                        AmountToPay = 0,
                        Paid = 0,
                        RaffleNo = addToCartDto.RaffleId.ToString(),
                        LotteryStatus = 0,
                        UserId = _user.Id,
                        LotteryNo = convertedNumbers,
                    };

                    await _lotteryContext.AddAsync(lotteryNo);
                    await _lotteryContext.SaveChangesAsync();

                    var lotteryToReturn = new TblLotteryAdvancedDto
                    {
                        Id = lotteryNo.Id,
                        AddOn = DateTime.UtcNow,
                        AmountToPay = 0,
                        Paid = 0,
                        RaffleNo = addToCartDto.RaffleId.ToString(),
                        LotteryStatus = 0,
                        UserId = _user.Id,
                        LotteryNo = convertedNumbers,
                        LotteryName = existingRaffle.RaffleName,
                        Numbers = addToCartDto.CartNumbers
                    };

                    return Ok(lotteryToReturn);
                }
                catch (Exception ex)
                {
                    return BadRequest("Error occured while saving cart item! " + ex.Message);
                }
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

        [HttpPost("GetCartItems")]
        public async Task<ActionResult<IEnumerable<Tbllotteryno>>> GetCartItems(AuthDto authDto)
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
                var items = await _lotteryContext.Tbllotterynos.Where(x => x.UserId == _user.Id).ToListAsync();


                if (items == null)
                {
                    return Ok(null);
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
