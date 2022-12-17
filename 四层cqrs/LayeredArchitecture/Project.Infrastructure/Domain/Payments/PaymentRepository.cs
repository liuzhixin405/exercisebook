using Microsoft.EntityFrameworkCore;
using Project.Domain.Payments;
using Project.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Domain.Payments
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly OrdersContext _;
        public PaymentRepository(OrdersContext context)
        {
            _ = context??throw new ArgumentNullException(nameof(context));
        }
        public async Task AddAssync(Payment payment)
        {
            await _.Payments.AddAsync(payment);
        }

        public async Task<Payment> GetByIdAsync(PaymentId id)
        {
           return await _.Payments.SingleAsync(x=>x.Id== id);
        }
    }
}
