using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;

namespace Authorization.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration; // app stetings access
        private readonly ILogger<EmailSender> _logger;
        private readonly string _smtpServer; // smtp Server adress
        private readonly int _smtpPort;
        private readonly string  _formEmail; //senders email
        private readonly string _password; // smtp password

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _smtpServer = _configuration["EmailSettings:SmtpServer"];
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            _formEmail = _configuration["EmailSettings:FromEmail"];
            _password = _configuration["EmailSettings:Password"];
        }



        public async Task SendEmailAsync(string email,string subject,string htmlMessage)
        {
            try
            {
                using var message = new MailMessage()
                {
                    From = new MailAddress(email),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };

                message.To.Add(new MailAddress(email));

                using var client = new SmtpClient(_smtpServer, _smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(_formEmail, _password)
                };
                await client.SendMailAsync(message);
                _logger.LogInformation($"Email sent succesfully to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email to {email}, Error:{ex.Message}");
                throw;
            }
        }
    }
}
