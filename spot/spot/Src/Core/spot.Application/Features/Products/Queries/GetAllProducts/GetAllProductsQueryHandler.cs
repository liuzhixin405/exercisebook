using MediatR;
using spot.Application.Interfaces.Repositories;
using spot.Application.Wrappers;
using spot.Domain.Products.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace spot.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Response<List<Product>>>
    {
        private readonly IProductRepository _productRepository;

        public GetAllProductsQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Response<List<Product>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var products = await _productRepository.GetAllAsync();
                var productsList = products.ToList();
                return new Response<List<Product>>(productsList);
            }
            catch (Exception ex)
            {
                return new Response<List<Product>>($"An error occurred while retrieving products: {ex.Message}");
            }
        }
    }
}