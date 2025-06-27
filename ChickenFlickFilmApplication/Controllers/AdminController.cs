using Microsoft.AspNetCore.Mvc;

namespace ChickenFlickFilmApplication.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Auditoriums()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Movies()
        {
            return View();
        }
        public IActionResult Showtimes()
        {
            return View();
        }
        public IActionResult Users()
        {
            return View();
        }
        public IActionResult Theaters()
        {
            return View();
        }
    }
}
