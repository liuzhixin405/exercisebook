using spot.Domain.Products.Entities;
using spot.Domain.Trades.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace spot.Application.Interfaces.Repositories
{
    public interface ITradeRepository : IGenericRepository<Trade>
    {      
        Task AddTrades(List<Trade> trades);
        Task<Trade> GetLastTradeByProductId(string productId);
        Task<List<Trade>> GetTradesByProductId(string productId,int count);
    }
}
