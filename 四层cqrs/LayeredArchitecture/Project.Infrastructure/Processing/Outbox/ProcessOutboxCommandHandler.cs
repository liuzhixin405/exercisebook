using Dapper;
using MediatR;
using Project.Application.Configuration.Commands;
using Project.Application.Configuration.Data;
using Project.Application.Configuration.DomainEvents;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Processing.Outbox
{
    public class ProcessOutboxCommandHandler : ICommandHandler<ProcessOutboxCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly ISqlConnectionFactory _sqlConnectionFactory;
        public ProcessOutboxCommandHandler(IMediator mediator, ISqlConnectionFactory sqlConnectionFactory)
        {
            this._mediator = mediator;
            this._sqlConnectionFactory = sqlConnectionFactory;
        }
        public async Task<Unit> Handle(ProcessOutboxCommand command, CancellationToken cancellationToken)
        {
            var connection = this._sqlConnectionFactory.GetOpenConnection();
            const string sql = "SELECT " +
                               "[OutboxMessage].[Id], " +
                               "[OutboxMessage].[Type], " +
                               "[OutboxMessage].[Data] " +
                               "FROM [app].[OutboxMessages] AS [OutboxMessage] " +
                               "WHERE [OutboxMessage].[ProcessedDate] IS NULL";

            var messages = await connection.QueryAsync<OutboxMessageDto>(sql);
            var messagesList = messages.AsList();

            const string sqlUpdateProcessedDate = "UPDATE [app].[OutboxMessages] " +
                                                  "SET [ProcessedDate] = @Date " +
                                                  "WHERE [Id] = @Id";
            if (messagesList.Count > 0)
            {
                foreach (var message in messagesList)
                {
                    Type type = Assemblies.Application.GetType(message.Type)??throw new Exception(nameof(message));
                    IDomainEventNotification request =(IDomainEventNotification) System.Text.Json.JsonSerializer.Deserialize(message.Data, type);

                    using (LogContext.Push(new OutboxMessageContextEnricher(request)))
                    {
                        await this._mediator.Publish(request, cancellationToken);

                        await connection.ExecuteAsync(sqlUpdateProcessedDate, new
                        {
                            Date = DateTime.UtcNow,
                            message.Id
                        });
                    }

                }
            }

            return Unit.Value;
        }
        private class OutboxMessageContextEnricher : ILogEventEnricher
        {
            private readonly IDomainEventNotification _notification;

            public OutboxMessageContextEnricher(IDomainEventNotification notification)
            {
                _notification = notification;
            }
            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                logEvent.AddOrUpdateProperty(new LogEventProperty("Context", new ScalarValue($"OutboxMessage:{_notification.Id.ToString()}")));
            }
        }
    }
}
