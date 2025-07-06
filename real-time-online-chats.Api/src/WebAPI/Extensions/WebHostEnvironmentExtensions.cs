namespace real_time_online_chats.Server.Extensions;

public static class WebHostEnvironmentExtensions
{
    public static bool IsDocker(this IWebHostEnvironment webHostEnvironment) => webHostEnvironment.IsEnvironment("Docker");
}
