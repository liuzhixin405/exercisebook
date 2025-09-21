using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly ECommerceDbContext _context;

        public AddressRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Address>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Address?> GetByIdAsync(Guid id)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Address?> GetDefaultByUserIdAsync(Guid userId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);
        }

        public async Task<Address> CreateAsync(Address address)
        {
            // 如果设置为默认地址，先取消其他默认地址
            if (address.IsDefault)
            {
                await ClearDefaultAddressAsync(address.UserId);
            }

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<Address> UpdateAsync(Address address)
        {
            // 如果设置为默认地址，先取消其他默认地址
            if (address.IsDefault)
            {
                await ClearDefaultAddressAsync(address.UserId);
            }

            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
                return false;

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetAsDefaultAsync(Guid id, Guid userId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            
            if (address == null)
                return false;

            // 先取消其他默认地址
            await ClearDefaultAddressAsync(userId);

            // 设置当前地址为默认
            address.IsDefault = true;
            address.UpdatedAt = DateTime.UtcNow;

            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Addresses
                .AnyAsync(a => a.Id == id);
        }

        private async Task ClearDefaultAddressAsync(Guid userId)
        {
            var defaultAddresses = await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault)
                .ToListAsync();

            foreach (var addr in defaultAddresses)
            {
                addr.IsDefault = false;
                addr.UpdatedAt = DateTime.UtcNow;
            }

            if (defaultAddresses.Any())
            {
                _context.Addresses.UpdateRange(defaultAddresses);
            }
        }
    }
}
