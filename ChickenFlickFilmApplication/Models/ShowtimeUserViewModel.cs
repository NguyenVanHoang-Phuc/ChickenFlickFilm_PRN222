using BusinessObjects.Models;

namespace ChickenFlickFilmApplication.Models
{
    public class ShowtimeUserViewModel
    {
        

        public int MovieId { get; set; }

        public string? ShowDate { get; set; }

        public string? DayOfWeek { get; set; }

        public List<Showtime> ShowTimes { get; set; } = new List<Showtime>();

        public string? Format { get; set; }

        public string? TheaterName { get; set; }
    }
}
