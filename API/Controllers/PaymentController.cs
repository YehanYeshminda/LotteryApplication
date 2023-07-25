using API.Helpers;
using API.Repos;
using API.Repos.Dtos;
using API.Repos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;

namespace API.Controllers
{
    public class PaymentController : BaseApiController
    {
        private readonly StripeSettings _stripeSettings;
        private readonly LotteryContext _lotteryContext;

        public PaymentController(IOptions<StripeSettings> stripeSettings, LotteryContext lotteryContext)
        {
            _stripeSettings = stripeSettings.Value;
            _lotteryContext = lotteryContext;
        }

        [HttpPost("create-payment-intent")]
        public IActionResult CreatePaymentIntent(PaymentIntentRequest request)
        {
            StripeConfiguration.ApiKey = _stripeSettings.SECRET_KEY;

            var options = new PaymentIntentCreateOptions
            {
                Amount = request.Amount,
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" }
            };

            var service = new PaymentIntentService();
            var paymentIntent = service.Create(options);

            return Ok(paymentIntent);
        }

        public class MakePaymentDto
        {
            public AuthDto AuthDto { get; set; }
            public string RaffleNo { get; set; }
            public int MatchCount { get; set; } = 0;
            public int Credit { get; set; }
            public int Paid { get; set; }
        }

        [HttpPost("Add-Payment")]
        public async Task<IActionResult> AddPayment(MakePaymentDto makePaymentDto)
        {
            if (makePaymentDto.AuthDto == null)
            {
                return BadRequest("Invalid data!");
            }

            HelperAuth decodedValues = PasswordHelpers.DecodeValue(makePaymentDto.AuthDto.Hash);

            var _user = await _lotteryContext.Tblregisters.FirstOrDefaultAsync(x => x.Id == decodedValues.UserId && x.Hash == makePaymentDto.AuthDto.Hash);

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
                    var lotteryByLotteryId = await _lotteryContext.Tbllotterywinners.Where(x => x.RaffleUniqueId == makePaymentDto.RaffleNo).ToListAsync();
                    var newPayment = new Tblmoneycredit
                    {
                        Credit = makePaymentDto.Credit,
                        MoneyCreditDate = DateTime.UtcNow,
                        Paid = makePaymentDto.Paid,
                        RaffleNo = makePaymentDto.RaffleNo,
                        UserId = decodedValues.UserId,
                        WinNo = lotteryByLotteryId.Count().ToString(),
                    };

                    await _lotteryContext.Tblmoneycredits.AddAsync(newPayment);
                    await _lotteryContext.SaveChangesAsync();

                    return Ok(newPayment);
                }
                catch (Exception ex)
                {
                    return BadRequest("Error occurred while getting Draw Histories!" + ex.Message);
                }
            }
            else
            {
                return Unauthorized("Invalid Authentication Details");
            }
        }
        public record PaymentIntentRequest(int Amount);
    }
}
