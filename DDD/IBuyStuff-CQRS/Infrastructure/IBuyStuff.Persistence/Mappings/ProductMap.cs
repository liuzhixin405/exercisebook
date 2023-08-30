using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBuyStuff.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IBuyStuff.Persistence.Mappings
{
    public class ProductMap : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.OwnsOne(o => o.UnitPrice, m =>
            {
                m.OwnsOne(z => z.Currency,
                    c => { 
                        c.Ignore(v => v.Name);        
                    }
                    
                    );
                
            });
        }
    }
}
