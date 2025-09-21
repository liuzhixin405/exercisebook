using ECommerce.Domain.Entities;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Repositories
{
    /// <summary>
    /// 购物车仓储实现
    /// </summary>
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ECommerceDbContext _context;

        public ShoppingCartRepository(ECommerceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 根据用户ID获取购物车
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>购物车</returns>
        public async Task<ShoppingCart?> GetByUserIdAsync(Guid userId)
        {
            return await _context.ShoppingCarts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        /// <summary>
        /// 根据ID获取购物车
        /// </summary>
        /// <param name="id">购物车ID</param>
        /// <returns>购物车</returns>
        public async Task<ShoppingCart?> GetByIdAsync(Guid id)
        {
            return await _context.ShoppingCarts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// 添加购物车
        /// </summary>
        /// <param name="shoppingCart">购物车</param>
        /// <returns>购物车</returns>
        public async Task<ShoppingCart> AddAsync(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = Guid.NewGuid();
            shoppingCart.CreatedAt = DateTime.UtcNow;
            shoppingCart.UpdatedAt = DateTime.UtcNow;
            
            _context.ShoppingCarts.Add(shoppingCart);
            await _context.SaveChangesAsync();
            
            return shoppingCart;
        }

        /// <summary>
        /// 更新购物车
        /// </summary>
        /// <param name="shoppingCart">购物车</param>
        /// <returns>购物车</returns>
        public async Task<ShoppingCart> UpdateAsync(ShoppingCart shoppingCart)
        {
            shoppingCart.UpdatedAt = DateTime.UtcNow;
            _context.ShoppingCarts.Update(shoppingCart);
            await _context.SaveChangesAsync();
            
            return shoppingCart;
        }

        /// <summary>
        /// 删除购物车
        /// </summary>
        /// <param name="id">购物车ID</param>
        /// <returns>是否删除成功</returns>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var shoppingCart = await _context.ShoppingCarts.FindAsync(id);
            if (shoppingCart == null)
                return false;

            _context.ShoppingCarts.Remove(shoppingCart);
            await _context.SaveChangesAsync();
            
            return true;
        }

        /// <summary>
        /// 添加购物车项
        /// </summary>
        /// <param name="item">购物车项</param>
        /// <returns>购物车项</returns>
        public async Task<ShoppingCartItem> AddItemAsync(ShoppingCartItem item)
        {
            item.Id = Guid.NewGuid();
            item.CreatedAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;
            
            _context.ShoppingCartItems.Add(item);
            await _context.SaveChangesAsync();
            
            return item;
        }

        /// <summary>
        /// 更新购物车项
        /// </summary>
        /// <param name="item">购物车项</param>
        /// <returns>购物车项</returns>
        public async Task<ShoppingCartItem> UpdateItemAsync(ShoppingCartItem item)
        {
            item.UpdatedAt = DateTime.UtcNow;
            _context.ShoppingCartItems.Update(item);
            await _context.SaveChangesAsync();
            
            return item;
        }

        /// <summary>
        /// 删除购物车项
        /// </summary>
        /// <param name="id">购物车项ID</param>
        /// <returns>是否删除成功</returns>
        public async Task<bool> DeleteItemAsync(Guid id)
        {
            var item = await _context.ShoppingCartItems.FindAsync(id);
            if (item == null)
                return false;

            _context.ShoppingCartItems.Remove(item);
            await _context.SaveChangesAsync();
            
            return true;
        }

        /// <summary>
        /// 根据购物车ID和产品ID获取购物车项
        /// </summary>
        /// <param name="cartId">购物车ID</param>
        /// <param name="productId">产品ID</param>
        /// <returns>购物车项</returns>
        public async Task<ShoppingCartItem?> GetItemByCartAndProductAsync(Guid cartId, Guid productId)
        {
            return await _context.ShoppingCartItems
                .FirstOrDefaultAsync(i => i.ShoppingCartId == cartId && i.ProductId == productId);
        }

        /// <summary>
        /// 清空购物车
        /// </summary>
        /// <param name="cartId">购物车ID</param>
        /// <returns>是否清空成功</returns>
        public async Task<bool> ClearCartAsync(Guid cartId)
        {
            var items = await _context.ShoppingCartItems
                .Where(i => i.ShoppingCartId == cartId)
                .ToListAsync();
                
            if (items.Count == 0)
                return true;

            _context.ShoppingCartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
            
            return true;
        }
    }
}