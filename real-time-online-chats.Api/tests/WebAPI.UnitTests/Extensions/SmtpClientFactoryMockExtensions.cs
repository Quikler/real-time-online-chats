using Moq;
using real_time_online_chats.Server.Factories.SmtpClient;
using real_time_online_chats.Server.Wrappers.SmtpClient;

namespace WebAPI.UnitTests.Extensions;

public static class SmtpClientFactoryMockExtensions
{
    public static void SetupCreateClient(this Mock<ISmtpClientFactory> smtpClientFactoryMock, Mock<ISmtpClientWrapper> smtpClientMock)
    {
        smtpClientFactoryMock
            .Setup(generator => generator.CreateClient())
            .Returns(smtpClientMock.Object);
    }
}