using API.Helpers;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quartz;
using static API.Repos.Dtos.LottoDto;

namespace API.Repos.Services
{
    public class VerifyLottoService : IJob
    {
        private readonly IServiceProvider _serviceProvider;

        public VerifyLottoService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task Execute(IJobExecutionContext context)
        {
            context.NextFireTimeUtc.ToString();

            try
            {
                GetNoticationResultForNumbers();
            }
            catch (Exception ex)
            {
                throw;
            }

            return Task.CompletedTask;
        }

        public async Task<string> GetNoticationResultForNumbers()
        {
            try
            {
                Console.WriteLine("Lotto ran");
                using (var scope = _serviceProvider.CreateScope())
                {
                    var lotteryContext = scope.ServiceProvider.GetRequiredService<LotteryContext>();
                    var generators = scope.ServiceProvider.GetRequiredService<Generators>();

                    TimeZoneInfo indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                    DateTime indianTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indianTimeZone);
                    DateTime dateFrom = indianTimeNow.AddMinutes(-15); // 15 minutes ago
                    DateTime dateTo = indianTimeNow; // Current time

                    var existingNumbers = await lotteryContext.Tbllottoorderhistories
                    .Where(x => x.AddOn >= dateFrom && x.AddOn <= dateTo)
                    .ToListAsync();

                    var existingCompany = await lotteryContext.Tblcompanies.FirstOrDefaultAsync();
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


                    var existingLotto = await lotteryContext.Tbllottos.FirstOrDefaultAsync(x => x.Id == 1);

                    if (existingLotto == null)
                    {
                        return "Unable to find lotto!";
                    }

                    var winningUsers = await lotteryContext.Tbllottoorderhistories
                        .Where(x => x.UserId.HasValue && x.LottoNumbers.StartsWith(existingCompany.CompanyCode + "-" + "0" + keyWithLowestValue) && x.AddOn >= dateFrom && x.AddOn <= dateTo)
                        .GroupBy(x => x.UserId)  // Group by user to count their entries
                        .Select(g => new { UserId = g.Key.Value, EntryCount = g.Count() })
                        .ToListAsync();

                    // Update user balances based on the number of entries they have
                    foreach (var userEntry in winningUsers)
                    {
                        var getExistingUser = await lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == userEntry.UserId);

                        if (getExistingUser == null)
                        {
                            return "Error while getting user";
                        }

                        // Add 100 times the number of entries to the user's balance
                        getExistingUser.AccountBalance += userEntry.EntryCount * 100;
                    }

                    existingLotto.WinnerNo = keyWithLowestValue.ToString();
                    await lotteryContext.SaveChangesAsync();

                    return "";
                }
            }
            catch (Exception ex)
            {
                return "Error occurred while buying lottos! " + ex.Message;
            }
        }
    }
}
