using real_time_online_chats.Server.Contracts.V1.Responses.Auth;
using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Mapping;

public static class DomainToApiContract
{
    public static AuthSuccessResponse ToResponse(this AuthSuccess authResult) => new()
    {
        Token = authResult.Token,
        RefreshToken = authResult.RefreshToken,
        User = authResult.User.ToResponse(),
    };

    public static UserResponse ToResponse(this UserResult userResult) => new()
    {
        Id = userResult.Id,
        FirstName = userResult.FirstName,
        LastName = userResult.LastName,
        Email = userResult.Email,
    };
}