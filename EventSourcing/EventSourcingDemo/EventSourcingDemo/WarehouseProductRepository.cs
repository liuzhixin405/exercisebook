using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcingDemo
{
    internal class WarehouseProductRepository
    {
        private readonly Dictionary<string, IList<IEvent>> _inMemoryStreams = new();

        public WarehouseProduct Get(string sku)
        {
            var product = new WarehouseProduct(sku);
            if (_inMemoryStreams.ContainsKey(sku))
            {
                foreach (var @event in _inMemoryStreams[sku])
                {
                    product.AddEvent(@event);
                }
            }
            return product;
        }

        public void Save(WarehouseProduct product)
        {
            _inMemoryStreams[product.Sku] = product.GetEvents();
        }
    }
}
