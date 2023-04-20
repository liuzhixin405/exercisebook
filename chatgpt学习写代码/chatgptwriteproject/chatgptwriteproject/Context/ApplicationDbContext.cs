using chatgptwriteproject.Models;
using Microsoft.EntityFrameworkCore;

namespace chatgptwriteproject.Context
{
    public class ApplicationDbContext:DbContext,IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }
        public DbSet<Product> Products { get; set; }

        public Task<int> SaveChangeAsync(CancellationToken cancellationToken = default)
        {
           return base.SaveChangesAsync(cancellationToken);
        }
    }
}
