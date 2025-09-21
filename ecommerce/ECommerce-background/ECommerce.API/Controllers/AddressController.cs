using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class AddressController : BaseController
    {
        private readonly IAddressService _addressService;
        private readonly ILogger<AddressController> _logger;

        public AddressController(IAddressService addressService, ILogger<AddressController> logger)
        {
            _addressService = addressService;
            _logger = logger;
        }

        /// <summary>
        /// 获取用户的所有地址
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressDto>>> GetUserAddresses()
        {
            if (CurrentUserId == null)
                return BadRequest("Invalid user");

            try
            {
                var addresses = await _addressService.GetUserAddressesAsync(CurrentUserId.Value);
                return Ok(addresses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting addresses for user {UserId}", CurrentUserId.Value);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 获取指定地址详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AddressDto>> GetAddress(Guid id)
        {
            if (CurrentUserId == null)
                return BadRequest("Invalid user");

            try
            {
                var address = await _addressService.GetAddressByIdAsync(id, CurrentUserId.Value);
                if (address == null)
                    return NotFound("Address not found");

                return Ok(address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting address {AddressId} for user {UserId}", id, CurrentUserId.Value);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 获取默认地址
        /// </summary>
        [HttpGet("default")]
        public async Task<ActionResult<AddressDto>> GetDefaultAddress()
        {
            if (CurrentUserId == null)
                return BadRequest("Invalid user");

            try
            {
                var address = await _addressService.GetDefaultAddressAsync(CurrentUserId.Value);
                if (address == null)
                    return NotFound("No default address found");

                return Ok(address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default address for user {UserId}", CurrentUserId.Value);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 创建新地址
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AddressDto>> CreateAddress([FromBody] CreateAddressDto createAddressDto)
        {
            if (CurrentUserId == null)
                return BadRequest("Invalid user");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var address = await _addressService.CreateAddressAsync(CurrentUserId.Value, createAddressDto);
                return CreatedAtAction(nameof(GetAddress), new { id = address.Id }, address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating address for user {UserId}", CurrentUserId.Value);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 更新地址
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<AddressDto>> UpdateAddress(Guid id, [FromBody] UpdateAddressDto updateAddressDto)
        {
            if (CurrentUserId == null)
                return BadRequest("Invalid user");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var address = await _addressService.UpdateAddressAsync(id, CurrentUserId.Value, updateAddressDto);
                return Ok(address);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating address {AddressId} for user {UserId}", id, CurrentUserId.Value);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 删除地址
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAddress(Guid id)
        {
            if (CurrentUserId == null)
                return BadRequest("Invalid user");

            try
            {
                var result = await _addressService.DeleteAddressAsync(id, CurrentUserId.Value);
                if (!result)
                    return NotFound("Address not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting address {AddressId} for user {UserId}", id, CurrentUserId.Value);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 设置为默认地址
        /// </summary>
        [HttpPut("{id}/set-default")]
        public async Task<ActionResult> SetAsDefault(Guid id)
        {
            if (CurrentUserId == null)
                return BadRequest("Invalid user");

            try
            {
                var result = await _addressService.SetAsDefaultAsync(id, CurrentUserId.Value);
                if (!result)
                    return NotFound("Address not found");

                return Ok(new { message = "Address set as default successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting address {AddressId} as default for user {UserId}", id, CurrentUserId.Value);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 验证地址是否存在
        /// </summary>
        [HttpPost("validate")]
        public async Task<ActionResult<bool>> ValidateAddress([FromBody] ValidateAddressRequest request)
        {
            if (CurrentUserId == null)
                return BadRequest("Invalid user");

            try
            {
                var exists = await _addressService.ValidateAddressExistsAsync(request.AddressId, CurrentUserId.Value);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating address {AddressId} for user {UserId}", request.AddressId, CurrentUserId.Value);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class ValidateAddressRequest
    {
        public Guid AddressId { get; set; }
    }
}
