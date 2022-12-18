using MediatR;
using Project.Application.Configuration.Processing;
using Project.Application.Payments.SendEmailAfterPayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Payments
{
    public class PaymentCreatedNotificationHandler:INotificationHandler<PaymentCreatedNotification>
    {
        private readonly ICommandsScheduler _commandsScheduler;

        public PaymentCreatedNotificationHandler(ICommandsScheduler commandsScheduler)
        {
            _commandsScheduler = commandsScheduler;
        }

        public async Task Handle(PaymentCreatedNotification request, CancellationToken cancellationToken)
        {
            await _commandsScheduler.EnqueueAsync(
                new SendEmailAfterPaymentCommand(Guid.NewGuid(), request.PaymentId));
        }
    }
}
