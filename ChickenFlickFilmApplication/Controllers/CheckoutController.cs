using ChickenFlickFilmApplication.Services.VnPay;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChickenFlickFilmApplication.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly IEmailSender _emailSender;
        public CheckoutController(IVnPayService vnPayService, IEmailSender emailSender)
        {
            _vnPayService = vnPayService;
            _emailSender = emailSender;
        }

        private static readonly HttpClient client = new HttpClient();
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            return Json(response);
        }

    }
}
