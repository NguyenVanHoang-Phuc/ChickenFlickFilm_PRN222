using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class SeatBooking
{
    public int SeatBookingId { get; set; }

    public int BookingId { get; set; }

    public int SeatId { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Seat Seat { get; set; } = null!;
}
