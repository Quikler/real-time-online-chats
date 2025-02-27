using FluentValidation;
using real_time_online_chats.Server.Contracts.V1.Requests.Chat;

namespace real_time_online_chats.Server.Validators.Chat;

public class ChatsPaginationRequestValidator : AbstractValidator<ChatsPaginationRequest>
{
    public ChatsPaginationRequestValidator()
    {
        Include(new PaginationRequestValidator()); // Reuse PaginationRequestValidator rules

        RuleFor(x => x.CountOfMessages)
            .InclusiveBetween(2, 5).WithMessage("Count of messages should be between 2 and 5");
    }
}