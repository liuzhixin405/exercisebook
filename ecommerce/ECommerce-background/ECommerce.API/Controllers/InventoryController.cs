using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryController : BaseController
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        /// <summary>
        /// 检查产品库存
        /// </summary>
        [HttpGet("check/{productId}")]
        [AllowAnonymous]
        public async Task<ActionResult<InventoryCheckResult>> CheckStock(Guid productId, [FromQuery] int quantity = 1)
        {
            try
            {
                var result = await _inventoryService.CheckStockAsync(productId, quantity);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stock check failed for product: {ProductId}", productId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// 获取产品库存信息
        /// </summary>
        [HttpGet("info/{productId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductInventoryInfo>> GetProductInventory(Guid productId)
        {
            try
            {
                var result = await _inventoryService.GetProductInventoryAsync(productId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get inventory info for product: {ProductId}", productId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// 扣减库存
        /// </summary>
        [HttpPost("deduct")]
        public async Task<ActionResult<InventoryOperationResult>> DeductStock([FromBody] InventoryUpdateRequest request)
        {
            if (!IsAdmin)
                return Forbid();

            try
            {
                var result = await _inventoryService.DeductStockAsync(request.ProductId, request.Quantity);
                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(new { message = result.Message, details = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stock deduction failed for product: {ProductId}", request.ProductId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// 恢复库存
        /// </summary>
        [HttpPost("restore")]
        public async Task<ActionResult<InventoryOperationResult>> RestoreStock([FromBody] InventoryUpdateRequest request)
        {
            if (!IsAdmin)
                return Forbid();

            try
            {
                var result = await _inventoryService.RestoreStockAsync(request.ProductId, request.Quantity);
                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(new { message = result.Message, details = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stock restoration failed for product: {ProductId}", request.ProductId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// 锁定库存
        /// </summary>
        [HttpPost("lock")]
        public async Task<ActionResult<InventoryOperationResult>> LockStock([FromBody] InventoryLockRequest request)
        {
            if (!IsAdmin)
                return Forbid();

            try
            {
                var result = await _inventoryService.LockStockAsync(request.ProductId, request.Quantity, request.OrderId);
                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(new { message = result.Message, details = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stock locking failed for product: {ProductId}", request.ProductId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// 释放锁定的库存
        /// </summary>
        [HttpPost("release")]
        public async Task<ActionResult<InventoryOperationResult>> ReleaseLockedStock([FromBody] InventoryLockRequest request)
        {
            if (!IsAdmin)
                return Forbid();

            try
            {
                var result = await _inventoryService.ReleaseLockedStockAsync(request.ProductId, request.Quantity, request.OrderId);
                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(new { message = result.Message, details = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stock release failed for product: {ProductId}", request.ProductId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// 批量更新库存
        /// </summary>
        [HttpPost("batch-update")]
        public async Task<ActionResult<BatchInventoryUpdateResult>> BatchUpdateInventory([FromBody] List<InventoryUpdate> updates)
        {
            if (!IsAdmin)
                return Forbid();

            try
            {
                var result = await _inventoryService.BatchUpdateInventoryAsync(updates);
                if (result.OverallSuccess)
                    return Ok(result);
                else
                    return BadRequest(new { message = result.Message, details = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Batch inventory update failed");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// 获取库存操作类型
        /// </summary>
        [HttpGet("operation-types")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<object>> GetOperationTypes()
        {
            var operationTypes = Enum.GetValues<InventoryOperationType>()
                .Select(ot => new
                {
                    Value = ot.ToString(),
                    Name = ot switch
                    {
                        InventoryOperationType.Add => "增加库存",
                        InventoryOperationType.Deduct => "扣减库存",
                        InventoryOperationType.Reserve => "预留库存",
                        InventoryOperationType.Release => "释放预留",
                        InventoryOperationType.Lock => "锁定库存",
                        InventoryOperationType.Unlock => "解锁库存",
                        InventoryOperationType.Adjust => "调整库存",
                        InventoryOperationType.Set => "设置库存",
                        _ => ot.ToString()
                    },
                    Description = ot switch
                    {
                        InventoryOperationType.Add => "增加产品库存数量",
                        InventoryOperationType.Deduct => "扣减产品库存数量",
                        InventoryOperationType.Reserve => "预留库存用于特定用途",
                        InventoryOperationType.Release => "释放之前预留的库存",
                        InventoryOperationType.Lock => "锁定库存防止超卖",
                        InventoryOperationType.Unlock => "解锁之前锁定的库存",
                        InventoryOperationType.Adjust => "调整库存到指定数量",
                        InventoryOperationType.Set => "设置库存为指定数量",
                        _ => "未知操作类型"
                    }
                });

            return Ok(operationTypes);
        }

        // --------------- Inventory Transactions (moved from InventoryTransactionsController) ---------------

        /// <summary>
        /// 获取指定产品的库存事务记录
        /// </summary>
        [HttpGet("transactions/product/{productId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<InventoryTransaction>>> GetTransactionsByProduct(Guid productId, [FromQuery] int limit = 50)
        {
            try
            {
                if (limit <= 0 || limit > 1000)
                {
                    return BadRequest("Limit must be between 1 and 1000");
                }

                var transactions = await _inventoryService.GetProductInventoryTransactionsAsync(productId, limit);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory transactions for product {ProductId}", productId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 获取库存事务记录详情
        /// </summary>
        [HttpGet("transactions/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InventoryTransaction>> GetTransactionById(Guid id)
        {
            try
            {
                var transaction = await _inventoryService.GetInventoryTransactionAsync(id);
                if (transaction == null)
                    return NotFound();

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory transaction {TransactionId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 根据操作类型获取库存事务记录（仓储未实现，暂返回空集合）
        /// </summary>
        [HttpGet("transactions/operation/{operationType}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<InventoryTransaction>>> GetTransactionsByOperation(string operationType, [FromQuery] int limit = 50)
        {
            try
            {
                if (limit <= 0 || limit > 1000)
                {
                    return BadRequest("Limit must be between 1 and 1000");
                }

                if (!Enum.TryParse<InventoryOperationType>(operationType, out var parsedOperationType))
                {
                    return BadRequest($"Invalid operation type: {operationType}");
                }

                await Task.CompletedTask;
                return Ok(new List<InventoryTransaction>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory transactions for operation type {OperationType}", operationType);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    /// <summary>
    /// 库存更新请求
    /// </summary>
    public class InventoryUpdateRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    /// <summary>
    /// 库存锁定请求
    /// </summary>
    public class InventoryLockRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid OrderId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
