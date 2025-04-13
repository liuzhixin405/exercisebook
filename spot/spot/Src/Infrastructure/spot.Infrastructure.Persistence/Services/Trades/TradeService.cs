using spot.Application.Interfaces;
using spot.Application.Interfaces.Repositories;
using spot.Application.Interfaces.Trades;
using spot.Domain.Trades.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spot.Infrastructure.Persistence.Services.Trades
{
    public class TradeService : ITradeService
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly IUnitOfWork _unitOfWork;
        public TradeService(ITradeRepository tradeRepository,IUnitOfWork unitOfWork)
        {
            _tradeRepository = tradeRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task AddTrades(List<Trade> trades)
        {
           await _tradeRepository.AddTrades(trades);
            await _unitOfWork.SaveChangesAsync();

        }


        public Task<Trade> GetLastTradeByProductId(string productId)
        {
            return _tradeRepository.GetLastTradeByProductId(productId);
        }

        public Task<List<Trade>> GetTradesByProductId(string productId, int count)
        {
            return _tradeRepository.GetTradesByProductId(productId, count);
        }
    }
}
