using spot.Application.Features.Products.Queries.GetProductById;
using spot.Domain.Products.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spot.Application.Interfaces.Products
{
    public interface IProductService
    {
       Task<Product> GetProductByIdQuery(string id);
        Task<IReadOnlyList<Product>> GetAllProducts();
    }
}
