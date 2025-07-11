using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Seat
{
    public int SeatId { get; set; }

    public int AuditoriumId { get; set; }

    public string SeatNumber { get; set; } = null!;

    public string? SeatType { get; set; }

    public bool? IsAvailable { get; set; }

    public virtual Auditorium Auditorium { get; set; } = null!;

    public virtual ICollection<SeatBooking> SeatBookings { get; set; } = new List<SeatBooking>();

    public virtual PriceByType? SeatTypeNavigation { get; set; }
}
