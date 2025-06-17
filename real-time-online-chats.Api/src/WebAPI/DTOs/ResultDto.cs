//using real_time_online_chats.Server.Extensions;

namespace real_time_online_chats.Server.DTOs;

public readonly struct ResultDto<TValue>
{
    private readonly TValue? _value;
    private readonly FailureDto? _error;

    private ResultDto(TValue value)
    {
        IsError = false;
        _value = value;
        _error = default;
    }

    private ResultDto(FailureDto error)
    {
        IsError = true;
        _value = default;
        _error = error;
    }

    public bool IsError { get; }
    public bool IsSuccess => !IsError;

    public static implicit operator ResultDto<TValue>(TValue value) => new(value);
    public static implicit operator ResultDto<TValue>(FailureDto error) => new(error);

    public TResult Match<TResult>(Func<TValue, TResult> success, Func<FailureDto, TResult> failure)
        => IsSuccess ? success(_value!) : failure(_error!);

    public Task<TResult> MatchAsync<TResult>(Func<TValue, Task<TResult>> success, Func<FailureDto, TResult> failure)
        => IsSuccess ? success(_value!) : Task.FromResult(failure(_error!));

    //public TResult MatchSuccess<TResult>(Func<TValue, TResult> success)
        //=> IsSuccess ? success(_value!) : _error.ToActionResult();
}
