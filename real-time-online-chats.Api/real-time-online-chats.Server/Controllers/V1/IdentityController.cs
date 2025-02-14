using Microsoft.AspNetCore.Mvc;
using real_time_online_chats.Server.Contracts.V1;
using real_time_online_chats.Server.Contracts.V1.Requests.Auth;
using real_time_online_chats.Server.Contracts.V1.Responses;
using real_time_online_chats.Server.Contracts.V1.Responses.Auth;
using real_time_online_chats.Server.Extensions;
using real_time_online_chats.Server.Mapping;
using real_time_online_chats.Server.Services.Identity;

namespace real_time_online_chats.Server.Controllers.V1;

public class IdentityController(IIdentityService identityService) : ControllerBase
{
    private readonly IIdentityService _identityService = identityService;

    [HttpPost(ApiRoutes.Identity.Signup)]
    public async Task<IActionResult> Signup([FromBody] SignupUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new FailureResponse(ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))));
        }

        var signupUser = request.ToDto();

        var result = await _identityService.SignupAsync(signupUser);

        return result.Match(
            authSuccessDto => 
            {
                HttpContext.SetHttpOnlyRefreshToken(authSuccessDto.RefreshToken);
                return Ok(authSuccessDto.ToResponse());
            },
            failure => failure.ToActionResult()
        );
    }

    [HttpPost(ApiRoutes.Identity.Login)]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new FailureResponse(ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))));
        }

        var loginUser = request.ToDto();

        var result = await _identityService.LoginAsync(loginUser);

        return result.Match(
            authSuccessDto => 
            {
                HttpContext.SetHttpOnlyRefreshToken(authSuccessDto.RefreshToken);
                return Ok(authSuccessDto.ToResponse());
            },
            failure => failure.ToActionResult()
        );
    }

    [HttpPost(ApiRoutes.Identity.Refresh)]
    public async Task<IActionResult> Refresh()
    {
        if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken)) return Unauthorized();

        var result = await _identityService.RefreshTokenAsync(refreshToken);

        return result.Match(
            authSuccessDto => 
            {
                HttpContext.SetHttpOnlyRefreshToken(authSuccessDto.RefreshToken);
                return Ok(authSuccessDto.ToResponse());
            },
            failure => failure.ToActionResult()
        );
    }

    [HttpPost(ApiRoutes.Identity.Logout)]
    public IActionResult Logout()
    {
        HttpContext.Response.Cookies.Delete("refreshToken");
        return NoContent();
    }

    [HttpGet(ApiRoutes.Identity.Me)]
    public async Task<IActionResult> Me()
    {
        if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken)) return Unauthorized();

        var result = await _identityService.MeAsync(refreshToken);

        return result.Match(
            authSuccessDto => Ok(authSuccessDto.ToResponse()),
            failure => failure.ToActionResult()
        );
    }
}