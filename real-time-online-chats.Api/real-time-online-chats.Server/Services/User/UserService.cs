using Microsoft.EntityFrameworkCore;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs.User;
using real_time_online_chats.Server.Mapping;

namespace real_time_online_chats.Server.Services.User;

public class UserService(AppDbContext dbContext) : IUserService
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<Result<UserProfileDto, UserFailureDto>> GetUserProfileAsync(Guid userId)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .Include(u => u.Friends)
            //.Include(u => u.OwnedChats)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null) return new UserFailureDto("User not found");

        return user.ToUserProfile();
    }
}