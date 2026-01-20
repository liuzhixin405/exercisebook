using Microsoft.EntityFrameworkCore;
using IdentityPaymentApi.Domain.Models;

namespace IdentityPaymentApi.Models;

// REMOVE: PaymentsDbContext 只在 Domain 层定义
// public class PaymentsDbContext : DbContext
// {
//     public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options)
//         : base(options)
//     {
//     }
//
//     public DbSet<PaymentRecord> Payments => Set<PaymentRecord>();
// }
