using IdentityPaymentApi.Domain.Models;

namespace IdentityPaymentApi.Application.DTOs;

public class PaymentStatusUpdateRequest
{
    public PaymentStatus Status { get; set; }
    public string? Notes { get; set; }
}
