using API.Helpers;
using API.Repos.Dtos;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

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
            public string RaffleId { get; set; }
            public int UserId { get; set; }
            public int LotteryStatus { get; set; } = 0;
            public string Name { get; set; }
            public decimal Paid { get; set; }
            public decimal Price { get; set; }
            public string Type { get; set; }
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
                    if (addToCartDto.Type == "Draw") 
                    {
                        var existingRaffle = await _lotteryContext.Tblraffles.SingleOrDefaultAsync(x => x.Id == Convert.ToInt32(addToCartDto.RaffleId));

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

                        var selectedRaffle = _lotteryContext.Tblraffles.FirstOrDefault(x => x.UniqueRaffleId == existingRaffle.UniqueRaffleId);

                        var lotteryNo = new Tbllotteryno
                        {
                            LotteryName = selectedRaffle.RaffleName,
                            AddOn = DateTime.UtcNow,
                            AmountToPay = addToCartDto.Price,
                            Paid = selectedRaffle.RafflePrice,
                            RaffleNo = addToCartDto.RaffleId.ToString(),
                            LotteryStatus = addToCartDto.LotteryStatus,
                            UserId = _user.Id,
                            LotteryNo = convertedNumbers,
                            LotteryReferenceId = GenerateUniqueLotteryNumber(),
                        };

                        await _lotteryContext.AddAsync(lotteryNo);
                        await _lotteryContext.SaveChangesAsync();

                        var lotteryToReturn = new TblLotteryAdvancedDto
                        {
                            Id = lotteryNo.Id,
                            AddOn = DateTime.UtcNow.ToLocalTime(),
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
                    } else if (addToCartDto.Type == "Package")
                    {
                        var existingPackage = await _lotteryContext.Tblpackages.FirstOrDefaultAsync(x => x.PackgeUniqueId == addToCartDto.RaffleId);

                        if (existingPackage == null)
                        {
                            return BadRequest("Unable to find package with this Id");
                        }

                        var lotteryNo = new Tbllotteryno
                        {
                            LotteryName = existingPackage.PackageName,
                            AddOn = DateTime.UtcNow,
                            AmountToPay = addToCartDto.Price,
                            Paid = _lotteryContext.Tblpackages.FirstOrDefault(x => x.Id == existingPackage.Id).PackagePrice,
                            RaffleNo = addToCartDto.RaffleId.ToString(),
                            LotteryStatus = addToCartDto.LotteryStatus,
                            UserId = _user.Id,
                            LotteryNo = "0",
                            LotteryReferenceId = GenerateUniqueLotteryNumber(),
                        };

                        await _lotteryContext.Tbllotterynos.AddAsync(lotteryNo);
                        await _lotteryContext.SaveChangesAsync();
                        return Ok(lotteryNo);
                    } else if (addToCartDto.Type == "Lotto")
                    {
                        var existingCompany = await _lotteryContext.Tblcompanies.FirstOrDefaultAsync(x => x.CompanyCode == addToCartDto.RaffleId);

                        if (existingCompany == null)
                        {
                            return BadRequest("Missing Company");
                        }

                        var existingLotto = await _lotteryContext.Tbllottos.FirstOrDefaultAsync(x => Convert.ToInt32(x.LottoCompanyId) == existingCompany.Id);

                        if (existingLotto == null)
                        {
                            return BadRequest("Unable to find package with this Id");
                        }

                        var lotteryNo = new Tbllotteryno
                        {
                            LotteryName = existingLotto.LottoName,
                            AddOn = DateTime.UtcNow,
                            AmountToPay = addToCartDto.Price,
                            Paid = existingLotto.LottoPrice,
                            RaffleNo = existingLotto.LottoUniqueId,
                            LotteryStatus = addToCartDto.LotteryStatus,
                            UserId = _user.Id,
                            LotteryNo = "0",
                            LotteryReferenceId = GenerateUniqueLotteryNumber(),
                        };

                        await _lotteryContext.Tbllotterynos.AddAsync(lotteryNo);
                        await _lotteryContext.SaveChangesAsync();
                        return Ok(lotteryNo);
                    }

                    return Ok();

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
        public async Task<IActionResult> GetCartItems(AuthDto authDto)
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


        [NonAction]
        public string GenerateUniqueTblPackageOrderHistoryNumber()
        {
            string orderNo;
            do
            {
                orderNo = _generators.GenerateRandomStringForTblPackageOrderHistory(13);
            } while (!_generators.IsUniqueOrderForPackage(orderNo));

            return orderNo;
        }

        public class ReturnFromDeleteInCart
        {
            public string PackageId { get; set; }
        }

        [HttpPost("DeleteAllFromCart")]
        public async Task<ActionResult<ReturnFromDeleteInCart>> RemoveAllCartItemsById(AuthDto AuthDto)
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

                    var orderList = new List<Tblorderhistory>();
                    var purchaseList = new List<Tblpackageorderhistory>();
                    var raffleId = new List<string>();
                    var packageId = GenerateUniqueTblPackageOrderHistoryNumber();

                    foreach (var item in items)
                    {
                        if (item.LotteryStatus == 1)
                        {
                            var newPurchaseOrder = new Tblpackageorderhistory
                            {
                                PackageName = item.LotteryName,
                                PackagePrice = item.Paid.ToString() ?? "0",
                                PackageUniqueId = item.RaffleNo,
                                UserId = (uint)_user.Id,
                                AddOn = IndianTimeHelper.GetIndianLocalTime(),
                                PackageOrderUniqueId = packageId,
                            };

                            _lotteryContext.Tblpackageorderhistories.AddRange(newPurchaseOrder);
                            _user.AccountBalance += item.Paid;
                        }
                    }

                    await _lotteryContext.SaveChangesAsync();

                    var newDataToReturn = new ReturnFromDeleteInCart
                    {
                        PackageId = packageId
                    };

                    return Ok(newDataToReturn);
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
