using BusinessObjects.Models;
using ChickenFlickFilmApplication.Models;
using ChickenFlickFilmApplication.PaymentGatewayIntegration.VnPay;
using ChickenFlickFilmApplication.Services.VnPay;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Security.Policy;
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
        private readonly IPaymentService _paymentService;
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
            IMovieService movieService, IAuditoriumService auditoriumService, ITheaterService theaterService, 
            ISeatBookingService seatBookingService, IPaymentService paymentService)
        {

            _vnPayService = vnPayService;
            _bookingService = bookingService;
            _showtimeService = showtimeService;
            _movieService = movieService;
            _auditoriumService = auditoriumService;
            _theaterService = theaterService;
            _seatBookingService = seatBookingService;
            _paymentService = paymentService;
        }

        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            if (model == null)
            {
                return Content("Invalid payment data.");
            }
            //get time at starting making payment
            DateTime currTime = DateTime.Now;
            TempData["CurrentTime"] = currTime;
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Redirect(url);
        }
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            PaymentResponseModel response = _vnPayService.PaymentExecute(Request.Query);
            // get data from response.OrderID = BookingId
            int bookingId = int.Parse(response.OrderId.Split('_')[0]);
            Console.WriteLine($"BookingId: {bookingId} ");
            if (bookingId>0)
            {
                Booking booking = await _bookingService.GetBookingByIdAsync(bookingId);

                Payment payment = new Payment()
                {

                    BookingId = booking.BookingId,
                    Amount = response.Amount,
                    PaymentMethod = response.PaymentMethod,
                    PaymentStatus = response.VnPayResponseCode,
                    TransactionId = response.TransactionId,
                    TransactionDate = response.TransactionDate,
                    VnpayResponseCode = response.VnPayResponseCode
                };
                
                if ("00".Equals(response.VnPayResponseCode))
                {
                    Console.WriteLine($"Thanh toan thanh cong");
                    //Change Booking status
                    await _bookingService.ChangeBookingStatus(booking, "Success");

                    int showtimeId = booking.ShowtimeId;
                    Showtime showtime = await _showtimeService.GetShowtimeByIdAsync(showtimeId);
                    int movieId = showtime.MovieId;
                    Movie movie = await _movieService.GetMovieByIdAsync(movieId);
                    int auditoriumId = showtime.AuditoriumId;
                    Auditorium auditorium = await _auditoriumService.GetAuditoriumByIdAsync(auditoriumId);
                    Theater theater = await _theaterService.GetTheaterByAuditoriumIdAsync(auditoriumId);
                    List<SeatBooking> seatBookings = await _seatBookingService.GetSeatBookingsByBookingIdAsync(bookingId);
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

                    payment.PaymentStatus = "Thành công";
                    await _paymentService.AddPaymentAsync(payment);
                    return View("MakePaymentSuccess", model);

                }

                else
                {
                    _bookingService.ChangeBookingStatus(booking, "Failed");
                    payment.PaymentStatus = "Thất bại";
                    await _paymentService.AddPaymentAsync(payment);
                    return View("MakePaymentFailed");
                }
                
            }
            else
            {
                return Content("Invalid booking ID format");
            }
        }


    }
}
