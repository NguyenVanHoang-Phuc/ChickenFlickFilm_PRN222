using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace ChickenFlickFilmApplication.Controllers
{
    public class AuditoriumController : Controller
    {
        private readonly ISeatService seatService;
        private readonly IAuditoriumService auditoriumService;
        public AuditoriumController(ISeatService seatService, IAuditoriumService auditoriumService)
        {
            this.seatService = seatService;
            this.auditoriumService = auditoriumService;
        }
        public IActionResult Auditorium()
        {
            Auditorium audi = auditoriumService.GetAuditoriumById(1);
            if  (audi == null)
            {
                return Content("Auditorium not found.");
            } else
            {
                audi.Seats = seatService.GetAllSeatsByAuditorium(1).ToList();
                return View(audi);
            }
        }

        public IActionResult SelectSeat( int seatId ) 
        {
            Seat seat = seatService.GetSeatById(seatId);
            if (seat == null)
            {
                return Content("Seat not found.");
            }
            else
            {
                seatService.UpdateSeatStatus(seatId);
                return RedirectToAction("Auditorium");
            }
        }
    }
}
