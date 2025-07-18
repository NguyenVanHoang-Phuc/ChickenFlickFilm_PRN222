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
        public PaymentController(IVnPayService vnPayService)
        {

            _vnPayService = vnPayService;
        }

        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }
       

    }
}
