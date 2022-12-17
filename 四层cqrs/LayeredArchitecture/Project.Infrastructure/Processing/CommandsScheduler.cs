using Dapper;
using Project.Application.Configuration.Commands;
using Project.Application.Configuration.Data;
using Project.Application.Configuration.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Processing
{
    public class CommandsScheduler:ICommandsScheduler
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public CommandsScheduler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task EnqueueAsync<T>(ICommand<T> command)
        {
            var connection = this._sqlConnectionFactory.GetOpenConnection();

            const string sqlInsert = "INSERT INTO [app].[InternalCommands] ([Id], [EnqueueDate] , [Type], [Data]) VALUES " +
                                     "(@Id, @EnqueueDate, @Type, @Data)";

            await connection.ExecuteAsync(sqlInsert, new
            {
                command.Id,
                EnqueueDate = DateTime.UtcNow,
                Type = command.GetType().FullName,
                Data = System.Text.Json.JsonSerializer.Serialize(command)
            });
        }
    }
}
