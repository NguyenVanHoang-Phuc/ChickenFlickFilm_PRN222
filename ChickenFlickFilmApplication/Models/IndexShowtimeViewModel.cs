namespace ChickenFlickFilmApplication.Models
{
    public class IndexShowtimeViewModel
    {
        public int ShowtimeId { get; set; }
        public TimeOnly ShowTime { get; set; }
        public string? Status { get; set; }
        public int AuditoriumId { get; set; }
        public string TheaterName { get; set; }
    }

}
