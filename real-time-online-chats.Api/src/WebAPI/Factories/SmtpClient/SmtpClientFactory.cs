using Microsoft.Extensions.Options;
using real_time_online_chats.Server.Configurations;
using real_time_online_chats.Server.Wrappers.SmtpClient;

namespace real_time_online_chats.Server.Factories.SmtpClient;

public class SmtpClientFactory(IOptions<MailConfiguration> mailConfiguration) : ISmtpClientFactory
{
    private readonly MailConfiguration _mailConfiguration = mailConfiguration.Value;

    public ISmtpClientWrapper CreateClient()
    {
        return new SmtpClientWrapper
        {
            Host = "smtp.gmail.com",
            Port = 587,
            UseDefaultCredentials = false,
            Credentials = new System.Net.NetworkCredential(_mailConfiguration.Mail, _mailConfiguration.MailPassword),
            EnableSsl = true, 
        };
    }
}