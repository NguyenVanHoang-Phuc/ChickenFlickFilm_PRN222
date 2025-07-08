using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Auditorium
{
    public int AuditoriumId { get; set; }

    public int TheaterId { get; set; }

    public string AuditoriumName { get; set; } = null!;

    public string? AuditoriumType { get; set; }

    public int RowNumber { get; set; }

    public int ColumnNumber { get; set; }

    public int TotalSeats { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual PriceByType? AuditoriumTypeNavigation { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();

    public virtual Theater Theater { get; set; } = null!;
}
