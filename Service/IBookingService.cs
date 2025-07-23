using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IBookingService
    {
        Task<Booking?> GetBookingByIdAsync(int bookingId);
        Task<int> AddBookingAsync(Booking booking);
        Task ChangeBookingStatus(Booking booking, string bookingStatus);

    }
}
