using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace ChickenFlickFilmApplication.Controllers
{
    public class AuditoriumController : Controller
    {
        private readonly ISeatService seatService;
        private readonly IAuditoriumService auditoriumService;
        private readonly IShowtimeService showtimeService;
        private readonly IMovieService movieService;
        private readonly ITheaterService theaterService;
        public AuditoriumController(ISeatService seatService, IAuditoriumService auditoriumService, IShowtimeService showtimeService, IMovieService movieService, ITheaterService theaterService)
        {
            this.seatService = seatService;
            this.auditoriumService = auditoriumService;
            this.showtimeService = showtimeService;
            this.movieService = movieService;
            this.theaterService = theaterService;
        }
        public IActionResult Auditorium()
        {
            return RedirectToAction("ShowAuditorium", new { showtimeId = 2 });
        }

        public IActionResult ShowAuditorium(int showtimeId)
        {
            Showtime showtime = showtimeService.GetShowtimeByIdAsync(showtimeId).Result;
            if (showtime == null)
            {
                return Content("Showtime not found.");
            }
            Auditorium audi = auditoriumService.GetAuditoriumById(showtime.AuditoriumId);
            Movie movie = movieService.GetMovieByIdAsync(showtime.MovieId).Result;
            if (audi == null || movie == null)
            {
                return Content("Auditorium or movie was not found.");
            }
            else
            {
                audi.Seats = seatService.GetAllSeatsByAuditorium(audi.AuditoriumId).ToList();
                audi.Theater = theaterService.GetTheaterByIdAsync(audi.TheaterId).Result;
                movie.Showtimes = showtimeService.GetShowtimesByMovieIdAsync(movie.MovieId).Result.ToList();
                showtime.Auditorium = audi;
                showtime.Movie = movie;
                return View("Auditorium",showtime);
            }
        }

        public IActionResult ChangeShowtime(int showtimeid)
        {
            return RedirectToAction("ShowAuditorium", new { showtimeId = showtimeid });
        }

        [HttpPost]
        public IActionResult OnPay(List<int> listSeat, int total)
        {
            if (listSeat == null || listSeat.Count == 0)
            {
                return Content("No seats selected.");
            }
            string message = $"Payment of {total} for seats {string.Join(", ", listSeat)} was successful.";
            return Content(message);
        }

    }
}
