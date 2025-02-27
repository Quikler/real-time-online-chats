using FluentValidation;
using real_time_online_chats.Server.Contracts.V1.Requests.Auth;

namespace real_time_online_chats.Server.Validators.Auth;

public partial class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email should be in correct format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }
}