using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ILogger<AddressService> _logger;

        public AddressService(IAddressRepository addressRepository, ILogger<AddressService> logger)
        {
            _addressRepository = addressRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<AddressDto>> GetUserAddressesAsync(Guid userId)
        {
            var addresses = await _addressRepository.GetByUserIdAsync(userId);
            return addresses.Select(MapToDto);
        }

        public async Task<AddressDto?> GetAddressByIdAsync(Guid id, Guid userId)
        {
            var address = await _addressRepository.GetByIdAsync(id);
            if (address == null || address.UserId != userId)
                return null;

            return MapToDto(address);
        }

        public async Task<AddressDto?> GetDefaultAddressAsync(Guid userId)
        {
            var address = await _addressRepository.GetDefaultByUserIdAsync(userId);
            return address != null ? MapToDto(address) : null;
        }

        public async Task<AddressDto> CreateAddressAsync(Guid userId, CreateAddressDto createAddressDto)
        {
            var address = new Address
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = createAddressDto.Name,
                Phone = createAddressDto.Phone,
                Province = createAddressDto.Province,
                City = createAddressDto.City,
                District = createAddressDto.District,
                Street = createAddressDto.Street,
                PostalCode = createAddressDto.PostalCode,
                IsDefault = createAddressDto.IsDefault,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdAddress = await _addressRepository.CreateAsync(address);
            _logger.LogInformation("Created address {AddressId} for user {UserId}", createdAddress.Id, userId);

            return MapToDto(createdAddress);
        }

        public async Task<AddressDto> UpdateAddressAsync(Guid id, Guid userId, UpdateAddressDto updateAddressDto)
        {
            var address = await _addressRepository.GetByIdAsync(id);
            if (address == null || address.UserId != userId)
                throw new ArgumentException("Address not found or access denied");

            address.Name = updateAddressDto.Name;
            address.Phone = updateAddressDto.Phone;
            address.Province = updateAddressDto.Province;
            address.City = updateAddressDto.City;
            address.District = updateAddressDto.District;
            address.Street = updateAddressDto.Street;
            address.PostalCode = updateAddressDto.PostalCode;
            address.IsDefault = updateAddressDto.IsDefault;
            address.UpdatedAt = DateTime.UtcNow;

            var updatedAddress = await _addressRepository.UpdateAsync(address);
            _logger.LogInformation("Updated address {AddressId} for user {UserId}", updatedAddress.Id, userId);

            return MapToDto(updatedAddress);
        }

        public async Task<bool> DeleteAddressAsync(Guid id, Guid userId)
        {
            var address = await _addressRepository.GetByIdAsync(id);
            if (address == null || address.UserId != userId)
                return false;

            var result = await _addressRepository.DeleteAsync(id);
            if (result)
            {
                _logger.LogInformation("Deleted address {AddressId} for user {UserId}", id, userId);
            }

            return result;
        }

        public async Task<bool> SetAsDefaultAsync(Guid id, Guid userId)
        {
            var result = await _addressRepository.SetAsDefaultAsync(id, userId);
            if (result)
            {
                _logger.LogInformation("Set address {AddressId} as default for user {UserId}", id, userId);
            }

            return result;
        }

        public async Task<bool> ValidateAddressExistsAsync(Guid addressId, Guid userId)
        {
            var address = await _addressRepository.GetByIdAsync(addressId);
            return address != null && address.UserId == userId;
        }

        private static AddressDto MapToDto(Address address)
        {
            return new AddressDto
            {
                Id = address.Id,
                UserId = address.UserId,
                Name = address.Name,
                Phone = address.Phone,
                Province = address.Province,
                City = address.City,
                District = address.District,
                Street = address.Street,
                PostalCode = address.PostalCode,
                IsDefault = address.IsDefault,
                FullAddress = address.FullAddress,
                CreatedAt = address.CreatedAt,
                UpdatedAt = address.UpdatedAt
            };
        }
    }
}
