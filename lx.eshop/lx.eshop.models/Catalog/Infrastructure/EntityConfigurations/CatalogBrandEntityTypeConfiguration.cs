using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lx.eshop.models.Catalog.Infrastructure.EntityConfigurations
{
    public class CatalogBrandEntityTypeConfiguration : IEntityTypeConfiguration<CatalogBrand>
    {
        public void Configure(EntityTypeBuilder<CatalogBrand> builder)
        {
            builder.ToTable("CatalogBrand");
            builder.HasKey(ci => ci.Id);

            builder.Property(ci=>ci.Id).UseHiLo("catalog_brand_hilo").IsRequired();
            builder.Property(cb=>cb.Brand).IsRequired().HasMaxLength(100);
        }
    }
}
