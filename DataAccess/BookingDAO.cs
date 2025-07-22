using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class BookingDAO
    {
        private readonly MovieDbContext _context;
        public BookingDAO(MovieDbContext context)
        {
            _context = context;
        }
        public async Task<int> AddBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return booking.BookingId;
        }
        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            return await _context.Bookings
                         .FirstOrDefaultAsync(b => b.BookingId == bookingId);
        }
        
        public async Task ChangeBookingStatus(int bookingId, string bookingStatus)
        {
            Booking booking = await _context.Bookings
                         .FirstOrDefaultAsync(b => b.BookingId == bookingId);
            if (booking == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy Booking theo bookingId: {bookingId}");
            }
            booking.BookingStatus = bookingStatus;
            await _context.SaveChangesAsync();
        }

    }
}
