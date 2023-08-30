
using IBuyStuff.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IBuyStuff.Persistence.Mappings
{
    //public class CurrencyMap : ComplexTypeConfiguration<Currency>
    //{
    //    public CurrencyMap()
    //    {
    //        // Properties		
    //        Ignore(p => p.Name);

    //        Property(p => p.Symbol)
    //            .IsRequired()
    //            .HasColumnName("");
    //    }
    //}
    public class CurrencyMap : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {

            // Properties    
           
            builder.Ignore(p => p.Name);

            builder.Property(p => p.Symbol)
                .IsRequired()
                .HasColumnName("Symbol");
        }
    }

}
