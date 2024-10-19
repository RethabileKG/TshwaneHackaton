using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Team_12.Models;
using Team_12.Repositories;
using Team_12.Services;

namespace Team_12.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly DiscountService _discountService;
        private readonly IEmailService _emailService;
        private readonly PayFastService _payFastService;
        private readonly QRVerificationService _qrVerificationService;

        public BookingController(IBookingRepository bookingRepository, DiscountService discountService, IEmailService emailService, PayFastService payFastService, QRVerificationService qrVerificationService)
        {
            _bookingRepository = bookingRepository;
            _discountService = discountService;
            _emailService = emailService;
            _payFastService = payFastService;
            _qrVerificationService = qrVerificationService;
        }

        // Create a new booking
        [HttpPost("book")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            // Check facility availability
            var isAvailable = await _bookingRepository.IsFacilityAvailable(request.FacilityId, request.BookingDate, request.StartTime, request.EndTime);
            if (!isAvailable)
            {
                return BadRequest("Facility is fully booked.");
            }

            // Calculate total cost and apply discounts
            var totalCost = request.TotalCost;
            var discount = _discountService.CalculateDiscount(request.ClientTypes, totalCost);
            var finalPrice = totalCost - discount;

            // Create booking
            var booking = new Booking
            {
                FacilityId = request.FacilityId,
                UserId = request.UserId,
                BookingDate = request.BookingDate,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                TotalCost = totalCost,
                DiscountApplied = discount,
                FinalPrice = finalPrice,
                ClientTypes = request.ClientTypes,
                Status = "Pending"
            };

            var createdBooking = await _bookingRepository.CreateBooking(booking);

            // Generate QR code content using the QR verification service
            var qrContent = _qrVerificationService.GenerateQRContent(createdBooking);

            // Send booking confirmation email with QR code
            await _emailService.SendBookingConfirmationEmail(
                request.UserEmail,
                createdBooking.Facility.Name,
                createdBooking.BookingDate,
                createdBooking.BookingId.ToString(),
                qrContent
            );

            // Generate PayFast payment URL
            var paymentUrl = _payFastService.GeneratePaymentUrl(finalPrice, createdBooking.BookingId.ToString());

            return Ok(new { bookingId = createdBooking.BookingId, paymentUrl });
        }

        // Get all bookings
        [HttpGet("all")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingRepository.GetAllBookings();
            return Ok(bookings);
        }

        // Get booking by ID
        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetBookingById(int bookingId)
        {
            var booking = await _bookingRepository.GetBookingById(bookingId);
            if (booking == null)
            {
                return NotFound("Booking not found.");
            }
            return Ok(booking);
        }

        // Update booking
        [HttpPut("{bookingId}")]
        public async Task<IActionResult> UpdateBooking(int bookingId, [FromBody] BookingRequest request)
        {
            var existingBooking = await _bookingRepository.GetBookingById(bookingId);
            if (existingBooking == null)
            {
                return NotFound("Booking not found.");
            }

            // Update booking details
            existingBooking.FacilityId = request.FacilityId;
            existingBooking.BookingDate = request.BookingDate;
            existingBooking.StartTime = request.StartTime;
            existingBooking.EndTime = request.EndTime;
            existingBooking.TotalCost = request.TotalCost;
            existingBooking.ClientTypes = request.ClientTypes;

            // Recalculate discount and final price
            var discount = _discountService.CalculateDiscount(request.ClientTypes, existingBooking.TotalCost);
            existingBooking.DiscountApplied = discount;
            existingBooking.FinalPrice = existingBooking.TotalCost - discount;

            var updatedBooking = await _bookingRepository.UpdateBooking(existingBooking);

            return Ok(updatedBooking);
        }

        // Delete a booking
        [HttpDelete("{bookingId}")]
        public async Task<IActionResult> DeleteBooking(int bookingId)
        {
            var deleted = await _bookingRepository.DeleteBooking(bookingId);
            if (!deleted)
            {
                return NotFound("Booking not found.");
            }
            return NoContent();
        }

        // Handle payment confirmation from PayFast
        [HttpPost("payment-confirmation")]
        public async Task<IActionResult> HandlePaymentConfirmation([FromForm] PaymentNotification notification)
        {
            // Process payment notification
            _payFastService.HandlePaymentNotification(notification);

            // Update booking status
            var booking = await _bookingRepository.GetBookingById(int.Parse(notification.PaymentReference));
            if (booking != null)
            {
                booking.Status = "Paid";
                await _bookingRepository.UpdateBooking(booking);

                // Send payment confirmation email
                await _emailService.SendPaymentConfirmationEmail(booking.User.Email, booking.BookingId.ToString(), booking.FinalPrice);
            }

            return Ok();
        }

        // Issue free booking based on loyalty points
        [HttpPost("free-booking")]
        public async Task<IActionResult> CreateFreeBooking([FromBody] FreeBookingRequest request)
        {
            var userLoyalty = await _bookingRepository.GetUserLoyalty(request.UserId);
            if (userLoyalty == null || userLoyalty.Points < 100)
            {
                return BadRequest("Insufficient loyalty points.");
            }

            // Get least booked facility
            var leastBookedFacility = await _bookingRepository.GetLeastBookedFacility();
            if (leastBookedFacility == null)
            {
                return BadRequest("No facilities available for free booking.");
            }

            // Create free booking
            var freeBooking = new Booking
            {
                FacilityId = leastBookedFacility.FacilityId,
                UserId = request.UserId,
                BookingDate = DateTime.Now,
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(17),
                TotalCost = 0,
                DiscountApplied = 0,
                FinalPrice = 0,
                IsFreeBooking = true,
                Status = "Free Booking",
                ClientTypes = new List<string>()
            };

            var createdFreeBooking = await _bookingRepository.CreateBooking(freeBooking);

            // Deduct loyalty points
            await _bookingRepository.DeductLoyaltyPoints(request.UserId);

            // Send free booking notification email
            await _emailService.SendFreeBookingNotificationEmail(request.UserEmail, leastBookedFacility.Name);

            return Ok(createdFreeBooking);
        }
    }
}