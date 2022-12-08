using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Play.Inventory.Service.Clients
{
    public class CatalogClient
    {
        private readonly HttpClient _httpClient;
        public CatalogClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemsAsync()
        {
            var items = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>("/items");
            return items;
        }
    }
}
