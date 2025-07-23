using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using DataAccess;

namespace Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentDAO _paymentDAO;
        public PaymentRepository(PaymentDAO paymentDAO)
        {
            this._paymentDAO = paymentDAO;
        }

        public Payment getPaymentByBookingid(int bookingid)
        {
            return _paymentDAO.getPaymentByBookingid(bookingid);
        }
        public async Task AddPaymentAsync(Payment payment)
        {
            await _paymentDAO.AddPaymentAsync(payment);
        }
    }
}
