using API.Helpers;
using API.Models;
using API.Repos.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static API.Repos.Dtos.DrawDto;
using static API.Repos.Dtos.LottoDto;

namespace API.Controllers
{
    public class LottoController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;
        private readonly Generators _generators;
        private ResponseDto _response;
        public LottoController(LotteryContext lotteryContext, Generators generators)
        {
            _lotteryContext = lotteryContext;
            _generators = generators;
            _response = new ResponseDto();
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
        public async Task<ResponseDto> BuyLottoNumbers(BuyLottoDto lottoDto)
        {
            if (lottoDto.AuthDto == null)
            {
                _response.Message = "Invalid data!";
                _response.IsSuccess = false;
                return _response;
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(lottoDto.AuthDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == lottoDto.AuthDto.Hash);

            if (_user == null)
            {
                _response.Message = "Invalid Authentication Details";
                _response.IsSuccess = false;
                return _response;
            }

            var decryptedDateWithOffset = decodedValues.Date.AddDays(1);
            var currentDate = DateTime.UtcNow.Date;

            if (currentDate < decryptedDateWithOffset.Date)
            {
                try
                {
                    var existingCompany = await _lotteryContext.Tblcompanies.FirstOrDefaultAsync();
                    var existingLotto = await _lotteryContext.Tbllottos.FirstOrDefaultAsync(x => x.LottoCompanyId == existingCompany.Id.ToString());

                    if (existingCompany == null)
                    {
                        _response.Message = "Unable to find company";
                        _response.IsSuccess = false;
                        return _response;
                    }

                    if (_user.AccountBalance < existingLotto.LottoPrice)
                    {
                        _response.Message = "Insufficient Account Balance!";
                        _response.IsSuccess = false;
                        return _response;
                    }

                    var newLotto = new Tbllottoorderhistory
                    {
                        LottoNumbers = lottoDto.LottoNumber,
                        ReferenceId = existingLotto.LottoUniqueId,
                        UserId = _user.Id,
                        AddOn = IndianTimeHelper.GetIndianLocalTime(),
                        LottoUnqueReferenceId = existingLotto.LottoUniqueId,
                        Price = existingLotto.LottoPrice.ToString()
                    };

                    _user.AccountBalance -= existingLotto.LottoPrice;

                    await _lotteryContext.Tbllottoorderhistories.AddAsync(newLotto);
                    await _lotteryContext.SaveChangesAsync();

                    _response.Message = "Lotto bought successfully!";
                    _response.IsSuccess = true;

                    var newLottoToReturn = new LottoToReturn
                    {
                        LottoNumbers = newLotto.LottoNumbers,
                        LottoUnqueReferenceId = newLotto.LottoUnqueReferenceId,
                        ReferenceId = newLotto.ReferenceId,
                    };

                    _response.Result = newLottoToReturn;
                    return _response;
                }
                catch (Exception ex)
                {
                    _response.Message = "Error occurred while buying lotto! " + ex.Message;
                    _response.IsSuccess = false;
                    return _response;
                }
            }
            else
            {
                _response.Message = "Invalid Authentication Details";
                _response.IsSuccess = false;
                return _response;
            }
        }

        [HttpPost("CheckForNumberNoti")]
        public async Task<IActionResult> GetNoticationResultForNumbers(CheckForLottoNoDependingOnDates checkForLottoNoDto)
        {
            try
            {
                var existingNumbers = await _lotteryContext.Tbllottoorderhistories
                    .Where(x => x.AddOn >= checkForLottoNoDto.DateFrom && x.AddOn <= checkForLottoNoDto.DateTo)
                    .ToListAsync();

                var existingCompany = await _lotteryContext.Tblcompanies.FirstOrDefaultAsync();
                var numberCounts = new Dictionary<int, int>();

                foreach (var lotto in existingNumbers)
                {
                    if (lotto.LottoNumbers.StartsWith(existingCompany.CompanyCode + "-"))
                    {
                        string numberSubstring = lotto.LottoNumbers.Substring(existingCompany.CompanyCode.Length + 1);

                        if (int.TryParse(numberSubstring, out int number))
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
                }


                var response = numberCounts.Select(kv => new LottoNumberCount { No = kv.Key, Count = kv.Value }).ToList();

                int keyWithLowestValue = numberCounts.OrderBy(kv => kv.Value).FirstOrDefault().Key;
                int lowestValue = numberCounts[keyWithLowestValue];


                var existingLotto = await _lotteryContext.Tbllottos.FirstOrDefaultAsync(x => x.Id == 1);

                if (existingLotto == null)
                {
                    return BadRequest("Unable to find lotto!");
                }

                existingLotto.WinnerNo = keyWithLowestValue.ToString();
                await _lotteryContext.SaveChangesAsync();

                return Ok(keyWithLowestValue.ToString());
            }
            catch (Exception ex)
            {
                return BadRequest("Error occurred while buying lottos! " + ex.Message);
            }
        }

        public class LottoHistoryToReturn
        {
            public DateTime? AddOn { get; set; }
            public string LottoNumbers { get; set; }
            public string LottoUnqueReferenceId { get; set; }
            public string Price { get; set; }
        }


        [HttpGet("GetAllLottoHistory")]
        public async Task<IActionResult> GetAllLottoHistory()
        {
            try
            {
                DateTime fifteenMinutesAgo = IndianTimeHelper.GetIndianLocalTime().AddMinutes(-15);

                var existingLottoHistory = await _lotteryContext.Tbllottoorderhistories
                    .Where(x => x.AddOn >= fifteenMinutesAgo)
                    .ToListAsync();

                if (existingLottoHistory == null)
                {
                    return BadRequest("No lotto history found!");
                }

                var response = existingLottoHistory.Select(x => new LottoHistoryToReturn
                {
                    AddOn = x.AddOn,
                    LottoNumbers = x.LottoNumbers,
                    LottoUnqueReferenceId = x.LottoUnqueReferenceId,
                    Price = x.Price,
                }).OrderByDescending(x => x.AddOn).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest("Error occurred while fetching lotto history! " + ex.Message);
            }
        }


        [HttpPost("AddNewLotto")]
        public async Task<IActionResult> AddNewLotto(AddNewLottoDto addNewLottoDto)
        {
            if (addNewLottoDto.AuthDto == null)
            {
                return BadRequest("Invalid data!");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(addNewLottoDto.AuthDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == addNewLottoDto.AuthDto.Hash);

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
                    var existingCompany = await _lotteryContext.Tblcompanies.FirstOrDefaultAsync(x => x.Id == addNewLottoDto.LottoCompanyId);

                    if (existingCompany == null)
                    {
                        return BadRequest("Unable to find company with the Id");
                    }

                    var existingLottoForCompanyCode = await _lotteryContext.Tbllottos.FirstOrDefaultAsync(x => Convert.ToInt32(x.LottoCompanyId) == addNewLottoDto.LottoCompanyId);

                    if (existingLottoForCompanyCode != null)
                    {
                        return BadRequest("Lottery for this company already exists!");
                    }

                    var newLottoType = new Tbllotto
                    {
                        LottoCompanyId = existingCompany.Id.ToString(),
                        LottoName = addNewLottoDto.LottoName ?? "",
                        LottoPrice = addNewLottoDto.LottoPrice,
                        LottoUniqueId = GenerateUniqueLottoNumber(),
                    };

                    await _lotteryContext.Tbllottos.AddAsync(newLottoType);
                    await _lotteryContext.SaveChangesAsync();

                    return Ok(newLottoType);
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

        [HttpPost("GetLottoTransactionHistory")]
        public async Task<IActionResult> GetLottoTrasacntionHistory(AuthDto authDto)
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
                    var newHistoryToReturn = new List<LottoHistoryToReturn>();
                    var existingLottosForUser = await _lotteryContext.Tbllottoorderhistories.Where(x => x.UserId == _user.Id).ToListAsync();

                    if (existingLottosForUser == null)
                    {
                        return BadRequest("Unable to lottos for this user");
                    }

                    foreach (var item in existingLottosForUser)
                    {
                        var newItem = new LottoHistoryToReturn
                        {
                            AddOn = item.AddOn,
                            LottoNumbers = item.LottoNumbers,
                            LottoUnqueReferenceId = item.LottoUnqueReferenceId,
                            Price = item.Price
                        };

                        newHistoryToReturn.Add(newItem);
                    }
                    return Ok(newHistoryToReturn);

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
    }
}
