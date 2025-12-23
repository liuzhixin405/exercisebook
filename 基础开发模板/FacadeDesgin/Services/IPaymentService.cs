namespace FacadeDesgin.Services;

using FacadeDesgin.Models;

public interface IPaymentService
{
    Task<(bool success, string? transactionId)> ChargeAsync(PaymentInfo payment, decimal amount);
}
