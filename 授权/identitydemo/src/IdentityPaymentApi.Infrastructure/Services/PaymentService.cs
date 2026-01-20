using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityPaymentApi.Application.Services;
using IdentityPaymentApi.Application.DTOs;
using IdentityPaymentApi.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using IdentityPaymentApi.Domain;

namespace IdentityPaymentApi.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly PaymentsDbContext _context;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(PaymentsDbContext context, ILogger<PaymentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaymentRecord> CreatePaymentAsync(BusinessUser user, PaymentRequest request)
    {
        var record = new PaymentRecord
        {
            Id = Guid.NewGuid(),
            Amount = request.Amount,
            Currency = request.Currency,
            Method = request.Method,
            Description = request.Description,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(record);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created payment {PaymentId} for user {UserId}", record.Id, user.Id);

        return record;
    }

    public async Task<IEnumerable<PaymentResponse>> GetPaymentsAsync(BusinessUser user)
    {
        var records = await _context.Payments
            .Where(p => p.UserId == user.Id)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return records.Select(ToResponse);
    }

    public async Task<PaymentResponse?> GetPaymentAsync(BusinessUser user, Guid paymentId)
    {
        var record = await _context.Payments
            .FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == user.Id);

        return record is null ? null : ToResponse(record);
    }

    private static PaymentResponse ToResponse(PaymentRecord record) => new()
    {
        Id = record.Id,
        Amount = record.Amount,
        Currency = record.Currency,
        Method = record.Method.ToString(),
        Description = record.Description,
        Status = record.Status,
        CreatedAt = record.CreatedAt
    };
}
