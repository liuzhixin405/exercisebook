 using cat.Data;
using Microsoft.EntityFrameworkCore;

namespace cat.DbProvider
{
    public interface IContextProvider
    {
        ApplicationDbContext Get();
    }

    public class ContextProvider : IContextProvider
    {
        public ApplicationDbContext Get()
        {
            //var contextBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("").Options;
            var contextBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("memory").Options;
            return new ApplicationDbContext(contextBuilder);
        }
    }
}
