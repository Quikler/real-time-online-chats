using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.DTOs.Auth;
using real_time_online_chats.Server.DTOs.Chat;

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
            Title = createChatDto.Title,
            Members = [..createChatDto.UsersIdToAdd.Select(id => new UserEntity
            {
                Id = id,
            })]
        };
    }
}