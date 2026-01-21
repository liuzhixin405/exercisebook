using System;
using IdentityPaymentApi.Domain.Models;

namespace IdentityPaymentApi.Application.DTOs;

public class PaymentQueryParameters
{
    public PaymentStatus? Status { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }

    private const int MaxPageSize = 100;
    private int _pageSize = 20;

    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value is <= 0 ? 20 : Math.Min(MaxPageSize, value);
    }
}
