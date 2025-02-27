using FluentValidation;
using real_time_online_chats.Server.Contracts.V1.Requests.Chat;

namespace real_time_online_chats.Server.Validators.Chat;

public class UpdateChatRequestValidator : AbstractValidator<UpdateChatRequest>
{
    public UpdateChatRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required");
    }
}