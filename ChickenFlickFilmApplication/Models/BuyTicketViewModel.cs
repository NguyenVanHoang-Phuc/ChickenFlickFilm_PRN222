using BusinessObjects.Models;

namespace ChickenFlickFilmApplication.Models
{
    public class BuyTicketViewModel
    {
        public List<Theater> listTheater;
        public Theater selectedTheater;
        public List<Movie> listMovie;
        public Movie selectedMovie;
        public List<Showtime> listShowtime;
    }
}
