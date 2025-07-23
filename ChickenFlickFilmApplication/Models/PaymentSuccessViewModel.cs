namespace ChickenFlickFilmApplication.Models
{
    public class PaymentSuccessViewModel
    {
        public string MovieNameTicket { get; set; }
        public string TheaterNameTicket { get; set; }
        public string ShowtimeTicket { get; set; }
        public string AuditoriumTicket { get; set; }
        public List<string> SeatNumbers { get; set; }
        public decimal Amount { get; set; }
    }
}
