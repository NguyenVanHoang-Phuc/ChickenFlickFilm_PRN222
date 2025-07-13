namespace ChickenFlickFilmApplication.Controllers
{
    using System.Net;
    using System.Net.Mail;
    using System.Text;

    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var smtpClient = new SmtpClient(_config["Email:SmtpServer"])
            {
                Port = int.Parse(_config["Email:Port"]),
                Credentials = new NetworkCredential(_config["Email:Username"], _config["Email:Password"]),
                EnableSsl = true,
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_config["Email:From"]),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };

            mail.To.Add(toEmail);
            await smtpClient.SendMailAsync(mail);
        }

        public async Task SendInvitationEmailAsync(string toEmail, string recipientName, string eventTitle,
            string eventDate, string eventTime, string eventLocation, string personalMessage = "",
            string rsvpLink = "")
        {
            var subject = $"You're Invited: {eventTitle}";
            var htmlMessage = CreateInvitationHtml(recipientName, eventTitle, eventDate, eventTime,
                eventLocation, personalMessage, rsvpLink);

            await SendEmailAsync(toEmail, subject, htmlMessage);
        }

        private string CreateInvitationHtml(string recipientName, string eventTitle, string eventDate,
            string eventTime, string eventLocation, string personalMessage, string rsvpLink)
        {
            var html = new StringBuilder();

            html.Append(@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Invitation</title>
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f5f5f5;
            color: #333;
        }
        .container {
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        }
        .header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 40px 30px;
            text-align: center;
        }
        .header h1 {
            margin: 0;
            font-size: 28px;
            font-weight: 300;
            letter-spacing: 1px;
        }
        .content {
            padding: 40px 30px;
        }
        .greeting {
            font-size: 18px;
            margin-bottom: 20px;
            color: #2c3e50;
        }
        .event-title {
            font-size: 24px;
            font-weight: bold;
            color: #667eea;
            margin: 20px 0;
            text-align: center;
            padding: 20px;
            background-color: #f8f9ff;
            border-radius: 8px;
            border-left: 4px solid #667eea;
        }
        .event-details {
            background-color: #f8f9fa;
            padding: 25px;
            border-radius: 8px;
            margin: 20px 0;
        }
        .detail-item {
            margin: 12px 0;
            display: flex;
            align-items: center;
        }
        .detail-icon {
            width: 20px;
            height: 20px;
            margin-right: 12px;
            font-size: 16px;
        }
        .detail-label {
            font-weight: bold;
            color: #2c3e50;
            min-width: 80px;
        }
        .detail-value {
            color: #34495e;
        }
        .personal-message {
            background-color: #fff3cd;
            border: 1px solid #ffeaa7;
            border-radius: 8px;
            padding: 20px;
            margin: 20px 0;
            font-style: italic;
            color: #856404;
        }
        .rsvp-section {
            text-align: center;
            margin: 30px 0;
        }
        .rsvp-button {
            display: inline-block;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 15px 30px;
            text-decoration: none;
            border-radius: 25px;
            font-weight: bold;
            font-size: 16px;
            transition: transform 0.2s;
        }
        .rsvp-button:hover {
            transform: translateY(-2px);
        }
        .footer {
            background-color: #2c3e50;
            color: white;
            padding: 20px 30px;
            text-align: center;
            font-size: 14px;
        }
        .divider {
            height: 2px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            margin: 20px 0;
            border-radius: 1px;
        }
        @media (max-width: 600px) {
            .container {
                margin: 0 10px;
            }
            .header, .content {
                padding: 20px;
            }
            .event-title {
                font-size: 20px;
            }
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎬 You're Invited! 🎬</h1>
        </div>
        
        <div class='content'>
            <div class='greeting'>
                Hello " + recipientName + @",
            </div>
            
            <div class='event-title'>
                " + eventTitle + @"
            </div>
            
            <div class='divider'></div>
            
            <div class='event-details'>
                <div class='detail-item'>
                    <span class='detail-icon'>📅</span>
                    <span class='detail-label'>Date:</span>
                    <span class='detail-value'>" + eventDate + @"</span>
                </div>
                <div class='detail-item'>
                    <span class='detail-icon'>🕒</span>
                    <span class='detail-label'>Time:</span>
                    <span class='detail-value'>" + eventTime + @"</span>
                </div>
                <div class='detail-item'>
                    <span class='detail-icon'>📍</span>
                    <span class='detail-label'>Location:</span>
                    <span class='detail-value'>" + eventLocation + @"</span>
                </div>
            </div>");

            if (!string.IsNullOrEmpty(personalMessage))
            {
                html.Append(@"
            <div class='personal-message'>
                <strong>Personal Message:</strong><br>
                " + personalMessage + @"
            </div>");
            }

            if (!string.IsNullOrEmpty(rsvpLink))
            {
                html.Append(@"
            <div class='rsvp-section'>
                <a href='" + rsvpLink + @"' class='rsvp-button'>RSVP Now</a>
            </div>");
            }

            html.Append(@"
            <div class='divider'></div>
            
            <p style='text-align: center; color: #7f8c8d; font-size: 14px;'>
                We can't wait to see you there! 🎉
            </p>
        </div>
        
        <div class='footer'>
            <p>This invitation was sent by ChickenFlick Film Application</p>
            <p style='font-size: 12px; margin-top: 10px;'>
                If you have any questions, please contact us.
            </p>
        </div>
    </div>
</body>
</html>");

            return html.ToString();
        }

        // Alternative method for custom HTML content
        public async Task SendCustomInvitationEmailAsync(string toEmail, string subject,
            string customHtmlContent)
        {
            await SendEmailAsync(toEmail, subject, customHtmlContent);
        }
    }
}