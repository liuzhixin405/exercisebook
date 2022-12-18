using MediatR;
using Project.Application.Configuration.Commands;
using Project.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project.Application.Payments.SendEmailAfterPayment
{
    public class SendEmailAfterPaymentCommand:InternalCommandBase<Unit>
    {
        public PaymentId PaymentId { get; }

        [JsonConstructor]
        public SendEmailAfterPaymentCommand(Guid id, PaymentId paymentId) : base(id)
        {
            this.PaymentId = paymentId;
        }
    }
}
