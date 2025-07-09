using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Service;
using X.PagedList.Extensions;

namespace ChickenFlickFilmApplication.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITheaterService _theaterService;
        private readonly IMovieService _movieService;
        private readonly IWebHostEnvironment _env;

        public AdminController(IUserService userService, ITheaterService theaterService, IMovieService movieService, IWebHostEnvironment env)
        {
            _userService = userService;
            _theaterService = theaterService;
            _movieService = movieService;
            _env = env;
        }
        public IActionResult Auditoriums()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
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


            await _movieService.UpdateMovieAsync(model); 
            TempData["SuccessMessage"] = "Movie updated!";
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
            TempData["SuccessMessage"] = "Movie was created!";
            return RedirectToAction("Movies");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmedMovie(int id)
        {
            await _movieService.DeleteMovieAsync(id); 
            TempData["SuccessMessage"] = "Movie deleted successfully!";
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
        public IActionResult Showtimes()
        {
            return View();
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
            TempData["SuccessMessage"] = "Account updated successfully!";
            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmedUser(int id)
        {
            await _userService.DeleteUserAsync(id); 
            TempData["SuccessMessage"] = "Account deleted successfully!";
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
    }
}
