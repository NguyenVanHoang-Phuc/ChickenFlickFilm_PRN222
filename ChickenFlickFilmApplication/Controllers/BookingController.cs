using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Service;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace ChickenFlickFilmApplication.Controllers
{
    [Authorize(Roles = "Customer")]

    public class BookingController : Controller
    {
        private readonly IBookingService bookingService;
        private readonly ISeatBookingService seatBookingService;
        private readonly ILogger<BookingController> _logger;

        // Constructor injection for IBookingService and ISeatBookingService
        public BookingController(IBookingService bookingService, ISeatBookingService seatBookingService, ILogger<BookingController> logger)
        {
            this.bookingService = bookingService;
            this.seatBookingService = seatBookingService;
            _logger = logger;
        }

        public IBookingService GetBookingService()
        {
            return bookingService;
        }

        public async Task<IActionResult> CreateBookingAsync(List<int> ListSeat, decimal total, string Name, string OrderDescription, string OrderType, int ShowtimeId, int UserId)
        {
            // Validate inputs
            if (ShowtimeId < 1 || UserId < 1)
            {
                return Content("Invalid Showtime or User ID.");
            }

            if (ListSeat == null || !ListSeat.Any())
            {
                return Content("No seats selected.");
            }

            try
            {
                _logger.LogInformation($"Creating booking for ShowtimeId: {ShowtimeId}, UserId: {UserId}");

                var booking = new Booking
                {
                    BookingStatus = "Pending",
                    ShowtimeId = ShowtimeId,
                    UserId = UserId,
                    CreatedAt = DateTime.Now
                };

                // Add booking asynchronously
                int bookingId = await bookingService.AddBookingAsync(booking);

                // Build seat bookings and add them
                var seatBookings = buildSeatBooking(ListSeat, bookingId);
                await seatBookingService.AddSeatBookingAsync(seatBookings);

                // Log successful booking creation
                _logger.LogInformation($"Booking created successfully. BookingId: {bookingId}");

                // Redirect to payment action with the bookingId
                return RedirectToAction("CreatePaymentUrlVnpay", "Payment", new
                {
                    Amount = total,
                    Name = Name,
                    OrderDescription = OrderDescription,
                    OrderType = OrderType,
                    BookingId = bookingId
                });
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "An error occurred while creating the booking.");
                return Content("An error occurred while processing your request.");
            }
        }

        private List<SeatBooking> buildSeatBooking(List<int> ListSeat, int bookingId)
        {
            List<SeatBooking> seatBookings = new List<SeatBooking>();

            foreach (var seatId in ListSeat)
            {
                var seatBooking = new SeatBooking
                {
                    BookingId = bookingId,
                    SeatId = seatId
                };

                seatBookings.Add(seatBooking);
            }

            return seatBookings;
        }
    }
}
