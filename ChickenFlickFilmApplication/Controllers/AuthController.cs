using Microsoft.AspNetCore.Mvc;

namespace ChickenFlickFilmApplication.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Signup()
        {
            return View();
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        public IActionResult UserProfile()
        {
            return View();
        }
    }
}
