using real_time_online_chats.Server.Contracts.V1.Requests.Auth;
using real_time_online_chats.Server.Contracts.V1.Requests.Chat;
using real_time_online_chats.Server.Contracts.V1.Requests.Message;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs.Auth;
using real_time_online_chats.Server.DTOs.Chat;
using real_time_online_chats.Server.DTOs.Message;

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

    public static CreateChatDto ToDto(this CreateChatRequest request)
    {
        return new CreateChatDto
        {
            Title = request.Title,
            UsersIdToAdd = request.UsersIdToAdd,
        };
    }

    public static UpdateChatDto ToDto(this UpdateChatRequest request)
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
            ChatId = request.ChatId,
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
}