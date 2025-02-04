using real_time_online_chats.Server.Contracts.V1.Responses.User;
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

    public static UserFailureResponse ToResponse(this UserFailureDto userFailureDto)
    {
        return new UserFailureResponse(userFailureDto.Errors);
    }
}