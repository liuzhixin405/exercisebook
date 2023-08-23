
using IBuyStuff.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IBuyStuff.Persistence.Mappings
{
    public class ExpiryDateMap : IEntityTypeConfiguration<ExpiryDate>
    {
      
        public void Configure(EntityTypeBuilder<ExpiryDate> builder)
        {
            builder.HasNoKey();
            builder.Ignore(p => p.When);
        }
    }
}
