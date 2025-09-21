using ECommerce.Domain.Models;

namespace ECommerce.Domain.Interfaces
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetUserAddressesAsync(Guid userId);
        Task<AddressDto?> GetAddressByIdAsync(Guid id, Guid userId);
        Task<AddressDto?> GetDefaultAddressAsync(Guid userId);
        Task<AddressDto> CreateAddressAsync(Guid userId, CreateAddressDto createAddressDto);
        Task<AddressDto> UpdateAddressAsync(Guid id, Guid userId, UpdateAddressDto updateAddressDto);
        Task<bool> DeleteAddressAsync(Guid id, Guid userId);
        Task<bool> SetAsDefaultAsync(Guid id, Guid userId);
        Task<bool> ValidateAddressExistsAsync(Guid addressId, Guid userId);
    }
}
