
using IBuyStuff.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IBuyStuff.Persistence.Mappings
{
    public class OrderMap : IEntityTypeConfiguration<Order>
    {
        

        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // Primary Key
            builder.HasKey(t => t.OrderId);

            // Table and relationships 
            builder.ToTable("Orders");
            builder.HasOne(o => o.Buyer);
            builder.HasMany(o => o.Items);
            builder.OwnsOne(o => o.Total, m =>
            {
                m.OwnsOne(z => z.Currency, c => { c.Ignore(v => v.Name); });

            });
        }
    }
}