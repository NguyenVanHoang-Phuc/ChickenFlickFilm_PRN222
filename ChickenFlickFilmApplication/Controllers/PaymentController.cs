using BusinessObjects.Models;
using ChickenFlickFilmApplication.PaymentGatewayIntegration.VnPay;
using ChickenFlickFilmApplication.Services.VnPay;
using Humanizer;
using Microsoft.AspNetCore.Mvc;

namespace ChickenFlickFilmApplication.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
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
        public PaymentController(IVnPayService vnPayService)
        {

            _vnPayService = vnPayService;
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
        public IActionResult PaymentCallbackVnpay()
        {
            PaymentResponseModel response = _vnPayService.PaymentExecute(Request.Query);
            if ("00".Equals(response.VnPayResponseCode))
                {

                // Get dữ liệu
                var MovieName = TempData["MovieName"];
                var TheaterName = TempData["TheaterName"];
                var Showtime= TempData["Showtime"];
                var Auditorium = TempData["Auditorium"];
                var listSeat = TempData["listSeat"];
                var totalString = TempData["total"] as string;
                if (MovieName == null || TheaterName == null || Showtime == null || Auditorium == null || listSeat == null || totalString == null)
                {
                    return Content("Passing Ticket's details doesn't work");
                }
                decimal total = 0;
                if (!decimal.TryParse(totalString, out total))
                {
                    return Content("Invalid total value");
                }
                return View("MakePaymentSuccess");
                // change status của ghế và của payment

                }
            else
            {
                return View("MakePaymentFailed");
            }
            
        }
        [HttpPost]
        public IActionResult ShowTicketDetails(string MovieName,string TheaterName, string Showtime, string Auditorium, List<int> listSeat ,decimal total)
        {
            if(MovieName == null || TheaterName == null || Showtime == null || Auditorium == null || listSeat == null || total==0)
            {
                return Content("Ticket details are null");
            }
            //return Content("MovieName: " + MovieName
            //        + "\nTheaterName: " + TheaterName
            //        + "\n Showtime: " + Showtime
            //        + "\n Auditorium: " + Auditorium
            //        + "\nlistSeat:  " + listSeat.Count()
            //        + "\ntotal: " + total);
            TempData["MovieName"] = MovieName;
            TempData["TheaterName"] = TheaterName;
            TempData["Showtime"] = Showtime;
            TempData["Auditorium"] = Auditorium;
            TempData["listSeat"] = listSeat;
            TempData["total"] = total.ToString();
            return RedirectToAction("PaymentCallbackVnpay");

        }


    }
}
