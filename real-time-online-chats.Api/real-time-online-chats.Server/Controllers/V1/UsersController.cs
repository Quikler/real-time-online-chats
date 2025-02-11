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

        return result.Match<IActionResult>(
            userProfileDto => Ok(userProfileDto.ToResponse()),
            userFailureDto => BadRequest(userFailureDto.ToResponse())
        );
    }

    [HttpPut(ApiRoutes.Users.EditProfile)]
    public async Task<IActionResult> EditProfile([FromRoute] Guid userId, [FromForm] UpdateUserProfileRequest request)
    {
        if (!HttpContext.TryGetUserId(out var currentUserId)) return Unauthorized();
        if (userId != currentUserId) return Forbid();

        var result = await _userService.UpdateUserProfileAsync(userId, request.ToDto());

        return result.Match<IActionResult>(
            userProfileDto => Ok(userProfileDto.ToResponse()),
            failure => failure.FailureCode switch
            {
                Common.FailureCode.BadRequest => BadRequest(failure.ToResponse()),
                Common.FailureCode.NotFound => NotFound(failure.ToResponse()),
                _ => StatusCode(StatusCodes.Status500InternalServerError),
            }
        );
    }
}