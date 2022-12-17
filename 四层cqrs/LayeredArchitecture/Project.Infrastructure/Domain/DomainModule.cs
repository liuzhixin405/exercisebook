using Autofac;
using Project.Application.Customers.DomainServices;
using Project.Domain.Customers;
using Project.Domain.ForeignExchange;
using Project.Infrastructure.Domain.ForeignExchanges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Domain
{
    public class DomainModule:Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CustomerUniquenessChecker>()
            .As<ICustomerUniquenessChecker>()
            .InstancePerLifetimeScope();

            builder.RegisterType<ForeignExchange>()
                .As<IForeignExchange>()
                .InstancePerLifetimeScope();
        }
    }
}
