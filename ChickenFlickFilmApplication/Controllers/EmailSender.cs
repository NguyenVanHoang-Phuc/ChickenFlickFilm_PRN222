using System.Net;
using System.Net.Mail;
using System.Text;

namespace ChickenFlickFilmApplication.Controllers
{

    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var server = _config["Email:SmtpServer"];
            var port = _config["Email:Port"];
            var from = _config["Email:From"];
            var smtpClient = new SmtpClient(server)
            {
                Port = int.Parse(_config["Email:Port"]),
                Credentials = new NetworkCredential(
                    _config["Email:Username"],
                    _config["Email:Password"]
                ),
                EnableSsl = true,
                UseDefaultCredentials = false,
                Timeout = 10000 // 10 seconds timeout
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_config["Email:From"]),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
            smtpClient.Dispose();
        }

        public async Task SendForgotPasswordEmailAsync(string toEmail, string recipientName, string resetLink)
        {
            var subject = "Password Reset Request";
            var htmlMessage = CreateForgotPasswordHtml(recipientName, resetLink);

            await SendEmailAsync(toEmail, subject, htmlMessage);
        }

        private string CreateForgotPasswordHtml(string recipientName, string resetLink)
        {
            var html = new StringBuilder();

            html.Append(@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Password Reset</title>
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f9f9f9;
            color: #333;
            margin: 0;
            padding: 0;
        }
        .container {
            max-width: 600px;
            margin: 50px auto;
            background-color: #ffffff;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            border-radius: 8px;
            overflow: hidden;
        }
        .header {
            background: linear-gradient(135deg, #ff6a00 0%, #ee0979 100%);
            color: white;
            padding: 30px;
            text-align: center;
        }
        .content {
            padding: 30px;
        }
        .content h2 {
            margin-top: 0;
            color: #2c3e50;
        }
        .button {
            display: inline-block;
            margin-top: 20px;
            padding: 15px 25px;
            background-color: #ee0979;
            color: white;
            text-decoration: none;
            border-radius: 5px;
            font-weight: bold;
            transition: background-color 0.3s ease;
        }
        .button:hover {
            background-color: #d1066d;
        }
        .footer {
            background-color: #f1f1f1;
            color: #666;
            text-align: center;
            padding: 15px;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Password Reset</h1>
        </div>
        <div class='content'>
            <h2>Hello " + recipientName + @",</h2>
            <p>You recently requested to reset your password for your account. Click the button below to reset it.</p>
            <p><strong>This password reset link is valid for a limited time.</strong></p>
            <a href='" + resetLink + @"' class='button'>Reset Your Password</a>
            <p>If you didn't request a password reset, you can safely ignore this email. Your password will not change.</p>
        </div>
        <div class='footer'>
            &copy; " + DateTime.Now.Year + @" ChickenFlick Film. All rights reserved.
        </div>
    </div>
</body>
</html>");

            return html.ToString();
        }

        public async Task SendConfirmationEmailAsync(string toEmail, string recipientName, string actionType,
            string details, string confirmationNumber = "", string nextSteps = "")
        {
            var subject = $"Confirmation: {actionType}";
            var htmlMessage = CreateConfirmationHtml(recipientName, actionType, details, confirmationNumber, nextSteps);

            await SendEmailAsync(toEmail, subject, htmlMessage);
        }
        /// <summary>
        /// usage: await emailSender.SendConfirmationEmailAsync(
        ///"user@example.com",
        ///"John Doe",
        ///"Account Registration",
        ///"Your account has been successfully created and is now active.",
        ///"CF-" + DateTime.Now.Ticks,
        ///"Please log in to your account to complete your profile setup."
        ///)
        /// </summary>
        private string CreateConfirmationHtml(string recipientName, string actionType, string details,
            string confirmationNumber, string nextSteps)
        {
            var html = new StringBuilder();

            html.Append(@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Confirmation</title>
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
            margin: 20px auto;
            background-color: #ffffff;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
            border-radius: 8px;
            overflow: hidden;
        }
        .header {
            background-color: #4CAF50;
            color: white;
            padding: 30px;
            text-align: center;
        }
        .header h1 {
            margin: 0;
            font-size: 24px;
            font-weight: 300;
        }
        .checkmark {
            font-size: 48px;
            margin-bottom: 10px;
        }
        .content {
            padding: 30px;
        }
        .greeting {
            font-size: 18px;
            margin-bottom: 20px;
            color: #2c3e50;
        }
        .confirmation-details {
            background-color: #f8f9fa;
            padding: 20px;
            border-radius: 8px;
            margin: 20px 0;
            border-left: 4px solid #4CAF50;
        }
        .detail-row {
            margin: 10px 0;
            color: #555;
        }
        .detail-label {
            font-weight: bold;
            color: #2c3e50;
        }
        .confirmation-number {
            background-color: #e8f5e8;
            padding: 15px;
            border-radius: 5px;
            text-align: center;
            margin: 20px 0;
            font-size: 16px;
            font-weight: bold;
            color: #2e7d32;
        }
        .next-steps {
            background-color: #fff3cd;
            border: 1px solid #ffeaa7;
            border-radius: 8px;
            padding: 20px;
            margin: 20px 0;
            color: #856404;
        }
        .next-steps h3 {
            margin-top: 0;
            color: #856404;
        }
        .footer {
            background-color: #f1f1f1;
            color: #666;
            padding: 20px;
            text-align: center;
            font-size: 14px;
        }
        @media (max-width: 600px) {
            .container {
                margin: 10px;
            }
            .header, .content {
                padding: 20px;
            }
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='checkmark'>✓</div>
            <h1>Confirmation</h1>
        </div>
        
        <div class='content'>
            <div class='greeting'>
                Hello " + recipientName + @",
            </div>
            
            <p>This email confirms that your <strong>" + actionType + @"</strong> has been successfully processed.</p>
            
            <div class='confirmation-details'>
                <div class='detail-row'>
                    <span class='detail-label'>Action:</span> " + actionType + @"
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Details:</span> " + details + @"
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Date:</span> " + DateTime.Now.ToString("MMMM dd, yyyy") + @"
                </div>
                <div class='detail-row'>
                    <span class='detail-label'>Time:</span> " + DateTime.Now.ToString("h:mm tt") + @"
                </div>
            </div>");

            if (!string.IsNullOrEmpty(confirmationNumber))
            {
                html.Append(@"
            <div class='confirmation-number'>
                Confirmation Number: " + confirmationNumber + @"
            </div>");
            }

            if (!string.IsNullOrEmpty(nextSteps))
            {
                html.Append(@"
            <div class='next-steps'>
                <h3>Next Steps:</h3>
                <p>" + nextSteps + @"</p>
            </div>");
            }

            html.Append(@"
            <p>If you have any questions or concerns, please don't hesitate to contact our support team.</p>
            
            <p style='margin-top: 30px; color: #7f8c8d;'>
                Thank you for using ChickenFlick Film!
            </p>
        </div>
        
        <div class='footer'>
            <p>&copy; " + DateTime.Now.Year + @" ChickenFlick Film. All rights reserved.</p>
            <p style='font-size: 12px; margin-top: 10px;'>
                This is an automated message. Please do not reply to this email.
            </p>
        </div>
    </div>
</body>
</html>");

            return html.ToString();
        }
    }
}