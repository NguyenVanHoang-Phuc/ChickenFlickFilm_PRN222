namespace ChickenFlickFilmApplication.Models
{
    public class ShowtimeUserViewModel
    {
        public string? ShowDate { get; set; }

        public string? DayOfWeek { get; set; }

        public List<string> ShowTimes { get; set; } = new List<string>();

        public string? Format { get; set; }

        public string? TheaterName { get; set; }
    }
}
