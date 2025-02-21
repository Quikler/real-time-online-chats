using real_time_online_chats.Server.Wrappers.SmtpClient;

namespace real_time_online_chats.Server.Factories.SmtpClient
{
    public interface ISmtpClientFactory
    {
        ISmtpClientWrapper CreateClient();
    }
}