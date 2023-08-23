
using IBuyStuff.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IBuyStuff.QueryModel.Persistence.Mappings
{
    public class FidelityCardMap : IEntityTypeConfiguration<FidelityCard>
    {
        public void Configure(EntityTypeBuilder<FidelityCard> builder)
        {
            // Properties	
            builder.HasKey(c => c.Number);
            builder.Property(c => c.Number)
                .IsRequired()
                .HasColumnName("Number");
            builder.ToTable("FidelityCards");
            builder.Property(c => c.Owner).IsRequired();
        }

    }
}
