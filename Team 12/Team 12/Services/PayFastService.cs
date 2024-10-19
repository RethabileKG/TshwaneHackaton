
using Team_12.DBContext;
using Team_12.Models;

namespace Team_12.Services
{
    public class PayFastService
    {
        private readonly IConfiguration _configuration;
        private readonly Team12DbContext _context;  // Inject the DbContext

        public PayFastService(IConfiguration configuration, Team12DbContext context)  // Add DbContext to constructor
        {
            _configuration = configuration;
            _context = context;
        }

        public string GeneratePaymentUrl(decimal amount, string bookingReference)
        {
            var payFastUrl = _configuration["PayFast:Url"];
            var merchantId = _configuration["PayFast:MerchantId"];
            var merchantKey = _configuration["PayFast:MerchantKey"];
            var returnUrl = _configuration["PayFast:ReturnUrl"];
            var notifyUrl = _configuration["PayFast:NotifyUrl"];
            var cancelUrl = _configuration["PayFast:CancelUrl"];

            // Construct the payment URL
            var paymentUrl = $"{payFastUrl}?merchant_id={merchantId}&merchant_key={merchantKey}&amount={amount}&item_name=Booking&return_url={returnUrl}&notify_url={notifyUrl}&cancel_url={cancelUrl}&m_payment_id={bookingReference}";

            return paymentUrl;
        }

        public void HandlePaymentNotification(PaymentNotification notification)
        {
            // Process the notification from PayFast (based on PayFast’s IPN)
            if (notification.PaymentStatus == "COMPLETE")
            {
                // Find the booking by the payment reference
                var booking = _context.Bookings.FirstOrDefault(b => b.BookingId.ToString() == notification.PaymentReference);
                if (booking != null)
                {
                    booking.Status = "Paid";  // Update status to "Paid"
                    _context.SaveChanges();
                }
            }
        }
    }
}
