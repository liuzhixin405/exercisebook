using MediatR;
using spot.Application.Interfaces;
using spot.Application.Interfaces.Repositories;
using spot.Domain.Products.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace spot.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductCommandResponse>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateProductCommandResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var productId = $"{request.BaseCurrency}-{request.QuoteCurrency}";
                
                // Check if product already exists
                var existingProduct = await _productRepository.GetByIdAsync(productId);
                if (existingProduct != null)
                {
                    return new CreateProductCommandResponse($"Product with ID {productId} already exists");
                }

                var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var product = new Product
                {
                    Id = productId,
                    CreatedAt = now,
                    UpdatedAt = now,
                    BaseCurrency = request.BaseCurrency,
                    QuoteCurrency = request.QuoteCurrency,
                    BaseMinSize = request.BaseMinSize,
                    BaseMaxSize = request.BaseMaxSize,
                    QuoteMinSize = request.QuoteMinSize,
                    QuoteMaxSize = request.QuoteMaxSize,
                    BaseScale = request.BaseScale,
                    QuoteScale = request.QuoteScale,
                    QuoteIncrement = request.QuoteIncrement
                };

                var createdProduct = await _productRepository.AddAsync(product);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new CreateProductCommandResponse(createdProduct);
            }
            catch (Exception ex)
            {
                return new CreateProductCommandResponse($"An error occurred while creating the product: {ex.Message}");
            }
        }
    }
}