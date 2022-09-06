using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CAP.SqlServer.Model
{
    public class CoreDbContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer(@"data source=DESKTOP-11HNB9C\SQLEXPRESS;initial catalog=Stock;user id=sa;password=12301230");
 
         public virtual DbSet<Stock> Stock { get; set; }
    }
}
