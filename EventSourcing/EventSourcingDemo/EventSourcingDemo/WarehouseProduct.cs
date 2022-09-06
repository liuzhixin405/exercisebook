using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcingDemo
{
    public class CurrentState
    {
        public int QuantityOnHand { get; internal set; } = 10;
    }
    internal class WarehouseProduct
    {
        private string sku;
        private readonly IList<IEvent> _events = new List<IEvent>();
        private readonly CurrentState _currentState = new();
        public WarehouseProduct(string sku)
        {
            this.sku = sku;
        }

        public void ShipProduct(int quantity)
        {
            if(quantity > _currentState.QuantityOnHand)
            {
                throw new InvalidDataException("not enough");
            }
            AddEvent(new ProductShipped(sku, quantity, DateTime.Now));
        }

        public void ReceiveProduct(int quantity)
        {
            AddEvent(new ProductReceived(sku, quantity, DateTime.Now));
        }

        public void AdjustInventory(int quantity,string reason)
        {
            if ((quantity + _currentState.QuantityOnHand)<0)
            {
                throw new InvalidDataException("not enough");
            }
            AddEvent(new InventoryAdjusted(sku, quantity, reason,DateTime.Now));
        }

        public void AddEvent(IEvent @event)
        {
            switch (@event)
            {
                case ProductReceived productReceived:
                    Apply(productReceived);
                    break;
                    case InventoryAdjusted inventoryAdjusted:
                    Apply(inventoryAdjusted);
                    break;
                    case ProductShipped productShipped:
                    Apply(productShipped);
                    break;
                default:
                    throw new InvalidOperationException("unsupported event");
            }
            _events.Add(@event);
        }

        private void Apply(ProductReceived @event)
        {
            _currentState.QuantityOnHand += @event.Quantity;
        }
        private void Apply(ProductShipped @event)
        {
            _currentState.QuantityOnHand -= @event.Quantity;
        }
        private void Apply(InventoryAdjusted @event)
        {
            _currentState.QuantityOnHand += @event.Quantity;
        }
        public IList<IEvent> GetEvents() => _events;
        public string Sku => sku;
        public int GetQuantityOnHand()
        {
            return _currentState.QuantityOnHand;
        }
        
    }
}
