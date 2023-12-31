﻿using API.Helpers;
using API.Repos.Dtos;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class GeneratorController : BaseApiController
    {
        private readonly LotteryContext _lotteryContext;
        private readonly Generators _generators;

        public GeneratorController(LotteryContext lotteryContext, Generators generators)
        {
            _lotteryContext = lotteryContext;
            _generators = generators;
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
                    // Fetch existing raffle numbers with ID 2
                    var existingRaffleNumbers = await _lotteryContext.Tblraffles
                        .Where(x => x.Id == 2)
                        .Select(x => x.TicketNo) // This might be the cause of the error
                        .ToListAsync();

                    var count = 0;
                    var random = new Random();
                    var randomNumbers = new List<int>();

                    do
                    {
                        int value;

                        // Generate a random number
                        do
                        {
                            value = random.Next(0, 31);

                            // Introduce a 1% chance of generating a number that matches an existing number
                            if (random.Next(100) < 1 && existingRaffleNumbers.Contains(value.ToString()))
                            {
                                value = random.Next(0, 31);
                            }
                        } while (randomNumbers.Contains(value));

                        // Add the unique random number to the list
                        randomNumbers.Add(value);
                        count++;

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


        [NonAction]
        public string GenerateUniqueRaffleNumber()
        {
            string raffleNumber;
            do
            {
                raffleNumber = _generators.GenerateRandomNumericStringForRaffle(8);
            } while (!_generators.IsUniqueRaffleNoAndId(raffleNumber));

            return raffleNumber;
        }

        [NonAction]
        public string GenerateUniqueRaffleNumberMegaDraw()
        {
            string raffleNumber;
            do
            {
                raffleNumber = _generators.GenerateRandomNumericStringForRaffle(12);
            } while (!_generators.IsUniqueRaffleNoAndId(raffleNumber));

            return raffleNumber;
        }

        public class VerifyBasedOnOrderDto
        {
            public string UniqueRaffleId { get; set; }
        }

        public class GetRaffleBasedDto
        {
            public int UserId { get; set; }
            public string Email { get; set; }
            public string WonTicketNo { get; set; }
            public string Matches { get; set; }
            public string RaffleId { get; set; }
            public string UniqueRaffleId { get; set; }
        }

        [HttpGet("servertime")]
        public IActionResult GetServerTime()
        {
            var currentTime = IndianTimeHelper.GetIndianLocalTime();
            return Ok(currentTime);
        }

        [HttpPost("VerifyBasedOnOrder")]
        public async Task<ActionResult<GetRaffleBasedDto>> VerifyBasedOnOrders([FromQuery] string raffleName)
        {
            try
            {
                var existingTicketNo = await _lotteryContext.Tblraffles.FirstOrDefaultAsync(x => x.RaffleName == raffleName);

                if (existingTicketNo == null)
                {
                    return BadRequest("Invalid Raffle Id");
                }

                var orderHistories = await _lotteryContext.Tblorderhistories
                    .Where(x => x.RaffleUniqueId == existingTicketNo.UniqueRaffleId && x.TicketNo != null)
                    .ToListAsync();

                if (orderHistories.Count == 0)
                {
                    return BadRequest("No orders found for this raffle.");
                }

                string existingTicketNoString = existingTicketNo.TicketNo.ToString();
                List<int> resultList = new List<int>();

                // Convert existingTicketNoString to a list of integers
                for (int i = 0; i < existingTicketNoString.Length; i += 2)
                {
                    if (i + 1 < existingTicketNoString.Length)
                    {
                        string substring = existingTicketNoString.Substring(i, 2);
                        int value = int.Parse(substring);
                        resultList.Add(value);
                    }
                }

                List<int> matchingIndexes = new List<int>();

                foreach (var orderHistory in orderHistories)
                {
                    // Convert the order history ticket number to a list of integers
                    List<int> orderHistoryList = new List<int>();
                    for (int i = 0; i < orderHistory.TicketNo.Length; i += 2)
                    {
                        if (i + 1 < orderHistory.TicketNo.Length)
                        {
                            string substring = orderHistory.TicketNo.Substring(i, 2);
                            int value = int.Parse(substring);
                            orderHistoryList.Add(value);
                        }
                    }

                    // Compare each digit and count the matches
                    int matchCount = 0;
                    for (int i = 0; i < resultList.Count && i < orderHistoryList.Count; i++)
                    {
                        if (resultList[i] == orderHistoryList[i])
                        {
                            matchCount++;
                        }
                    }
                    matchingIndexes.Add(matchCount);
                }

                // + 1 to the existing draw count
                existingTicketNo.DrawCount = existingTicketNo.DrawCount + 1;

                var newDraw = new Tbldrawhistory
                {
                    DrawDate = IndianTimeHelper.GetIndianLocalTime(),
                    LotteryId = (int)existingTicketNo.Id,
                    Sequence = existingTicketNo.TicketNo,
                    UniqueLotteryId = existingTicketNo.UniqueRaffleId
                };

                // saving to the draw history
                await _lotteryContext.Tbldrawhistories.AddAsync(newDraw);
                await _lotteryContext.SaveChangesAsync();

                // saving the old ticket number
                var oldTicketNo = existingTicketNo.TicketNo;

                var winner = new Tbllotterywinner();

                // getting the winner index numbers
                for (int i = 0; i < matchingIndexes.Count; i++)
                {
                    if (matchingIndexes[i] > 0)
                    {
                        var newWin = new Tbllotterywinner
                        {
                            AddOn = IndianTimeHelper.GetIndianLocalTime(),
                            DrawDate = newDraw.DrawDate,
                            RaffleUniqueId = existingTicketNo.UniqueRaffleId,
                            TicketNo = orderHistories[i].TicketNo,
                            UserId = orderHistories[i].UserId,
                            Matches = matchingIndexes[i],
                            RaffleId = (int)existingTicketNo.Id
                        };

                        await _lotteryContext.Tbllotterywinners.AddAsync(newWin);

                        var uniqueLottery = await _lotteryContext.Tblraffles.FirstOrDefaultAsync(x => x.UniqueRaffleId == existingTicketNo.UniqueRaffleId);
                        var userById = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == orderHistories[i].UserId);

                        // Adjust account balance based on matching index
                        switch (matchingIndexes[i])
                        {
                            case 1:
                                userById.AccountBalance += 1000;
                                break;
                            case 2:
                                userById.AccountBalance += 10000;
                                break;
                            case 3:
                                userById.AccountBalance += 100000;
                                break;
                            case 4:
                                userById.AccountBalance += 1000000;
                                break;
                            case 5:
                                userById.AccountBalance += 10000000;
                                break;
                            case 6:
                                userById.AccountBalance += 100000000;
                                break;
                            default:
                                break;
                        }

                        await _lotteryContext.SaveChangesAsync();
                    }
                }

                if (existingTicketNo.RaffleName == "MegaDraw")
                {
                    existingTicketNo.RaffleDate = IndianTimeHelper.GetIndianLocalTime().AddHours(1);
                    existingTicketNo.EndOn = existingTicketNo.RaffleDate?.AddMinutes(5);
                    existingTicketNo.TicketNo = GenerateUniqueRaffleNumberMegaDraw(); // adding the new ticket no for the raffle no
                    existingTicketNo.UniqueRaffleId = _generators.GenerateRandomNumericStringForRaffle(6);
                }
                else if (existingTicketNo.RaffleName == "EasyDraw")
                {
                    existingTicketNo.RaffleDate = IndianTimeHelper.GetIndianLocalTime().AddMinutes(30);
                    existingTicketNo.EndOn = existingTicketNo.RaffleDate?.AddMinutes(5);
                    existingTicketNo.TicketNo = GenerateUniqueRaffleNumber(); // adding the new ticket no for the raffle no
                    existingTicketNo.UniqueRaffleId = _generators.GenerateRandomNumericStringForRaffle(6); //  generating a unique raffle id for the raffle
                }

                await _lotteryContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Error occurred while checking and saving winners! " + ex.Message);
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

                    existingTicketNo.DrawCount = existingTicketNo.DrawCount + 1;

                    var newDraw = new Tbldrawhistory
                    {
                        DrawDate = IndianTimeHelper.GetIndianLocalTime(),
                        LotteryId = verifyEasyDrawGenDto.RaffleId,
                        Sequence = String.Join("",verifyEasyDrawGenDto.TicketNos.ToArray()),
                    };

                    await _lotteryContext.Tbldrawhistories.AddAsync(newDraw);
                    await _lotteryContext.SaveChangesAsync();

                    var oldTicketNo = existingTicketNo.TicketNo;

                    var newWin = new Tbllotterywinner
                    {
                        AddOn = IndianTimeHelper.GetIndianLocalTime(),
                        DrawDate = newDraw.DrawDate,
                        RaffleUniqueId = existingTicketNo.UniqueRaffleId,
                        TicketNo = oldTicketNo,
                        UserId = _user.Id
                    };

                    existingTicketNo.TicketNo = GenerateUniqueRaffleNumber();
                    await _lotteryContext.SaveChangesAsync();

                    if (matchingIndexes == 0)
                    {
                        return BadRequest("No matches found.");
                    }
                    else if (matchingIndexes == 1)
                    {
                        newWin.Matches = matchingIndexes;
                        await _lotteryContext.Tbllotterywinners.AddAsync(newWin);
                        await _lotteryContext.SaveChangesAsync();
                        return Ok("1 won");
                    }
                    else if (matchingIndexes == 2)
                    {
                        newWin.Matches = matchingIndexes;
                        await _lotteryContext.Tbllotterywinners.AddAsync(newWin);
                        await _lotteryContext.SaveChangesAsync();
                        return Ok("2 won");
                    }
                    else if (matchingIndexes == 3)
                    {
                        newWin.Matches = matchingIndexes;
                        await _lotteryContext.Tbllotterywinners.AddAsync(newWin);
                        await _lotteryContext.SaveChangesAsync();
                        return Ok("3 won");
                    }
                    else if (matchingIndexes == 4)
                    {
                        newWin.Matches = matchingIndexes;
                        await _lotteryContext.Tbllotterywinners.AddAsync(newWin);
                        await _lotteryContext.SaveChangesAsync();
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
        
        [HttpGet("MegaDrawValues")]
        public async Task<ActionResult> MegaDrawGet(int until)
        {
            var arr = Enumerable.Range(1, until);
            return Ok(new { arr });
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

                    existingTicketNo.DrawCount = existingTicketNo.DrawCount + 1;

                    var newDraw = new Tbldrawhistory
                    {
                        DrawDate = IndianTimeHelper.GetIndianLocalTime(),
                        LotteryId = megaDrawGenDto.RaffleId,
                        Sequence = String.Join("", megaDrawGenDto.TicketNos.ToArray()),
                    };

                    await _lotteryContext.Tbldrawhistories.AddAsync(newDraw);
                    await _lotteryContext.SaveChangesAsync();

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
