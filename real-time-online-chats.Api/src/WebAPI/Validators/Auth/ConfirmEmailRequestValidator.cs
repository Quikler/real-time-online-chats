using FluentValidation;
using real_time_online_chats.Server.Contracts.V1.Requests.Auth;

namespace real_time_online_chats.Server.Validators.Auth;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User Id is required");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required");
    }
}