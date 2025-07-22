using BusinessObjects.Models;
using Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public class SeatBookingService : ISeatBookingService
    {
        private readonly ISeatBookingRepository _seatBookingRepository;

        // Constructor with dependency injection
        public SeatBookingService(ISeatBookingRepository seatBookingRepository)
        {
            _seatBookingRepository = seatBookingRepository;
        }

        // Method to add a list of seat bookings asynchronously
        public async Task AddSeatBookingAsync(List<SeatBooking> seatBookings)
        {
            // You can add business logic or validation here if needed
            if (seatBookings == null || seatBookings.Count == 0)
            {
                throw new ArgumentException("Seat bookings cannot be null or empty", nameof(seatBookings));
            }

            // Call repository to add the seat bookings
            await _seatBookingRepository.AddSeatBookingAsync(seatBookings);
        }

        public async Task<List<SeatBooking>> GetSeatBookingsByBookingIdAsync(int bookingId)
        {
            return await _seatBookingRepository.GetSeatBookingsByBookingIdAsync(bookingId);
        }
    }
}
