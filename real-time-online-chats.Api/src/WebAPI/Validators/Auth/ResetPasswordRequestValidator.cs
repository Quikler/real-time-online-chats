using FluentValidation;
using real_time_online_chats.Server.Contracts.V1.Requests.Auth;

namespace real_time_online_chats.Server.Validators.Auth;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email should be in correct format");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Password is required");
    }
}
