using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class PriceByType
{
    public string Type { get; set; } = null!;

    public decimal? Price { get; set; }

    public virtual ICollection<Auditorium> Auditoria { get; set; } = new List<Auditorium>();

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
