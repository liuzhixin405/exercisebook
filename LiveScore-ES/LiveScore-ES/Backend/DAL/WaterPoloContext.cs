using LiveScore_ES.Backend.ReadModel.Dto;
using Microsoft.EntityFrameworkCore;

namespace LiveScore_ES.Backend.DAL
{
    public class WaterPoloContext:DbContext
    {
        public WaterPoloContext(DbContextOptions<WaterPoloContext> contextOptions):base(contextOptions)
        {

        }
        public DbSet<LiveMatch> Matches { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
