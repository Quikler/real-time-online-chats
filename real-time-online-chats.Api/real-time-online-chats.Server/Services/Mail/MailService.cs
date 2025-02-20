using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using real_time_online_chats.Server.Configurations;

namespace real_time_online_chats.Server.Services.Mail;

public class MailService(IOptions<MailConfiguration> mailConfiguration) : IMailService
{
    private readonly MailConfiguration _mailConfiguration = mailConfiguration.Value;

    public async Task SendMessageAsync(string[] toEmails, string subject, string body)
    {
        var fromMail = _mailConfiguration.Mail;
        var fromMailPassword = _mailConfiguration.MailPassword;

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

        var smptClient = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential(fromMail, fromMailPassword),
            EnableSsl = true,
        };

        await smptClient.SendMailAsync(message);
    }
}