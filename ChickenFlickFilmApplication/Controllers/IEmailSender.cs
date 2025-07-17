namespace ChickenFlickFilmApplication.Controllers
{
    public interface IEmailSender
    {
        Task SendConfirmationEmailAsync(string v1, string v2, string v3, string v4, string v5, string v6);
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
        Task SendForgotPasswordEmailAsync(string email, string? fullName, string? resetUrl);
    }
}
