using Microsoft.EntityFrameworkCore;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.DTOs.User;
using real_time_online_chats.Server.Mapping;

namespace real_time_online_chats.Server.Repositories.User;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<UserChatDto?> GetUserChatDto(Guid userId)
    {
        var userChatDto = await _dbContext.Users
            .Where(u => u.Id == userId)
            .Select(u => u.ToUserChat())
            .FirstOrDefaultAsync();

        return userChatDto;
    }

    public async Task<UserFriendDto?> GetUserFriendDto(Guid userId)
    {
        var userFriendDto = await _dbContext.Users
            .Where(u => u.Id == userId)
            .Select(u =>  u.ToUserFriend())
            .FirstOrDefaultAsync();

        return userFriendDto;
    }

    public async Task<UserGlobalDto?> GetUserGlobalDto(Guid userId)
    {
        var userGlobalDto = await _dbContext.Users
            .Where(u => u.Id == userId)
            .Select(u => u.ToUserGlobal())
            .FirstOrDefaultAsync();

        return userGlobalDto;
    }

    public async Task<UserProfileDto?> GetUserProfileDto(Guid userId)
    {
        var userProfileDto = await _dbContext.Users
            .Where(u => u.Id == userId)
            .Select(u => u.ToUserProfile())
            .FirstOrDefaultAsync();

        return userProfileDto;
    }
}