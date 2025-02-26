using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.Chat;
using real_time_online_chats.Server.DTOs.Message;
using real_time_online_chats.Server.DTOs.User;

namespace real_time_online_chats.Server.Mapping;

public static class DomainToDTO
{
    public static UserProfileDto ToUserProfile(this UserEntity userEntity)
    {
        return new UserProfileDto
        {
            Id = userEntity.Id,
            Email = userEntity.Email,
            FirstName = userEntity.FirstName,
            LastName = userEntity.LastName,
            AboutMe = userEntity.AboutMe,
            ActivityStatus = userEntity.ActivityStatus.ToString(),
            CasualStatus = userEntity.CasualStatus.ToString(),
            GamingStatus = userEntity.GamingStatus.ToString(),
            MoodStatus = userEntity.MoodStatus.ToString(),
            WorkStatus = userEntity.WorkStatus.ToString(),
            Friends = userEntity.Friends.Select(f => f.ToUserFriend()),
            AvatarUrl = userEntity.AvatarUrl,
        };
    }

    public static UserFriendDto ToUserFriend(this UserEntity userEntity)
    {
        return new UserFriendDto
        {
            Id = userEntity.Id,
            Email = userEntity.Email,
            FirstName = userEntity.FirstName,
            LastName = userEntity.LastName,
        };
    }

    public static UserGlobalDto ToUserGlobal(this UserEntity userEntity)
    {
        return new UserGlobalDto
        {
            Id = userEntity.Id,
            Email = userEntity.Email,
            FirstName = userEntity.FirstName,
            LastName = userEntity.LastName,
        };
    }

    public static UserChatDto ToUserChat(this UserEntity userEntity)
    {
        return new UserChatDto
        {
            Id = userEntity.Id,
            Email = userEntity.Email,
            FirstName = userEntity.FirstName,
            LastName = userEntity.LastName,
            AvatarUrl = userEntity.AvatarUrl,
        };
    }

    public static PaginationDto<TResult> ToPagination<T, TResult>(this List<T> list, Func<T, TResult> itemsSelect, int totalRecords, int pageNumber, int pageSize)
    {
        return new PaginationDto<TResult>
        {
            TotalCount = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Items = list.Select(itemsSelect)
        };
    }

    public static ChatPreviewDto ToChatPreview(this ChatEntity chatEntity)
    {
        return new ChatPreviewDto
        {
            Id = chatEntity.Id,
            Title = chatEntity.Title,
        };
    }

    public static ChatDetailedDto ToChatDetailed(this ChatEntity chatEntity)
    {
        return new ChatDetailedDto
        {
            Id = chatEntity.Id,
            OwnerId = chatEntity.OwnerId,
            Title = chatEntity.Title,
            CreationTime = chatEntity.CreationTime,
            Messages = chatEntity.Messages.Select(m => new MessageChatDto
            {
                Id = m.Id,
                User = m.User.ToUserChat(),
                Content = m.Content,
            }),
            Users = chatEntity.Members.Append(chatEntity.Owner).Select(m => m.ToUserChat())
        };
    }

    public static MessageChatDto ToMessageChat(this MessageEntity messageEntity)
    {
        return new MessageChatDto
        {
            Id = messageEntity.Id,
            Content = messageEntity.Content,
            User = messageEntity.User.ToUserChat(),
        };
    }
}