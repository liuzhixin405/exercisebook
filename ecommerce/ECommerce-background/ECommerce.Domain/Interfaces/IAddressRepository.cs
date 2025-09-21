using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Interfaces
{
    public interface IAddressRepository
    {
        Task<IEnumerable<Address>> GetByUserIdAsync(Guid userId);
        Task<Address?> GetByIdAsync(Guid id);
        Task<Address?> GetDefaultByUserIdAsync(Guid userId);
        Task<Address> CreateAsync(Address address);
        Task<Address> UpdateAsync(Address address);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> SetAsDefaultAsync(Guid id, Guid userId);
        Task<bool> ExistsAsync(Guid id);
    }
}
