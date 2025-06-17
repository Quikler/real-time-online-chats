using FluentValidation;
using real_time_online_chats.Server.Contracts.V1.Requests.Message;

namespace real_time_online_chats.Server.Validators.Message;

public class CreateMessageRequestValidator : AbstractValidator<CreateMessageRequest>
{
    public CreateMessageRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Message is required");
    }
}
