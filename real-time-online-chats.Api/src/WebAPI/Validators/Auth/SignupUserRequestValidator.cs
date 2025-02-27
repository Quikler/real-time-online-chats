using System.Text.RegularExpressions;
using FluentValidation;
using real_time_online_chats.Server.Contracts.V1.Requests.Auth;

namespace real_time_online_chats.Server.Validators.Auth;

public partial class SignupUserRequestValidator : AbstractValidator<SignupUserRequest>
{
    public SignupUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email should be in correct format");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .NotNull().WithMessage("Phone number is required")
            .MinimumLength(10).WithMessage("Phone number must not be less than 10 characters")
            .MaximumLength(20).WithMessage("Phone number must not exceed 50 characters")
            .Matches(PhoneNumberRegex()).WithMessage("Phone number not valid");

        RuleFor(x => x.Password)
            .Equal(x => x.ConfirmPassword).WithMessage("Passwords should match");
    }

    [GeneratedRegex(@"(\+\d{1,2}\s)?\d{3}[ -]?\d{3}[ -]?\d{4}")]
    private static partial Regex PhoneNumberRegex();
}