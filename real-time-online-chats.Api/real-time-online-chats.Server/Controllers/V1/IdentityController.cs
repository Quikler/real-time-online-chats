using Microsoft.AspNetCore.Mvc;
using real_time_online_chats.Server.Contracts.V1;
using real_time_online_chats.Server.Contracts.V1.Requests.Auth;
using real_time_online_chats.Server.Contracts.V1.Responses.Auth;
using real_time_online_chats.Server.Domain;
using real_time_online_chats.Server.Extensions;
using real_time_online_chats.Server.Services.Identity;

namespace real_time_online_chats.Server.Controllers.V1;

public class IdentityController(IIdentityService identityService) : ControllerBase
{
    private readonly IIdentityService _identityService = identityService;

    [HttpPost(ApiRoutes.Identity.Signup)]
    public async Task<IActionResult> Signup([FromBody] UserSignupRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new AuthFailedResponse
            {
                Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage)),
            });
        }

        var signupUser = new SignupUser
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Password = request.Password,
            Phone = request.Phone,
            RememberMe = request.RememberMe,
        };

        var authResponse = await _identityService.SignupAsync(signupUser);

        if (!authResponse.Succeded)
        {
            return BadRequest(new AuthFailedResponse
            {
                Errors = authResponse.Errors,
            });
        }

        HttpContext.SetHttpOnlyRefreshToken(authResponse.RefreshToken);

        return Ok(new AuthSuccessResponse
        {
            Token = authResponse.Token,
            RefreshToken = authResponse.RefreshToken,
            User = new UserResponse 
            {
                Email = authResponse.User.Email,
                FirstName = authResponse.User.FirstName,
                LastName = authResponse.User.LastName,
            }
        });
    }

    [HttpPost(ApiRoutes.Identity.Login)]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new AuthFailedResponse
            {
                Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage)),
            });
        }

        var loginUser = new LoginUser
        {
            Email = request.Email,
            Password = request.Password,
        };

        var authResponse = await _identityService.LoginAsync(loginUser);

        if (!authResponse.Succeded)
        {
            return BadRequest(new AuthFailedResponse
            {
                Errors = authResponse.Errors,
            });
        }

        HttpContext.SetHttpOnlyRefreshToken(authResponse.RefreshToken);

        return Ok(new AuthSuccessResponse
        {
            Token = authResponse.Token,
            RefreshToken = authResponse.RefreshToken,
            User = new UserResponse 
            {
                Email = authResponse.User.Email,
                FirstName = authResponse.User.FirstName,
                LastName = authResponse.User.LastName,
            }
        });
    }

    [HttpPost(ApiRoutes.Identity.Refresh)]
    public async Task<IActionResult> Refresh()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new AuthFailedResponse
            {
                Errors = ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage)),
            });
        }

        if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken)) return Unauthorized();
        
        var authResponse = await _identityService.RefreshTokenAsync(refreshToken);

        if (!authResponse.Succeded)
        {
            return BadRequest(new AuthFailedResponse
            {
                Errors = authResponse.Errors,
            });
        }

        HttpContext.SetHttpOnlyRefreshToken(authResponse.RefreshToken);

        return Ok(new AuthSuccessResponse
        {
            Token = authResponse.Token,
            RefreshToken = authResponse.RefreshToken,
            User = new UserResponse 
            {
                Email = authResponse.User.Email,
                FirstName = authResponse.User.FirstName,
                LastName = authResponse.User.LastName,
            }
        });
    }

    [HttpGet(ApiRoutes.Identity.Me)]
    public async Task<IActionResult> Me() 
    {
        if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken)) return Unauthorized();
        
        var me = await _identityService.MeAsync(refreshToken);
        return Ok(me);
    }
}