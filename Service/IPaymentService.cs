﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Service
{
    public interface IPaymentService
    {
        Payment getPaymentByBookingid(int bookingid);
        Task AddPaymentAsync(Payment payment);
    }
}
