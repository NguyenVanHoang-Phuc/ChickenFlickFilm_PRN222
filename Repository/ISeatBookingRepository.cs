using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface ISeatBookingRepository
    {
        Task AddSeatBookingAsync(List<SeatBooking> seatBookings);
        Task<List<SeatBooking>> GetSeatBookingsByBookingIdAsync(int bookingId);
    }
}
