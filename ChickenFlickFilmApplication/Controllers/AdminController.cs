using BusinessObjects.Models;
using ChickenFlickFilmApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service;
using X.PagedList.Extensions;

namespace ChickenFlickFilmApplication.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITheaterService _theaterService;
        private readonly IMovieService _movieService;
        private readonly IShowtimeService _showtimeService;
        private readonly IAuditoriumService _auditoriumService;
        private readonly IWebHostEnvironment _env;

        public AdminController(IUserService userService, ITheaterService theaterService, IMovieService movieService, IShowtimeService showtimeService, IAuditoriumService auditoriumService ,IWebHostEnvironment env)
        {
            _userService = userService;
            _theaterService = theaterService;
            _movieService = movieService;
            _showtimeService = showtimeService;
            _auditoriumService = auditoriumService;
            _env = env;
        }
        public IActionResult Auditoriums()
        {
            return View();
        }
        public async Task<IActionResult> Dashboard()
        {
            var model = new DashboardViewModel
            {
                TotalUsers = await _userService.GetTotalUsersAsync(),
                TotalMovies = await _movieService.GetTotalMoviesAsync(),
                TotalShowtimes = await _showtimeService.GetTotalShowtimesAsync(),
                TotalTheaters = await _theaterService.GetTotalTheatersAsync(),
                TotalAuditoriums = await _auditoriumService.GetTotalAuditoriumsAsync(),
                MonthlyRevenue = 20000000,
            };

            return View(model);
        }

        // Manage Movies
        public async Task<IActionResult> Movies(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var movies = await _movieService.GetAllMoviesAsync();
            return View(movies.ToPagedList(pageNumber, pageSize));
        }

        // GET
        public IActionResult GetCreateMovieModal()
        {
            ViewData["Title"] = "Thêm phim mới";
            ViewData["Action"] = "CreateMovie";
            return PartialView("Partial/_CreateOrEditModalMovie", new Movie());
        }

        public async Task<IActionResult> GetEditMovieModal(int id)
        {
            var movie = await _movieService.GetMovieByIdAsync(id); 
            ViewData["Title"] = "Chỉnh sửa";
            ViewData["Action"] = "EditMovie";
            return PartialView("Partial/_CreateOrEditModalMovie", movie);
        }

        public async Task<IActionResult> GetDeleteMovieModal(int id)
        {
            var movie = await _movieService.GetMovieByIdAsync(id); 
            return PartialView("Partial/_DeleteConfirmModalMovie", movie);
        }


        // Post
        [HttpPost]
        public async Task<IActionResult> EditMovie (Movie model)
        {
            var existingMovie = await _movieService.GetMovieByIdAsync(model.MovieId);

            if (model.PosterFile == null)
                model.PosterUrl = existingMovie.PosterUrl;

            if (model.BannerFile == null)
                model.BannerUrl = existingMovie.BannerUrl;

            if (model.PosterFile != null)
            {
                var fileName = $"{Guid.NewGuid()}_{model.PosterFile.FileName}";
                var path = Path.Combine(_env.WebRootPath, "uploads/posters", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.PosterFile.CopyToAsync(stream);
                }
                existingMovie.PosterUrl = "/uploads/posters/" + fileName;
            }

            if (model.BannerFile != null)
            {
                var fileName = $"{Guid.NewGuid()}_{model.BannerFile.FileName}";
                var path = Path.Combine(_env.WebRootPath, "uploads/banners", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.BannerFile.CopyToAsync(stream);
                }
                existingMovie.BannerUrl = "/uploads/banners/" + fileName;
            }

            // Cập nhật các trường khác
            existingMovie.Title = model.Title;
            existingMovie.Genre = model.Genre;
            existingMovie.Duration = model.Duration;
            existingMovie.AgeRating = model.AgeRating;
            existingMovie.Format = model.Format;
            existingMovie.Language = model.Language;
            existingMovie.Status = model.Status;
            existingMovie.Description = model.Description;
            existingMovie.Country = model.Country;
            existingMovie.Director = model.Director;
            existingMovie.Cast = model.Cast;
            existingMovie.Rating = model.Rating;
            existingMovie.TrailerUrl = model.TrailerUrl;
            existingMovie.ReleaseDate = model.ReleaseDate;
            existingMovie.EndDate = model.EndDate;

            await _movieService.UpdateMovieAsync(existingMovie);
            TempData["SuccessMessage"] = "Phim đã được cập nhật!";
            return RedirectToAction("Movies");
        }

        [HttpPost]
        public async Task<IActionResult> CreateMovie(Movie model)
        {
            // Xử lý ảnh Poster
            if (model.PosterFile != null)
            {
                var fileName = $"{Guid.NewGuid()}_{model.PosterFile.FileName}";
                var path = Path.Combine(_env.WebRootPath, "uploads/posters", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.PosterFile.CopyToAsync(stream);
                }
                model.PosterUrl = "/uploads/posters/" + fileName;
            }

            // Xử lý ảnh Banner
            if (model.BannerFile != null)
            {
                var fileName = $"{Guid.NewGuid()}_{model.BannerFile.FileName}";
                var path = Path.Combine(_env.WebRootPath, "uploads/banners", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.BannerFile.CopyToAsync(stream);
                }
                model.BannerUrl = "/uploads/banners/" + fileName;
            }


            await _movieService.AddMovieAsync(model);
            TempData["SuccessMessage"] = "Phim được tạo thành công!";
            return RedirectToAction("Movies");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmedMovie(int id)
        {
            await _movieService.DeleteMovieAsync(id); 
            TempData["SuccessMessage"] = "Xóa phim thành công!";
            return RedirectToAction("Movies");
        }

        [HttpPost]
        public async Task<IActionResult> SearchMovie(IFormCollection form, int? page)
        {
            string searchKey = form["searchKeyMovie"];
            string status = form["statusMovie"];
            string format = form["formatMovie"];

            var movies = await _movieService.GetAllMoviesAsync();

            // Lọc theo tên hoặc thể loại
            if (!string.IsNullOrEmpty(searchKey))
            {
                movies = movies.Where(m =>
                    m.Title.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                    m.Genre.Contains(searchKey, StringComparison.OrdinalIgnoreCase));
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                movies = movies.Where(m => m.Status == status);
            }

            // Lọc theo định dạng
            if (!string.IsNullOrEmpty(format))
            {
                movies = movies.Where(m => m.Format == format);
            }

            int pageSize = 5;
            int pageNumber = page ?? 1;

            return View("Movies", movies.ToPagedList(pageNumber, pageSize));
        }

        // Manage Showtimes
        public async Task<IActionResult> Showtimes(int? page)
        {
            var showtimes = await _showtimeService.GetShowtimesAsync();
            var showtimeVMs = new List<ShowtimeViewModel>();

            foreach (var st in showtimes)
            {
                var theater = await _theaterService.GetTheaterByAuditoriumIdAsync(st.AuditoriumId);
                showtimeVMs.Add(new ShowtimeViewModel
                {
                    Showtime = st,
                    TheaterName = theater?.TheaterName ?? "Không tìm thấy rạp"
                });
            }

            int pageSize = 5;
            int pageNumber = page ?? 1;

            return View(showtimeVMs.ToPagedList(pageNumber,pageSize));
        }

        // GET
        public async Task<IActionResult> GetCreateShowtimeModal()
        {
            ViewBag.MovieList = new SelectList(await _movieService.GetAllMoviesAsync(), "MovieId", "Title");
            ViewBag.AuditoriumList = new SelectList(await _auditoriumService.GetAllAuditoriumsAsync(), "AuditoriumId", "AuditoriumName");
            ViewData["Title"] = "Thêm suất chiếu mới";
            ViewData["Action"] = "CreateShowtime";
            return PartialView("Partial/_CreateOrEditModalShowtime", new Showtime());
        }
        public async Task<IActionResult> GetEditShowtimeModal(int id)
        {
            ViewBag.MovieList = new SelectList(await _movieService.GetAllMoviesAsync(), "MovieId", "Title");
            ViewBag.AuditoriumList = new SelectList(await _auditoriumService.GetAllAuditoriumsAsync(), "AuditoriumId", "AuditoriumName");
            var showtime = await _showtimeService.GetShowtimeByIdAsync(id); 
            ViewData["Title"] = "Chỉnh sửa suất chiếu";
            ViewData["Action"] = "EditShowtime";
            return PartialView("Partial/_CreateOrEditModalShowtime", showtime);
        }
        public async Task<IActionResult> GetDeleteShowtimeModal(int id)
        {
            var showtime = await _showtimeService.GetShowtimeByIdAsync(id); 
            return PartialView("Partial/_DeleteConfirmModalShowtime", showtime);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> EditShowtime(Showtime model)
        {
            var existingShowtime = await _showtimeService.GetShowtimeByIdAsync(model.ShowtimeId);
            if (existingShowtime == null) return NotFound();

            existingShowtime.MovieId = model.MovieId;
            existingShowtime.AuditoriumId = model.AuditoriumId;
            existingShowtime.ShowDate = model.ShowDate;
            existingShowtime.ShowTime = model.ShowTime;
            existingShowtime.Status = model.Status;

            await _showtimeService.UpdateShowtimeAsync(existingShowtime);
            TempData["SuccessMessage"] = "Suất chiếu đã được cập nhật!";
            return RedirectToAction("Showtimes");
        }
        [HttpPost]
        public async Task<IActionResult> CreateShowtime(Showtime model)
        {
            model.Auditorium = await _auditoriumService.GetAuditoriumByIdAsync(model.AuditoriumId);
            model.Movie = await _movieService.GetMovieByIdAsync(model.MovieId);

            await _showtimeService.AddShowtimeAsync(model);
            TempData["SuccessMessage"] = "Suất chiếu đã được tạo!";
            return RedirectToAction("Showtimes");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmedShowtime(int id)
        {
            await _showtimeService.DeleteShowtimeAsync(id); 
            TempData["SuccessMessage"] = "Suất chiếu đã được xóa!";
            return RedirectToAction("Showtimes");
        }
        [HttpPost]
        public async Task<IActionResult> SearchShowtime(IFormCollection form, int? page)
        {
            string searchKey = form["searchKeyShowtime"];
            string status = form["statusShowtime"];
            string dateStr = form["dateShowtime"];

            var showtimes = await _showtimeService.GetShowtimesAsync();

            if (!string.IsNullOrEmpty(searchKey))
            {
                showtimes = showtimes.Where(s =>
                    s.Movie.Title.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                    s.Auditorium.Theater.TheaterName.Contains(searchKey, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(status))
            {
                showtimes = showtimes.Where(s => s.Status == status);
            }

            if (!string.IsNullOrEmpty(dateStr) && DateOnly.TryParse(dateStr, out var parsedDate))
            {
                showtimes = showtimes.Where(s => s.ShowDate == parsedDate);
            }

            var showtimeVMs = new List<ShowtimeViewModel>();
            foreach (var st in showtimes)
            {
                var theater = await _theaterService.GetTheaterByAuditoriumIdAsync(st.AuditoriumId);
                showtimeVMs.Add(new ShowtimeViewModel
                {
                    Showtime = st,
                    TheaterName = theater?.TheaterName ?? "Không tìm thấy rạp"
                });
            }

            int pageSize = 5;
            int pageNumber = page ?? 1;

            return View("Showtimes", showtimeVMs.ToPagedList(pageNumber, pageSize));
        }






        // Manage Users
        public async Task<IActionResult> Users(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var users = await _userService.GetAllUsersAsync();
            return View(users.ToPagedList(pageNumber, pageSize));
        }

        // GET
        public async Task<IActionResult> GetEditUserModal(int id)
        {
            var acc = await _userService.GetUserByIdAsync(id); // Sử dụng await để nhận giá trị User
            ViewData["Title"] = "Chỉnh sửa";
            ViewData["Action"] = "EditUser";
            return PartialView("Partial/_CreateOrEditModalAccount", acc);
        }

        public async Task<IActionResult> GetDeleteUserModal(int id)
        {
            var acc = await _userService.GetUserByIdAsync(id); // Sử dụng await để nhận giá trị User
            return PartialView("Partial/_DeleteConfirmModalAccount", acc);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> EditUser(User account)
        {
            var existing = await _userService.GetUserByIdAsync(account.UserId); 
            if (existing == null) return NotFound();

            existing.Role = account.Role;
            await _userService.UpdateUserAsync(existing); 
            TempData["SuccessMessage"] = "Cập nhật vai trò thành công!";
            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmedUser(int id)
        {
            await _userService.DeleteUserAsync(id); 
            TempData["SuccessMessage"] = "Xóa người dùng thành công!";
            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> SearchUser(IFormCollection form, int? page)
        {
            string searchKey = form["searchKeyUser"];
            string role = form["role"];

            var allUsers = await _userService.GetAllUsersAsync();

            var filtered = allUsers.Where(u =>
                (string.IsNullOrEmpty(searchKey) ||
                 u.FullName.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                 u.Email.Contains(searchKey, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(role) || u.Role == role));

            int pageSize = 5;
            int pageNumber = page ?? 1;
            var pagedUsers = filtered.ToPagedList(pageNumber, pageSize);

            return View("Users", pagedUsers);
        }

        // Manage Theaters
        public async Task<IActionResult> Theaters(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var theaters = await _theaterService.GetAllTheatersAsync();
            return View(theaters.ToPagedList(pageNumber, pageSize));
        }

        // GET
        public IActionResult GetCreateTheaterModal()
        {
            ViewData["Title"] = "Thêm rạp chiếu phim mới";
            ViewData["Action"] = "CreateTheater";
            return PartialView("Partial/_CreateOrEditModalTheater", new Theater());
        }

        public async Task<IActionResult> GetEditTheaterModal(int id)
        {
            var theater = await _theaterService.GetTheaterByIdAsync(id); 
            ViewData["Title"] = "Chỉnh sửa";
            ViewData["Action"] = "EditTheater";
            return PartialView("Partial/_CreateOrEditModalTheater", theater);
        }

        public async Task<IActionResult> GetDeleteTheaterModal(int id)
        {
            var theater = await _theaterService.GetTheaterByIdAsync(id); 
            return PartialView("Partial/_DeleteConfirmModalTheater", theater);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> EditTheater(Theater model)
        {
            var existingTheater = await _theaterService.GetTheaterByIdAsync(model.TheaterId);
            if (existingTheater == null) return NotFound();
            existingTheater.MapUrl = model.MapUrl;
            existingTheater.TheaterName = model.TheaterName;
            existingTheater.TotalTheaters = model.TotalTheaters;
            existingTheater.Location = model.Location;
            await _theaterService.UpdateTheaterAsync(existingTheater);
            TempData["SuccessMessage"] = "Rạp đã được cập nhật!";
            return RedirectToAction("Theaters");
        }

        [HttpPost]
        public async Task<IActionResult> CreateTheater(Theater model)
        {
            model.CreatedAt = DateTime.Now;
            if (ModelState.IsValid)
            {
                await _theaterService.AddTheaterAsync(model);
                TempData["SuccessMessage"] = "Rạp mới đã được tạo!";
                return RedirectToAction("Theaters");
            }
            return View("Theaters", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmedTheater(int id)
        {
            await _theaterService.DeleteTheaterAsync(id); 
            TempData["SuccessMessage"] = "Xóa rạp thành công!";
            return RedirectToAction("Theaters");
        }
        [HttpPost]
        public async Task<IActionResult> SearchTheater(IFormCollection form, int? page)
        {
            string searchKey = form["searchKeyTheater"];

            var theaters = await _theaterService.GetAllTheatersAsync();

            // Lọc theo tên rạp hoặc địa chỉ (không phân biệt hoa thường)
            if (!string.IsNullOrEmpty(searchKey))
            {
                searchKey = searchKey.ToLower();
                theaters = theaters.Where(t =>
                    t.TheaterName.ToLower().Contains(searchKey) ||
                    t.Location.ToLower().Contains(searchKey)).ToList();
            }

            int pageSize = 5;
            int pageNumber = page ?? 1;

            return View("Theaters", theaters.ToPagedList(pageNumber, pageSize));
        }

    }
}
