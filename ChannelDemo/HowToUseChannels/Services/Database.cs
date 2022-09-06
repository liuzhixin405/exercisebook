using Microsoft.EntityFrameworkCore;

namespace HowToUseChannels.Services
{
    public class Database:DbContext
    {
        public Database(DbContextOptions<Database> options):base(options)
        {

        }
        //DbContextOptionsBuilder.EnableSensitiveDataLogging
     
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }
    }
}
