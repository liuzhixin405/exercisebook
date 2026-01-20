using System.ComponentModel.DataAnnotations;
using IdentityPaymentApi.Domain.Models;

namespace IdentityPaymentApi.Application.DTOs;

public class PaymentRequest
{
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public string Currency { get; set; } = "USD";

    [Required]
    public PaymentMethod Method { get; set; }

    public string? Description { get; set; }
}
