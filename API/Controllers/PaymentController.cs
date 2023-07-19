using API.Helpers;
using API.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;

namespace API.Controllers
{
    public class PaymentController : BaseApiController
    {
        private readonly StripeSettings _stripeSettings;

        public PaymentController(IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings.Value;
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
        public record PaymentIntentRequest(int Amount);
    }
}
