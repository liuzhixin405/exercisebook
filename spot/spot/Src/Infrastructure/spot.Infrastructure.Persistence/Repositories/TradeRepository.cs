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
    public class TradeRepository : GenericRepository<Trade>, ITradeRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TradeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task AddTrades(List<Trade> trades)
        {
            _dbContext.AddAsync(trades);
            return Task.CompletedTask;
        }

        public async Task<Trade> GetLastTradeByProductId(string productId)
        {
           return await _dbContext.Trades.Where(x=>x.ProductId.Equals(productId, System.StringComparison.CurrentCultureIgnoreCase)).OrderByDescending(x=>x.Id).FirstOrDefaultAsync();
        }

        public async Task<List<Trade>> GetTradesByProductId(string productId, int count)
        {
            return await _dbContext.Trades.Where(x => x.ProductId.Equals(productId, System.StringComparison.CurrentCultureIgnoreCase)).OrderByDescending(x => x.Id).Take(count).ToListAsync();
        }
    }
}
