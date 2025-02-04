using real_time_online_chats.Server.Domain;
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
}