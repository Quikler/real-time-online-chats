using Microsoft.EntityFrameworkCore;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.User;
using real_time_online_chats.Server.Mapping;

namespace real_time_online_chats.Server.Services.Chat;

public class ChatUserService(AppDbContext dbContext) : IChatUserService
{
    public async Task<List<UserChatDto>> GetAllUsersByChatId(Guid chatId)
    {
        var users = await dbContext.Chats
            .Where(c => c.Id == chatId)
            .Include(c => c.Members)
            .Include(c => c.Owner)
            //.SelectMany(c => c.Members.Append(c.Owner).Select(u => u.ToUserChat())) // TODO: introduce projections
            .ToListAsync();

        return users.SelectMany(c => c.Members.Append(c.Owner).Select(u => u.ToUserChat())).ToList(); // TODO: introduce projections;
    }

    public async Task<ResultDto<UserChatDto>> AddUser(Guid chatId, Guid userId)
    {
        var chat = await dbContext.Chats
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat is null)
        {
            return FailureDto.NotFound("Chat not found");
        }

        if (chat.Members.Any(u => u.Id == userId) || chat.OwnerId == userId)
        {
            return FailureDto.Conflict("User already in chat");
        }

        var user = await dbContext.Users.FindAsync(userId);
        if (user is null)
        {
            return FailureDto.NotFound("User not found");
        }

        chat.Members.Add(user);
        int rows = await dbContext.SaveChangesAsync();

        return rows == 0 ? FailureDto.BadRequest("Cannot join chat") : user.ToUserChat();
    }

    public async Task<ResultDto<UserChatDto>> DeleteUser(Guid chatId, Guid userId)
    {
        var chat = await dbContext.Chats
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat is null)
        {
            return FailureDto.NotFound("Chat not found");
        }

        if (chat.OwnerId == userId)
        {
            return FailureDto.BadRequest("Cannot leave chat when you are owner");
        }

        var user = await dbContext.Users.FindAsync(userId);
        if (user is null)
        {
            return FailureDto.NotFound("User not found");
        }

        chat.Members.Remove(user);
        int rows = await dbContext.SaveChangesAsync();

        return rows == 0 ? FailureDto.BadRequest("Cannot leave chat") : user.ToUserChat();
    }

    public async Task<ResultDto<UserChatDto>> DeleteUser(Guid chatId, Guid userId, Guid whoDeletesId)
    {
        var chat = await dbContext.Chats
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat is null)
        {
            return FailureDto.NotFound("Chat not found");
        }

        if (chat.OwnerId == userId)
        {
            return FailureDto.BadRequest("Cannot leave chat when you are owner");
        }

        if (chat.OwnerId != whoDeletesId)
        {
            return FailureDto.Forbidden("User is not owner of the chat");
        }

        var user = await dbContext.Users.FindAsync(userId);
        if (user is null)
        {
            return FailureDto.NotFound("User not found");
        }

        chat.Members.Remove(user);
        int rows = await dbContext.SaveChangesAsync();

        return rows == 0 ? FailureDto.BadRequest("Cannot delete member of the chat") : user.ToUserChat();
    }
}
