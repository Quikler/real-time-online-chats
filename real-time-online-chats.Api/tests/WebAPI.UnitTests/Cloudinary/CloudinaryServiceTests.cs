using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Moq;
using real_time_online_chats.Server.Services.Cloudinary;
using Shouldly;
using WebAPI.UnitTests.Extensions;

namespace WebApi.UnitTests.Cloudinary;

public class CloudinaryServiceTests
{
    private static Uri TestUploadUrl { get; } = new("https://cloudinary.com");
    private static Guid TestUserId { get; } = Guid.NewGuid();

    private readonly Mock<ICloudinary> _cloudinaryMock;
    private readonly CloudinaryService _cloudinaryService;

    public CloudinaryServiceTests()
    {
        _cloudinaryMock = new Mock<ICloudinary>();
        _cloudinaryService = new CloudinaryService(_cloudinaryMock.Object);
    }

    [Fact]
    public async Task UploadAvatarToCloudinaryAsync_ShouldUploadSuccessfully_WhenUploadAsyncSucceeds()
    {
        // Arrange
        _cloudinaryMock
            .Setup(cloudinary => cloudinary.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken?>()))
            .ReturnsAsync(new ImageUploadResult
            {
                PublicId = TestUserId.ToString(),
                Url = TestUploadUrl,
                StatusCode = System.Net.HttpStatusCode.OK,
            });

        // Act
        using var streamToUpload = new MemoryStream();
        var uploadUrl = await _cloudinaryService.UploadAvatarToCloudinaryAsync(streamToUpload, TestUserId);

        // Assert
        uploadUrl.ShouldNotBeNull();

        var urlsEqual = Uri.Compare(new(uploadUrl), TestUploadUrl, UriComponents.Host, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase);
        urlsEqual.ShouldBe(0);

        _cloudinaryMock.VerifyUploadAsync(TestUserId.ToString(), streamToUpload);
    }

    [Fact]
    public async Task UploadAvatarToCloudinaryAsync_ShouldFail_WhenUploadAsyncFails()
    {
        // Arrange
        _cloudinaryMock
            .Setup(cloudinary => cloudinary.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken?>()))
            .ReturnsAsync(new ImageUploadResult
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
            });

        // Act
        using var streamToUpload = new MemoryStream();
        var uploadUrl = await _cloudinaryService.UploadAvatarToCloudinaryAsync(streamToUpload, TestUserId);

        // Assert
        uploadUrl.ShouldBeNull();

        _cloudinaryMock.VerifyUploadAsync(TestUserId.ToString(), streamToUpload);
    }
}