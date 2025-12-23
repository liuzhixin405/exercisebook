namespace FacadeDesgin.Controllers;

using FacadeDesgin.Facades;
using FacadeDesgin.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderFacade _facade;

    public OrdersController(IOrderFacade facade)
    {
        _facade = facade;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderRequest request)
    {
        try
        {
            var order = await _facade.PlaceOrderAsync(request);
            return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> Get(Guid id)
    {
        var order = await _facade.FetchOrderAsync(id);
        if (order is null) return NotFound();
        return Ok(order);
    }
}
