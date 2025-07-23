using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Repository;

namespace Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository PaymentRepository;
        public PaymentService(IPaymentRepository PaymentRepository)
        {
            this.PaymentRepository = PaymentRepository;
        }

        public Payment getPaymentByBookingid(int bookingid)
        {
            return PaymentRepository.getPaymentByBookingid(bookingid);
        }
        public async Task AddPaymentAsync(Payment payment)
        {
            await PaymentRepository.AddPaymentAsync(payment);
        }
    }
}
