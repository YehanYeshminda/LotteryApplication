//using System;
//using System.Threading;
//using API.Controllers;
//using API.Models;
//using API.Repos.Dtos;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using static API.Controllers.GeneratorController;

//public class CronJob
//{
//    private readonly GeneratorController _generatorController;
//    private Timer timer;

//    public CronJob(GeneratorController generatorController)
//    {
//        _generatorController = generatorController;

//        // The interval in milliseconds (5 seconds = 5 * 1000 ms)
//        int intervalMilliseconds = 5 * 1000;

//        // Create a Timer that will execute ExecuteJobForEasyDraw every intervalMilliseconds
//        timer = new Timer(ExecuteJobForEasyDraw, null, 0, intervalMilliseconds);
//    }

//    public async void ExecuteJobForEasyDraw(object state)
//    {
//        using (var scope = _generatorController.HttpContext.RequestServices.CreateScope())
//        {
//            var lotteryContext = scope.ServiceProvider.GetRequiredService<LotteryContext>();

//            // Fetch existingLottery inside the scope to avoid ObjectDisposedException
//            var existingLottery = await lotteryContext.Tblraffles.FirstOrDefaultAsync(x => x.Id == 2);

//            // VerifyBasedOnOrders will also be executed within the same scope
//            await _generatorController.VerifyBasedOnOrders(new VerifyBasedOnOrderDto
//            {
//                UniqueRaffleId = existingLottery.UniqueRaffleId
//            });
//        }
//    }
//}
