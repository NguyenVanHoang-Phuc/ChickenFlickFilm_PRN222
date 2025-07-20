using BusinessObjects.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChickenFlickFilmApplication.Models
{
    public class MovieUserViewModel
    {
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

        [NotMapped]
        public IFormFile? PosterFile { get; set; }

        [NotMapped]
        public IFormFile? BannerFile { get; set; }

        public List<ShowtimeUserViewModel> Showtimes { get; set; } = new List<ShowtimeUserViewModel>();
    }
}
