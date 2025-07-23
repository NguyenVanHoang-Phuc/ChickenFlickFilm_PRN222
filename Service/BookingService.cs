using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<int> AddBookingAsync(Booking booking)
        {
            return await _bookingRepository.AddBookingAsync(booking);
        }

        public Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            return _bookingRepository.GetBookingByIdAsync(bookingId);
        }
        public async Task ChangeBookingStatus(Booking booking, string bookingStatus)
        {
            if (booking == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy Booking theo bookingId: {booking.BookingId}");
            }
            else
            {
                Console.WriteLine("Em den duoc day roi chi oi!!!!");
                booking.BookingStatus = bookingStatus;
                await _bookingRepository.UpdateBookingAsync(booking);
            }
        }

        public List<Booking> GetAllBookingByUserId(int userid)
        {
            return _bookingRepository.GetAllBookingByUserId(userid);
        }
        public async Task<decimal> GetTotalAmountAsync()
        {
            return await _bookingRepository.GetTotalAmountAsync();
        }
    }
}
}
