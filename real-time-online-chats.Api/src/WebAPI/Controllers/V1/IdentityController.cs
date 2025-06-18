using Microsoft.AspNetCore.Mvc;
using real_time_online_chats.Server.Attributes;
using real_time_online_chats.Server.Contracts.V1;
using real_time_online_chats.Server.Contracts.V1.Requests.Auth;
using real_time_online_chats.Server.Contracts.V1.Responses;
using real_time_online_chats.Server.Extensions;
using real_time_online_chats.Server.Mapping;
using real_time_online_chats.Server.Services.Identity;
using real_time_online_chats.Server.Services.Mail;

namespace real_time_online_chats.Server.Controllers.V1;

public class IdentityController(IIdentityService identityService, IMailService mailService) : ControllerBase
{
    [HttpGet(ApiRoutes.Identity.ConfirmEmail)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest request)
    {
        var result = await identityService.ConfirmEmailAsync(request.UserId, request.Token);

        return result.Match(
            success => Ok("Email confirmed successfully. Now you can login."),
            failure => failure.ToActionResult()
        );
    }

    [ReCAPTCHA]
    [HttpPost(ApiRoutes.Identity.Signup)]
    public async Task<IActionResult> Signup([FromBody] SignupUserRequest request)
    {
        var result = await identityService.SignupAsync(request.ToDto());

        return await result.MatchAsync(
            async emailConfirmDto =>
            {
                var confirmationLink = Url.ActionLink(
                    nameof(ConfirmEmail),
                    values: new { userId = emailConfirmDto.UserId, token = emailConfirmDto.Token });

                var sent = await mailService.SendMessageAsync([request.Email], "Confirm your email",
                    $"""
                    <div style="background: linear-gradient(90deg, black, #3903f9);text-align: center;color: white;padding: 32px;">
                        <h1 style="margin: 0;">Welcome to <span style="color:cc;background-image: linear-gradient(180deg, #00ffab, #e22bac);color: transparent;background-clip: text;">ROC</span>!</h1>
                        <p style="margin: 0;">Thank you for signing up to ROC.</p>
                        <p style="margin: 0;margin-bottom: 16px;">Please confirm your email by clicking the link below.</p>
                        <a href="{confirmationLink}" style="font-style: italic; color: aquamarine;">Click here to confirm your email.</a>
                    </div>
                    """);

                return sent
                    ? Ok($"Account created. Before login please confirm your email. [Link only for tests]: {confirmationLink}")
                    : BadRequest(new FailureResponse(["Failed to send confirmation email."]));
            },
            failure => failure.ToActionResult()
        );
    }

    [ReCAPTCHA]
    [HttpPost(ApiRoutes.Identity.Login)]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var loginUser = request.ToDto();

        var result = await identityService.LoginAsync(loginUser);

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

        var result = await identityService.RefreshTokenAsync(refreshToken);

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

        var result = await identityService.MeAsync(refreshToken);

        return result.Match(
            authSuccessDto => Ok(authSuccessDto.ToResponse()),
            failure => failure.ToActionResult()
        );
    }
}
