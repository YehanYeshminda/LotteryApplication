using API.Helpers;
using API.Models;
using API.Repos.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static API.Repos.Dtos.CompanyDto;

namespace API.Controllers
{
    public class CompanyController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;

        public CompanyController(LotteryContext lotteryContext)
        {
            _lotteryContext = lotteryContext;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewCompany(AddNewCompanyDto companyDto)
        {
            if (companyDto.AuthDto.Hash == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(companyDto.AuthDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == companyDto.AuthDto.Hash);

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
                    var existingCompany = await _lotteryContext.Tblcompanies.FirstOrDefaultAsync(x => x.CompanyName == companyDto.CompanyName || x.CompanyCode == companyDto.CompanyCode);
                    
                    if (existingCompany != null)
                    {
                        return BadRequest("Company with this and name or code already exists");
                    }

                    var newCompany = new Tblcompany
                    {
                        CompanyCode = companyDto.CompanyCode,
                        CompanyName = companyDto.CompanyName,
                    };

                    await _lotteryContext.Tblcompanies.AddAsync(newCompany);
                    await _lotteryContext.SaveChangesAsync();

                    return Ok(newCompany);
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

        [HttpGet("GetAllCompany")]
        public async Task<IActionResult> GetAllCompany()
        {
            try
            {
                var existingCompany = await _lotteryContext.Tblcompanies.ToListAsync();

                if (existingCompany == null)
                {
                    return NotFound("No company found");
                }

                return Ok(existingCompany);
            }
            catch (Exception ex)
            {
                return BadRequest("Error occured while getting company! " + ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCompany(UpdateCompanyDto companyDto)
        {
            if (companyDto.AuthDto.Hash == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(companyDto.AuthDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == companyDto.AuthDto.Hash);

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
                    var companyToUpdate = await _lotteryContext.Tblcompanies.FirstOrDefaultAsync(x => x.Id == Convert.ToInt32(companyDto.CompanyId));

                    if (companyToUpdate == null)
                    {
                        return NotFound("Company not found");
                    }

                    companyToUpdate.CompanyCode = companyDto.CompanyCode;
                    companyToUpdate.CompanyName = companyDto.CompanyName;

                    await _lotteryContext.SaveChangesAsync();
                    return Ok(companyToUpdate);
                }
                catch (Exception ex)
                {
                    return BadRequest("Error occurred while updating company! " + ex.Message);
                }
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

        public class GetCompanyCodeDto
        {
            public string CompanyCode { get; set; }
        }

        [HttpGet("CompanyCode")]
        public async Task<IActionResult> GetCompanyCode()
        {
            try
            {
                var existingCompany = await _lotteryContext.Tblcompanies.FirstOrDefaultAsync();

                if (existingCompany == null)
                {
                    return NotFound("No company found");
                }

                var toReturn = new GetCompanyCodeDto
                {
                    CompanyCode = existingCompany.CompanyCode
                };

                return Ok(toReturn);
            }
            catch (Exception ex)
            {
                return BadRequest("Error occured while getting company code! " + ex.Message);
            }
        }
    }
}
