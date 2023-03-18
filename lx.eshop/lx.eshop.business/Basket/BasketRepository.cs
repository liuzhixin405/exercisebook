using lx.eshop.models.Basket;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace lx.eshop.business.Basket
{
    public class BasketRepository : IBasketRepository
    {
        private readonly ILogger _logger;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _databse;
        public BasketRepository(ILogger<BasketRepository> logger, ConnectionMultiplexer redis)
        {
            _logger = logger;
            _redis = redis;
            _databse = redis.GetDatabase();
        }

        public async Task<bool> DeleteBasketAsync(string id)
        {
            return await _databse.KeyDeleteAsync(id);
        }

        public async Task<CustomerBasket> GetBasketAsync(string customerId)
        {
            var data =await _databse.StringGetAsync(customerId);
            if (data.IsNullOrEmpty)
            {
                return null;
            }
            return JsonSerializer.Deserialize<CustomerBasket>(data, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public IEnumerable<string> GetUsers()
        {
            var server = GetServer();
            var data = server.Keys();
            return data?.Select(key => key.ToString());
;        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            var created = await _databse.StringSetAsync(basket.BuyerId, JsonSerializer.Serialize(basket));
            if (!created)
            {
                _logger.LogInformation("problem occur persisting thye item.");
                return null;
            }
            _logger.LogInformation("Basket item persisted succesfully.");

            return await GetBasketAsync(basket.BuyerId);
        }
        private IServer GetServer()
        {
            var endpoint = _redis.GetEndPoints();
            return _redis.GetServer(endpoint.First());
        }
    }
}
