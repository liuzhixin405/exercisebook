using ECommerce.API.Application;
using ECommerce.API.Infrastucture;
using ECommerce.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    // API Controller
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentStrategyFactory _paymentFactory;
        private readonly IEventBus _eventBus;
        private readonly IValidationHandler<OrderRequest> _validationChain;

        public OrdersController(
            IOrderService orderService,
            IPaymentStrategyFactory paymentFactory,
            IEventBus eventBus,
            IValidationHandler<OrderRequest> validationChain)
        {
            _orderService = orderService;
            _paymentFactory = paymentFactory;
            _eventBus = eventBus;
            _validationChain = validationChain;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            // 责任链验证
            if (!await _validationChain.ValidateAsync(request))
            {
                return BadRequest(_validationChain.ErrorMessage);
            }

            // 策略模式支付
            var paymentStrategy = _paymentFactory.CreateStrategy(request.PaymentType);
            var paymentResult = await paymentStrategy.ProcessPaymentAsync(request.Amount);

            if (!paymentResult.Success)
            {
                return BadRequest("Payment failed");
            }

            // 创建订单
            var order = await _orderService.CreateOrderAsync(request);

            // 发布领域事件
            await _eventBus.PublishAsync(new OrderCreatedEvent
            {
                OrderId = order.Id,
                Amount = order.Amount
            });

            return Ok(order);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }
    }
}
