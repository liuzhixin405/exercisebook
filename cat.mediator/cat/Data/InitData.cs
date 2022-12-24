using cat.DbProvider;
using cat.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace cat.Data
{
    public class InitData
    {

        public static void InitializationDb()
        {
          
            using (var context = new ContextProvider().Get())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                if (!context.Contracts.Any())
                {
                    context.Contracts.Add(Contract.CreateNew("BTC"));
                    context.SaveChanges();
                }
            }
        }
    }
}
