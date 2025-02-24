using System.Net.Mail;
using Moq;
using real_time_online_chats.Server.Wrappers.SmtpClient;

namespace WebAPI.UnitTests.Extensions;

public static class SmtpClientWrapperMockExtensions
{
    public static void VerifySendMailAsync(this Mock<ISmtpClientWrapper> smtpClientWrapperMock,
        string body,
        string subject,
        string fromMail,
        string fromDisplayName,
        string[] mailsToSend)
    {
        smtpClientWrapperMock.Verify(smtpClient => smtpClient.SendMailAsync(
            It.Is<MailMessage>(mailMessage =>
                mailMessage.IsBodyHtml &&
                mailMessage.Body == body &&
                mailMessage.Subject == subject &&
                mailMessage.From != null &&
                mailMessage.From.Address == fromMail &&
                mailMessage.From.DisplayName == fromDisplayName &&
                mailMessage.To.Select(to => to.Address).SequenceEqual(mailsToSend)
            )));
    }
}