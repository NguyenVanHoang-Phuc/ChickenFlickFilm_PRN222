﻿namespace ChickenFlickFilmApplication.PaymentGatewayIntegration.VnPay
{
    public class PaymentInformationModel
    {
        public string OrderType { get; set; }
        public decimal Amount { get; set; }
        public string OrderDescription { get; set; }
        public string Name { get; set; }

        public int BookingId { get; set; }

    }
}
