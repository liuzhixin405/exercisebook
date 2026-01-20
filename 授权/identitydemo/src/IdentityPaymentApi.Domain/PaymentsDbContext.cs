using Microsoft.EntityFrameworkCore;
using IdentityPaymentApi.Domain.Models;

namespace IdentityPaymentApi.Domain;

public class PaymentsDbContext : DbContext
{
    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
        : base(options)
    {
    }

    public DbSet<PaymentRecord> Payments => Set<PaymentRecord>();
}
