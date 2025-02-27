using FluentValidation;
using real_time_online_chats.Server.Contracts.V1.Requests.Message;

namespace real_time_online_chats.Server.Validators.Message;

public class UpdateMessageRequestValidator : AbstractValidator<UpdateMessageRequest>
{
    public UpdateMessageRequestValidator()
    {
        RuleFor(x => x.ChatId)
            .NotNull().WithMessage("Chat Id is required");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Message is required");
    }
}