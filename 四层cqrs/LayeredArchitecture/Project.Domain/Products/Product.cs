using Project.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Products
{
    public class Product:Entity,IAggregateRoot
    {
        public ProductId Id { get; private set; }
        public string Name { get; private set; }
        private List<ProductPrice> _prices;
        public Product()
        {

        }
    }
}
