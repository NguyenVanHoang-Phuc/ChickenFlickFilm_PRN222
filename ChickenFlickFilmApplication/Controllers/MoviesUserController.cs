using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace ChickenFlickFilmApplication.Controllers
{
    public class MoviesUserController : Controller
    {
        private readonly IMovieService _movieService;
       
        public MoviesUserController(IMovieService movieService)
        {
            _movieService = movieService;
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

        public IActionResult DetailFilm(int id)
        {
            return View();
        }
    }
}
