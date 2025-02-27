using Microsoft.AspNetCore.Mvc;
using real_time_online_chats.Server.Contracts.V1;
using real_time_online_chats.Server.Contracts.V1.Requests.User;
using real_time_online_chats.Server.Extensions;
using real_time_online_chats.Server.Mapping;
using real_time_online_chats.Server.Services.User;

namespace real_time_online_chats.Server.Controllers.V1;

public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet(ApiRoutes.Users.GetProfile)]
    public async Task<IActionResult> GetProfile(Guid userId)
    {
        var result = await _userService.GetUserProfileAsync(userId);

        return result.Match(
            userProfileDto => Ok(userProfileDto.ToResponse()),
            failure => failure.ToActionResult()
        );
    }

    [HttpPut(ApiRoutes.Users.EditProfile)]
    public async Task<IActionResult> EditProfile([FromRoute] Guid userId, [FromForm] UpdateUserProfileRequest request)
    {
        if (!HttpContext.TryGetUserId(out var currentUserId)) return Unauthorized();
        if (userId != currentUserId) return Forbid();

        var result = await _userService.UpdateUserProfileAsync(userId, request.ToDto());

        return result.Match(
            userProfileDto => Ok(userProfileDto.ToResponse()),
            failure => failure.ToActionResult()
        );
    }

    [HttpGet(ApiRoutes.Users.OwnerChats)]
    public async Task<IActionResult> OwnerChats([FromRoute] Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return BadRequest("Invalid page size or page number.");
        }
        
        var result = await _userService.GetUserOwnerChatsAsync(page, pageSize, userId);

        return result.Match(
            paginationDto => Ok(paginationDto.ToResponse(c => c.ToResponse())),
            failure => failure.ToActionResult()
        );
    }

    [HttpGet(ApiRoutes.Users.MemberChats)]
    public async Task<IActionResult> MemberChats([FromRoute] Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var result = await _userService.GetUserMemberChatsAsync(page, pageSize, userId);

        return result.Match(
            paginationDto => Ok(paginationDto.ToResponse(c => c.ToResponse())),
            failure => failure.ToActionResult()
        );
    }
}