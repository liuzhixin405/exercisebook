namespace FacadeDesgin.Services;

using FacadeDesgin.Models;

public class PaymentService : IPaymentService
{
    public Task<(bool success, string? transactionId)> ChargeAsync(PaymentInfo payment, decimal amount)
    {
        // simulate payment processing: decline if card number ends with '0'
        if (string.IsNullOrWhiteSpace(payment.CardNumber))
            return Task.FromResult((false, (string?)null));

        var last = payment.CardNumber.Trim().Last();
        if (last == '0')
            return Task.FromResult((false, (string?)null));

        return Task.FromResult((true, Guid.NewGuid().ToString()));
    }
}
