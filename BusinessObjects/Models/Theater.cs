using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Theater
{
    public int TheaterId { get; set; }

    public string TheaterName { get; set; } = null!;

    public string? MapUrl { get; set; }

    public string Location { get; set; } = null!;

    public int TotalTheaters { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Auditorium> Auditoria { get; set; } = new List<Auditorium>();
}
