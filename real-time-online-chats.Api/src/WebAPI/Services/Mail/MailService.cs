using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using real_time_online_chats.Server.Configurations;
using real_time_online_chats.Server.Factories.SmtpClient;

namespace real_time_online_chats.Server.Services.Mail;

public class MailService(IOptions<MailConfiguration> mailConfiguration, ISmtpClientFactory smtpClientFactory) : IMailService
{
    private readonly MailConfiguration _mailConfiguration = mailConfiguration.Value;

    public async Task<bool> SendMessageAsync(string[] toEmails, string subject, string body)
    {
        var fromMail = _mailConfiguration.Mail;

        try
        {
            var message = new MailMessage
            {
                From = new MailAddress(fromMail, "ROC Team"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            foreach (var email in toEmails)
            {
                message.To.Add(new MailAddress(email));
            }

            var smtpClient = smtpClientFactory.CreateClient();
            await smtpClient.SendMailAsync(message);

            return true;
        }
        catch
        {
            return false;
        }
    }
}