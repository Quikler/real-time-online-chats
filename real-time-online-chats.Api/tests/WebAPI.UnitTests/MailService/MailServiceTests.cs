using real_time_online_chats.Server.Configurations;
using Microsoft.Extensions.Options;
using real_time_online_chats.Server.Factories.SmtpClient;
using Moq;
using real_time_online_chats.Server.Wrappers.SmtpClient;
using System.Net.Mail;
using Shouldly;
using MailServices = real_time_online_chats.Server.Services.Mail;

namespace WebAPI.UnitTests.MailService
{
    public class MailServiceTests
    {
        private const string Mail = "test@test.com", MailPassword = "Test1234";

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
                Mail = Mail,
                MailPassword = MailPassword,
            });

            _mailService = new MailServices.MailService(mailConfigurationOptions, _smtpClientFactoryMock.Object);
        }

        [Fact]
        public async Task SendMessageAsync_ShouldReturnTrue_WhenEmailIsSentSuccessfully()
        {
            // Arrange
            string[] mailsToSend = ["test@test.com", "test2@test.com", "test3@test.com"];
            string subject = "Subject";
            string body = "Body";

            _smtpClientMock
                .Setup(client => client.SendMailAsync(
                    It.Is<MailMessage>(mailMessage =>
                        mailMessage.IsBodyHtml &&
                        mailMessage.Body == body &&
                        mailMessage.Subject == subject &&
                        mailMessage.From != null &&
                        mailMessage.From.Address == Mail &&
                        mailMessage.From.DisplayName == "ROC Team" &&
                        mailMessage.To.Select(to => to.Address).SequenceEqual(mailsToSend)
                    )))
                .Returns(Task.CompletedTask);

            _smtpClientFactoryMock.Setup(generator => generator.CreateClient())
                .Returns(_smtpClientMock.Object);

            // Act
            var sendResult = await _mailService.SendMessageAsync(mailsToSend, subject, body);

            // Assert
            sendResult.ShouldBeTrue();
        }

        [Fact]
        public async Task SendMessageAsync_ShouldReturnFalse_WhenEmailIsNotSentSuccessfully()
        {
            // Arrange
            string[] mailsToSend = ["test@test.com", "test2@test.com", "test3@test.com"];
            string subject = "Subject";
            string body = "Body";

            _smtpClientMock
                .Setup(client => client.SendMailAsync(
                    It.IsAny<MailMessage>()
                ))
                .ThrowsAsync(new SmtpException());

            _smtpClientFactoryMock.Setup(generator => generator.CreateClient())
                .Returns(_smtpClientMock.Object);

            // Act
            var sendResult = await _mailService.SendMessageAsync(mailsToSend, subject, body);

            // Assert
            sendResult.ShouldBeFalse();
        }
    }
}