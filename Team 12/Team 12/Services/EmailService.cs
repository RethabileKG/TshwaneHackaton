using System.Net;
using System.Net.Mail;

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

        private SmtpClient GetSmtpClient()
        {
            var smtpServer = _configuration["SmtpSettings:Server"];
            var smtpPort = _configuration["SmtpSettings:Port"];
            var username = _configuration["SmtpSettings:Username"];
            var password = _configuration["SmtpSettings:Password"];
            var enableSsl = bool.Parse(_configuration["SmtpSettings:EnableSsl"]);

            if (string.IsNullOrEmpty(smtpPort))
            {
                throw new ArgumentNullException("SMTP port cannot be null");
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

        public async Task SendTaskReassignmentEmail(string toEmail, string taskName, string reassignedBy)
        {
            var smtpClient = GetSmtpClient();
            var mailMessage = CreateMailMessage(toEmail, "Task Reassigned", $"The task '{taskName}' has been reassigned to you by {reassignedBy}.");
            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendGenericEmail(string toEmail, string subject, string body)
        {
            var smtpClient = GetSmtpClient();
            var mailMessage = CreateMailMessage(toEmail, subject, body);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
