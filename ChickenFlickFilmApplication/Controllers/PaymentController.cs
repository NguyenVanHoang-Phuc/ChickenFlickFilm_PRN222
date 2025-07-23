using BusinessObjects.Models;
using ChickenFlickFilmApplication.Models;
using ChickenFlickFilmApplication.PaymentGatewayIntegration.VnPay;
using ChickenFlickFilmApplication.Services.VnPay;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;

namespace ChickenFlickFilmApplication.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly IBookingService _bookingService;
        private readonly IShowtimeService _showtimeService;
        private readonly ISeatBookingService _seatBookingService;
        private readonly IMovieService _movieService;
        private readonly IAuditoriumService _auditoriumService;
        private readonly ITheaterService _theaterService;
        public IActionResult BookingDetails()
        {
            return View();
        }
        public IActionResult MakePaymentSuccess()
        {
            return View();
        }
        public IActionResult MakePaymentFailed()
        {
            return View();
        }
        public PaymentController(IVnPayService vnPayService, IBookingService bookingService, IShowtimeService showtimeService, 
            IMovieService movieService, IAuditoriumService auditoriumService, ITheaterService theaterService, ISeatBookingService seatBookingService)
        {

            _vnPayService = vnPayService;
            _bookingService = bookingService;
            _showtimeService = showtimeService;
            _movieService = movieService;
            _auditoriumService = auditoriumService;
            _theaterService = theaterService;
            _seatBookingService = seatBookingService;
        }

        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            if (model == null)
            {
                return Content("Invalid payment data.");
            }

            if (string.IsNullOrEmpty(model.Name))
            {
                return Content("Missing or invalid payment details.");
            }
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            PaymentResponseModel response = _vnPayService.PaymentExecute(Request.Query);
            // get data from response.OrderID = BookingId
            var bookingId = response.OrderId;
            Console.WriteLine($"BookingId: {bookingId} ");
            if (int.TryParse(bookingId, out int parsedBookingId))
            {
                if ("00".Equals(response.VnPayResponseCode))
                {
                    //Change Booking status
                    Booking booking = _bookingService.GetBookingByIdAsync(parsedBookingId).Result;
                     await _bookingService.ChangeBookingStatus(parsedBookingId, "Success");

                    int showtimeId = booking.ShowtimeId;
                    Showtime showtime = _showtimeService.GetShowtimeByIdAsync(showtimeId).Result;
                    int movieId = showtime.MovieId;
                    Movie movie = _movieService.GetMovieByIdAsync(movieId).Result;
                    int auditoriumId = showtime.AuditoriumId;
                    Auditorium auditorium = _auditoriumService.GetAuditoriumByIdAsync(auditoriumId).Result;
                    Theater theater = _theaterService.GetTheaterByAuditoriumIdAsync(auditoriumId).Result;
                    List<SeatBooking> seatBookings = await _seatBookingService.GetSeatBookingsByBookingIdAsync(parsedBookingId);
                    string movieNameTicket = movie.Title;
                    string theaterNameTicket = theater.TheaterName;
                    string showtimeTicket = showtime.ShowTime + "," + showtime.ShowDate;
                    string auditoriumTicket = auditorium.AuditoriumName;
                    List<string> seatNumbers = seatBookings
                                                .Select(sb => sb.Seat.SeatNumber)  // Access SeatNumber from the related Seat
                                                .ToList();
                    decimal amount = response.Amount;

                    // Create a view model
                    var model = new PaymentSuccessViewModel
                    {
                        MovieNameTicket = movieNameTicket,
                        TheaterNameTicket = theaterNameTicket,
                        ShowtimeTicket = showtimeTicket,
                        AuditoriumTicket = auditoriumTicket,    
                        SeatNumbers = seatNumbers,
                        Amount = amount

                    };
                    Console.WriteLine($"MovieNameTicket : {model.MovieNameTicket} \n" +
                        $"TheaterNameTicket: {model.TheaterNameTicket} \n" +
                        $"ShowtimeTicket: {model.ShowtimeTicket}\n" +
                        $"AuditoriumTicket: {model.AuditoriumTicket}\n" +
                        $"SeatNumbers: {model.SeatNumbers.Count()}\n" +
                        $"Amount : {model.Amount}");

                    return View("MakePaymentSuccess",model);

                }


                // Get dữ liệu
                //var MovieName = TempData["MovieName"];
                //var TheaterName = TempData["TheaterName"];
                //var Showtime= TempData["Showtime"];
                //var Auditorium = TempData["Auditorium"];
                //var listSeat = TempData["listSeat"];
                //var totalString = TempData["total"] as string;
                //if (MovieName == null || TheaterName == null || Showtime == null || Auditorium == null || listSeat == null || totalString == null)
                //{
                //    return Content("Passing Ticket's details doesn't work");
                //}
                //decimal total = 0;
                //if (!decimal.TryParse(totalString, out total))
                //{
                //    return Content("Invalid total value");
                //}

                // change status của ghế và của payment


                else
                {
                    _bookingService.ChangeBookingStatus(parsedBookingId, "Failed");
                    return View("MakePaymentFailed");
                }
            }
            else
            {
               return Content("Invalid booking ID format");
            }
        }
        //[HttpPost]
        //public IActionResult ShowTicketDetails(string MovieName,string TheaterName, string Showtime, string Auditorium, List<int> listSeat ,decimal total)
        //{
        //    if(MovieName == null || TheaterName == null || Showtime == null || Auditorium == null || listSeat == null || total==0)
        //    {
        //        return Content("Ticket details are null");
        //    }
        //    //return Content("MovieName: " + MovieName
        //    //        + "\nTheaterName: " + TheaterName
        //    //        + "\n Showtime: " + Showtime
        //    //        + "\n Auditorium: " + Auditorium
        //    //        + "\nlistSeat:  " + listSeat.Count()
        //    //        + "\ntotal: " + total);
        //    TempData["MovieName"] = MovieName;
        //    TempData["TheaterName"] = TheaterName;
        //    TempData["Showtime"] = Showtime;
        //    TempData["Auditorium"] = Auditorium;
        //    TempData["listSeat"] = listSeat;
        //    TempData["total"] = total.ToString();
        //    return RedirectToAction("PaymentCallbackVnpay");

        //}


    }
}
