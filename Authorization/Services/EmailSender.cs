using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Authorization.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email,string subject,string htmlMessage)
        {
            // sending email
            return Task.CompletedTask;
        }
    }
}
