using real_time_online_chats.Server.Configurations;
using Microsoft.Extensions.Options;
using real_time_online_chats.Server.Factories.SmtpClient;
using Moq;
using real_time_online_chats.Server.Wrappers.SmtpClient;
using System.Net.Mail;
using Shouldly;
using MailServices = real_time_online_chats.Server.Services.Mail;
using WebAPI.UnitTests.Extensions;

namespace WebAPI.UnitTests.Mail
{
    public class MailServiceTests
    {
        private const string FromMail = "test@test.com", MailPassword = "Test1234";
        private const string Body = "Body", Subject = "Subject", DisplayName = "ROC Team";
        private static readonly string[] s_mailsToSend = ["test@test.com", "test2@test.com", "test3@test.com"];

        private readonly MockRepository _mockRepository;

        private readonly Mock<ISmtpClientFactory> _smtpClientFactoryMock;
        private readonly Mock<ISmtpClientWrapper> _smtpClientMock;
        private readonly MailServices.MailService _mailService;

        public MailServiceTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);

            _smtpClientMock = _mockRepository.Create<ISmtpClientWrapper>();
            _smtpClientFactoryMock = _mockRepository.Create<ISmtpClientFactory>();

            var mailConfigurationOptions = Options.Create(new MailConfiguration
            {
                Mail = FromMail,
                MailPassword = MailPassword,
            });

            _mailService = new MailServices.MailService(mailConfigurationOptions, _smtpClientFactoryMock.Object);
        }

        [Fact]
        public async Task SendMessageAsync_ShouldReturnTrue_WhenEmailIsSentSuccessfully()
        {
            // Arrange
            _smtpClientMock
                .Setup(client => client.SendMailAsync(It.IsAny<MailMessage>()))
                .Returns(Task.CompletedTask);

            _smtpClientFactoryMock.SetupCreateClient(_smtpClientMock);

            // Act
            var sendResult = await _mailService.SendMessageAsync(s_mailsToSend, Subject, Body);

            // Assert
            sendResult.ShouldBeTrue();

            _smtpClientMock.VerifySendMailAsync(Body, Subject, FromMail, DisplayName, s_mailsToSend);
        }

        [Fact]
        public async Task SendMessageAsync_ShouldReturnFalse_WhenEmailIsNotSentSuccessfully()
        {
            // Arrange
            _smtpClientMock
                .Setup(client => client.SendMailAsync(It.IsAny<MailMessage>()))
                .ThrowsAsync(new SmtpException());

            _smtpClientFactoryMock.SetupCreateClient(_smtpClientMock);

            // Act
            var sendResult = await _mailService.SendMessageAsync(s_mailsToSend, Subject, Body);

            // Assert
            sendResult.ShouldBeFalse();

            _smtpClientMock.VerifySendMailAsync(Body, Subject, FromMail, DisplayName, s_mailsToSend);
        }
    }
}