using System.Net.Mail;
using System.Net;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace Team_12.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmail(string toEmail, string code);
        Task SendPasswordResetEmail(string toEmail, string token);
        Task SendRegistrationConfirmationEmail(string toEmail);
        Task SendGenericEmail(string toEmail, string subject, string body);
        Task SendBookingConfirmationEmail(string toEmail, string facilityName, DateTime bookingDate, string bookingId, string qrContent);
        Task SendPaymentConfirmationEmail(string toEmail, string bookingReference, decimal amount);
        Task SendLoyaltyPointsUpdateEmail(string toEmail, int points);
        Task SendFreeBookingNotificationEmail(string toEmail, string facilityName);
        Task SendBookingCancellationEmail(string toEmail, string facilityName, DateTime bookingDate);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendVerificationEmail(string toEmail, string code)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SMTPServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Login"], _configuration["EmailSettings:Password"]),
                EnableSsl = true,
                UseDefaultCredentials = false  // This ensures that the credentials you provide are used for authentication
            };


            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
                Subject = "Your Verification Code",
                Body = $"Your verification code is {code}",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendPasswordResetEmail(string toEmail, string resetUrl)
        {
            string portConfig = _configuration["EmailSettings:Port"];
            if (string.IsNullOrEmpty(portConfig))
            {
                throw new ArgumentNullException(nameof(portConfig), "SMTP port is missing from configuration.");
            }
            int smtpPort = int.Parse(portConfig);

            var smtpClient = new SmtpClient(_configuration["EmailSettings:SMTPServer"])
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(
                    _configuration["EmailSettings:Login"],
                    _configuration["EmailSettings:Password"]),
                EnableSsl = true,
                UseDefaultCredentials = false
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
                Subject = "Password Reset Request",
                Body = $"Please reset your password using the following link: <a href='{resetUrl}'>Reset Password</a>",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }


        public async Task SendRegistrationConfirmationEmail(string toEmail)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SMTPServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Login"], _configuration["EmailSettings:Password"]),
                EnableSsl = true,
                UseDefaultCredentials = false  // This ensures that the credentials you provide are used for authentication
            };


            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
                Subject = "Registration Successful",
                Body = "You have been registered to the system successfully.",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendGenericEmail(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SMTPServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Login"], _configuration["EmailSettings:Password"]),
                EnableSsl = true,
                UseDefaultCredentials = false  // This ensures that the credentials you provide are used for authentication
            };


            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        // New Email Methods for Booking System

        public async Task SendBookingConfirmationEmail(string toEmail, string facilityName, DateTime bookingDate, string bookingId, string qrContent)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SMTPServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Login"], _configuration["EmailSettings:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
                Subject = "Booking Confirmation",
                Body = $@"
                    <html>
                    <body>
                        <h2>Booking Confirmation</h2>
                        <p>Your booking for {facilityName} on {bookingDate:dddd, dd MMMM yyyy} has been confirmed.</p>
                        <p>Booking Reference: {bookingId}</p>
                        <p>Please present the attached QR code when you arrive at the facility.</p>
                    </body>
                    </html>",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            // Generate and attach QR code
            using (var qrGenerator = new QRCodeGenerator())
            {
                var qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
                using (var qrCode = new QRCode(qrCodeData))
                using (var qrCodeImage = qrCode.GetGraphic(20))
                using (var ms = new MemoryStream())
                {
                    qrCodeImage.Save(ms, ImageFormat.Png);
                    ms.Position = 0;
                    var attachment = new Attachment(ms, "booking-qrcode.png", "image/png");
                    mailMessage.Attachments.Add(attachment);

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
        }


        public async Task SendPaymentConfirmationEmail(string toEmail, string bookingReference, decimal amount)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SMTPServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Login"], _configuration["EmailSettings:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
                Subject = "Payment Confirmation",
                Body = $"Your payment of {amount:C} for booking reference {bookingReference} has been successfully processed.",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendLoyaltyPointsUpdateEmail(string toEmail, int points)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SMTPServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Login"], _configuration["EmailSettings:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
                Subject = "Loyalty Points Updated",
                Body = $"You have earned {points} loyalty points for your recent booking. Your total points are now {points}.",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendFreeBookingNotificationEmail(string toEmail, string facilityName)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SMTPServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Login"], _configuration["EmailSettings:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
                Subject = "Congratulations! Free Booking Awarded",
                Body = $"Congratulations! You've earned a free booking at {facilityName}. Please log into the system to claim your reward.",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendBookingCancellationEmail(string toEmail, string facilityName, DateTime bookingDate)
        {
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SMTPServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Login"], _configuration["EmailSettings:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
                Subject = "Booking Cancellation",
                Body = $"Your booking for {facilityName} on {bookingDate.ToString("dddd, dd MMMM yyyy")} has been canceled.",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
