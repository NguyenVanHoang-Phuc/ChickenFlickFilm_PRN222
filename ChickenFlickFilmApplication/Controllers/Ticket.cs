using System.Threading.Tasks;
using BusinessObjects.Models;
using ChickenFlickFilmApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace ChickenFlickFilmApplication.Controllers
{
    public class Ticket : Controller
    {
        private readonly ITheaterService theaterService;
        private readonly IAuditoriumService auditoriumService;
        private readonly IShowtimeService showtimeService;
        private readonly IMovieService movieService;
        private BuyTicketViewModel buyTicketViewModel = new BuyTicketViewModel();

        public Ticket(ITheaterService theaterService, IAuditoriumService auditoriumService, IShowtimeService showtimeService, IMovieService movieService)
        {
            this.theaterService = theaterService;
            this.auditoriumService = auditoriumService;
            this.showtimeService = showtimeService;
            this.movieService = movieService;
        }
        public async Task<IActionResult> BuyTicket()
        {
            List<Theater> listTheater = await theaterService.GetAllTheatersAsync();
            buyTicketViewModel.listTheater = listTheater;
            return View(buyTicketViewModel);
        }

        public async Task<IActionResult> GetMoviesByLocation(int theaterId)
        {
            List<Auditorium> listAu = auditoriumService.getAllAuditoriumByTheaterId(theaterId);
            List<Showtime> listSh = new List<Showtime>();
            buyTicketViewModel.selectedTheater = await theaterService.GetTheaterByIdAsync(theaterId);
            foreach (Auditorium au in listAu)
            {
                listSh.AddRange(showtimeService.getAllShowtimeByAuditoriumId(au.AuditoriumId));
            }
            List<Movie> movieList = new List<Movie>();
            foreach (Showtime showtime in listSh)
            {
                Movie m = await movieService.GetMovieByIdAsync(showtime.MovieId);
                if ( !movieList.Contains(m) )
                {
                    movieList.Add(m);
                }
            }
            return Json(movieList);
        }

        public async Task<IActionResult> SelectMovie(int movieId, int theaterId)
        {
            Movie movie = await movieService.GetMovieByIdAsync(movieId);
            List<Theater> listTheater = await theaterService.GetAllTheatersAsync();
            buyTicketViewModel.listTheater = listTheater;
            buyTicketViewModel.selectedMovie = movie;
            buyTicketViewModel.selectedTheater = await theaterService.GetTheaterByIdAsync(theaterId);
            return View("BuyTicket", buyTicketViewModel); 
        }


        public async Task<IActionResult> GetShowtimesByMovie(int movieId, int theaterId)
        {
            try
            {
                IEnumerable<Showtime> listS = await showtimeService.GetShowtimesByMovieIdAsync(movieId);
                List<Showtime> result = new List<Showtime>();
                Theater theaterSelected = await theaterService.GetTheaterByIdAsync(theaterId);
                if (theaterSelected == null)
                {
                    return NotFound();
                }
                foreach (Showtime showtime in listS)
                {
                    Auditorium au = auditoriumService.GetAuditoriumById(showtime.AuditoriumId);
                    if (au.TheaterId == theaterSelected.TheaterId)
                    {
                        result.Add(showtime);
                    }
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
           

        }
    }
}
