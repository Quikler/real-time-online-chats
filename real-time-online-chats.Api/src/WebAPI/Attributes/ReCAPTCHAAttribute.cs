using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using real_time_online_chats.Server.Common.Constants;
using real_time_online_chats.Server.Configurations;
using real_time_online_chats.Server.Contracts.V1.Responses;
using real_time_online_chats.Server.Contracts.V1.Responses.Google;

namespace real_time_online_chats.Server.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ReCAPTCHAAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderConstants.ReCAPTCHAToken, out var token) || string.IsNullOrWhiteSpace(token))
        {
            context.Result = new BadRequestObjectResult(new FailureResponse("Captcha token is missing"));
            return;
        }

        var reCAPTCHAConfiguration = context.HttpContext.RequestServices.GetRequiredService<IOptions<ReCAPTCHAConfiguration>>().Value;

        try
        {
            var httpClientFactory = context.HttpContext.RequestServices.GetRequiredService<IHttpClientFactory>();
            using var hc = httpClientFactory.CreateClient(HttpClientNameConstants.GoogleRecaptchaApi);

            var verifyResponse = await hc.PostAsync(
                $"siteverify?secret={reCAPTCHAConfiguration.Secret}&response={token}",
                null
            );

            var verifyContent = await verifyResponse.Content.ReadFromJsonAsync<ReCAPTCHAResponse>();
            if (verifyContent is null || !verifyContent.Success)
            {
                context.Result = new BadRequestObjectResult(new FailureResponse("Captcha token is invalid"));
                return;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occured when verifying reCAPTCHA: {e.Message}");
            return;
        }

        await next();
    }
}
