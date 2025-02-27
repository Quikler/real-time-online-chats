using FluentValidation;
using real_time_online_chats.Server.Contracts.V1.Requests.Chat;

namespace real_time_online_chats.Server.Validators.Chat;

public class CreateChatRequestValidator : AbstractValidator<CreateChatRequest>
{
    public CreateChatRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required");
    }
}