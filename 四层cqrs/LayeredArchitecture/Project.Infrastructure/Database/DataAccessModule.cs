using Autofac;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;
using Project.Application.Configuration.Data;
using Project.Domain.Customers.Orders;
using Project.Domain.Payments;
using Project.Domain.Products;
using Project.Domain.SeedWork;
using Project.Infrastructure.Domain.Customers;
using Project.Infrastructure.Domain.Payments;
using Project.Infrastructure.Domain.Products;
using Project.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.Infrastructure.SeedWork;

namespace Project.Infrastructure.Database
{
    public class DataAccessModule:Module
    {
        private readonly string _databaseConnectionString;
        public DataAccessModule(string databaseConnectionString)
        {
            _databaseConnectionString = databaseConnectionString;
        }
        protected override void Load(ContainerBuilder builder)
        {
           builder.RegisterType<SqlConnectionFactory>().As<ISqlConnectionFactory>().WithParameter("connectionString",_databaseConnectionString)
                .InstancePerLifetimeScope() ;

            builder.RegisterType<UnitOfWork>()
              .As<IUnitOfWork>()
              .InstancePerLifetimeScope();


            builder.RegisterType<CustomerRepository>()
                .As<ICustomerRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<ProductRepository>()
                .As<IProductRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<PaymentRepository>()
                .As<IPaymentRepository>()
                .InstancePerLifetimeScope();

            builder.RegisterType<StronglyTypedIdValueConverterSelector>()
                .As<IValueConverterSelector>()
                .SingleInstance();

            builder
                .Register(c =>
                {
                    var dbContextOptionsBuilder = new DbContextOptionsBuilder<OrdersContext>();
                    dbContextOptionsBuilder.UseSqlServer(_databaseConnectionString);
                    dbContextOptionsBuilder
                        .ReplaceService<IValueConverterSelector, StronglyTypedIdValueConverterSelector>();

                    return new OrdersContext(dbContextOptionsBuilder.Options);
                })
                .AsSelf()
                .As<DbContext>()
                .InstancePerLifetimeScope();
        }

    }
}
