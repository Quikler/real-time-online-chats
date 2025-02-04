using Microsoft.EntityFrameworkCore;
using real_time_online_chats.Server.Common;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.User;
using real_time_online_chats.Server.Mapping;

namespace real_time_online_chats.Server.Services.User;

public class UserService(AppDbContext dbContext) : IUserService
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<Result<UserProfileDto, FailureDto>> GetUserProfileAsync(Guid userId)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .Include(u => u.Friends)
            //.Include(u => u.OwnedChats)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null) return FailureDto.NotFound("User not found");

        return user.ToUserProfile();
    }
}