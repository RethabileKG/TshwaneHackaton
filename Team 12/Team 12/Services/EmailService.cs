using System.Net.Mail;
using System.Net;

namespace Team_12.Services
{
    public interface IEmailService
    {
        Task SendVerificationEmail(string toEmail, string code);
        Task SendPasswordResetEmail(string toEmail, string token);
        Task SendRegistrationConfirmationEmail(string toEmail);
        Task SendTaskReassignmentEmail(string toEmail, string taskName, string reassignedBy);

        Task SendGenericEmail(string toEmail, string subject, string body);

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
            // Construct the SMTP client
            var smtpClient = new SmtpClient(_configuration["EmailSettings:SMTPServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Login"], _configuration["EmailSettings:Password"]),
                EnableSsl = true,
            };

            // Construct the email message
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:FromEmail"]),
                Subject = "Password Reset Request",
                Body = $"Please reset your password using the following link: <a href='{resetUrl}'>Reset Password</a>",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            // Send the email
            await smtpClient.SendMailAsync(mailMessage);
        }


        public async Task SendRegistrationConfirmationEmail(string toEmail)
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
                Subject = "Registration Successful",
                Body = "You have been registered to the system successfully.",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
        // New method for sending task reassignment email notification
        public async Task SendTaskReassignmentEmail(string toEmail, string taskName, string reassignedBy)
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
                Subject = "Task Reassigned",
                Body = $"The task '{taskName}' has been reassigned to you by {reassignedBy}.",
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
    }
}