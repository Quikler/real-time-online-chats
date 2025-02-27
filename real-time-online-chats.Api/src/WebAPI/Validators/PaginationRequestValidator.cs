using FluentValidation;
using real_time_online_chats.Server.Contracts.V1.Requests;

namespace real_time_online_chats.Server.Validators;

public class PaginationRequestValidator : AbstractValidator<PaginationRequest>
{
    public PaginationRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 10).WithMessage("Page size must be between 1 and 10");
    }
}