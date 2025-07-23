using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? UserId { get; set; }

    public int ShowtimeId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? BookingStatus { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual ICollection<SeatBooking> SeatBookings { get; set; } = new List<SeatBooking>();

    public virtual Showtime Showtime { get; set; } = null!;

    public virtual User? User { get; set; }
}
