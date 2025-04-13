using MediatR;
using spot.Application.Wrappers;
using spot.Domain.Products.Entities;
using System.Collections.Generic;

namespace spot.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQuery : IRequest<Response<List<Product>>>
    {
    }
}