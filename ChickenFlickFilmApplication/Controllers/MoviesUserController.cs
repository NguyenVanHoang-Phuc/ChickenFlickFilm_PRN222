using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace ChickenFlickFilmApplication.Controllers
{
    public class MoviesUserController : Controller
    {
        private readonly IShowtimeService _showtimeService;
       
        public MoviesUserController(IShowtimeService showtimeService)
        {
            _showtimeService = showtimeService;
        }
        public async Task<IActionResult> ListFilm(string status)
        {
            // 1. Lấy tất cả showtimes trong 7 ngày tới
            var showtimes = await _showtimeService.GetShowtimesForNext7DaysAsync();

            // 2. Xác định ngày hôm nay (chỉ phần date)
            var today = DateOnly.FromDateTime(DateTime.Now);

            // 3. Group theo Movie và phân loại
            var nowShowing = new List<Movie>();
            var upcoming = new List<Movie>();

            var byMovie = showtimes
                .Where(s => s.ShowDate.HasValue)
                .GroupBy(s => s.Movie);

            foreach (var grp in byMovie)
            {
                var dates = grp.Select(s => s.ShowDate.Value);
                if (dates.Any(d => d == today))
                    nowShowing.Add(grp.Key);
                else if (dates.Any(d => d > today))
                    upcoming.Add(grp.Key);
            }

            // 4. Đưa vào ViewBag hoặc ViewModel
            ViewBag.NowShowing = nowShowing;
            ViewBag.Upcoming = upcoming;

            return View();
        }

        public IActionResult DetailFilm(int id)
        {
            return View();
        }
    }
}
