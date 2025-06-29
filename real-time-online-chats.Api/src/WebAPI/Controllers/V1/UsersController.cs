using Microsoft.AspNetCore.Mvc;
using real_time_online_chats.Server.Attributes;
using real_time_online_chats.Server.Contracts.V1;
using real_time_online_chats.Server.Contracts.V1.Requests;
using real_time_online_chats.Server.Contracts.V1.Requests.User;
using real_time_online_chats.Server.Extensions;
using real_time_online_chats.Server.Mapping;
using real_time_online_chats.Server.Services.User;

namespace real_time_online_chats.Server.Controllers.V1;

public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet(ApiRoutes.Users.GetProfile)]
    [Cached(600)]
    public async Task<IActionResult> GetProfile(Guid userId)
    {
        var result = await userService.GetUserProfileAsync(userId);

        return result.Match(
            userProfileDto => Ok(userProfileDto.ToResponse()),
            failure => failure.ToActionResult()
        );
    }

    [HttpPut(ApiRoutes.Users.EditProfile)]
    [InvalidateCache(ApiRoutes.Users.GetProfile, 600)]
    public async Task<IActionResult> EditProfile([FromRoute] Guid userId, [FromForm] UpdateUserProfileRequest request)
    {
        if (!HttpContext.TryGetUserId(out var currentUserId)) return Unauthorized();
        if (userId != currentUserId) return Forbid();

        var result = await userService.UpdateUserProfileAsync(userId, request.ToDto());

        return result.Match(
            userProfileDto => Ok(userProfileDto.ToResponse()),
            failure => failure.ToActionResult()
        );
    }

    [HttpGet(ApiRoutes.Users.GetOwnerChats)]
    public async Task<IActionResult> GetOwnerChats(Guid userId, [FromQuery] PaginationRequest request)
    {
        var result = await userService.GetUserOwnerChatsAsync(request.PageNumber, request.PageSize, userId);

        return result.Match(
            paginationDto => Ok(paginationDto.ToResponse(c => c.ToResponse())),
            failure => failure.ToActionResult()
        );
    }

    [HttpGet(ApiRoutes.Users.GetMemberChats)]
    public async Task<IActionResult> GetMemberChats(Guid userId, [FromQuery] PaginationRequest request)
    {
        var result = await userService.GetUserMemberChatsAsync(request.PageNumber, request.PageSize, userId);

        return result.Match(
            paginationDto => Ok(paginationDto.ToResponse(c => c.ToResponse())),
            failure => failure.ToActionResult()
        );
    }
}
