namespace real_time_online_chats.Server.Contracts.V1;

public static class ApiRoutes
{
    public const string ROOT = "/api";
    public const string VERSION = "v1";

    public const string BASE = $"{ROOT}/{VERSION}";

    public static class Identity
    {
        public const string ConfirmEmail = BASE + "/identity/email";
        public const string Login = BASE + "/identity/login";
        public const string Signup = BASE + "/identity/signup";
        public const string Refresh = BASE + "/identity/refresh";
        public const string Logout = BASE + "/identity/logout";
        public const string Me = BASE + "/identity/me";
    }

    public static class Google
    {
        public const string Login = BASE + "/google/login";
        public const string Signup = BASE + "/google/signup";
    }

    public static class Chats
    {
        public const string GetAll = BASE + "/chats";
        public const string Get = BASE + "/chats/{chatId}";
        public const string Create = BASE + "/chats";
        public const string GetInfo = BASE + "/chats/{chatId}/info";
        public const string UpdateTitle = BASE + "/chats/{chatId}";
        public const string UpdateOwner = BASE + "/chats/{chatId}/owner";
        public const string Delete = BASE + "/chats/{chatId}";
        public const string AddMemberMe = BASE + "/chats/{chatId}/members/me";
        public const string DeleteMemberMe = BASE + "/chats/{chatId}/members/me";
        public const string DeleteMember = BASE + "/chats/{chatId}/members/{memberId}";
    }

    public static class Messages
    {
        public const string GetAll = BASE + "/messages";
        public const string Get = BASE + "/messages/{messageId}";
        public const string Create = BASE + "/messages";
        public const string Update = BASE + "/messages/{messageId}";
        public const string Delete = BASE + "/messages/{messageId}";
    }

    public static class ChatUsers
    {
        public const string GetAll = BASE + "/chats/{chatId}/users";
        public const string Get = BASE + "/chats/{chatId}/users/{userId}";
        public const string AddUser = BASE + "/chats/{chatId}/users/{userId}";
        public const string AddUserMe = BASE + "/chats/{chatId}/users/me";
        public const string DeleteUser = BASE + "/chats/{chatId}/users/{userId}";
        public const string DeleteUserMe = BASE + "/chats/{chatId}/users/me";
    }

    public static class Users
    {
        public const string GetProfile = BASE + "/users/{userId}/profile";
        public const string EditProfile = BASE + "/users/{userId}/profile";

        public const string GetOwnerChats = BASE + "/users/me/owner-chats";
        public const string GetMemberChats = BASE + "/users/me/member-chats";
    }
}
