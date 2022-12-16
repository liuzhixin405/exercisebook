using System.Data.Entity;
using NoSqlEvents.Backend.ReadModel.Dto;

namespace LiveScoreEs.Backend.DAL
{
    public class WaterpoloContext : DbContext
    {
        public WaterpoloContext() : base("naa4e_Waterpolo")
        {
        }

        public DbSet<LiveMatch> Matches { get; set; }
    }
}