using IdentityPaymentApi.Application.Services;
using IdentityPaymentApi.Application.DTOs;
using IdentityPaymentApi.Application.Wrappers;
using IdentityPaymentApi.Domain.Models;
using IdentityPaymentApi.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

    public async Task<BaseResult<PaymentResponse>> CreatePaymentAsync(BusinessUser user, PaymentCreateRequest request)
    {
        var record = new PaymentRecord
        {
            Id = Guid.NewGuid(),
            Amount = request.Amount,
            Currency = request.Currency,
            Method = request.Method,
            Description = request.Description,
            UserId = user.Id,
            Reference = string.IsNullOrWhiteSpace(request.Reference) ? Guid.NewGuid().ToString("N") : request.Reference,
            Status = request.RequiresConfirmation ? PaymentStatus.Pending : PaymentStatus.Completed,
            ProcessedAt = request.RequiresConfirmation ? null : DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(record);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created payment {PaymentId} for user {UserId} with status {Status}", record.Id, user.Id, record.Status);

        return BaseResult<PaymentResponse>.Ok(ToResponse(record));
    }

    public async Task<PagedResponse<PaymentResponse>> GetPaymentsAsync(BusinessUser user, PaymentQueryParameters query)
    {
        var baseFilter = _context.Payments.AsNoTracking().Where(p => p.UserId == user.Id);
        var filtered = ApplyFilters(baseFilter, query);

        var totalCount = await filtered.CountAsync();
        var items = await filtered
            .OrderByDescending(p => p.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var dtoList = items.Select(ToResponse).ToList();
        var pagination = new PaginationResponseDto<PaymentResponse>(dtoList, totalCount, query.PageNumber, query.PageSize);

        return PagedResponse<PaymentResponse>.Ok(pagination);
    }

    public async Task<BaseResult<PaymentResponse>> GetPaymentAsync(BusinessUser user, Guid paymentId)
    {
        var record = await _context.Payments.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == user.Id);

        if (record == null)
            return BaseResult<PaymentResponse>.Failure(new Error(ErrorCode.NotFound, "Payment record not found."));

        return BaseResult<PaymentResponse>.Ok(ToResponse(record));
    }

    public async Task<BaseResult<PaymentResponse>> UpdatePaymentStatusAsync(BusinessUser user, Guid paymentId, PaymentStatusUpdateRequest request)
    {
        var record = await _context.Payments.FirstOrDefaultAsync(p => p.Id == paymentId && p.UserId == user.Id);

        if (record == null)
            return BaseResult<PaymentResponse>.Failure(new Error(ErrorCode.NotFound, "Payment record not found."));

        if (record.Status != PaymentStatus.Pending && request.Status == PaymentStatus.Pending)
            return BaseResult<PaymentResponse>.Failure(new Error(ErrorCode.FieldDataInvalid, "Cannot revert to pending status."));

        record.Status = request.Status;
        record.StatusNotes = request.Notes;
        record.ProcessedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Payment {PaymentId} status updated to {Status} by user {UserId}", record.Id, record.Status, user.Id);

        return BaseResult<PaymentResponse>.Ok(ToResponse(record));
    }

    private static IQueryable<PaymentRecord> ApplyFilters(IQueryable<PaymentRecord> query, PaymentQueryParameters parameters)
    {
        if (parameters.Status.HasValue)
            query = query.Where(p => p.Status == parameters.Status.Value);

        if (parameters.CreatedAfter.HasValue)
            query = query.Where(p => p.CreatedAt >= parameters.CreatedAfter.Value);

        if (parameters.CreatedBefore.HasValue)
            query = query.Where(p => p.CreatedAt <= parameters.CreatedBefore.Value);

        return query;
    }

    private static PaymentResponse ToResponse(PaymentRecord record) => new()
    {
        Id = record.Id,
        Amount = record.Amount,
        Currency = record.Currency,
        Method = record.Method,
        Description = record.Description,
        Status = record.Status,
        StatusNotes = record.StatusNotes,
        Reference = record.Reference,
        CreatedAt = record.CreatedAt,
        ProcessedAt = record.ProcessedAt
    };
}
