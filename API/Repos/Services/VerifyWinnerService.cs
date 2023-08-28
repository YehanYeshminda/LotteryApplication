using API.Helpers;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace API.Repos.Services
{
    public class VerifyWinnerService : IJob
    {
        private readonly IServiceProvider _serviceProvider;

        public VerifyWinnerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task Execute(IJobExecutionContext context)
        {
            context.NextFireTimeUtc.ToString();

            try
            {
                VerifyBasedOnOrders("EasyDraw");
            }
            catch (Exception ex)
            {
                throw;
            }

            return Task.CompletedTask;
        }

        public async Task<string> VerifyBasedOnOrders(string raffleName)
        {
            try
            {
                Console.WriteLine("Easy ran");
                using (var scope = _serviceProvider.CreateScope())
                {
                    var lotteryContext = scope.ServiceProvider.GetRequiredService<LotteryContext>();
                    var generators = scope.ServiceProvider.GetRequiredService<Generators>();

                    var existingTicketNo = await lotteryContext.Tblraffles.FirstOrDefaultAsync(x => x.RaffleName == raffleName);

                    if (existingTicketNo == null)
                    {
                        return "Invalid Raffle Id";
                    }

                    var orderHistories = await lotteryContext.Tblorderhistories
                        .Where(x => x.RaffleUniqueId == existingTicketNo.UniqueRaffleId && x.TicketNo != null)
                        .ToListAsync();

                    if (orderHistories.Count == 0)
                    {
                        return "No orders found for this raffle.";
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
                    await lotteryContext.Tbldrawhistories.AddAsync(newDraw);
                    await lotteryContext.SaveChangesAsync();

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
                            await lotteryContext.Tbllotterywinners.AddAsync(newWin);

                            var uniqueLottery = await lotteryContext.Tblraffles.FirstOrDefaultAsync(x => x.UniqueRaffleId == existingTicketNo.UniqueRaffleId);
                            var userById = await lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == orderHistories[i].UserId);

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

                            await lotteryContext.SaveChangesAsync();
                        }
                    }

                    if (existingTicketNo.RaffleName == "MegaDraw")
                    {
                        existingTicketNo.RaffleDate = IndianTimeHelper.GetIndianLocalTime().AddHours(1);
                        existingTicketNo.EndOn = existingTicketNo.RaffleDate?.AddMinutes(5);
                        existingTicketNo.TicketNo = GenerateUniqueRaffleNumberMegaDraw(generators); // adding the new ticket no for the raffle no
                        existingTicketNo.UniqueRaffleId = generators.GenerateRandomNumericStringForRaffle(6);
                    }
                    else if (existingTicketNo.RaffleName == "EasyDraw")
                    {
                        existingTicketNo.RaffleDate = IndianTimeHelper.GetIndianLocalTime().AddMinutes(30);
                        existingTicketNo.EndOn = existingTicketNo.RaffleDate?.AddMinutes(5);
                        existingTicketNo.TicketNo = GenerateUniqueRaffleNumber(generators); // adding the new ticket no for the raffle no
                        existingTicketNo.UniqueRaffleId = generators.GenerateRandomNumericStringForRaffle(6); //  generating a unique raffle id for the raffle
                    }

                    await lotteryContext.SaveChangesAsync();

                    return "";
                }
            }
            catch (Exception ex)
            {
                return "Error occurred while checking and saving winners! " + ex.Message;
            }
        }

        public string GenerateUniqueRaffleNumber(Generators generators)
        {
            string raffleNumber;
            do
            {
                raffleNumber = generators.GenerateRandomNumericStringForRaffle(8);
            } while (!generators.IsUniqueRaffleNoAndId(raffleNumber));

            return raffleNumber;
        }

        public string GenerateUniqueRaffleNumberMegaDraw(Generators generators)
        {
            string raffleNumber;
            do
            {
                raffleNumber = generators.GenerateRandomNumericStringForRaffle(12);
            } while (!generators.IsUniqueRaffleNoAndId(raffleNumber));

            return raffleNumber;
        }
    }
}
