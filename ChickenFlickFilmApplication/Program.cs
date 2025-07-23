using BusinessObjects.Models;
using ChickenFlickFilmApplication.Controllers;
using ChickenFlickFilmApplication.Services.VnPay;
using DataAccess;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Repository;
using Service;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<UserDAO>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<MovieDAO>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieService, MovieService>();

builder.Services.AddScoped<TheaterDAO>();
builder.Services.AddScoped<ITheaterRepository, TheaterRepository>();
builder.Services.AddScoped<ITheaterService, TheaterService>();

builder.Services.AddScoped<ShowtimeDAO>();
builder.Services.AddScoped<IShowtimeRepository, ShowtimeRepository>();
builder.Services.AddScoped<IShowtimeService, ShowtimeService>();

builder.Services.AddScoped<BookingDAO>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddScoped<AuditoriumDAO>();
builder.Services.AddScoped<IAuditoriumRepository, AuditoriumRepository>();
builder.Services.AddScoped<IAuditoriumService, AuditoriumService>();

builder.Services.AddScoped<SeatDAO>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<ISeatService, SeatService>();

builder.Services.AddScoped<PaymentDAO>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddScoped<SeatBookingDAO>();
builder.Services.AddScoped<ISeatBookingRepository, SeatBookingRepository>();
builder.Services.AddScoped<ISeatBookingService, SeatBookingService>();

builder.Services.AddScoped<CommentDAO>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();

builder.Services.AddScoped<PriceByTypeDAO>();
builder.Services.AddScoped<IPriceByTypeRepository, PriceByTypeRepository>();
builder.Services.AddScoped<IPriceByTypeService, PriceByTypeService>();

//Connect VNPay API
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 7108;
});

// Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.CallbackPath = "/signin-google";
    options.SaveTokens = true; // Add this to save Google tokens

    options.Events.OnCreatingTicket = async context =>
    {
        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();

        var email = context.Principal.FindFirst(ClaimTypes.Email)?.Value;
        var fullName = context.Principal.FindFirst(ClaimTypes.Name)?.Value;
        var googleId = context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var picture = context.Principal.FindFirst("picture")?.Value;

        if (!string.IsNullOrEmpty(email))
        {
            var existingUser = await userService.GetAsync(u => u.Email == email);
            User userToSignIn;

            if (existingUser == null)
            {
                // Create new user
                var newUser = new User
                {
                    FullName = fullName ?? "Google User",
                    Email = email,
                    Avatar = picture ?? "/image/default-avatar.png",
                    Birthday = DateTime.UtcNow.AddYears(-18), // Default age
                    Gender = true, // Default to male
                    IsConfirmed = true, // Google users are pre-confirmed
                    CreatedAt = DateTime.UtcNow,
                    PhoneNumber = "", // Empty by default
                    Role = "Customer",
                    Password = "" // Google users don't need password
                };

                await userService.AddUserAsync(newUser);
                userToSignIn = newUser;
            }
            else
            {
                userToSignIn = existingUser;

                // Optionally update avatar if it's still default
                if (existingUser.Avatar == "/image/default-avatar.png" && !string.IsNullOrEmpty(picture))
                {
                    existingUser.Avatar = picture;
                    await userService.UpdateUserAsync(existingUser);
                }
            }

            // Add custom claims to the identity
            var identity = (ClaimsIdentity)context.Principal.Identity;

            // Clear existing claims and add our custom ones
            identity.RemoveClaim(identity.FindFirst(ClaimTypes.NameIdentifier));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userToSignIn.UserId.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Role, userToSignIn.Role));
            identity.AddClaim(new Claim(ClaimTypes.Gender, userToSignIn.Gender.ToString()));
        }
    };
});

builder.Services.AddMemoryCache();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(2);
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseCookiePolicy();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
