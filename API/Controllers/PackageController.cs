using API.Helpers;
using API.Models;
using API.Repos.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class PackageController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;
        private readonly Generators _generators;

        public PackageController(LotteryContext lotteryContext, Generators generators)
        {
            _lotteryContext = lotteryContext;
            _generators = generators;
        }

        [HttpPost]
        public async Task<IActionResult> GetAllPackages(AuthDto authDto)
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
                    var existingPackges = await _lotteryContext.Tblpackages.ToListAsync();

                    if (existingPackges == null)
                    {
                        return null;
                    }

                    return Ok(existingPackges);
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

        [HttpPost("AddNewPackage")]
        public async Task<IActionResult> AddNewPackage(CreateNewPackageDto createNewPackageDto)
        {
            if (createNewPackageDto.AuthDto.Hash == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(createNewPackageDto.AuthDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == createNewPackageDto.AuthDto.Hash);

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
                    var existingPackage = await _lotteryContext.Tblpackages.Where(x => x.PackageName == createNewPackageDto.PackageName).FirstOrDefaultAsync();
                    
                    if (existingPackage != null)
                    {
                        return BadRequest("Package with this name already exists");
                    }

                    if (createNewPackageDto.PackagePrice < 75)
                    {
                        return BadRequest("Package price cannot be lower than 75");
                    }

                    var newPackage = new Tblpackage
                    {
                        PackageName = createNewPackageDto.PackageName,
                        PackagePrice = createNewPackageDto.PackagePrice,
                        PackgeUniqueId = _generators.GenerateForPackageRandomString(6),
                        AddOn = IndianTimeHelper.GetIndianLocalTime(),
                    };

                    await _lotteryContext.Tblpackages.AddAsync(newPackage);
                    await _lotteryContext.SaveChangesAsync();

                    return Ok(newPackage);
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
        
        [HttpPost("BuyNewPackage")]
        public async Task<IActionResult> BuyNewPackage(PurchaseNewPackageDto newPackageDto)
        {
            if (newPackageDto.authDto.Hash == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(newPackageDto.authDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == newPackageDto.authDto.Hash);

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
                    var existingPackage = await _lotteryContext.Tblpackages.FirstOrDefaultAsync(x => x.PackgeUniqueId == newPackageDto.PackageUniqueId);
                    if (existingPackage == null)
                    {
                        return BadRequest("Package with this id does not exist");
                    }

                    // TODO AFTER CART ADDING
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
    }
}
