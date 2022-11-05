using EFCoreDB.DataLayer.Model;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;

namespace EFCoreDB.DataLayer
{
    public class BlogContext_01 : DbContext
    {
        public BlogContext_01(DbContextOptions<BlogContext_01> options) : base(options)
        {
           
        }

        public DbSet<Blog> Blogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditEntry>();
        }
    }
}
