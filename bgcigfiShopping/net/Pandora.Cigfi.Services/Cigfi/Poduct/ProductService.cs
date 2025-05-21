using Dapper;
using Pandora.Cigfi.IServices.Cigfi;
using Pandora.Cigfi.Models.Cigfi.Product;
using Pandora.Cigfi.Models.ResponseMsg;
using FXH.Common.DapperService;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pandora.Cigfi.Services.Cigfi
{
    public class ProductService : BaseRepository<Product>, IProductService
    {
        private readonly ILogger<ProductService> _logger;
        
        public ProductService(IDapperRepository context, ILogger<ProductService> logger) : base(context) 
        {
            _logger = logger;
        }

        public async Task<PagedResult<ProductViewModel>> GetPageListAsync(Hashtable queryHt, int page = 1, int limit = 20)
        {
            string sql = "SELECT * FROM cigfi_product ";
            var parameters = new DynamicParameters();
            sql += GetWhere(queryHt, parameters);
            sql += $" ORDER BY Id DESC LIMIT {(page - 1) * limit},{limit}";
            using (var connection = DbContext.Connection)
            {
                var list = await connection.QueryAsync<ProductViewModel>(sql, parameters);
                return new PagedResult<ProductViewModel>
                {
                    Total = await CountAsync(queryHt),
                    Items = list.ToList()
                };
            }
        }

        public async Task<int> CountAsync(Hashtable queryHt)
        {
            string sql = "SELECT COUNT(*) FROM cigfi_product ";
            var parameters = new DynamicParameters();
            sql += GetWhere(queryHt, parameters);
            using (var connection = DbContext.Connection)
            {
                return await connection.ExecuteScalarAsync<int>(sql, parameters);
            }
        }

        public async Task<ProductViewModel> GetByIdAsync(int id)
        {
            string sql = "SELECT * FROM cigfi_product WHERE Id=@Id";
            using (var connection = DbContext.Connection)
            {
                return await connection.QueryFirstOrDefaultAsync<ProductViewModel>(sql, new { Id = id });
            }
        }

        public async Task<bool> AddAsync(Product model)
        {
            // 去重校验：同名同分类下不能重复
            string checkSql = "SELECT COUNT(1) FROM cigfi_product WHERE Name=@Name AND CategoryId=@CategoryId";
            using (var connection = DbContext.Connection)
            {
                var exists = await connection.ExecuteScalarAsync<int>(checkSql, new { model.Name, model.CategoryId });
                if (exists > 0)
                {
                    _logger.LogWarning($"Duplicate product: {model.Name} in category {model.CategoryId}");
                    return false;
                }
                string sql = "INSERT INTO cigfi_product (Name, Description, Price, ImageUrl, ThumbnailUrls, SoldCount, Stock, CategoryId, Params, CreatedAt, UpdatedAt) VALUES (@Name, @Description, @Price, @ImageUrl, @ThumbnailUrls, @SoldCount, @Stock, @CategoryId, @Params, @CreatedAt, @UpdatedAt)";
                try 
                {
                    var affectedRows = await connection.ExecuteAsync(sql, model);
                    if (affectedRows > 0)
                    {
                        _logger.LogInformation($"Product added successfully: {model.Name}");
                        return true;
                    }
                    _logger.LogWarning("No rows affected when adding product");
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error adding product: {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<bool> UpdateAsync(Product model)
        {
            // 去重校验：同名同分类下不能重复（排除自身）
            string checkSql = "SELECT COUNT(1) FROM cigfi_product WHERE Name=@Name AND CategoryId=@CategoryId AND Id<>@Id";
            using (var connection = DbContext.Connection)
            {
                var exists = await connection.ExecuteScalarAsync<int>(checkSql, new { model.Name, model.CategoryId, model.Id });
                if (exists > 0)
                {
                    _logger.LogWarning($"Duplicate product on update: {model.Name} in category {model.CategoryId}");
                    return false;
                }
                string sql = "UPDATE cigfi_product SET Name=@Name, Description=@Description, Price=@Price, ImageUrl=@ImageUrl, ThumbnailUrls=@ThumbnailUrls, SoldCount=@SoldCount, Stock=@Stock, CategoryId=@CategoryId, Params=@Params, UpdatedAt=@UpdatedAt WHERE Id=@Id";
                return await connection.ExecuteAsync(sql, model) > 0;
            }
        }

        public async Task<bool> DeleteAsync(List<int> ids)
        {
            string sql = "DELETE FROM cigfi_product WHERE Id IN @Ids";
            using (var connection = DbContext.Connection)
            {
                return await connection.ExecuteAsync(sql, new { Ids = ids }) > 0;
            }
        }

        private string GetWhere(Hashtable queryHt, DynamicParameters parameters)
        {
            string where = string.Empty;
            if (queryHt.Count > 0)
            {
                if (queryHt.Contains("keyword"))
                {
                    where += " Name LIKE @Keyword ";
                    parameters.Add("Keyword", $"%{queryHt["keyword"]}%");
                }
            }
            if (!string.IsNullOrEmpty(where))
            {
                where = " WHERE " + where;
            }
            return where;
        }
    }
}
