using spot.Domain.Trades.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spot.Application.Interfaces.Trades
{
    public interface ITradeService
    {
        Task AddTrades(List<Trade> trades);
        Task<Trade> GetLastTradeByProductId(string productId);
        Task<List<Trade>> GetTradesByProductId(string productId, int count);
    }
}
