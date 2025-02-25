using AutoFixture;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Moq;
using real_time_online_chats.Server.Services.Cloudinary;
using Shouldly;
using WebAPI.UnitTests;
using WebAPI.UnitTests.Extensions;

namespace WebApi.UnitTests.Cloudinary;

public class CloudinaryServiceTests : BaseUnitTests
{
    private readonly Uri _uploadUrl;
    private readonly Guid _userId;

    private readonly Mock<ICloudinary> _cloudinaryMock;
    private readonly CloudinaryService _cloudinaryService;

    public CloudinaryServiceTests()
    {
        _uploadUrl = Fixture.Create<Uri>();
        _userId = Guid.NewGuid();

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
                PublicId = _userId.ToString(),
                Url = _uploadUrl,
                StatusCode = System.Net.HttpStatusCode.OK,
            });

        // Act
        using var streamToUpload = new MemoryStream();
        var uploadUrl = await _cloudinaryService.UploadAvatarToCloudinaryAsync(streamToUpload, _userId);

        // Assert
        uploadUrl.ShouldNotBeNull();

        var urlsEqual = Uri.Compare(new(uploadUrl), _uploadUrl, UriComponents.Host, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase);
        urlsEqual.ShouldBe(0);

        _cloudinaryMock.VerifyUploadAsync(_userId.ToString(), streamToUpload);
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
        var uploadUrl = await _cloudinaryService.UploadAvatarToCloudinaryAsync(streamToUpload, _userId);

        // Assert
        uploadUrl.ShouldBeNull();

        _cloudinaryMock.VerifyUploadAsync(_userId.ToString(), streamToUpload);
    }
}