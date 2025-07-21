using BusinessObjects.Models;

namespace ChickenFlickFilmApplication.Models
{
    public class IndexViewModel
    {
        public IEnumerable<Movie> Movies { get; set; } = new List<Movie>();

        // Dictionary: Date -> MovieId -> List of Showtimes
        public Dictionary<DateOnly, Dictionary<int, List<Showtime>>> ShowtimesByDate { get; set; }
            = new Dictionary<DateOnly, Dictionary<int, List<Showtime>>>();

        public DateOnly SelectedDate { get; set; }

        // Add TheaterLookup property that the view expects
        public Dictionary<int, Theater> TheaterLookup { get; set; }
            = new Dictionary<int, Theater>();

        // Helper method to get dates for the next week
        public List<DateDisplayInfo> GetWeekDates()
        {
            var dates = new List<DateDisplayInfo>();
            var today = DateTime.Today;
            for (int i = 0; i < 7; i++)
            {
                var date = today.AddDays(i);
                var dateOnly = DateOnly.FromDateTime(date);
                dates.Add(new DateDisplayInfo
                {
                    Date = dateOnly,
                    DisplayName = GetDateDisplayName(date, today),
                    ShortDate = dateOnly.ToString("dd/MM")
                });
            }
            return dates;
        }

        private static string GetDateDisplayName(DateTime date, DateTime today)
        {
            if (date.Date == today.Date)
                return "Hôm nay";
            else if (date.Date == today.AddDays(1).Date)
                return "Mai";
            else
                return date.ToString("dddd", new System.Globalization.CultureInfo("vi-VN"));
        }

        // Helper method to get showtimes for a specific movie on selected date
        public List<Showtime> GetShowtimesForMovie(int movieId)
        {
            if (ShowtimesByDate.ContainsKey(SelectedDate) &&
                ShowtimesByDate[SelectedDate].ContainsKey(movieId))
            {
                return ShowtimesByDate[SelectedDate][movieId];
            }
            return new List<Showtime>();
        }
    }

    public class DateDisplayInfo
    {
        public DateOnly Date { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string ShortDate { get; set; } = string.Empty;
    }
}