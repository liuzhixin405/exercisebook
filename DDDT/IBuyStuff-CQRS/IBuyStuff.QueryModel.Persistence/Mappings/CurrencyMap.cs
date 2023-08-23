using IBuyStuff.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IBuyStuff.QueryModel.Persistence.Mappings
{
    public class CurrencyMap : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.HasNoKey();
            // Properties		
            builder.Ignore(p => p.Name);

            builder.Property(p => p.Symbol)
                .IsRequired()
                .HasColumnName("Symbol");
        }
    }
}
