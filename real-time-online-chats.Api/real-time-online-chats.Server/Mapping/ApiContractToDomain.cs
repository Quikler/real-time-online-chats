using real_time_online_chats.Server.Contracts.V1.Requests.Auth;
using real_time_online_chats.Server.Domain;

namespace real_time_online_chats.Server.Mapping;

public static class ApiContractToDomain
{
    public static LoginUser ToDomain(this LoginUserRequest request) => new()
    {
        Email = request.Email,
        Password = request.Password,
    };

    public static SignupUser ToDomain(this SignupUserRequest request) => new()
    {
        Email = request.Email,
        Password = request.Password,
        FirstName = request.FirstName,
        LastName = request.LastName,
        Phone = request.Phone,
        RememberMe = request.RememberMe,
    };
}