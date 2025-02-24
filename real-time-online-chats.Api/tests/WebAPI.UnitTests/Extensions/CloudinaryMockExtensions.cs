using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Moq;

namespace WebAPI.UnitTests.Extensions;

public static class CloudinaryMockExtensions
{
    public static void VerifyUploadAsync(this Mock<ICloudinary> cloudinaryMock, string publicId, Stream uploadStream)
    {
        cloudinaryMock.Verify(cloudinary => cloudinary.UploadAsync(
            It.Is<ImageUploadParams>(parameters =>
                parameters.PublicId == publicId &&
                parameters.Overwrite == true &&
                parameters.File.FileName == publicId &&
                parameters.File.Stream == uploadStream
            ),
            It.IsAny<CancellationToken?>()
        ));
    }
}