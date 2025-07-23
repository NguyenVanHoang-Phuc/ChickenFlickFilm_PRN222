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
        public async Task UpdateBookingAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }
       
        



        public List<Booking> GetAllBookingByUserId(int userid)
        {
            return _context.Bookings.Where(b => b.UserId == userid).ToList();
        }

        public async Task<decimal> GetTotalAmountAsync()
        {
            var total = await (from p in _context.Payments
                               join b in _context.Bookings on p.BookingId equals b.BookingId
                               where p.PaymentStatus == "Thành công" && b.BookingStatus == "Confirmed"
                               select p.Amount).SumAsync();

            return total;
        }
    }
}
