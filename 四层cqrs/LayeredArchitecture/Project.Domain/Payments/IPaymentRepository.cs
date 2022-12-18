using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Payments
{
    public interface IPaymentRepository
    {
        Task<Payment> GetByIdAsync(PaymentId id);
        Task AddAsync(Payment payment);
    }
}
