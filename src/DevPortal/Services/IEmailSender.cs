using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DevPortal.Web.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(IEnumerable<string> email, string subject, string message);
    }

    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(IEnumerable<string> email, string subject, string message)
        {
            return Task.Delay(10);
        }
    }

    public class EmailSender : IEmailSender
    {
        private AppCode.Config.SendGrid _config;
        public EmailSender(IOptions<AppCode.Config.SendGrid> sendGridOptions)
        {
            _config = sendGridOptions.Value;
        }

        public Task SendEmailAsync(IEnumerable<string> emails, string subject, string message)
        {

            return Execute(_config.Key, subject, message, emails);
        }

        public Task Execute(string apiKey, string subject, string message, IEnumerable<string> emails)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("notifications@vyvojari.sk", "Vyvojari.sk"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };

            foreach (var email in emails)
            {
                msg.AddTo(new EmailAddress(email));
            }

            Task response = client.SendEmailAsync(msg);
            return response;
        }
    }




    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(new[] { email }, "Potvrdte svoj email",
                $"Prosím, potvrďte svoj email kliknutím na tento : <a href='{HtmlEncoder.Default.Encode(link)}'>odkaz</a>");
        }
    }
}
