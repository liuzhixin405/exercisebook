using System;
using System.Threading.Tasks;
using IdentityPaymentApi.Application.DTOs;
using IdentityPaymentApi.Application.Wrappers;
using IdentityPaymentApi.Domain.Models;

namespace IdentityPaymentApi.Application.Services;

public interface IPaymentService
{
    Task<BaseResult<PaymentResponse>> CreatePaymentAsync(BusinessUser user, PaymentCreateRequest request);
    Task<PagedResponse<PaymentResponse>> GetPaymentsAsync(BusinessUser user, PaymentQueryParameters query);
    Task<BaseResult<PaymentResponse>> GetPaymentAsync(BusinessUser user, Guid paymentId);
    Task<BaseResult<PaymentResponse>> UpdatePaymentStatusAsync(BusinessUser user, Guid paymentId, PaymentStatusUpdateRequest request);
}
