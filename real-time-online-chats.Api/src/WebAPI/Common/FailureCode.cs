namespace real_time_online_chats.Server.Common;

public enum FailureCode
{
    BadRequest = 400,
    Unauthorized = 401,
    //PaymentRequired
    Forbidden = 403,
    NotFound = 404,
    Conflict = 409,
    InternalServer = 500,
}