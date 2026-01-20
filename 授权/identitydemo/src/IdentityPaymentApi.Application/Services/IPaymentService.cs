using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityPaymentApi.Application.DTOs;
using IdentityPaymentApi.Domain.Models;

namespace IdentityPaymentApi.Application.Services;

public interface IPaymentService
{
    Task<PaymentRecord> CreatePaymentAsync(BusinessUser user, PaymentRequest request);
    Task<IEnumerable<PaymentResponse>> GetPaymentsAsync(BusinessUser user);
    Task<PaymentResponse?> GetPaymentAsync(BusinessUser user, Guid paymentId);
}
