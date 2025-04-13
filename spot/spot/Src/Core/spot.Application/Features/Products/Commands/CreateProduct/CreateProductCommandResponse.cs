using spot.Application.Wrappers;
using spot.Domain.Products.Entities;

namespace spot.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandResponse : Response<Product>
    {
        public CreateProductCommandResponse(Product product) : base(product)
        {
        }

        public CreateProductCommandResponse(string message) : base(message)
        {
        }

        public CreateProductCommandResponse(string message, bool succeeded) : base(message, succeeded)
        {
        }
    }
}