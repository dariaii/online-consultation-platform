using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OnlineConsultationPlatform.Core.Services
{
    public interface IEmailService
    {
        void SendEmail(List<string> receivers, string subject, string body);
    }

    public class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
    {
        private readonly EmailSettingsDTO _emailSettings = new(configuration);
        private readonly ILogger<EmailService> _logger = logger;

        public void SendEmail(List<string> receivers, string subject, string body)
        {
            try
            {
                MailMessage email = new()
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.DisplayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                receivers.ForEach(receiver =>
                {
                    email.To.Add(receiver);
                });

                SmtpClient smtpClient = new(_emailSettings.Host, _emailSettings.Port)
                {
                    Credentials = new NetworkCredential(_emailSettings.CredentialsAccount, _emailSettings.CredentialsPassword),
                    EnableSsl = true
                };

                smtpClient.Send(email);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $" {string.Join(",", receivers)}, {subject}, {body}, {ex.StackTrace}");
            }
        }
    }

    public class EmailSettingsDTO
    {
        public EmailSettingsDTO(IConfiguration configuration)
        {
            var settings = configuration.GetSection("EmailSettings");
            SenderEmail = settings["SenderEmail"];
            DisplayName = settings["DisplayName"];
            Host = settings["Host"];
            Port = Convert.ToInt32(settings["Port"]);
            CredentialsAccount = settings["CredentialsAccount"];
            CredentialsPassword = settings["CredentialsPassword"];
        }

        public string SenderEmail { get; set; }

        public string DisplayName { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string CredentialsAccount { get; set; }

        public string CredentialsPassword { get; set; }
    }
}
