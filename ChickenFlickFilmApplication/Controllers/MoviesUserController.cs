using Microsoft.AspNetCore.Mvc;

namespace ChickenFlickFilmApplication.Controllers
{
    public class MoviesUserController : Controller
    {
        public IActionResult ListFilm()
        {
            return View();
        }

        public IActionResult DetailFilm(int id)
        {
            return View();
        }
    }
}
