using BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace ChickenFlickFilmApplication.Controllers
{
    [Authorize(Roles = "Customer")]
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

        public IActionResult Auditorium(int ShowtimeId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }
            Showtime showtime = showtimeService.GetShowtimeByIdAsync(ShowtimeId).Result;
            if (showtime == null)
            {
                return Content("Showtime not found.");
            }
            Auditorium audi = auditoriumService.GetAuditoriumById(showtime.AuditoriumId);
            Movie movie = movieService.GetMovieByIdAsync(showtime.MovieId).Result;
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            List<Showtime> listShowtime = showtimeService.GetShowtimesByMovieIdAsync(movie.MovieId).Result.ToList();
            var filterShowtime = listShowtime.Where(s => s.ShowDate == showtime.ShowDate).ToList();
            if (audi == null || movie == null)
            {
                return Content("Auditorium or movie was not found.");
            }
            else
            {
                audi.Seats = seatService.GetAllSeatsByAuditorium(audi.AuditoriumId).ToList();
                audi.Theater = theaterService.GetTheaterByIdAsync(audi.TheaterId).Result;
                movie.Showtimes = filterShowtime;
                showtime.Auditorium = audi;
                showtime.Movie = movie;
                return View(showtime);
            }
        }

        [HttpPost]
        public IActionResult OnPay(List<int> listSeat, decimal total, string Name, string OrderDescription, string OrderType, int ShowtimeId, int UserId)
        {
            if (listSeat == null || listSeat.Count == 0)
            {
                return Content("No seats selected.");
            }

            Console.WriteLine($"total: {total}, Name :{Name}, OrderDescription: {OrderDescription}, " +
                $"OrderType: {OrderType}, ShowtimeId: {ShowtimeId},UserId: {UserId}   ");

            return RedirectToAction("CreateBooking", "Booking",
                new
                {
                    total = total,
                    Name = Name,
                    OrderDescription = OrderDescription,
                    OrderType = OrderType,
                    ShowtimeId = ShowtimeId,
                    UserId = UserId,
                    ListSeat = listSeat
                });
            
        }

    }
}
