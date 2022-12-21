using cat.Models;
using Microsoft.EntityFrameworkCore;

namespace cat.Data
{
    public sealed class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
        {
        }

        public DbSet<Contract> Contracts { get; set; }
    }
}
