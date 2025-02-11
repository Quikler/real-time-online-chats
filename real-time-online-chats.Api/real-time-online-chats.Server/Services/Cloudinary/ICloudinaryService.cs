namespace real_time_online_chats.Server.Services.Cloudinary;

public interface ICloudinaryService
{
    Task<string?> UploadAvatarToCloudinaryAsync(Stream avatarStream, Guid userId);
}