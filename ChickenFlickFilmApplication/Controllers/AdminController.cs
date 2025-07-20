using BusinessObjects.Models;
using ChickenFlickFilmApplication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service;
using System.Threading.Tasks;
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
        // Dashboard
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

        // Auditoriums
        public async Task<IActionResult> Auditoriums(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var auditoriuums = await _auditoriumService.GetAllAuditoriumsAsync();
            return View(auditoriuums.ToPagedList(pageNumber, pageSize));
        }
        // GET
        public IActionResult GetCreateAuditoriumModal()
        {
            ViewBag.Theaters = new SelectList(_theaterService.GetAllTheatersAsync().Result, "TheaterId", "TheaterName");
            ViewData["Title"] = "Thêm phòng chiếu mới";
            ViewData["Action"] = "CreateAuditorium";
            return PartialView("Partial/_CreateOrEditModalAuditorium", new Auditorium());
        }
        public async Task<IActionResult> GetEditAuditoriumModal(int id)
        {
            ViewBag.Theaters = new SelectList(await _theaterService.GetAllTheatersAsync(), "TheaterId", "TheaterName");
            var auditorium = await _auditoriumService.GetAuditoriumByIdAsync(id); 
            ViewData["Title"] = "Chỉnh sửa";
            ViewData["Action"] = "EditAuditorium";
            return PartialView("Partial/_CreateOrEditModalAuditorium", auditorium);
        }
        public async Task<IActionResult> GetDeleteAuditoriumModal(int id)
        {
            var auditorium = await _auditoriumService.GetAuditoriumByIdAsync(id); 
            return PartialView("Partial/_DeleteConfirmModalAuditorium", auditorium);
        }
        // POST
        [HttpPost]
        public async Task<IActionResult> EditAuditorium(Auditorium model)
        {
            var existingAuditorium = await _auditoriumService.GetAuditoriumByIdAsync(model.AuditoriumId);
            if (existingAuditorium == null) return NotFound();

            existingAuditorium.AuditoriumName = model.AuditoriumName;
            existingAuditorium.AuditoriumType = model.AuditoriumType;
            existingAuditorium.RowNumber = model.RowNumber;
            existingAuditorium.ColumnNumber = model.ColumnNumber;
            existingAuditorium.TotalSeats = model.RowNumber * model.ColumnNumber;
            existingAuditorium.TheaterId = model.TheaterId;

            await _auditoriumService.UpdateAuditoriumAsync(existingAuditorium);
            TempData["SuccessMessage"] = "Phòng chiếu đã được cập nhật!";
            return RedirectToAction("Auditoriums");
        }
        [HttpPost]
        public async Task<IActionResult> CreateAuditorium(Auditorium model)
        {
            model.CreatedAt = DateTime.Now;
            model.TotalSeats = model.RowNumber * model.ColumnNumber;
            
            await _auditoriumService.AddAuditoriumAsync(model);
            TempData["SuccessMessage"] = "Phòng chiếu mới đã được tạo!";
            return RedirectToAction("Auditoriums");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmedAuditorium(int id)
        {
            await _auditoriumService.DeleteAuditoriumAsync(id); 
            TempData["SuccessMessage"] = "Xóa phòng chiếu thành công!";
            return RedirectToAction("Auditoriums");
        }
        [HttpPost]
        public async Task<IActionResult> SearchAuditorium(IFormCollection form, int? page)
        {
            string searchKey = form["searchKeyAuditorium"];
            string format = form["formatAuditorium"];

            var auditoriums = await _auditoriumService.GetAllAuditoriumsAsync(); 

            // Lọc theo từ khóa tìm kiếm (tên phòng hoặc tên rạp)
            if (!string.IsNullOrEmpty(searchKey))
            {
                auditoriums = auditoriums.Where(a =>
                    a.AuditoriumName.Contains(searchKey, StringComparison.OrdinalIgnoreCase) ||
                    a.Theater.TheaterName.Contains(searchKey, StringComparison.OrdinalIgnoreCase));
            }

            // Lọc theo loại phòng (2D, 3D, IMAX)
            if (!string.IsNullOrEmpty(format))
            {
                auditoriums = auditoriums.Where(a =>
                    a.AuditoriumType != null &&
                    a.AuditoriumType.Contains(format, StringComparison.OrdinalIgnoreCase));
            }

            int pageSize = 5;
            int pageNumber = page ?? 1;

            return View("Auditoriums", auditoriums.ToPagedList(pageNumber, pageSize));
        }



        // Manage Movies
        public async Task<IActionResult> Movies(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            var movies = (await _movieService.GetAllMoviesAsync())
                    .OrderByDescending(m => m.ReleaseDate)
                    .ToList();
            var today = DateOnly.FromDateTime(DateTime.Now);

            foreach (var movie in movies)
            {
                if (movie.ReleaseDate <= today && movie.EndDate >= today)
                {
                    movie.Status = "Đang chiếu";
                }
                else if (movie.ReleaseDate > today)
                {
                    movie.Status = "Sắp chiếu";
                }
                else if (movie.EndDate < today)
                {
                    movie.Status = "Đã kết thúc";
                }

                await _movieService.UpdateMovieAsync(movie); 
            }


            return View(movies.ToPagedList(pageNumber, pageSize));
        }

        // GET
        public async Task<IActionResult> GetCreateMovieModal()
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
            ViewBag.MovieListRaw = await _movieService.GetMoviesByEnableStatusAsync();
            var now = DateTime.Now;

            var showtimes = (await _showtimeService.GetShowtimesAsync())
                .OrderByDescending(s => s.ShowDate)
                .ThenByDescending(s => s.ShowTime)
                .ToList();

            var showtimeVMs = new List<ShowtimeViewModel>();

            foreach (var st in showtimes)
            {
                var movie = await _movieService.GetMovieByIdAsync(st.MovieId);

                var showDateTime = (st.ShowDate ?? default).ToDateTime(st.ShowTime);
                var endDateTime = showDateTime.AddMinutes(movie?.Duration ?? 0);

                // Xác định status mới
                string newStatus = string.Empty;

                if (showDateTime > now)
                    newStatus = "Sắp chiếu";
                else if (now >= showDateTime && now < endDateTime)
                    newStatus = "Đang chiếu";
                else
                    newStatus = "Đã chiếu";

                // Chỉ cập nhật nếu khác với status cũ để tránh lỗi constraint
                if (st.Status != newStatus)
                {
                    st.Status = newStatus;
                    await _showtimeService.UpdateShowtimeAsync(st);
                }

                var theater = await _theaterService.GetTheaterByAuditoriumIdAsync(st.AuditoriumId);

                showtimeVMs.Add(new ShowtimeViewModel
                {
                    Showtime = st,
                    TheaterName = theater?.TheaterName ?? "Không tìm thấy rạp"
                });
            }

            int pageSize = 5;
            int pageNumber = page ?? 1;

            return View(showtimeVMs.ToPagedList(pageNumber, pageSize));
        }


        // GET
        public async Task<IActionResult> GetCreateShowtimeModal()
        {
            var movieList = await _movieService.GetMoviesByEnableStatusAsync();  
            ViewBag.MovieRaw = movieList;

            var auditoriumList = await _auditoriumService.GetAllAuditoriumsAsync();
            ViewBag.AuditoriumList = new SelectList(auditoriumList, "AuditoriumId", "AuditoriumName");

            ViewBag.MovieList = new SelectList(movieList, "MovieId", "Title");
            ViewData["Title"] = "Thêm suất chiếu mới";
            ViewData["Action"] = "CreateShowtime";
            return PartialView("Partial/_CreateOrEditModalShowtime", new Showtime());
        }
        public async Task<IActionResult> GetEditShowtimeModal(int id)
        {
            var movieList = await _movieService.GetMoviesByEnableStatusAsync();  
            ViewBag.MovieRaw = movieList;
       
            var auditoriumList = await _auditoriumService.GetAllAuditoriumsAsync();
            ViewBag.AuditoriumList = new SelectList(auditoriumList, "AuditoriumId", "AuditoriumName");

            ViewBag.MovieList = new SelectList(movieList, "MovieId", "Title");
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
            ViewData["Title"] = "Chỉnh sửa vai trò";
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
