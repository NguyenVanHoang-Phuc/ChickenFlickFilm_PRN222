using BusinessObjects.Models;
using ChickenFlickFilmApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Service;
using System.Diagnostics;

namespace ChickenFlickFilmApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieService _movieService;
        private readonly IShowtimeService _showtimeService;
        private readonly ITheaterService _theaterService;

        public HomeController(
            ILogger<HomeController> logger,
            IMovieService movieService,
            IShowtimeService showtimeService,
            ITheaterService theaterService)
        {
            _logger = logger;
            _movieService = movieService;
            _showtimeService = showtimeService;
            _theaterService = theaterService;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _movieService.GetAllMoviesAsync();
            var startDate = DateOnly.FromDateTime(DateTime.Today);
            var endDate = startDate.AddDays(6);

            var showtimes = await _showtimeService.GetAllAsync(s => s.ShowDate >= startDate && s.ShowDate <= endDate);

            // Sequentially load theaters to avoid DbContext threading issues
            var theaterLookup = new Dictionary<int, Theater>();
            foreach (var auditoriumId in showtimes.Select(s => s.AuditoriumId).Distinct())
            {
                var theater = await _theaterService.GetTheaterByAuditoriumIdAsync(auditoriumId);
                theaterLookup[auditoriumId] = theater;
            }

            var filteredShowtimes = showtimes
                .Where(s => s.Status == "Đang chiếu" || s.Status == "Sắp chiếu")
                .ToList();

            var showtimesByDate = new Dictionary<DateOnly, Dictionary<int, List<Showtime>>>();

            foreach (var dateGroup in filteredShowtimes.GroupBy(s => s.ShowDate!.Value))
            {
                var movieGroups = new Dictionary<int, List<Showtime>>();

                foreach (var movieGroup in dateGroup.GroupBy(s => s.MovieId))
                {
                    var auditoriumGroups = movieGroup
                        .GroupBy(s => s.AuditoriumId)
                        .OrderBy(group =>
                        {
                            return theaterLookup.TryGetValue(group.Key, out var theater)
                                ? theater?.TheaterName ?? "ZZZ"
                                : "ZZZ";
                        });

                    var orderedShowtimes = auditoriumGroups
                        .SelectMany(g => g.OrderBy(s => s.ShowTime))
                        .ToList();

                    movieGroups[movieGroup.Key] = orderedShowtimes;
                }

                showtimesByDate[dateGroup.Key] = movieGroups;
            }

            var model = new IndexViewModel
            {
                Movies = movies,
                ShowtimesByDate = showtimesByDate,
                SelectedDate = startDate,
                TheaterLookup = theaterLookup
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetShowtimesForDate(DateOnly date)
        {
            var showtimes = await _showtimeService.GetAllAsync(s => s.ShowDate.HasValue && s.ShowDate.Value == date);

            // Sequentially load theaters to avoid DbContext threading issues
            var theaterLookup = new Dictionary<int, Theater>();
            foreach (var auditoriumId in showtimes.Select(s => s.AuditoriumId).Distinct())
            {
                var theater = await _theaterService.GetTheaterByAuditoriumIdAsync(auditoriumId);
                theaterLookup[auditoriumId] = theater;
            }

            var filteredShowtimes = showtimes
                .Where(s => s.Status == "Đang chiếu" || s.Status == "Sắp chiếu")
                .ToList();

            var showtimesByMovie = new Dictionary<int, List<IndexShowtimeViewModel>>();

            foreach (var movieGroup in filteredShowtimes.GroupBy(s => s.MovieId))
            {
                var auditoriumGroups = movieGroup
                    .GroupBy(s => s.AuditoriumId)
                    .OrderBy(group =>
                    {
                        return theaterLookup.TryGetValue(group.Key, out var theater)
                            ? theater?.TheaterName ?? "ZZZ"
                            : "ZZZ";
                    });

                var orderedShowtimes = movieGroup
                    .OrderBy(m => theaterLookup.TryGetValue(m.AuditoriumId, out var theater) ? theater.TheaterName : "ZZZ")
                    .ThenBy(m => m.ShowTime)
                    .Select(m => new IndexShowtimeViewModel
                    {
                        ShowtimeId = m.ShowtimeId,
                        ShowTime = m.ShowTime,
                        Status = m.Status,
                        AuditoriumId = m.AuditoriumId,
                        TheaterName = theaterLookup.TryGetValue(m.AuditoriumId, out var theater) ? theater.TheaterName : ""
                    }).ToList();
                showtimesByMovie[movieGroup.Key] = orderedShowtimes;
            }

            var theaterData = theaterLookup.ToDictionary(
                kvp => kvp.Key,
                kvp => new
                {
                    name = kvp.Value?.TheaterName ?? "Unknown Theater",
                    id = kvp.Key
                }
            );

            return Json(new { showtimes = showtimesByMovie, theaters = theaterData });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
