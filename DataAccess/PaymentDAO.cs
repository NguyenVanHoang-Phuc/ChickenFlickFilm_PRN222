using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace DataAccess
{
    public class PaymentDAO
    {
        private readonly MovieDbContext _context;
        public PaymentDAO(MovieDbContext context)
        {
            _context = context;
        }
        public Payment getPaymentByBookingid(int bookingid)
        {
            return _context.Payments.FirstOrDefault(p => p.BookingId == bookingid);
        }
    }
}
