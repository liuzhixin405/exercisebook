using System.Threading;
using System.Threading.Tasks;
using MediatR;
using spot.Application.Helpers;
using spot.Application.Interfaces;
using spot.Application.Interfaces.Repositories;
using spot.Application.Wrappers;
using spot.Domain.Products.Entities;

namespace spot.Application.Features.Products.Queries.GetProductById
{
    public class GetProductByIdQueryHandler(IProductRepository productRepository, ITranslator translator) : IRequestHandler<GetProductByIdQuery, BaseResult<Product>>
    {
        public async Task<BaseResult<Product>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await productRepository.GetByIdAsync(request.Id);

            if (product is null)
            {
                return new Error(ErrorCode.NotFound, translator.GetString(TranslatorMessages.ProductMessages.Product_NotFound_with_id(request.Id)), nameof(request.Id));
            }

            return new BaseResult<Product>() { Data=product, Success= true};
        }
    }
}
