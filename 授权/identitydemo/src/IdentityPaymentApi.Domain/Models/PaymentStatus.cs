namespace IdentityPaymentApi.Domain.Models;

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Cancelled,
    Rejected
}
