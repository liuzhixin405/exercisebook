using IdentityPaymentApi.Domain.Models;

namespace IdentityPaymentApi.Application.DTOs;

public class PaymentCreateRequest
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentMethod Method { get; set; }
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public bool RequiresConfirmation { get; set; }
}
