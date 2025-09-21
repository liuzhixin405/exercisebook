using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerce.Domain.Interfaces
{
    /// <summary>
    /// 购物车仓储接口
    /// </summary>
    public interface IShoppingCartRepository
    {
        /// <summary>
        /// 根据用户ID获取购物车
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>购物车</returns>
        Task<ShoppingCart?> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// 根据ID获取购物车
        /// </summary>
        /// <param name="id">购物车ID</param>
        /// <returns>购物车</returns>
        Task<ShoppingCart?> GetByIdAsync(Guid id);

        /// <summary>
        /// 添加购物车
        /// </summary>
        /// <param name="shoppingCart">购物车</param>
        /// <returns>购物车</returns>
        Task<ShoppingCart> AddAsync(ShoppingCart shoppingCart);

        /// <summary>
        /// 更新购物车
        /// </summary>
        /// <param name="shoppingCart">购物车</param>
        /// <returns>购物车</returns>
        Task<ShoppingCart> UpdateAsync(ShoppingCart shoppingCart);

        /// <summary>
        /// 删除购物车
        /// </summary>
        /// <param name="id">购物车ID</param>
        /// <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// 添加购物车项
        /// </summary>
        /// <param name="item">购物车项</param>
        /// <returns>购物车项</returns>
        Task<ShoppingCartItem> AddItemAsync(ShoppingCartItem item);

        /// <summary>
        /// 更新购物车项
        /// </summary>
        /// <param name="item">购物车项</param>
        /// <returns>购物车项</returns>
        Task<ShoppingCartItem> UpdateItemAsync(ShoppingCartItem item);

        /// <summary>
        /// 删除购物车项
        /// </summary>
        /// <param name="id">购物车项ID</param>
        /// <returns>是否删除成功</returns>
        Task<bool> DeleteItemAsync(Guid id);

        /// <summary>
        /// 根据购物车ID和产品ID获取购物车项
        /// </summary>
        /// <param name="cartId">购物车ID</param>
        /// <param name="productId">产品ID</param>
        /// <returns>购物车项</returns>
        Task<ShoppingCartItem?> GetItemByCartAndProductAsync(Guid cartId, Guid productId);

        /// <summary>
        /// 清空购物车
        /// </summary>
        /// <param name="cartId">购物车ID</param>
        /// <returns>是否清空成功</returns>
        Task<bool> ClearCartAsync(Guid cartId);
    }
}