using Play.Inventory.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Play.Inventory.Service
{
    public static class Extensions
    {
        public static InventoryItemDto AsDto(this InventoryItem item, string name, string description)
        {
            return new InventoryItemDto(item.CatalogItemId, name, description, item.Quantity, item.AcquiredDate);
        }
    }
}
