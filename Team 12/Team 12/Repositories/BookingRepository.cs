using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Team_12.DBContext;
using Team_12.Models;

namespace Team_12.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly Team12DbContext _context;

        public BookingRepository(Team12DbContext context)
        {
            _context = context;
        }

        // Create a new booking
        public async Task<Booking> CreateBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Add loyalty points for the user
            await AddLoyaltyPoints(booking.UserId);

            return booking;
        }

        // Get all bookings
        public async Task<IEnumerable<Booking>> GetAllBookings()
        {
            return await _context.Bookings
                .Include(b => b.Facility)
                .Include(b => b.User)
                .ToListAsync();
        }

        // Get booking by ID
        public async Task<Booking> GetBookingById(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Facility)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }

        // Update booking
        public async Task<Booking> UpdateBooking(Booking booking)
        {
            var existingBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == booking.BookingId);

            if (existingBooking == null) return null;

            existingBooking.FacilityId = booking.FacilityId;
            existingBooking.BookingDate = booking.BookingDate;
            existingBooking.StartTime = booking.StartTime;
            existingBooking.EndTime = booking.EndTime;
            existingBooking.TotalCost = booking.TotalCost;
            existingBooking.DiscountApplied = booking.DiscountApplied;
            existingBooking.FinalPrice = booking.FinalPrice;
            existingBooking.ClientTypes = booking.ClientTypes;
            existingBooking.IsUsed = booking.IsUsed;
            existingBooking.UsedDateTime = booking.UsedDateTime;

            _context.Bookings.Update(existingBooking);
            await _context.SaveChangesAsync();

            return existingBooking;
        }

        // Delete booking
        public async Task<bool> DeleteBooking(int bookingId)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null) return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        // Check facility availability
        public async Task<bool> IsFacilityAvailable(int facilityId, DateTime bookingDate, TimeSpan startTime, TimeSpan endTime)
        {
            var facility = await _context.Facilities
                .Include(f => f.Bookings)
                .FirstOrDefaultAsync(f => f.FacilityId == facilityId);

            if (facility == null) return false;

            var overlappingBookings = await _context.Bookings
                .Where(b => b.FacilityId == facilityId)
                .Where(b => b.BookingDate.Date == bookingDate.Date)
                .Where(b => b.StartTime < endTime && b.EndTime > startTime)
                .CountAsync();

            return overlappingBookings < facility.Capacity;
        }

        // Add loyalty points
        public async Task AddLoyaltyPoints(string userId)
        {
            var loyalty = await _context.UserLoyalties
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (loyalty == null)
            {
                loyalty = new UserLoyalty { UserId = userId, Points = 10, LastUpdated = DateTime.Now };
                _context.UserLoyalties.Add(loyalty);
            }
            else
            {
                loyalty.Points += 10;
                loyalty.LastUpdated = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        // Get user loyalty points
        public async Task<UserLoyalty> GetUserLoyalty(string userId)
        {
            return await _context.UserLoyalties
                .FirstOrDefaultAsync(l => l.UserId == userId);
        }

        // Deduct loyalty points after free booking
        public async Task DeductLoyaltyPoints(string userId)
        {
            var loyalty = await _context.UserLoyalties
                .FirstOrDefaultAsync(l => l.UserId == userId);

            if (loyalty != null)
            {
                loyalty.Points -= 100;
                await _context.SaveChangesAsync();
            }
        }

        // Get least booked facility
        public async Task<Facility> GetLeastBookedFacility()
        {
            return await _context.Facilities
                .Include(f => f.Bookings)
                .OrderBy(f => f.Bookings.Count)
                .FirstOrDefaultAsync();
        }
    }
}