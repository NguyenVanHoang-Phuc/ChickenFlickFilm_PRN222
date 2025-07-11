using Microsoft.AspNetCore.Mvc;

namespace ChickenFlickFilmApplication.Controllers
{
    public class AuditoriumController : Controller
    {
        public IActionResult Auditorium()
        {
            return View();
        }
    }
}
