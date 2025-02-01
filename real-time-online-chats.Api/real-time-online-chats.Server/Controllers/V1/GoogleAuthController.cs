using Microsoft.AspNetCore.Mvc;
using real_time_online_chats.Server.Contracts.V1;
using real_time_online_chats.Server.Contracts.V1.Responses.Auth;
using real_time_online_chats.Server.Extensions;
using real_time_online_chats.Server.Services.Google;

namespace real_time_online_chats.Server.Controllers.V1;

public class GoogleAuthController(IGoogleService googleService) : ControllerBase
{
    private readonly IGoogleService _googleService = googleService;

    [HttpGet(ApiRoutes.Google.Login)]
    public async Task<IActionResult> LoginGoogle([FromQuery] string credential)
    {
        var payload = await _googleService.ValidateGoogleTokenAsync(credential);
        if (payload is null) return BadRequest("Invalid Google credential.");

        var authResult = await _googleService.LoginAsync(payload);

        if (!authResult.Succeded)
        {
            return BadRequest(new AuthFailedResponse
            {
                Errors = authResult.Errors,
            });
        }

        HttpContext.SetHttpOnlyRefreshToken(authResult.RefreshToken);

        return Ok(new AuthSuccessResponse
        {
            Token = authResult.Token,
            RefreshToken = authResult.RefreshToken,
            User = new UserResponse
            {
                Id = authResult.User.Id,
                Email = authResult.User.Email,
                FirstName = authResult.User.FirstName,
                LastName = authResult.User.LastName,
            }
        });
    }

    [HttpGet(ApiRoutes.Google.Signup)]
    public async Task<IActionResult> SignupGoogle([FromQuery] string credential)
    {
        var payload = await _googleService.ValidateGoogleTokenAsync(credential);
        if (payload is null) return BadRequest("Invalid Google credential.");

        var authResult = await _googleService.SignupAsync(payload);

        if (!authResult.Succeded)
        {
            return BadRequest(new AuthFailedResponse
            {
                Errors = authResult.Errors,
            });
        }

        HttpContext.SetHttpOnlyRefreshToken(authResult.RefreshToken);

        return Ok(new AuthSuccessResponse
        {
            Token = authResult.Token,
            RefreshToken = authResult.RefreshToken,
            User = new UserResponse
            {
                Id = authResult.User.Id,
                Email = authResult.User.Email,
                FirstName = authResult.User.FirstName,
                LastName = authResult.User.LastName,
            }
        });
    }
}