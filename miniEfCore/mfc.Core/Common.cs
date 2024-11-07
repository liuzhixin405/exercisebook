using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mfc.Core
{
    public static class Common
    {
        // 映射数据库字段到实体（假设简单的字段映射，实际可能要考虑类型转换等）
        public static void MapReaderToEntity<TEntity>(DbDataReader reader, TEntity entity) where TEntity : class
        {
            var properties = typeof(TEntity).GetProperties();

            foreach (var property in properties)
            {
                // 尝试获取列索引，如果列不存在则返回-1
                var columnIndex = reader.GetOrdinal(property.Name);

                // 如果列存在且不是 DBNull，则设置值
                if (columnIndex >= 0 && reader.IsDBNull(columnIndex) == false)
                {
                    property.SetValue(entity, reader.GetValue(columnIndex));
                }
            }
        }
    }
}
