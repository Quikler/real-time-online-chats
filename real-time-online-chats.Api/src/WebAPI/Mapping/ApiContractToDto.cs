using real_time_online_chats.Server.Contracts.V1.Requests.Auth;
using real_time_online_chats.Server.Contracts.V1.Requests.Chat;
using real_time_online_chats.Server.Contracts.V1.Requests.Message;
using real_time_online_chats.Server.Contracts.V1.Requests.User;
using real_time_online_chats.Server.DTOs.Auth;
using real_time_online_chats.Server.DTOs.Chat;
using real_time_online_chats.Server.DTOs.Message;
using real_time_online_chats.Server.DTOs.User;

namespace real_time_online_chats.Server.Mapping;

public static class ApiContractToDto
{
    public static LoginUserDto ToDto(this LoginUserRequest request)
    {
        return new LoginUserDto
        {
            Email = request.Email,
            Password = request.Password,
        };
    }

    public static SignupUserDto ToDto(this SignupUserRequest request)
    {
        return new SignupUserDto
        {
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            RememberMe = request.RememberMe,
        };
    }

    public static CreateChatDto ToDto(this CreateChatRequest request, Guid ownerId)
    {
        return new CreateChatDto
        {
            OwnerId = ownerId,
            Title = request.Title,
            UsersIdToAdd = request.UsersIdToAdd,
        };
    }

    public static UpdateChatDto ToDto(this UpdateChatTitleRequest request)
    {
        return new UpdateChatDto
        {
            Title = request.Title,
        };
    }

    public static CreateMessageDto ToDto(this CreateMessageRequest request, Guid userId)
    {
        return new CreateMessageDto
        {
            Content = request.Content,
            UserId = userId,
        };
    }

    public static UpdateMessageDto ToDto(this UpdateMessageRequest request, Guid userId)
    {
        return new UpdateMessageDto
        {
            Content = request.Content,
            UserId = userId,
        };
    }

    public static UpdateUserProfileDto ToDto(this UpdateUserProfileRequest updateUserProfileRequest)
    {
        return new UpdateUserProfileDto
        {
            AboutMe = updateUserProfileRequest.AboutMe,
            ActivityStatus = updateUserProfileRequest.ActivityStatus,
            CasualStatus = updateUserProfileRequest.CasualStatus,
            GamingStatus = updateUserProfileRequest.GamingStatus,
            MoodStatus = updateUserProfileRequest.MoodStatus,
            WorkStatus = updateUserProfileRequest.WorkStatus,
            AvatarStream = updateUserProfileRequest.Avatar?.OpenReadStream(),
        };
    }

    public static ResetPasswordDto ToDto(this ResetPasswordRequest request)
    {
        return new ResetPasswordDto
        {
            Email = request.Email,
            Token = request.Token,
            NewPassword = request.NewPassword,
        };
    }
}
