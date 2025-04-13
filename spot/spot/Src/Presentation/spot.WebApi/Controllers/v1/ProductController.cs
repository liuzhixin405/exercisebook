using Microsoft.AspNetCore.Mvc;
using spot.Application.Features.Products.Commands.CreateProduct;
using spot.Application.Features.Products.Queries.GetAllProducts;
using spot.Application.Features.Products.Queries.GetProductById;
using System.Threading.Tasks;

namespace spot.WebApi.Controllers.v1
{
    public class ProductController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetAllProductsQuery()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            return Ok(await Mediator.Send(new GetProductByIdQuery { Id = id }));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}