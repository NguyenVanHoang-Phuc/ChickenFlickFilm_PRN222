using BusinessObjects.Models;
using ChickenFlickFilmApplication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
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
        private readonly IBookingService bookingService;
        private readonly IShowtimeService showtimeService;
        private readonly IPaymentService paymentService;
        public AuthController(IUserService userService, IEmailSender emailSender, IMemoryCache cache, IBookingService bookingService, IShowtimeService showtimeService, IPaymentService paymentService)
        {
            _userService = userService;
            _emailSender = emailSender;
            _cache = cache;
            this.bookingService = bookingService;
            this.showtimeService = showtimeService;
            this.paymentService = paymentService;
        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View(new RegisterViewModel());
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
                ModelState.AddModelError("Email", "Email này đã được sử dụng.");
                return View(model); // Return view with model, not redirect
            }

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Birthday = model.DateOfBirth,
                Gender = model.Gender,
                CreatedAt = DateTime.UtcNow,
                IsConfirmed = false,
                Avatar = "/image/default-avatar.png",
                PhoneNumber = model.PhoneNumber,
                Role = "Customer",
            };

            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, model.Password);

            try
            {
                await _userService.AddUserAsync(user);
                var code = $"CF-" + DateTime.Now.Ticks;
                await SenConfirmationEmailAsync(user.Email, code, user.FullName);
                return RedirectToAction("ConfirmEmail", new { email = user.Email, code });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi đăng ký. Vui lòng thử lại.");
                return View(model);
            }
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
            return View(model);
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
            if (model.Code.Equals(model.CodeEnter, StringComparison.OrdinalIgnoreCase))
            {
                user.IsConfirmed = true;
                ViewBag.Message = "Xác thực thành công";
                await _userService.UpdateUserAsync(user);
            }
            else
            {
                ViewBag.Error = "Mã xác thực không chính xác. Vui lòng kiểm tra lại mã bạn đã nhập.";
            }
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
                new(ClaimTypes.Gender, user.Gender.ToString()),
                new(ClaimTypes.Role, user.Role)
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

            if(user.Role == "Admin")
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            if (user.Role == "FilmManager")
            {
                return RedirectToAction("Movies", "Admin");
            }

            return RedirectToAction("Index", "Home");
        }

        // Google Authentication Challenge
        [HttpPost]
        public IActionResult GoogleLogin(string? returnUrl = null)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = returnUrl ?? Url.Action("Index", "Home")
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
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

        [HttpGet]
        public async Task<IActionResult> UserProfileAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetAsync(u => u.UserId.ToString() == userId);
            if (user != null)
            {
                List<Booking> bookings = bookingService.GetAllBookingByUserId(int.Parse(userId));
                Payment payment = new Payment();
                foreach (Booking booking in bookings)
                {
                    Showtime showtime = await showtimeService.GetShowtimeByIdAsync(booking.ShowtimeId);
                    payment = paymentService.getPaymentByBookingid(booking.BookingId);
                    if (showtime != null && payment != null)
                    {
                        booking.Showtime = showtime;
                        booking.Payment = payment;
                    } else
                    {
                        return NotFound();
                    }
                }
                var model = new UserProfileViewModel
                {
                    DateOfBirth = user.Birthday,
                    Email = user.Email,
                    FullName = user.FullName,
                    Gender = user.Gender ? "Male" : "Female",
                    PhoneNumber = user.PhoneNumber,
                    TotalSpending = 0,
                    bookings = bookings,
                };
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetAsync(u => u.UserId.ToString() == userId);

            if (user == null)
            {
                return NotFound();
            }

            var existPhone = await _userService.GetAsync(u => u.PhoneNumber.Equals(model.PhoneNumber));

            if (existPhone != null && user.UserId != existPhone.UserId)
            {
                TempData["Error"] = "Số điện thoại đã được sử dụng bởi người dùng khác.";
                return RedirectToAction("UserProfile");
            }
            else
            {
                user.PhoneNumber = model.PhoneNumber;
            }

            // Update user properties
            user.FullName = model.FullName;
            user.Birthday = model.DateOfBirth;
            user.Gender = model.Gender == "Male";

            try
            {
                await _userService.UpdateUserAsync(user);
                TempData["Success"] = "Thông tin cá nhân đã được cập nhật thành công.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Đã xảy ra lỗi khi cập nhật thông tin. Vui lòng thử lại.";
            }

            return RedirectToAction("UserProfile");
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