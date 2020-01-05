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
            return emailSender.SendEmailAsync(new[] { email }, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}
