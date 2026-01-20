using System;
namespace IdentityPaymentApi.Domain.Models;

public class PaymentRecord
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentMethod Method { get; set; }
    public string? Description { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
}
