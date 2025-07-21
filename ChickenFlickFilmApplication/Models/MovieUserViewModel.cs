using BusinessObjects.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChickenFlickFilmApplication.Models
{
    public class MovieUserViewModel
    {
        public Movie movies { get; set; } = new Movie();

        public List<ShowtimeUserViewModel> Showtimes { get; set; } = new List<ShowtimeUserViewModel>();

        public string? SelectedDate { get; set; }
        public List<string> SevenDays { get; set; }
    }
}
