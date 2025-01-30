namespace real_time_online_chats.Server.Contracts.V1;

public static class ApiRoutes
{
    public const string ROOT = "api";
    public const string VERSION = "v1";

    public const string BASE = $"{ROOT}/{VERSION}";

    public static class Identity
    {
        public const string Login = BASE + "/identity/login";
        public const string Signup = BASE + "/identity/signup";
        public const string Refresh = BASE + "/identity/refresh";
        public const string Logout = BASE + "/identity/logout";
        public const string Me = BASE + "/identity/me";
    }

    public static class Chats
    {
        public const string GetAll = BASE + "/chats";
        public const string Get = BASE + "/chats/{chatId}";
        public const string GetDetailed = BASE + "/chats/{chatId}/detailed";
        public const string Create = BASE + "/chats";
        public const string Update = BASE + "/chats/{chatId}";
        public const string Delete = BASE + "/chats/{chatId}";
        public const string Join = BASE + "/chats/{chatId}/join";
        public const string Leave = BASE + "/chats/{chatId}/leave";
    }

    public static class Messages
    {
        public const string GetAll = BASE + "/messages";
        public const string Get = BASE + "/messages/{messageId}";
        public const string Create = BASE + "/messages";
        public const string Update = BASE + "/messages/{messageId}";
        public const string Delete = BASE + "/messages/{messageId}";
    }
}