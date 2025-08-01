﻿using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IBookingRepository
    {
        Task<Booking?> GetBookingByIdAsync(int bookingId);
        Task<int> AddBookingAsync(Booking booking);
        Task UpdateBookingAsync(Booking booking);
        List<Booking> GetAllBookingByUserId(int userid);
        Task<decimal> GetTotalAmountAsync();
    }
}
