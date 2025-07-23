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
        private readonly PaymentDAO paymentDAO;
        public PaymentRepository(PaymentDAO paymentDAO)
        {
            this.paymentDAO = paymentDAO;
        }

        public Payment getPaymentByBookingid(int bookingid)
        {
            return paymentDAO.getPaymentByBookingid(bookingid);
        }
    }
}
