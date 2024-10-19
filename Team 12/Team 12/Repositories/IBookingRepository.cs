using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Team_12.Models;

namespace Team_12.Repositories
{
    public interface IBookingRepository
    {
        // CRUD Operations
        Task<Booking> CreateBooking(Booking booking);
        Task<IEnumerable<Booking>> GetAllBookings();
        Task<Booking> GetBookingById(int bookingId);
        Task<Booking> UpdateBooking(Booking booking);
        Task<bool> DeleteBooking(int bookingId);

        // Additional Booking-specific Methods
        Task<bool> IsFacilityAvailable(int facilityId, DateTime bookingDate, TimeSpan startTime, TimeSpan endTime);
        Task AddLoyaltyPoints(string userId);
        Task<UserLoyalty> GetUserLoyalty(string userId);
        Task<Facility> GetLeastBookedFacility();
        Task DeductLoyaltyPoints(string userId);
    }
}