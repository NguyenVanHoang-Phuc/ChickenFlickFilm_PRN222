using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? PaymentStatus { get; set; }

    [Column("vnpay_response_code")]
    public string? VnpayResponseCode { get; set; }

    public string? TransactionId { get; set; }

    public DateTime? TransactionDate { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
