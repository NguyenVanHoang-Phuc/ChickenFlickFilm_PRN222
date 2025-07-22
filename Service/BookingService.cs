using BusinessObjects.Models;
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

        public async Task ChangeBookingStatus(int bookingId, string bookingStatus)
        {
            await _bookingRepository.ChangeBookingStatus(bookingId,bookingStatus);
        }

        public Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            return _bookingRepository.GetBookingByIdAsync(bookingId);
        }
    }
}
