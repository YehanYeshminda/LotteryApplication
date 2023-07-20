using API.API.Repos.Models;
using API.Helpers;
using API.Repos;
using API.Repos.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class CartController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;
        private readonly Generators _generators;

        public CartController(LotteryContext lotteryContext, Generators generators)
        {
            _lotteryContext = lotteryContext;
            _generators = generators;
        }

        public class AddToCartDto
        {
            public AuthDto AuthDto { get; set; }
            public List<int> CartNumbers { get; set; }
            public int RaffleId { get; set; }
            public int UserId { get; set; }
            public int LotteryStatus { get; set; } = 0;
            public string Name { get; set; }
            public decimal Paid { get; set; }
            public decimal Price { get; set; }
        }

        [NonAction]
        public string GenerateUniqueLotteryNumber()
        {
            string raffleNumber;
            do
            {
                raffleNumber = _generators.GenerateRandomNumericStringForLottery(8);
            } while (!_generators.IsUniqueLotteryNoId(raffleNumber));

            return raffleNumber;
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
                        return BadRequest("Lottery number already exists!");
                    }

                    var lotteryNo = new Tbllotteryno
                    {
                        AddOn = DateTime.UtcNow,
                        AmountToPay = addToCartDto.Price,
                        Paid = addToCartDto.Paid,
                        RaffleNo = addToCartDto.RaffleId.ToString(),
                        LotteryStatus = (ulong?)addToCartDto.LotteryStatus,
                        UserId = _user.Id,
                        LotteryNo = convertedNumbers,
                        LotteryReferenceId = GenerateUniqueLotteryNumber(),
                    };

                    await _lotteryContext.AddAsync(lotteryNo);
                    await _lotteryContext.SaveChangesAsync();

                    var lotteryToReturn = new TblLotteryAdvancedDto
                    {
                        Id = lotteryNo.Id,
                        AddOn = DateTime.UtcNow,
                        AmountToPay = addToCartDto.Price,
                        Paid = addToCartDto.Paid,
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

        public class DeleteCartDto
        {
            public AuthDto AuthDto { get; set; }
            public int LotteryId { get; set; }
        }

        [HttpPost("DeleteFromCart")]
        public async Task<IActionResult> DeleteFromCart(DeleteCartDto deleteCartDto)
        {

            if (deleteCartDto.AuthDto.Hash == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(deleteCartDto.AuthDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == deleteCartDto.AuthDto.Hash);

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
                    var item = await _lotteryContext.Tbllotterynos.FirstOrDefaultAsync(x => x.Id == deleteCartDto.LotteryId && x.UserId == _user.Id);

                    if (item == null)
                    {
                        return BadRequest("Item does not exist!");
                    }

                    _lotteryContext.Remove(item);
                    await _lotteryContext.SaveChangesAsync();

                    return Ok();
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

        [HttpPost("DeleteAllFromCart")]
        public async Task<ActionResult> RemoveAllCartItemsById(AuthDto AuthDto)
        {
            if (AuthDto.Hash == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(AuthDto.Hash);

            var _user = await _lotteryContext.Tblregisters
                .FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == AuthDto.Hash);

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
                    var items = await _lotteryContext.Tbllotterynos
                        .Where(x => x.UserId == _user.Id)
                        .ToListAsync();

                    if (items == null)
                    {
                        return BadRequest("Item does not exist!");
                    }

                    _lotteryContext.Tbllotterynos.RemoveRange(items);
                    await _lotteryContext.SaveChangesAsync();

                    var newOrder = items.Select(elements => new Tblorderhistory
                    {
                        TicketNo = elements.LotteryNo,
                        RaffleId = Convert.ToInt32(elements.RaffleNo),
                        UserId = elements.UserId,
                        RaffleUniqueId = _lotteryContext.Tblraffles.FirstOrDefault(x => x.Id == Convert.ToInt32(elements.RaffleNo)).UniqueRaffleId,
                        AddOn = DateTime.UtcNow,
                        LotteryReferenceId = elements.LotteryReferenceId,
                    }).ToList();

                    _lotteryContext.Tblorderhistories.AddRange(newOrder);
                    await _lotteryContext.SaveChangesAsync();

                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest("Error occurred while deleting cart items! " + ex.Message);
                }
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

    }
}
