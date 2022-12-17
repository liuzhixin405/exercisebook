using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Project.Domain.Customers;
using Project.Domain.Customers.Orders;
using Project.Domain.Products;
using Project.Domain.SharedKernel;
using Project.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Domain.Customers
{
    internal sealed class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
    {
        internal const string OrderList = "_orders";
        internal const string OrderProducts = "_orderProducts";
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("", SchemaNames.Orders);
            builder.HasKey(b => b.Id);
            builder.Property("_welcomeEmailWasSent").HasColumnName("WelcomeEmailWasSent");
            builder.Property("_email").HasColumnName("Email");
            builder.Property("_name").HasColumnName("Name");

            builder.OwnsMany<Order>(OrderList, x => {
                x.WithOwner().HasForeignKey("CustomerId");
                x.ToTable("Orders",SchemaNames.Orders);
                x.Property<bool>("_isRemoved").HasColumnName("IsRemoved");
                x.Property<DateTime?>("_orderChangeDate").HasColumnName("OrderChangeDate");
                x.Property<OrderId>("Id");
                x.HasKey("Id");

                x.Property("_status").HasColumnName("StatusId").HasConversion(new EnumToNumberConverter<OrderStatus, byte>());
                x.OwnsMany<OrderProduct>(OrderProducts, y =>
                {
                    y.WithOwner().HasForeignKey("OrderId");

                    y.ToTable("OrderProducts", SchemaNames.Orders);
                    y.Property<OrderId>("OrderId");
                    y.Property<ProductId>("ProductId");

                    y.HasKey("OrderId", "ProductId");

                    y.OwnsOne<MoneyValue>("ValueInEUR", mv =>
                    {
                        mv.Property(prop => prop.Currency).HasColumnName("CurrencyEUR");
                        mv.Property(prop => prop.Value).HasColumnName("ValueInEUR");
                    });
                });

                x.OwnsOne<MoneyValue>("_valueInEUR", y =>
                {
                    y.Property(p => p.Currency).HasColumnName("CurrencyEUR");
                    y.Property(p => p.Value).HasColumnName("ValueInEUR");
                });
            });
        }
    }
}
