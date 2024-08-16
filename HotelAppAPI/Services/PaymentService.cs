using Stripe;

namespace HotelAppAPI.Services;

public class PaymentService
{
    public PaymentService()
    {
        StripeConfiguration.ApiKey = "your_stripe_api_key";
    }

    public Charge CreateCharge(string token, decimal amount, string currency)
    {
        var options = new ChargeCreateOptions
        {
            Amount = (long)(amount * 100), // Amount in cents
            Currency = currency,
            Description = "Hotel Booking",
            Source = token,
        };

        var service = new ChargeService();
        return service.Create(options);
    }
}
