using Autofac;
using MediatR;
using Project.Application.Configuration.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Project.Infrastructure.Processing
{
    public static class QueriesExecutor
    {
        public static async Task<TResult> Execute<TResult>(IQuery<TResult> query)
        {
            using(var scope = CompositionRoot.BeginLifetimeScope())
            {
                var mediator = scope.Resolve<IMediator>();
                return await mediator.Send<TResult>(query);
            }
        }
    }
}
