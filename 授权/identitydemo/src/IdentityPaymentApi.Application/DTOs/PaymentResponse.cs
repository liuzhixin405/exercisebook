using System;
using IdentityPaymentApi.Domain.Models;

namespace IdentityPaymentApi.Application.DTOs;

public class PaymentResponse
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public PaymentMethod Method { get; set; }
    public string? Description { get; set; }
    public PaymentStatus Status { get; set; }
    public string? StatusNotes { get; set; }
    public string Reference { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}
