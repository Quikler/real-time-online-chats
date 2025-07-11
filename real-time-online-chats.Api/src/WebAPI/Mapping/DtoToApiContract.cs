using Microsoft.EntityFrameworkCore.Metadata.Internal;
using real_time_online_chats.Server.Contracts.V1.Responses;
using real_time_online_chats.Server.Contracts.V1.Responses.Auth;
using real_time_online_chats.Server.Contracts.V1.Responses.Chat;
using real_time_online_chats.Server.Contracts.V1.Responses.Message;
using real_time_online_chats.Server.Contracts.V1.Responses.User;
using real_time_online_chats.Server.DTOs;
using real_time_online_chats.Server.DTOs.Auth;
using real_time_online_chats.Server.DTOs.Chat;
using real_time_online_chats.Server.DTOs.Message;
using real_time_online_chats.Server.DTOs.User;

namespace real_time_online_chats.Server.Mapping;

public static class DtoToApiContract
{
    public static UserProfileResponse ToResponse(this UserProfileDto userProfileDto)
    {
        return new UserProfileResponse
        {
            Id = userProfileDto.Id,
            Email = userProfileDto.Email,
            FirstName = userProfileDto.FirstName,
            LastName = userProfileDto.LastName,
            AboutMe = userProfileDto.AboutMe,
            ActivityStatus = userProfileDto.ActivityStatus.ToString(),
            CasualStatus = userProfileDto.CasualStatus.ToString(),
            GamingStatus = userProfileDto.GamingStatus.ToString(),
            MoodStatus = userProfileDto.MoodStatus.ToString(),
            WorkStatus = userProfileDto.WorkStatus.ToString(),
            Friends = userProfileDto.Friends.Select(f => f.ToResponse()),
            AvatarUrl = userProfileDto.AvatarUrl,
        };
    }

    public static UserFriendResponse ToResponse(this UserFriendDto userFriendDto)
    {
        return new UserFriendResponse
        {
            Id = userFriendDto.Id,
            Email = userFriendDto.Email,
            FirstName = userFriendDto.FirstName,
            LastName = userFriendDto.LastName,
        };
    }

    public static ChatPreviewResponse ToResponse(this ChatPreviewDto chatPreviewDto)
    {
        return new ChatPreviewResponse
        {
            Id = chatPreviewDto.Id,
            Title = chatPreviewDto.Title,
        };
    }

    public static PaginationResponse<TResult> ToResponse<T, TResult>(this PaginationDto<T> paginationDto, Func<T, TResult> itemsSelect)
    {
        return new PaginationResponse<TResult>
        {
            TotalCount = paginationDto.TotalCount,
            PageNumber = paginationDto.PageNumber,
            PageSize = paginationDto.PageSize,
            Items = paginationDto.Items.Select(itemsSelect),
        };
    }

    public static MessageChatResponse ToResponse(this MessageChatDto messageChatDto)
    {
        return new MessageChatResponse
        {
            Id = messageChatDto.Id,
            Content = messageChatDto.Content,
            User = messageChatDto.User.ToResponse(),
        };
    }

    public static UserChatResponse ToResponse(this UserChatDto userChatDto)
    {
        return new UserChatResponse
        {
            Id = userChatDto.Id,
            Email = userChatDto.Email,
            FirstName = userChatDto.FirstName,
            LastName = userChatDto.LastName,
            AvatarUrl = userChatDto.AvatarUrl,
        };
    }

    public static ChatDetailedResponse ToResponse(this ChatDetailedDto chatDetailedDto)
    {
        return new ChatDetailedResponse
        {
            Id = chatDetailedDto.Id,
            Title = chatDetailedDto.Title,
            OwnerId = chatDetailedDto.OwnerId,
            CreationTime = chatDetailedDto.CreationTime,
            Messages = chatDetailedDto.Messages.Select(m => m.ToResponse()),
            Users = chatDetailedDto.Users.Select(u => u.ToResponse()),
        };
    }

    public static AuthSuccessResponse ToResponse(this AuthSuccessDto authSuccessDto)
    {
        return new AuthSuccessResponse
        {
            Token = authSuccessDto.Token,
            User = authSuccessDto.User.ToResponse(),
        };
    }

    public static UserGlobalResponse ToResponse(this UserGlobalDto userGlobalDto)
    {
        return new UserGlobalResponse
        {
            Id = userGlobalDto.Id,
            Email = userGlobalDto.Email,
            FirstName = userGlobalDto.FirstName,
            LastName = userGlobalDto.LastName,
        };
    }

    public static ChatInfoResponse ToResponse(this ChatInfoDto dto)
    {
        return new ChatInfoResponse
        {
            Id = dto.Id,
            OwnerId = dto.OwnerId,
            Title = dto.Title,
            CreationTime = dto.CreationTime,
        };
    }

    public static FailureResponse ToResponse(this FailureDto failureDto) => new FailureResponse(failureDto.Errors);
}
