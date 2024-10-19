using System.Net;
using System.Net.Mail;
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
        Task SendAdminRegistrationEmail(string toEmail, string recipientName, string temporaryPassword);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SmtpClient GetSmtpClient()
        {
            var smtpServer = _configuration["SmtpSettings:Server"];
            var smtpPort = _configuration["SmtpSettings:Port"];
            var username = _configuration["SmtpSettings:Username"];
            var password = _configuration["SmtpSettings:Password"];
            var enableSsl = bool.Parse(_configuration["SmtpSettings:EnableSsl"]);

            if (string.IsNullOrEmpty(smtpPort))
            {
                throw new ArgumentNullException(nameof(smtpPort), "SMTP port cannot be null");
            }

            int port = int.Parse(smtpPort);

            return new SmtpClient(smtpServer)
            {
                Port = port,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl,
            };
        }

        private MailMessage CreateMailMessage(string toEmail, string subject, string body)
        {
            var fromEmail = _configuration["SmtpSettings:From"];
            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);
            return mailMessage;
        }

        public async Task SendVerificationEmail(string toEmail, string code)
        {
            var smtpClient = GetSmtpClient();
            var mailMessage = CreateMailMessage(toEmail, "Your Verification Code", $"Your verification code is {code}");
            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendPasswordResetEmail(string toEmail, string resetUrl)
        {
            var smtpClient = GetSmtpClient();
            var mailMessage = CreateMailMessage(toEmail, "Password Reset Request", $"Please reset your password using this link: <a href='{resetUrl}'>Reset Password</a>");
            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendRegistrationConfirmationEmail(string toEmail)
        {
            var smtpClient = GetSmtpClient();
            var mailMessage = CreateMailMessage(toEmail, "Registration Successful", "You have successfully registered to the system.");
            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendGenericEmail(string toEmail, string subject, string body)
        {
            var smtpClient = GetSmtpClient();
            var mailMessage = CreateMailMessage(toEmail, subject, body);
            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendAdminRegistrationEmail(string toEmail, string recipientName, string temporaryPassword)
        {
            var subject = "Admin Account Registration";
            var htmlContent = $@"
            <p>Welcome to Team 12 System, {recipientName}!</p>
            <p>You have been registered as an Administrator. To access your account, please visit 
               <a href='http://localhost:4200/'>our platform</a> and login with the following credentials:</p>
            <p><strong>Username:</strong> {toEmail}</p>
            <p><strong>Temporary Password:</strong> {temporaryPassword}</p>
            <p>For security reasons, please change your password immediately after your first login.</p>
            <p>Welcome aboard!</p>";

            await SendGenericEmail(toEmail, subject, htmlContent);
        }
    

    public async Task SendBookingConfirmationEmail(string toEmail, string facilityName, DateTime bookingDate, string bookingId, string qrContent)
        {
            var smtpClient = GetSmtpClient();
            var mailMessage = CreateMailMessage(toEmail, "Booking Confirmation", 
                $@"<html>
                    <body>
                        <h2>Booking Confirmation</h2>
                        <p>Your booking for {facilityName} on {bookingDate:dddd, dd MMMM yyyy} has been confirmed.</p>
                        <p>Booking Reference: {bookingId}</p>
                        <p>Please present the attached QR code when you arrive at the facility.</p>
                    </body>
                </html>");

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
            var smtpClient = GetSmtpClient();
            var mailMessage = CreateMailMessage(toEmail, "Payment Confirmation", 
                $"Your payment of {amount:C} for booking reference {bookingReference} has been successfully processed.");
            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendLoyaltyPointsUpdateEmail(string toEmail, int points)
        {
            var smtpClient = GetSmtpClient();
            var mailMessage = CreateMailMessage(toEmail, "Loyalty Points Updated", 
                $"You have earned {points} loyalty points for your recent booking. Your total points are now {points}.");
            await smtpClient.SendMailAsync(mailMessage);
        }


        public async Task SendFreeBookingNotificationEmail(string toEmail, string facilityName)
        {
            var smtpClient = GetSmtpClient();
            var mailMessage = CreateMailMessage(toEmail, "Congratulations! Free Booking Awarded", 
                $"Congratulations! You've earned a free booking at {facilityName}. Please log into the system to claim your reward.");
            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendBookingCancellationEmail(string toEmail, string facilityName, DateTime bookingDate)
        {
            var smtpClient = GetSmtpClient();
            var mailMessage = CreateMailMessage(toEmail, "Booking Cancellation", 
                $"Your booking for {facilityName} on {bookingDate:dddd, dd MMMM yyyy} has been canceled.");
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}