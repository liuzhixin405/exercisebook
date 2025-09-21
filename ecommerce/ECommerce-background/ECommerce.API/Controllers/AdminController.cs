using ECommerce.Application.Services;
using ECommerce.Domain.Models;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize] // 需要身份验证
    public class AdminController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IProductService productService, IOrderService orderService, ILogger<AdminController> logger)
        {
            _productService = productService;
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// 检查当前用户是否为管理员
        /// </summary>
        private bool IsAdmin()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            return roleClaim?.Value == "Admin";
        }

        /// <summary>
        /// 检查管理员权限，如果不是管理员则返回403
        /// </summary>
        private ActionResult? CheckAdminPermission()
        {
            if (!IsAdmin())
            {
                _logger.LogWarning("Non-admin user {UserId} attempted to access admin endpoint", CurrentUserId);
                return Forbid("Access denied. Admin privileges required.");
            }
            return null;
        }

        /// <summary>
        /// 获取所有产品（管理员视图）
        /// </summary>
        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products for admin");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 创建新产品
        /// </summary>
        [HttpPost("products")]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var product = await _productService.CreateProductAsync(createProductDto);
                _logger.LogInformation("Admin {AdminId} created product: {ProductId} - {ProductName}", 
                    CurrentUserId, product.Id, product.Name);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product in admin panel");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 获取单个产品详情
        /// </summary>
        [HttpGet("products/{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound();

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product {ProductId} for admin", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 更新产品信息
        /// </summary>
        [HttpPut("products/{id}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, UpdateProductDto updateProductDto)
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var product = await _productService.UpdateProductAsync(id, updateProductDto);
                _logger.LogInformation("Admin {AdminId} updated product: {ProductId} - {ProductName}", 
                    CurrentUserId, product.Id, product.Name);
                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId} in admin panel", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 删除产品
        /// </summary>
        [HttpDelete("products/{id}")]
        public async Task<ActionResult> DeleteProduct(Guid id)
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (!result)
                    return NotFound();

                _logger.LogInformation("Admin {AdminId} deleted product: {ProductId}", CurrentUserId, id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId} in admin panel", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 批量更新产品状态
        /// </summary>
        [HttpPut("products/batch-status")]
        public async Task<ActionResult> BatchUpdateProductStatus([FromBody] BatchUpdateProductStatusDto batchUpdateDto)
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var successCount = 0;
                var errors = new List<string>();

                foreach (var productId in batchUpdateDto.ProductIds)
                {
                    try
                    {
                        var updateDto = new UpdateProductDto
                        {
                            IsActive = batchUpdateDto.IsActive
                        };
                        
                        // 获取现有产品信息
                        var existingProduct = await _productService.GetProductByIdAsync(productId);
                        if (existingProduct != null)
                        {
                            updateDto.Name = existingProduct.Name;
                            updateDto.Description = existingProduct.Description;
                            updateDto.Price = existingProduct.Price;
                            updateDto.Stock = existingProduct.Stock;
                            updateDto.Category = existingProduct.Category;
                            updateDto.ImageUrl = existingProduct.ImageUrl;
                            
                            await _productService.UpdateProductAsync(productId, updateDto);
                            successCount++;
                        }
                        else
                        {
                            errors.Add($"Product {productId} not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error updating product {productId}: {ex.Message}");
                    }
                }

                _logger.LogInformation("Admin {AdminId} batch updated {SuccessCount} products status to {IsActive}", 
                    CurrentUserId, successCount, batchUpdateDto.IsActive);

                return Ok(new { 
                    SuccessCount = successCount, 
                    ErrorCount = errors.Count,
                    Errors = errors 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in batch update product status");
                return StatusCode(500, "Internal server error");
            }
        }

        #region 订单管理

        /// <summary>
        /// 获取所有订单（管理员视图）
        /// </summary>
        [HttpGet("orders")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders for admin");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 获取单个订单详情（管理员视图）
        /// </summary>
        [HttpGet("orders/{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound();

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order {OrderId} for admin", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 更新订单状态（管理员操作）
        /// </summary>
        [HttpPut("orders/{id}/status")]
        public async Task<ActionResult<OrderDto>> UpdateOrderStatus(Guid id, [FromBody] AdminUpdateOrderStatusDto updateOrderStatusDto)
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // 将字符串状态转换为OrderStatus枚举
                if (!Enum.TryParse<OrderStatus>(updateOrderStatusDto.Status, out var orderStatus))
                {
                    return BadRequest($"无效的订单状态: {updateOrderStatusDto.Status}");
                }

                var updateDto = new UpdateOrderStatusDto
                {
                    Status = orderStatus,
                    TrackingNumber = updateOrderStatusDto.TrackingNumber,
                    Notes = updateOrderStatusDto.Notes
                };

                var order = await _orderService.UpdateOrderStatusAsync(id, updateDto);
                _logger.LogInformation("Admin {AdminId} updated order {OrderId} status to {Status}", 
                    CurrentUserId, id, updateOrderStatusDto.Status);
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId} status in admin panel", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 确认订单（管理员操作）
        /// </summary>
        [HttpPut("orders/{id}/confirm")]
        public async Task<ActionResult<OrderDto>> ConfirmOrder(Guid id)
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            try
            {
                var result = await _orderService.ConfirmOrderAsync(id);
                if (!result)
                    return BadRequest("无法确认订单，订单状态可能不正确");

                var order = await _orderService.GetOrderByIdAsync(id);
                _logger.LogInformation("Admin {AdminId} confirmed order {OrderId}", CurrentUserId, id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming order {OrderId} in admin panel", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 发货订单（管理员操作）
        /// </summary>
        [HttpPut("orders/{id}/ship")]
        public async Task<ActionResult<OrderDto>> ShipOrder(Guid id, [FromBody] ShipOrderRequest request)
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _orderService.ShipOrderAsync(id, request.TrackingNumber);
                if (!result)
                    return BadRequest("无法发货订单，订单状态可能不正确");

                var order = await _orderService.GetOrderByIdAsync(id);
                _logger.LogInformation("Admin {AdminId} shipped order {OrderId} with tracking {TrackingNumber}", 
                    CurrentUserId, id, request.TrackingNumber);
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error shipping order {OrderId} in admin panel", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 标记订单为已送达（管理员操作）
        /// </summary>
        [HttpPut("orders/{id}/deliver")]
        public async Task<ActionResult<OrderDto>> DeliverOrder(Guid id)
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            try
            {
                var result = await _orderService.DeliverOrderAsync(id);
                if (!result)
                    return BadRequest("无法标记订单为已送达，订单状态可能不正确");

                var order = await _orderService.GetOrderByIdAsync(id);
                _logger.LogInformation("Admin {AdminId} delivered order {OrderId}", CurrentUserId, id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error delivering order {OrderId} in admin panel", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 取消订单（管理员操作）
        /// </summary>
        [HttpPut("orders/{id}/cancel")]
        public async Task<ActionResult<OrderDto>> CancelOrder(Guid id)
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            try
            {
                var result = await _orderService.CancelOrderAsync(id);
                if (!result)
                    return BadRequest("无法取消订单，订单状态可能不正确");

                var order = await _orderService.GetOrderByIdAsync(id);
                _logger.LogInformation("Admin {AdminId} cancelled order {OrderId}", CurrentUserId, id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId} in admin panel", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 批量更新订单状态
        /// </summary>
        [HttpPut("orders/batch-status")]
        public async Task<ActionResult> BatchUpdateOrderStatus([FromBody] BatchUpdateOrderStatusDto batchUpdateDto)
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var successCount = 0;
                var errors = new List<string>();

                // 将字符串状态转换为OrderStatus枚举
                if (!Enum.TryParse<OrderStatus>(batchUpdateDto.Status, out var orderStatus))
                {
                    return BadRequest($"无效的订单状态: {batchUpdateDto.Status}");
                }

                foreach (var orderId in batchUpdateDto.OrderIds)
                {
                    try
                    {
                        var updateDto = new UpdateOrderStatusDto
                        {
                            Status = orderStatus
                        };
                        
                        await _orderService.UpdateOrderStatusAsync(orderId, updateDto);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error updating order {orderId}: {ex.Message}");
                    }
                }

                _logger.LogInformation("Admin {AdminId} batch updated {SuccessCount} orders status to {Status}", 
                    CurrentUserId, successCount, batchUpdateDto.Status);

                return Ok(new { 
                    SuccessCount = successCount, 
                    ErrorCount = errors.Count,
                    Errors = errors 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in batch update order status");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 取消过期订单
        /// </summary>
        [HttpPost("orders/cancel-expired")]
        public async Task<ActionResult> CancelExpiredOrders()
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            try
            {
                var result = await _orderService.CancelExpiredOrdersAsync();
                _logger.LogInformation("Admin {AdminId} cancelled expired orders", CurrentUserId);
                return Ok(new { message = "Expired orders cancelled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling expired orders");
                return StatusCode(500, "Internal server error");
            }
        }

        #endregion
    }

    /// <summary>
    /// 批量更新产品状态的数据传输对象
    /// </summary>
    public class BatchUpdateProductStatusDto
    {
        public List<Guid> ProductIds { get; set; } = new List<Guid>();
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// 管理员更新订单状态的数据传输对象
    /// </summary>
    public class AdminUpdateOrderStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public string? TrackingNumber { get; set; }
        public string? Notes { get; set; }
    }

    /// <summary>
    /// 批量更新订单状态的数据传输对象
    /// </summary>
    public class BatchUpdateOrderStatusDto
    {
        public List<Guid> OrderIds { get; set; } = new List<Guid>();
        public string Status { get; set; } = string.Empty;
    }
}
