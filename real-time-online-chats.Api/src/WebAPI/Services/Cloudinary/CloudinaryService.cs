using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace real_time_online_chats.Server.Services.Cloudinary;

public class CloudinaryService : ICloudinaryService
{
    public async Task<string?> UploadAvatarToCloudinaryAsync(Stream avatarStream, Guid userId)
    {
        var account = new Account(
            CloudinaryConfiguration.CloudName,
            CloudinaryConfiguration.ApiKey,
            CloudinaryConfiguration.ApiSecret);

        var cloudinary = new CloudinaryDotNet.Cloudinary(account);
        cloudinary.Api.Secure = true;

        var publicId = userId.ToString();

        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(publicId, avatarStream),
            PublicId = publicId,
            Overwrite = true,
        };

        var uploadResult = await cloudinary.UploadAsync(uploadParams);
        return uploadResult.StatusCode == System.Net.HttpStatusCode.OK ? uploadResult.Url.AbsoluteUri : null;
    }
}