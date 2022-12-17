using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Project.Domain.Customers.Orders;
using Project.Domain.Payments;
using Project.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Domain.Payments
{
    internal sealed class PaymentEntityTypeConfiguration : IEntityTypeConfiguration<Payment>
    {

        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments", SchemaNames.Payments);
            builder.HasKey(b => b.Id);

            builder.Property<DateTime>("_createDate").HasColumnName("CreateDate");
            builder.Property<OrderId>("_orderId").HasColumnName("OrderId");
            builder.Property("_status").HasColumnName("StatusId").HasConversion(new EnumToNumberConverter<PaymentStatus, byte>());
            builder.Property<bool>("_emailNotificationIsSent").HasColumnName("EmailNotificationIsSent");
        }
    }
}
