using Microsoft.EntityFrameworkCore;
using spot.Application.Interfaces.Repositories;
using spot.Domain.Products.Entities;
using spot.Domain.Trades.Entities;
using spot.Infrastructure.Persistence.Contexts;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace spot.Infrastructure.Persistence.Repositories
{
    public class TradeRepository(ApplicationDbContext dbContext) : GenericRepository<Trade>(dbContext), ITradeRepository
    { 
        public Task AddTrades(List<Trade> trades)
        {
            dbContext.AddAsync(trades);
            return Task.CompletedTask;
        }

        public async Task<Trade> GetLastTradeByProductId(string productId)
        {
           return await dbContext.Trades.Where(x=>x.ProductId.Equals(productId, System.StringComparison.CurrentCultureIgnoreCase)).OrderByDescending(x=>x.Id).FirstOrDefaultAsync();
        }

        public async Task<List<Trade>> GetTradesByProductId(string productId, int count)
        {
            return await dbContext.Trades.Where(x => x.ProductId.Equals(productId, System.StringComparison.CurrentCultureIgnoreCase)).OrderByDescending(x => x.Id).Take(count).ToListAsync();
        }
    }
}
