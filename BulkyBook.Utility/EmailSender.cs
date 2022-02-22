using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace BulkyBook.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly SendgridOptions _sendgridOptions;

        public EmailSender(IOptions<SendgridOptions> options)
        {
            _sendgridOptions = options.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await Execute(_sendgridOptions.ApiKey, subject, htmlMessage, email);
        }

        private Task Execute(string sendgridApiKey, string subject, string message, string email)
        {
            var apiKey = _sendgridOptions.ApiKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("admin@localhost", "BulkyBook Admin");
            var to = new EmailAddress(email, "End User");
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", message);
            return client.SendEmailAsync(msg);
        }
    }
}
