using FluentValidation;
using IdentityPaymentApi.Application.DTOs;

namespace IdentityPaymentApi.Application.Validators;

public class PaymentQueryParametersValidator : AbstractValidator<PaymentQueryParameters>
{
    public PaymentQueryParametersValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber must be at least 1.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than zero.")
            .LessThanOrEqualTo(100).WithMessage("PageSize cannot exceed 100.");

        When(x => x.CreatedAfter.HasValue && x.CreatedBefore.HasValue, () =>
        {
            RuleFor(x => x.CreatedBefore)
                .GreaterThanOrEqualTo(x => x.CreatedAfter!.Value)
                .WithMessage("CreatedBefore must be after CreatedAfter.");
        });
    }
}
