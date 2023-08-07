using API.Helpers;
using API.Models;
using API.Repos.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using static API.Controllers.LottoController;

namespace API.Controllers
{
    public class LottoController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;
        private readonly Generators _generators;

        public LottoController(LotteryContext lotteryContext, Generators generators)
        {
            _lotteryContext = lotteryContext;
            _generators = generators;
        }

        public class GetLottoNo
        {
            public string LottoNo { get; set; }
        }

        [HttpPost("GetLottoNumbers")]
        public async Task<ActionResult<GetLottoNo>> GetLottoNumbers(AuthDto authDto)
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
                    var company = await _lotteryContext.Tblcompanies.FirstOrDefaultAsync();

                    if (company == null)
                    {
                        return BadRequest("Unable to find company with this name");
                    }

                    Random random = new Random();
                    int randomNumber = random.Next(1, 9);

                    string captcha = $"{company.CompanyCode}-0{randomNumber}";

                    return Ok(new GetLottoNo
                    {
                        LottoNo = captcha
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest("Error occurred while getting lotto numbers! " + ex.Message);
                }
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

        public class BuyLottoDto
        {
            public string LottoNumber { get; set; }
            public AuthDto AuthDto { get; set; }
        }

        [NonAction]
        public string GenerateUniqueLottoNumber()
        {
            string raffleNumber;
            do
            {
                raffleNumber = _generators.GenerateRandomNumericStringForLotto(8);
            } while (!_generators.IsUniqueRaffleNoAndId(raffleNumber));

            return raffleNumber;
        }

        [HttpPost("BuyLotto")]
        public async Task<IActionResult> BuyLottoNumbers(BuyLottoDto lottoDto)
        {
            if (lottoDto.AuthDto == null)
            {
                return BadRequest("Invalid data!");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(lottoDto.AuthDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == lottoDto.AuthDto.Hash);

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
                    var newLotto = new Tbllotto
                    {
                        LottoNumbers = lottoDto.LottoNumber,
                        ReferenceId = GenerateUniqueLottoNumber(),
                        UserId = _user.Id,
                        AddOn = IndianTimeHelper.GetIndianLocalTime(),
                    };

                    await _lotteryContext.Tbllottos.AddAsync(newLotto);
                    await _lotteryContext.SaveChangesAsync();
                    return Ok(newLotto);
                }
                catch (Exception ex)
                {
                    return BadRequest("Error occurred while buying lottos! " + ex.Message);
                }
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }

        public class CheckForLottoNoDependingOnDates
        {
            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
        }

        public class LottoNumberCount
        {
            public int No { get; set; }
            public int Count { get; set; }
        }

        [HttpPost("CheckForNumberNoti")]
        public async Task<IActionResult> GetNoticationResultForNumbers(CheckForLottoNoDependingOnDates checkForLottoNoDto)
        {
            try
            {
                var existingNumbers = await _lotteryContext.Tbllottos
                    .Where(x => x.AddOn >= checkForLottoNoDto.DateFrom && x.AddOn <= checkForLottoNoDto.DateTo)
                    .ToListAsync();

                var existingCompany = await _lotteryContext.Tblcompanies.FirstOrDefaultAsync();
                var numberCounts = new Dictionary<int, int>();

                foreach (var lotto in existingNumbers)
                {
                    if (lotto.LottoNumbers.StartsWith(existingCompany.CompanyCode + "-") && int.TryParse(lotto.LottoNumbers.Substring(3), out int number))
                    {
                        if (numberCounts.ContainsKey(number))
                        {
                            numberCounts[number]++;
                        }
                        else
                        {
                            numberCounts[number] = 1;
                        }
                    }
                }

                var response = numberCounts.Select(kv => new LottoNumberCount { No = kv.Key, Count = kv.Value }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest("Error occurred while buying lottos! " + ex.Message);
            }
        }

    }
}
