using Project.Domain.SeedWork;
using Project.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Products
{
    public class ProductPriceData:ValueObject
    {
        public ProductPriceData(ProductId productId,MoneyValue price)
        {
            this.ProductId = productId;
            this.Price = price;
        }
        public ProductId ProductId { get;  }
        public MoneyValue Price { get; }
    }
}
