using ChickenFlickFilmApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Diagnostics;

namespace ChickenFlickFilmApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IMovieService _movieService;
        private IShowtimeService _showtimeService;

        public HomeController(ILogger<HomeController> logger, IMovieService movieService, IShowtimeService showtimeService)
        {
            _logger = logger;
            _movieService = movieService;
            _showtimeService = showtimeService;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _movieService.GetAllMoviesAsync();
            var startDate = DateOnly.FromDateTime(DateTime.Today);
            var endDate = startDate.AddDays(6);
            var showtimes = await _showtimeService.GetAllAsync(s => s.ShowDate >= startDate && s.ShowDate <= endDate);

            // Group showtimes by date and movie (including both statuses)
            var showtimesByDate = showtimes
                .Where(s => s.Status == "Đang chiếu" || s.Status == "Sắp chiếu")
                .GroupBy(s => s.ShowDate.Value)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(s => s.MovieId)
                          .ToDictionary(mg => mg.Key, mg => mg.OrderBy(s => s.ShowTime).ToList())
                );

            var model = new IndexViewModel
            {
                Movies = movies,
                ShowtimesByDate = showtimesByDate,
                SelectedDate = startDate
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetShowtimesForDate(DateOnly date)
        {
            var showtimes = await _showtimeService.GetAllAsync(s => s.ShowDate.HasValue && s.ShowDate.Value == date);

            var activeShowtimes = showtimes
                .Where(s => s.Status == "Đang chiếu" || s.Status == "Sắp chiếu")
                .GroupBy(s => s.MovieId)
                .ToDictionary(g => g.Key, g => g.OrderBy(s => s.ShowTime).ToList());

            return Json(activeShowtimes);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}