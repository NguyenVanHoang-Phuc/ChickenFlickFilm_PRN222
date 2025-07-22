using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess
{
    public class SeatBookingDAO
    {
        private readonly MovieDbContext _context;

        // Constructor injection for MovieDbContext
        public SeatBookingDAO(MovieDbContext context)
        {
            _context = context;
        }

        // Asynchronous method to add a list of seat bookings to the database
        public async Task AddBookingAsync(List<SeatBooking> seatBookings)
        {
            if (seatBookings == null || seatBookings.Count == 0)
            {
                throw new ArgumentException("SeatBookings cannot be empty", nameof(seatBookings));
            }

            try
            {
                // Add the seat bookings to the SeatBookings table
                await _context.SeatBookings.AddRangeAsync(seatBookings); // Use AddRangeAsync for async insertion

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle and log the exception as needed
                throw new InvalidOperationException("An error occurred while adding seat bookings to the database.", ex);

            }
        }
        public async Task<List<SeatBooking>> GetSeatBookingsByBookingIdAsync(int bookingId)
        {
            // Use asynchronous querying to retrieve the SeatBookings associated with the provided bookingId
            return await _context.SeatBookings
                                 .Where(sb => sb.BookingId == bookingId) // Filter by bookingId
                                 .Include(sb => sb.Seat)
                                 .ToListAsync(); // Execute the query asynchronously and return the result as a list
        }

    }
}
