using Microsoft.AspNetCore.Mvc;
using real_time_online_chats.Server.Contracts.V1;
using real_time_online_chats.Server.Extensions;
using real_time_online_chats.Server.Mapping;
using real_time_online_chats.Server.Services.Google;

namespace real_time_online_chats.Server.Controllers.V1;

public class GoogleAuthController(IGoogleService googleService) : ControllerBase
{
    private readonly IGoogleService _googleService = googleService;

    [HttpPost(ApiRoutes.Google.Login)]
    public async Task<IActionResult> LoginGoogle([FromQuery] string credential)
    {
        var payload = await _googleService.ValidateGoogleTokenAsync(credential);
        if (payload is null) return BadRequest("Invalid Google credential.");

        var result = await _googleService.LoginAsync(payload);

        return result.Match(
            authResult =>
            {
                HttpContext.SetHttpOnlyRefreshToken(authResult.RefreshToken);
                return Ok(authResult.ToResponse());
            },
            failure => failure.ToActionResult()
        );
    }

    [HttpPost(ApiRoutes.Google.Signup)]
    public async Task<IActionResult> SignupGoogle([FromQuery] string credential)
    {
        var payload = await _googleService.ValidateGoogleTokenAsync(credential);
        if (payload is null) return BadRequest("Invalid Google credential.");

        var result = await _googleService.SignupAsync(payload);

        return result.Match(
            authResult =>
            {
                HttpContext.SetHttpOnlyRefreshToken(authResult.RefreshToken);
                return Ok(authResult.ToResponse());
            },
            failure => failure.ToActionResult()
        );
    }
}