using BusinessObjects.Models;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public class SeatBookingRepository : ISeatBookingRepository
    {
        private readonly SeatBookingDAO _seatBookingDAO;

        // Constructor injection for SeatBookingDAO
        public SeatBookingRepository(SeatBookingDAO seatBookingDAO)
        {
            _seatBookingDAO = seatBookingDAO;
        }

        // Asynchronous method to add a list of seat bookings
        public async Task AddSeatBookingAsync(List<SeatBooking> seatBookings)
        {
            if (seatBookings == null || seatBookings.Count == 0)
            {
                throw new ArgumentException("Seat bookings cannot be null or empty", nameof(seatBookings));
            }

            try
            {
                // Call the DAO method to add the seat bookings asynchronously
                await _seatBookingDAO.AddBookingAsync(seatBookings);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed (you could log the error or rethrow it)
                throw new InvalidOperationException("Error occurred while adding seat bookings.", ex);
            }
        }
        public async Task<List<SeatBooking>> GetSeatBookingsByBookingIdAsync(int bookingId)
        {
            // Fetch all seat bookings associated with a specific bookingId asynchronously
            return await _seatBookingDAO.GetSeatBookingsByBookingIdAsync(bookingId);
        }
    }
}
