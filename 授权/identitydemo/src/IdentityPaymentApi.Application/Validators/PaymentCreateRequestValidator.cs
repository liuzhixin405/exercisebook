using FluentValidation;
using IdentityPaymentApi.Application.DTOs;

namespace IdentityPaymentApi.Application.Validators;

public class PaymentCreateRequestValidator : AbstractValidator<PaymentCreateRequest>
{
    public PaymentCreateRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .MaximumLength(12).WithMessage("Currency max length is 12 characters.");

        RuleFor(x => x.Method)
            .IsInEnum().WithMessage("A valid payment method is required.");
    }
}
