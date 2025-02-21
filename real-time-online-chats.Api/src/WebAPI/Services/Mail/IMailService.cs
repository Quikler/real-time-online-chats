namespace real_time_online_chats.Server.Services.Mail;

public interface IMailService
{
    Task<bool> SendMessageAsync(string[] toEmails, string subject, string body);
}