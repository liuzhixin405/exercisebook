namespace IdentityPaymentApi.Models;

// REMOVE: PaymentRecord now lives in Domain.Models
// public class PaymentRecord
// {
//     public Guid Id { get; set; }
//     public decimal Amount { get; set; }
//     public string Currency { get; set; } = "USD";
//     public PaymentMethod Method { get; set; }
//     public string? Description { get; set; }
//     public string Status { get; set; } = "Processed";
//     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//     public string UserId { get; set; } = default!;
// }