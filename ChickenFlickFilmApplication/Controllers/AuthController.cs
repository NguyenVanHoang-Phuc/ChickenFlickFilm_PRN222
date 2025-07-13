using BusinessObjects.Models;
using ChickenFlickFilmApplication.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ChickenFlickFilmApplication.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        public AuthController(IUserService userService, IEmailSender emailSender)
        {
            _userService = userService;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignupAsync(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = await _userService.GetAsync(u => u.Email == model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("", "Username or email is already taken.");
                return View(model);
            }

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Birthday = model.DateOfBirth,
                Gender = model.Gender,
                CreatedAt = DateTime.UtcNow
            };

            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, model.Password);

            await _userService.AddUserAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Gender, user.Gender.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userService.GetAsync(u => u.Email == model.Email);

            if (user == null)
            {
                ViewBag.Error = "Email không tồn tại.";
                return View(model);
            }

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

            if (result != PasswordVerificationResult.Success)
            {
                ViewBag.Error = "Sai mật khẩu.";
                return View(model);
            }

            // Prepare claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Gender, user.Gender.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Define auth properties
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe, // <- This is what makes it "remember"
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(1)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);

            return RedirectToAction("Index, Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult ForgotPassword()
        {
            //// Generate a confirmation token (or use a Guid/Random Code)
            //var confirmationCode = Guid.NewGuid().ToString();
            //user.ConfirmationCode = confirmationCode;
            //user.IsConfirmed = false;
            //await _userService.AddUserAsync(user); // Save user with token

            //// Build confirmation link
            //var callbackUrl = Url.Action("ConfirmEmail", "Auth", new { email = user.Email, code = confirmationCode }, Request.Scheme);

            //// Send email
            //await _emailSender.SendEmailAsync(user.Email, "Xác nhận email",
            //    $"Vui lòng xác nhận tài khoản bằng cách <a href='{callbackUrl}'>nhấn vào đây</a>.");
            return View();
        }
        //public async Task<IActionResult> ConfirmEmail(string email, string code)
        //{
        //    var user = await _userService.GetAsync(u => u.Email == email && u.ConfirmationCode == code);
        //    if (user == null)
        //    {
        //        return NotFound("Mã xác nhận không hợp lệ.");
        //    }

        //    user.IsConfirmed = true;
        //    user.ConfirmationCode = null; // Optional: clear the token
        //    await _userService.UpdateUserAsync(user);

        //    return View("EmailConfirmed"); // Create a success page
        //}

        public IActionResult UserProfile()
        {
            return View();
        }
    }
}
