using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Processing.InternalCommands
{
    public class InternalCommandEntityTypeConfiguration : IEntityTypeConfiguration<InternalCommand>
    {
        public void Configure(EntityTypeBuilder<InternalCommand> builder)
        {
            builder.ToTable("InternalCommands", SchemaNames.Application);
            builder.HasKey(b => b.Id);
            builder.Property(b=>b.Id).ValueGeneratedNever();
        }
    }
}
