using Play.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Play.Inventory.Service.Entities
{
    public class CatalogItem:IEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
