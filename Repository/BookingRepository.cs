using BusinessObjects.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDAO _bookingDAO;

        public BookingRepository(BookingDAO bookingDAO)
        {
            _bookingDAO = bookingDAO;
        }

        public async Task<int> AddBookingAsync(Booking booking)
        {
            return await _bookingDAO.AddBookingAsync(booking);
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            await _bookingDAO.UpdateBookingAsync(booking);
        }

        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            return await _bookingDAO.GetBookingByIdAsync(bookingId);
        }
        public List<Booking> GetAllBookingByUserId(int userid)
        {
            return _bookingDAO.GetAllBookingByUserId(userid);
        }

        public async Task<decimal> GetTotalAmountAsync()
        {
            return await _bookingDAO.GetTotalAmountAsync();
        }
    }
}
