using MediatR;
using spot.Application.Wrappers;
using spot.Domain.Products.Entities;

namespace spot.Application.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQuery : IRequest<BaseResult<Product>>
    {
        public string Id { get; set; }
    }
}
