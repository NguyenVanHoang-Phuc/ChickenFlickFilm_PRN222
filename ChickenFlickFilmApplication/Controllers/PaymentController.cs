using ChickenFlickFilmApplication.PaymentGatewayIntegration.VnPay;
using ChickenFlickFilmApplication.Services.VnPay;
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
            if (response.VnPayResponseCode.Equals("00"))
                {
                return View("MakePaymentSuccess", response);
                }
            else
            {
                return View("MakePaymentFailed");
            }
            
        }


    }
}
