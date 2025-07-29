using BusinessObjects.Models;
using ChickenFlickFilmApplication.Models;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service;
using System.Diagnostics;
using System.Globalization;

namespace ChickenFlickFilmApplication.Controllers
{
    public class MoviesUserController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IShowtimeService _showtimeService;

        public MoviesUserController(IMovieService movieService, IShowtimeService showtimeService)
        {
            _movieService = movieService;
            _showtimeService = showtimeService;
        }
        public async Task<IActionResult> ListFilm()
        {
            var allMovies = await _movieService.GetAllMoviesAsync();

            var today = DateOnly.FromDateTime(DateTime.Now);

            var nowShowing = new List<Movie>();
            var upcoming = new List<Movie>();

            foreach (var movie in allMovies)
            {
                if (movie.ReleaseDate <= today && movie.EndDate >= today)
                {
                    nowShowing.Add(movie); 
                }
                else if (movie.ReleaseDate > today)
                {
                    upcoming.Add(movie); 
                }
            }
            ViewBag.NowShowing = nowShowing;
            ViewBag.Upcoming = upcoming;
            
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DetailFilm(int id, string selectedDate = null)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }
            var movie = await _movieService.GetMovieByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            var showtimes = await _showtimeService.GetShowtimesByMovieIdAsync(id);

            var sevenDays = _showtimeService.GetSevenDaysStartingFromToday();

            var showtimesGroupedByDate = showtimes
                .GroupBy(s => s.ShowDate)
                .OrderBy(g => g.Key)
                .ToList();

            string selectedDateParsed = selectedDate ?? DateTime.Today.ToString("yyyy-MM-dd");

            var selectedShowtimes = showtimesGroupedByDate
                 .Where(g => g.Key?.ToString("yyyy-MM-dd") == selectedDateParsed)
                 .Select(s => new ShowtimeUserViewModel
                 {
                    ShowDate = s.Key?.ToString("dd/MM/yyyy"),
                    DayOfWeek = s.Key?.ToString("dddd"),
                    ShowTimes = s.ToList(),
                    Format = movie.Format,
                    TheaterName = s.FirstOrDefault()?.Auditorium?.Theater?.TheaterName
                 }).ToList();

            // In ra thông tin của selectedShowtimes
            Console.WriteLine($"Selected Date: {selectedDateParsed}");
            foreach (var showtimeGroup in selectedShowtimes)
            {
                Console.WriteLine($"Showtime: {showtimeGroup.ShowDate}, DayOfWeek: {showtimeGroup.DayOfWeek}, Theater: {showtimeGroup.TheaterName}");
                Console.WriteLine($"ShowTimes: {string.Join(", ", showtimeGroup.ShowTimes)}");
            }

            

            var vm = new MovieUserViewModel
            {
                movies = movie,
                SevenDays = sevenDays,
                Showtimes = selectedShowtimes,
                SelectedDate = selectedDateParsed
            };
            // In ra thông tin chi tiết của MovieUserViewModel
            Console.WriteLine("Movie Details:");
            Console.WriteLine($"Movie ID: {vm.movies.MovieId}");
            Console.WriteLine($"Movie Title: {vm.movies.Title}");
            Console.WriteLine($"Movie Format: {vm.movies.Format}");
            Console.WriteLine($"Selected Date: {vm.SelectedDate}");

            // In ra thông tin về SevenDays
            Console.WriteLine("Seven Days:");
            foreach (var day in vm.SevenDays)
            {
                Console.WriteLine($"Day: {day}");
            }
            ViewBag.ShowtimeSelectedDate = selectedShowtimes;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> SelectDate(int id, string selectedDate)
        {
            var movie = await _movieService.GetMovieByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            var showtimes = await _showtimeService.GetShowtimesByMovieIdAsync(id);
            var sevenDays = _showtimeService.GetSevenDaysStartingFromToday();

            var showtimesGroupedByDate = showtimes
                .GroupBy(s => s.ShowDate)
                .OrderBy(g => g.Key)
                .ToList();

            string selectedDateParsed = selectedDate ?? DateTime.Today.ToString("yyyy-MM-dd");

            var selectedShowtimes = showtimesGroupedByDate
                 .Where(g => g.Key?.ToString("yyyy-MM-dd") == selectedDateParsed)
                 .Select(s => new ShowtimeUserViewModel
                 {
                     
                     ShowDate = s.Key?.ToString("dd/MM/yyyy"),
                     DayOfWeek = s.Key?.ToString("dddd"),
                     ShowTimes = s.ToList(),
                     Format = movie.Format,
                     TheaterName = s.FirstOrDefault()?.Auditorium?.Theater?.TheaterName
                 }).ToList();

            // Truyền selectedShowtimes vào ViewBag
            ViewBag.ShowtimeSelectedDate = selectedShowtimes;

            var vm = new MovieUserViewModel
            {
                movies = movie,
                SevenDays = sevenDays,
                Showtimes = selectedShowtimes,
                SelectedDate = selectedDateParsed
            };

            // Trả về View và ViewBag sẽ chứa dữ liệu cần thiết
            return View("DetailFilm", vm);  // Gọi trực tiếp View "DetailFilm" để render lại trang với dữ liệu mới
        }

        [HttpPost]
        public async Task<IActionResult> SearchFilm(IFormCollection form)
        {
            var searchTerm = form["keySearchFilm"];
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction("ListFilm");
            }

            var filteredMovies = await _movieService.SearchMoviesAsync(searchTerm);
            var nowShowing = new List<Movie>();
            var upcoming = new List<Movie>();


            var today = DateOnly.FromDateTime(DateTime.Now);


            foreach (var movie in filteredMovies)
            {

                if (movie.ReleaseDate <= today && movie.EndDate >= today)
                {
                    nowShowing.Add(movie);
                }

                else if (movie.ReleaseDate > today)
                {
                    upcoming.Add(movie);
                }
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.NowShowing = nowShowing;
            ViewBag.Upcoming = upcoming;


            return View("ListFilm");
        }
    }
}
