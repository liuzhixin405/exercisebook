
using IBuyStuff.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IBuyStuff.QueryModel.Persistence.Mappings
{
    public class OrderMap : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // Primary Key
            builder.HasKey(t => t.OrderId);

            // Table and relationships 
            builder.ToTable("Orders");
            //HasRequired(o => o.Buyer);
            builder.OwnsMany(o => o.Items);
        }
    }
}