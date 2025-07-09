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

        public AdminController(IUserService userService, ITheaterService theaterService)
        {
            _userService = userService;
            _theaterService = theaterService;
        }
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
            string searchKey = form["searchKey"];
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
