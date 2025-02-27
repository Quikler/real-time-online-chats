using Microsoft.EntityFrameworkCore;
using real_time_online_chats.Server.Common;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.Chat;
using real_time_online_chats.Server.DTOs.User;
using real_time_online_chats.Server.Mapping;
using real_time_online_chats.Server.Services.Cloudinary;

namespace real_time_online_chats.Server.Services.User;

public class UserService(AppDbContext dbContext, ICloudinaryService cloudinaryService) : IUserService
{
    public async Task<Result<UserGlobalDto, FailureDto>> GetUserGlobalAsync(Guid userId)
    {
        var userGlobalDto = await dbContext.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserGlobalDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
            })
            .FirstOrDefaultAsync();

        return userGlobalDto is null ? FailureDto.NotFound("User not found") : userGlobalDto;
    }

    public async Task<Result<PaginationDto<ChatPreviewDto>, FailureDto>> GetUserMemberChatsAsync(int pageNumber, int pageSize, Guid userId)
    {
        int totalRecords = await dbContext.Chats
            .CountAsync(c => c.Members.Any(u => u.Id == userId));

        IReadOnlyList<ChatEntity> memberChats = await dbContext.Users
            .Where(u => u.Id == userId)
            .Include(u => u.MemberChats
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
            )
            .SelectMany(u => u.MemberChats)
            .ToListAsync();

        return memberChats.ToPagination(c => c.ToChatPreview(), totalRecords, pageNumber, pageSize);
    }

    public async Task<Result<PaginationDto<ChatPreviewDto>, FailureDto>> GetUserOwnerChatsAsync(int pageNumber, int pageSize, Guid userId)
    {
        var query = dbContext.Chats
            .Where(c => c.OwnerId == userId)
            .AsNoTracking();

        var totalRecords = await query.CountAsync();
        IReadOnlyList<ChatEntity> ownedChats = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return ownedChats.ToPagination(c => c.ToChatPreview(), totalRecords, pageNumber, pageSize);
    }

    public async Task<Result<UserProfileDto, FailureDto>> GetUserProfileAsync(Guid userId)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .Include(u => u.Friends)
            //.Include(u => u.OwnedChats)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null) return FailureDto.NotFound("User not found");

        return user.ToUserProfile();
    }

    public async Task<Result<UserProfileDto, FailureDto>> UpdateUserProfileAsync(Guid userId, UpdateUserProfileDto updateUserProfileDto)
    {
        var user = await dbContext.Users
            .Include(u => u.Friends)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null) return FailureDto.NotFound("User not found.");

        user.AboutMe = updateUserProfileDto.AboutMe;
        user.ActivityStatus = updateUserProfileDto.ActivityStatus;
        user.CasualStatus = updateUserProfileDto.CasualStatus;
        user.GamingStatus = updateUserProfileDto.GamingStatus;
        user.WorkStatus = updateUserProfileDto.WorkStatus;
        user.MoodStatus = updateUserProfileDto.MoodStatus;

        if (updateUserProfileDto.AvatarStream is not null)
        {
            try
            {
                var avatarUrl = await cloudinaryService.UploadAvatarToCloudinaryAsync(updateUserProfileDto.AvatarStream, userId);
                if (avatarUrl is null) return FailureDto.BadRequest("Unable to update profile avatar.");
                user.AvatarUrl = avatarUrl;
            }
            finally
            {
                updateUserProfileDto.AvatarStream.Dispose();
            }
        }

        int rows = await dbContext.SaveChangesAsync();
        return rows == 0 ? FailureDto.BadRequest("Unable to update profile") : user.ToUserProfile();
    }
}