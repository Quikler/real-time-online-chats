using Microsoft.EntityFrameworkCore;
using real_time_online_chats.Server.Common;
using real_time_online_chats.Server.Data;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.Chat;
using real_time_online_chats.Server.DTOs.User;
using real_time_online_chats.Server.Mapping;
using real_time_online_chats.Server.Repositories.Chat;

namespace real_time_online_chats.Server.Services.Chat;

public class ChatService(AppDbContext dbContext,
    IChatAuthorizationService chatAuthorizationService,
    IChatRepository chatRepository)
    : IChatService
{
    public async Task<Result<PaginationDto<ChatPreviewDto>, FailureDto>> GetChatsAsync(int pageNumber, int pageSize, string filter = "")
    {
        var query = dbContext.Chats.AsNoTracking();

        int totalRecords = await query.CountAsync();
        
        List<ChatEntity> chats = [];
        query = query.OrderByDescending(c => c.CreationTime);

        if (string.IsNullOrWhiteSpace(filter))
        {
            chats = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        else
        {
            chats = await query
                .Where(c => c.Title.Contains(filter))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        return chats.ToPagination(c => c.ToChatPreview(), totalRecords, pageNumber, pageSize);
    }

    public async Task<Result<ChatPreviewDto, FailureDto>> GetChatPreviewByIdAsync(Guid chatId)
    {
        ChatPreviewDto? chatPreviewDto = await dbContext.Chats
            .Where(c => c.Id == chatId)
            .Select(c => c.ToChatPreview())
            .FirstOrDefaultAsync();

        return chatPreviewDto is not null ? chatPreviewDto : FailureDto.NotFound("Chat not found");
    }

    public async Task<Result<ChatDetailedDto, FailureDto>> GetChatDetailedByIdAsync(Guid chatId)
    {
        ChatEntity? chat = await dbContext.Chats
            .AsNoTracking()
            .AsSplitQuery()
            .Include(c => c.Owner)
            .Include(c => c.Members)
            .Include(c => c.Messages.Where(m => m.ChatId == chatId).OrderBy(m => m.CreationTime))
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        return chat is not null ? chat.ToChatDetailed() : FailureDto.NotFound("Chat not found");
    }

    public async Task<Result<ChatPreviewDto, FailureDto>> CreateChatAsync(CreateChatDto createChatDto)
    {
        ChatEntity chat = createChatDto.ToChat();
        List<UserEntity> existingUsers = await dbContext.Users
            .Where(u => createChatDto.UsersIdToAdd.Contains(u.Id))
            .ToListAsync();
        chat.Members.AddRange(existingUsers);

        int rows = await chatRepository.AddChatAsync(chat);
        return rows == 0 ? FailureDto.BadRequest("Cannot create chat") : chat.ToChatPreview();
    }

    public async Task<Result<bool, FailureDto>> UpdateChatTitleAsync(Guid chatId, UpdateChatDto updateChatDto, Guid userId)
    {
        var validationResult = await ValidateChatOwnershipAsync(chatId, userId);
        if (!validationResult.IsSuccess) return validationResult;

        int rows = await chatRepository.UpdateChatTitleAsync(chatId, updateChatDto.Title);

        return rows == 0 ? FailureDto.BadRequest("Cannot update chat") : true;
    }

    public async Task<Result<bool, FailureDto>> DeleteChatAsync(Guid chatId, Guid userId)
    {
        var validationResult = await ValidateChatOwnershipAsync(chatId, userId);
        if (!validationResult.IsSuccess) return validationResult;

        int rows = await chatRepository.DeleteChatAsync(chatId);

        return rows == 0 ? FailureDto.BadRequest("Cannot delete chat") : true;
    }

    public async Task<Result<(UserChatDto user, bool isAlreadyInChat), FailureDto>> UserJoinChatAsync(Guid chatId, Guid userId)
    {
        if (!await chatRepository.IsChatExistAsync(chatId)) return FailureDto.NotFound("Chat not found");
        if (await chatAuthorizationService.IsUserExistInChatAsync(chatId, userId))
        {
            var userChatDto = await dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => u.ToUserChat())
                .FirstOrDefaultAsync();

            return userChatDto is null ? FailureDto.NotFound("User not found") : (userChatDto, true);
        }

        var chat = await dbContext.Chats
            //.Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.Id == chatId);

        if (chat is null) return FailureDto.NotFound("Chat not found");

        var user = await dbContext.Users.FindAsync(userId);
        if (user is null) return FailureDto.NotFound("User not found");

        chat.Members.Add(user);
        int rows = await dbContext.SaveChangesAsync();

        return rows == 0 ? FailureDto.BadRequest("Cannot join chat") : (user.ToUserChat(), false);
    }

    public async Task<Result<UserChatDto, FailureDto>> UserLeaveChatAsync(Guid chatId, Guid userId)
    {
        // Find the user and include both OwnedChats and MemberChats
        var user = await dbContext.Users
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
            if (newOwner is not null)
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
            else
            {
                // No other members in the chat, so delete the chat
                dbContext.Chats.Remove(ownedChat);
            }
        }
        else
        {
            // User is a member of the chat
            var memberChat = user.MemberChats.FirstOrDefault(c => c.Id == chatId);
            if (memberChat is null) return FailureDto.BadRequest("User not the member of chat");

            // Remove the chat from the user's MemberChats
            user.MemberChats.Remove(memberChat);
        }

        // Save changes to the database
        int rows = await dbContext.SaveChangesAsync();

        return rows == 0 ? FailureDto.BadRequest("Cannot leave the chat") : user.ToUserChat();
    }

    public async Task<Result<bool, FailureDto>> KickMemberAsync(Guid chatId, Guid memberId, Guid userId)
    {
        if (!await chatAuthorizationService.IsUserOwnsChatAsync(chatId, userId)) return FailureDto.Forbidden("User doesn't own this chat");

        var chat = await dbContext.Chats
            .Where(c => c.Id == chatId)
            .Include(c => c.Members.Where(m => m.Id == memberId))
            .FirstOrDefaultAsync();

        if (chat is null) return FailureDto.NotFound("Chat not found");

        chat.Members.Clear();

        int rows = await dbContext.SaveChangesAsync();
        return rows == 0 ? FailureDto.BadRequest("Cannot kick user") : true;
    }

    public async Task<Result<bool, FailureDto>> UpdateOwnerAsync(Guid chatId, Guid newOwnerId, Guid userId)
    {
        if (!await chatAuthorizationService.IsUserOwnsChatAsync(chatId, userId)) return FailureDto.Forbidden("User doesn't own this chat");

        var chat = await dbContext.Chats
            .Where(c => c.Id == chatId)
            .Include(c => c.Owner)
            .Include(c => c.Members.Where(m => m.Id == newOwnerId))
            .FirstOrDefaultAsync();

        if (chat is null) return FailureDto.NotFound("Chat not found");

        var newOwner = chat.Members.FirstOrDefault();
        if (newOwner is null) return FailureDto.NotFound("User not found");
        var owner = chat.Owner;

        // Transfer ownership to the new owner
        chat.OwnerId = newOwner.Id;
        chat.Owner = newOwner;

        // Remove the chat from the current owner's OwnedChats
        owner.OwnedChats.Remove(chat);
        owner.MemberChats.Add(chat);

        // Add the chat to the new owner's OwnedChats and remove it from MemberChats because he's a new owner
        newOwner.OwnedChats.Add(chat);
        newOwner.MemberChats.Remove(chat);

        int rows = await dbContext.SaveChangesAsync();
        return rows == 0 ? FailureDto.BadRequest("Cannot change owner") : true;
    }

    public async Task<Result<ChatInfoDto, FailureDto>> GetChatInfo(Guid chatId)
    {
        var chatInfoDto = await dbContext.Chats.Select(c => new ChatInfoDto
        {
            Id = c.Id,
            Title = c.Title,
            CreationTime = c.CreationTime,
            OwnerId = c.OwnerId,
        }).FirstOrDefaultAsync(c => c.Id == chatId);

        return chatInfoDto is not null ? chatInfoDto : FailureDto.NotFound("Chat not found.");
    }

    private async Task<Result<bool, FailureDto>> ValidateChatOwnershipAsync(Guid chatId, Guid userId)
    {
        if (!await chatRepository.IsChatExistAsync(chatId))
            return FailureDto.NotFound("Chat not found");

        if (!await chatAuthorizationService.IsUserOwnsChatAsync(chatId, userId))
            return FailureDto.Forbidden("User doesn't own this chat");

        return true;
    }
}
