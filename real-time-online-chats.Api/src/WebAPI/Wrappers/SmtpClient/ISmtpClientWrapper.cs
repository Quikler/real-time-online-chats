using System.Net.Mail;

namespace real_time_online_chats.Server.Wrappers.SmtpClient;

public interface ISmtpClientWrapper
{
    Task SendMailAsync(MailMessage mail);
}