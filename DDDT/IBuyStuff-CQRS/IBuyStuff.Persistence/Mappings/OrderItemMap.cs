
using IBuyStuff.Domain.Customers;
using IBuyStuff.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IBuyStuff.Persistence.Mappings
{
    public class OrderItemMap : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id)
                .IsRequired()
                .HasColumnName("Id");

            // Properties			
            //builder.Property(t => t.Quantity)
            //    .IsRequired()
            //    .HasColumnName("Quantity");

            // Table and relationships 
            builder.ToTable("OrderItems");
            //builder.Property(o => o.Order).IsRequired();
            //builder.Property(o => o.Product).IsRequired();
        }
    }
}
