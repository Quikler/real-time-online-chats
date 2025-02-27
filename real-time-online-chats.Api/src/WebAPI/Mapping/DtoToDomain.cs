using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs.Auth;
using real_time_online_chats.Server.DTOs.Chat;
using real_time_online_chats.Server.DTOs.Message;

namespace real_time_online_chats.Server.Mapping;

public static class DtoToDomain
{
    public static UserEntity ToUser(this SignupUserDto signupUserDto)
    {
        return new UserEntity
        {
            Email = signupUserDto.Email,
            FirstName = signupUserDto.FirstName,
            LastName = signupUserDto.LastName,
            PhoneNumber = signupUserDto.Phone,
        };
    }

    public static ChatEntity ToChat(this CreateChatDto createChatDto)
    {
        return new ChatEntity
        {
            OwnerId = createChatDto.OwnerId,
            Title = createChatDto.Title,
        };
    }

    public static MessageEntity ToMessage(this CreateMessageDto createMessageDto)
    {
        return new MessageEntity
        {
            Content = createMessageDto.Content,
            ChatId = createMessageDto.ChatId,
            UserId = createMessageDto.UserId,
        };
    }
}