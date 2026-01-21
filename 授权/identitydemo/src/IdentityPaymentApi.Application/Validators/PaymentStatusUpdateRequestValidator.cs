using FluentValidation;
using IdentityPaymentApi.Application.DTOs;
using IdentityPaymentApi.Domain.Models;

namespace IdentityPaymentApi.Application.Validators;

public class PaymentStatusUpdateRequestValidator : AbstractValidator<PaymentStatusUpdateRequest>
{
    public PaymentStatusUpdateRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("A valid payment status is required.")
            .Must(status => status != PaymentStatus.Pending)
            .WithMessage("Only final states can be set explicitly.");
    }
}
