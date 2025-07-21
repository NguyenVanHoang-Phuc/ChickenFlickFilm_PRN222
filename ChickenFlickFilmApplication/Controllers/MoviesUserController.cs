using BusinessObjects.Models;
using ChickenFlickFilmApplication.Models;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> DetailFilm(int id)
        {
            var movie = await _movieService.GetMovieByIdAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            var showtimes = await _showtimeService.GetShowtimeForNext3DaysAsync();
            var movieShowtimes = showtimes
       .Where(s => s.MovieId == id) // Lọc theo movieId của phim
       .GroupBy(s => s.ShowDate) // Nhóm theo ngày
       .ToList();

            var vm = new MovieUserViewModel
            {
                Title = movie.Title,
                BannerUrl = movie.BannerUrl,
                PosterUrl = movie.PosterUrl,
                AgeRating = movie.AgeRating,
                Genre = movie.Genre,
                Duration = movie.Duration,
                ReleaseDate = movie.ReleaseDate,
                EndDate = movie.EndDate,
                Rating = movie.Rating,
                Status = movie.Status,
                Format = movie.Format,
                Language = movie.Language,
                Director = movie.Director,
                Cast = movie.Cast,
                Description = movie.Description,
                TrailerUrl = movie.TrailerUrl,
                Country = movie.Country,
                //Showtimes = movieShowtimes.Select(group => new ShowtimeUserViewModel
                //{
                //    ShowDate = group.Key.ToString("dd/MM/yyyy"),
                //    DayOfWeek = group.Key.ToString("dddd", new System.Globalization.CultureInfo("vi-VN")),
                //    Showtimes = group.Select(st => new ShowtimeDetailViewModel
                //    {
                //        ShowTime = st.ShowTime.ToString("HH:mm"),
                //        Format = st.Movie.Format,
                //    }).ToList()
                //}).ToList()
            };

            return View(vm);
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
