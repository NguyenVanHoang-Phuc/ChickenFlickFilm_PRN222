using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BusinessObjects.Models;

public partial class Movie
{
    public int MovieId { get; set; }

    public string Title { get; set; } = null!;

    public string? BannerUrl { get; set; }

    public string? PosterUrl { get; set; }

    public string? AgeRating { get; set; }

    public string Genre { get; set; } = null!;

    public int Duration { get; set; }

    public DateOnly ReleaseDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal? Rating { get; set; }

    public string? Status { get; set; }

    public string? Format { get; set; }

    public string? Language { get; set; }

    public string? Director { get; set; }

    public string? Cast { get; set; }

    public string? Description { get; set; }

    public string? TrailerUrl { get; set; }

    public string? Country { get; set; }
    [JsonIgnore]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    [JsonIgnore]
    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();

    [NotMapped]
    [JsonIgnore]
    public IFormFile? PosterFile { get; set; }

    [NotMapped]
    [JsonIgnore]
    public IFormFile? BannerFile { get; set; }
}