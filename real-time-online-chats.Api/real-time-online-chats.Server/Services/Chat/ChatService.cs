using Microsoft.EntityFrameworkCore;
using real_time_online_chats.Server.Common;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.Chat;
using real_time_online_chats.Server.DTOs.Message;
using real_time_online_chats.Server.DTOs.User;
using real_time_online_chats.Server.Mapping;

namespace real_time_online_chats.Server.Services.Chat;

public class ChatService(AppDbContext dbContext, IChatAuthorizationService chatAuthorizationService) : IChatService
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IChatAuthorizationService _chatAuthorizationService = chatAuthorizationService;

    public async Task<Result<PaginationDto<ChatPreviewDto>, FailureDto>> GetChatsAsync(int pageNumber, int pageSize)
    {
        int totalRecords = await _dbContext.Chats.CountAsync();
        List<ChatEntity> chats = await _dbContext.Chats
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return chats.ToPagination(c => c.ToChatPreview(), totalRecords, pageNumber, pageSize);
    }

    public async Task<Result<PaginationDto<ChatPreviewDto>, FailureDto>> GetAllOwnedChatsAsync(int pageNumber, int pageSize, Guid userId)
    {
        int totalRecords = await _dbContext.Chats.CountAsync(c => c.OwnerId == userId);
        List<ChatEntity> chats = await _dbContext.Chats
            .Where(c => c.OwnerId == userId)
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return chats.ToPagination(c => c.ToChatPreview(), totalRecords, pageNumber, pageSize);
    }

    public Task<PaginationDto<ChatPreviewDto>> GetOwnedChatsWithDetailsAsync(int pageNumber, int pageSize, Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<ChatDetailedDto, FailureDto>> GetChatDetailedByIdAsync(Guid chatId)
    {
        ChatEntity? chat = await _dbContext.Chats
            .AsNoTracking()
            .AsSplitQuery()
            .Include(c => c.Owner)
            .Include(c => c.Members)
            .Include(c => c.Messages.Where(m => m.ChatId == chatId).OrderBy(m => m.CreationTime))
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        // ChatEntity? chat = await _dbContext.Chats
        //     .AsNoTracking()
        //     .Include(c => c.Owner)
        //     .Include(c => c.Members)
        //         .ThenInclude(m => m.Messages.Where(m => m.ChatId == chatId).OrderBy(m => m.CreationTime))
        //     .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat is null) return FailureDto.NotFound("Chat not found");

        return new ChatDetailedDto
        {
            Id = chat.Id,
            OwnerId = chat.OwnerId,
            Title = chat.Title,
            Messages = chat.Messages.Select(m => new MessageChatDto
            {
                Id = m.Id,
                User = new UserChatDto
                {
                    Id = m.UserId,
                    Email = m.User.Email,
                    FirstName = m.User.FirstName,
                    LastName = m.User.LastName,
                },
                Content = m.Content,
            }),
            Users = chat.Members.Append(chat.Owner).Select(m => new UserChatDto
            {
                Id = m.Id,
                Email = m.Email,
                FirstName = m.FirstName,
                LastName = m.LastName,
            })
        };
    }

    public async Task<Result<ChatPreviewDto, FailureDto>> CreateChatAsync(CreateChatDto createChatDto)
    {
        ChatEntity chat = createChatDto.ToChat();

        await _dbContext.Chats.AddAsync(chat);
        int rows = await _dbContext.SaveChangesAsync();

        return rows == 0 ? FailureDto.BadRequest("Cannot create chat") : chat.ToChatPreview();
    }

    public async Task<Result<bool, FailureDto>> UpdateChatAsync(Guid chatId, UpdateChatDto updateChatDto, Guid userId)
    {
        if (!await _chatAuthorizationService.IsUserOwnsChatAsync(chatId, userId)) return FailureDto.Forbidden("User doesn't own this chat");

        int rows = await _dbContext.Chats
            .Where(c => c.Id == chatId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.Title, updateChatDto.Title)
            );

        return rows == 0 ? FailureDto.BadRequest("Cannot update chat") : true;
    }

    public async Task<Result<bool, FailureDto>> DeleteChatAsync(Guid chatId, Guid userId)
    {
        if (!await _chatAuthorizationService.IsUserOwnsChatAsync(chatId, userId)) return FailureDto.Forbidden("User doesn't own this chat");

        int rows = await _dbContext.Chats
            .Where(c => c.Id == chatId)
            .ExecuteDeleteAsync();
        
        return rows == 0 ? FailureDto.BadRequest("Cannot delete chat") : true;
    }

    public async Task<Result<bool, FailureDto>> UserJoinChatAsync(Guid chatId, Guid userId)
    {
        var chat = await _dbContext.Chats
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat is null) return FailureDto.NotFound("Chat not found");

        var user = await _dbContext.Users.FindAsync(userId);
        if (user is null) return FailureDto.NotFound("User not found");

        chat.Members.Add(user);
        int rows = await _dbContext.SaveChangesAsync();

        return rows == 0 ? FailureDto.BadRequest("Cannot join chat") : true;
    }

    public async Task<Result<bool, FailureDto>> UserLeaveChatAsync(Guid chatId, Guid userId)
    {
        // Find the user and include both OwnedChats and MemberChats
        var user = await _dbContext.Users
            .Include(u => u.OwnedChats.Where(c => c.Id == chatId))
                .ThenInclude(oc => oc.Members.Take(1))
            .Include(u => u.MemberChats)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null) return FailureDto.NotFound("User not found");

        // Check if the user is the owner of the chat
        var ownedChat = user.OwnedChats.FirstOrDefault(c => c.Id == chatId);
        if (ownedChat is not null)
        {
            // User is the owner of the chat
            // Find another member to transfer ownership to
            var newOwner = ownedChat.Members.FirstOrDefault();
            if (newOwner is null)
            {
                // No other members in the chat, so delete the chat
                _dbContext.Chats.Remove(ownedChat);
            }
            else
            {
                // Transfer ownership to the new owner
                ownedChat.OwnerId = newOwner.Id;
                ownedChat.Owner = newOwner;

                // Remove the chat from the current owner's OwnedChats
                user.OwnedChats.Remove(ownedChat);

                // Add the chat to the new owner's OwnedChats and remove it from MemberChats because he's a new owner
                newOwner.OwnedChats.Add(ownedChat);
                newOwner.MemberChats.Remove(ownedChat);
            }
        }
        else
        {
            // User is a member of the chat
            var memberChat = user.MemberChats.FirstOrDefault(c => c.Id == chatId);
            if (memberChat is null) return FailureDto.BadRequest("User doesn't exist in the chat"); // User is not a member of the chat

            // Remove the chat from the user's MemberChats
            user.MemberChats.Remove(memberChat);
        }

        // Save changes to the database
        int rows = await _dbContext.SaveChangesAsync();
        
        return rows == 0 ? FailureDto.BadRequest("Cannot leave the chat") : true;
    }

    public Task<Result<bool, FailureDto>> ChangeOwnerAsync(Guid chatId, Guid newOwnerId)
    {
        throw new NotImplementedException();
    }
}