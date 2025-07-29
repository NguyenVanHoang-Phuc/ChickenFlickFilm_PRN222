using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BusinessObjects.Models;

public partial class Showtime
{
    public int ShowtimeId { get; set; }

    public int MovieId { get; set; }

    public int AuditoriumId { get; set; }

    public DateOnly? ShowDate { get; set; }

    public TimeOnly ShowTime { get; set; }

    public string? Status { get; set; }
    [JsonIgnore]
    public virtual Auditorium Auditorium { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Movie Movie { get; set; } = null!;
}
