using HealthGuard.Core.Repository.contract;
using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HealthGuard.Core.Entities.Identity;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HealthGuard.Service.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly EmailSetting _emailSettings;

        public EmailService(IOptions<EmailSetting> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_emailSettings.FromEmail, _emailSettings.FromEmail));
                email.To.Add(new MailboxAddress(to, to));
                email.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = body };
                email.Body = builder.ToMessageBody();

                using (var smtp = new SmtpClient())
                {
                    await smtp.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, true);
                    await smtp.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
                    await smtp.SendAsync(email);
                    await smtp.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email to {to}: {ex.Message}");
                return false;
            }
        }
    }
}
