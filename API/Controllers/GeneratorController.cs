using API.Helpers;
using API.Repos;
using API.Repos.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class GeneratorController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;

        public GeneratorController(LotteryContext lotteryContext)
        {
            _lotteryContext = lotteryContext;
        }

        [HttpPost("EasyDraw")]
        public async Task<ActionResult<GetRandomNoApiResponse>> GetEasyDrawGen(AuthDto authDto)
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
                    var count = 0;
                    var random = new Random();
                    var randomNumbers = new List<int>();
                    int value;

                    do
                    {
                        value = random.Next(0, 31);
                        var existingRandomNo = await _lotteryContext.Tbllotterynos.SingleOrDefaultAsync(x => x.LotteryNo == value.ToString());

                        if (existingRandomNo == null)
                        {
                            if (randomNumbers.Count() != 4)
                            {
                                count++;
                                randomNumbers.Add(value);
                            }
                            else
                            {
                                break;
                            }
                        }


                    } while (count != 4);

                    return Ok(new GetRandomNoApiResponse { Result = randomNumbers });
                }
                catch (Exception ex)
                {
                    return BadRequest("Error occurred while generating random number! " + ex.Message);
                }

            }
            else
            {
                  return Unauthorized("Invalid Authentication Details");
            }
        }

        [HttpPost("VerifyEasyDraw")]
        public async Task<ActionResult<VerifyEasyDrawResponseDto>> VerifyEasyDrawGen(VerifyEasyDrawGenDto verifyEasyDrawGenDto)
        {
            if (verifyEasyDrawGenDto.authDto.Hash == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(verifyEasyDrawGenDto.authDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == verifyEasyDrawGenDto.authDto.Hash);

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
                    var existingTicketNo = await _lotteryContext.Tblraffles.FirstOrDefaultAsync(x => x.Id == verifyEasyDrawGenDto.RaffleId);

                    if (existingTicketNo == null)
                    {
                        return BadRequest("Invalid Raffle Id");
                    }

                    List<int> resultList = new List<int>();

                    for (int i = 0; i < existingTicketNo.TicketNo.ToString().Length; i += 2)
                    {
                        string substring = existingTicketNo.TicketNo.ToString().Substring(i, 2);
                        int value = int.Parse(substring);
                        resultList.Add(value);
                    }

                    List<int> frontEndList = verifyEasyDrawGenDto.TicketNos;

                    int matchingIndexes = 0;
                    for (int i = 0; i < resultList.Count; i++)
                    {
                        if (resultList[i] == frontEndList[i])
                        {
                            matchingIndexes++;
                        }
                    }

                    if (matchingIndexes == 0)
                    {
                        return BadRequest("No matches found.");
                    }
                    else if (matchingIndexes == 1)
                    {
                        return Ok("1 won");
                    }
                    else if (matchingIndexes == 2)
                    {
                        return Ok("2 won");
                    }
                    else if (matchingIndexes == 3)
                    {
                        return Ok("3 won");
                    }
                    else if (matchingIndexes == 4)
                    {
                        return Ok("Jackpot won");
                    }
                    else
                    {
                        return BadRequest("Invalid number of matches.");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest("Error occurred while generating random number! " + ex.Message);
                }

            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }
        
        [HttpPost("MegaDrawValues")]
        public async Task<ActionResult> MegaDrawGet(int until)
        {
            var arr = Enumerable.Range(1, until);
            return this.Ok(new { arr });
        }

        [HttpPost("VerifyMegaDraw")]
        public async Task<ActionResult<VerifyEasyDrawResponseDto>> VerifyMegaDrawGen(MegaDrawGenDto megaDrawGenDto)
        {
            if (megaDrawGenDto.authDto.Hash == null)
            {
                return Unauthorized("Missing Authentication Details");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(megaDrawGenDto.authDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == megaDrawGenDto.authDto.Hash);

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
                    var existingTicketNo = await _lotteryContext.Tblraffles.FirstOrDefaultAsync(x => x.Id == megaDrawGenDto.RaffleId);

                    if (existingTicketNo == null)
                    {
                        return BadRequest("Invalid Raffle Id");
                    }

                    List<int> resultList = new List<int>();

                    for (int i = 0; i < existingTicketNo.TicketNo.ToString().Length; i += 2)
                    {
                        string substring = existingTicketNo.TicketNo.ToString().Substring(i, 2);
                        int value = int.Parse(substring);
                        resultList.Add(value);
                    }

                    List<int> frontEndList = megaDrawGenDto.TicketNos;

                    int matchingIndexes = 0;
                    for (int i = 0; i < resultList.Count; i++)
                    {
                        if (resultList[i] == frontEndList[i])
                        {
                            matchingIndexes++;
                        }
                    }

                    if (matchingIndexes == 0)
                    {
                        return BadRequest("No matches found.");
                    }
                    else if (matchingIndexes == 1)
                    {
                        return Ok("1 won");
                    }
                    else if (matchingIndexes == 2)
                    {
                        return Ok("2 won");
                    }
                    else if (matchingIndexes == 3)
                    {
                        return Ok("3 won");
                    }
                    else if (matchingIndexes == 4)
                    {
                        return Ok("Jackpot won");
                    }
                    else
                    {
                        return BadRequest("Invalid number of matches.");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest("Error occurred while generating random number! " + ex.Message);
                }

            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }
    }
}
