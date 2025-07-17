using BusinessObjects.Models;
using ChickenFlickFilmApplication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Service;
using System.Security.Claims;

namespace ChickenFlickFilmApplication.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        private readonly IMemoryCache _cache;

        public AuthController(IUserService userService, IEmailSender emailSender, IMemoryCache cache)
        {
            _userService = userService;
            _emailSender = emailSender;
            _cache = cache;
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
                return RedirectToAction("Signup");
            }

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Birthday = model.DateOfBirth,
                Gender = model.Gender,
                CreatedAt = DateTime.UtcNow,
                IsConfirmed = false,
                Avatar = "/image/avatar.png",
                PhoneNumber = "0000000000"
            };

            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, model.Password);

            await _userService.AddUserAsync(user);

            var code = $"CF-" + DateTime.Now.Ticks;

            await SenConfirmationEmailAsync(user.Email, code, user.FullName);

            return RedirectToAction("ConfirmEmail", new { email = user.Email, code });
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmailAsync(string email, string code)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            {
                return NotFound("Invalid confirmation link.");
            }

            var user = await _userService.GetAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound("User not found.");
            }
            var model = new ConfirmEmailViewModel
            {
                Code = code,
                Email = email
            };
            return RedirectToAction("ConfirmEmail", model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmailAsync(ConfirmEmailViewModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.CodeEnter) || string.IsNullOrEmpty(model.Code))
            {
                return NotFound("Invalid confirmation link.");
            }

            var user = await _userService.GetAsync(u => u.Email.Equals(model.Email));

            if (user == null)
            {
                return NotFound("User not found.");
            }
            if (!model.Code.Equals(model.CodeEnter, StringComparison.OrdinalIgnoreCase))
            {
                user.IsConfirmed = true;
            }
            await _userService.UpdateUserAsync(user);

            return RedirectToAction("Login");
        }
        [HttpGet]
        public async Task<IActionResult> ResendConfirmationEmailAsync(string email)
        {
            var user = await _userService.GetAsync(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Error = "Không tìm thấy người dùng với email này.";
                return View("Login");
            }

            var code = $"CF-" + DateTime.Now.Ticks;
            await SenConfirmationEmailAsync(user.Email, code, user.FullName);

            return RedirectToAction("ConfirmEmailAsync", new { email, code });
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userService.GetAsync(u => u.Email == model.Email);

            if (user == null)
            {
                ViewBag.Error = "Email không tồn tại.";
                return View(model);
            }

            if (user.IsConfirmed == false)
            {
                ViewBag.Error = "Tài khoản chưa được xác nhận. Vui lòng kiểm tra email để xác nhận tài khoản của bạn.";
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
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Gender, user.Gender.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Define auth properties
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(2) : DateTimeOffset.UtcNow.AddHours(1)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home"); // Redirect if already logged in
            }

            var model = new ForgotPasswordViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userService.GetAsync(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Error = "Không tìm thấy người dùng";
                return View("ForgotPassword"); // Show error on the same view
            }

            // Generate reset token (simple version using Guid)
            var resetToken = Guid.NewGuid().ToString();
            _cache.Set($"reset_{user.Email}", resetToken, TimeSpan.FromMinutes(15)); // store token for 15 mins

            var resetUrl = Url.Action("ResetPassword", "Auth", new { email = user.Email, code = resetToken }, Request.Scheme);
            await _emailSender.SendForgotPasswordEmailAsync(user.Email, user.FullName, resetUrl);

            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string email, string code)
        {
            var user = await _userService.GetAsync(u => u.Email == email);

            var valid = _cache.TryGetValue($"reset_{email}", out string cachedToken);
            if (!valid || cachedToken != code)
            {
                return NotFound("Liên kết không hợp lệ hoặc đã hết hạn.");
            }

            var model = new ResetPasswordViewModel
            {
                Email = email,
                Code = code
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {

            var user = await _userService.GetAsync(u => u.Email == model.Email);

            var valid = _cache.TryGetValue($"reset_{model.Email}", out string cachedToken);
            if (!valid || cachedToken != model.Code)
            {
                return NotFound("Liên kết không hợp lệ hoặc đã hết hạn.");
            }

            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, model.NewPassword); // Apply hashing logic
            await _userService.UpdateUserAsync(user);

            TempData["Success"] = "Mật khẩu đã được thay đổi thành công.";
            _cache.Remove($"reset_{model.Email}");
            return View("Login");
        }

        public IActionResult UserProfile()
        {
            return View();
        }
        private async Task SenConfirmationEmailAsync(string email, string code, string fullname)
        {
            await _emailSender.SendConfirmationEmailAsync(
                $"{email}",
                $"{fullname}",
                "Account Registration",
                "Your account has been successfully created, activate your account now.",
                code,
                "Please log in to your account to complete your profile setup."
            );
        }
    }
}
