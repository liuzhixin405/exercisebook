﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperDal.Sql
{
    /// <summary>
    /// SQL Server方言类
    /// </summary>
    public class SqlServerDialect : SqlDialectBase
    {
        /// <inheritdoc />
        public override char OpenQuote
        {
            get { return '['; }
        }

        /// <inheritdoc />
        public override char CloseQuote
        {
            get { return ']'; }
        }

        /// <inheritdoc />
        public override string GetIdentitySql(string tableName)
        {
            return string.Format("SELECT CAST(SCOPE_IDENTITY()  AS BIGINT) AS [Id]");
        }

        /// <inheritdoc />
        public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
        {
            int startValue = (page * resultsPerPage) + 1;
            return GetSetSql(sql, startValue, resultsPerPage, parameters);
        }

        /// <inheritdoc />
        public override string GetSetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException("SQL");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters");
            }

            int selectIndex = GetSelectEnd(sql) + 1;
            string orderByClause = GetOrderByClause(sql);
            if (orderByClause == null)
            {
                orderByClause = "ORDER BY CURRENT_TIMESTAMP";
            }


            string projectedColumns = GetColumnNames(sql).Aggregate(new StringBuilder(), (sb, s) => (sb.Length == 0 ? sb : sb.Append(", ")).Append(GetColumnName("_proj", s, null)), sb => sb.ToString());
            string newSql = sql
                .Replace(" " + orderByClause, string.Empty)
                .Insert(selectIndex, string.Format("ROW_NUMBER() OVER(ORDER BY {0}) AS {1}, ", orderByClause.Substring(9), GetColumnName(null, "_row_number", null)));

            string result = string.Format("SELECT TOP({0}) {1} FROM ({2}) [_proj] WHERE {3} >= @_pageStartRow ORDER BY {3}",
                maxResults, projectedColumns.Trim(), newSql, GetColumnName("_proj", "_row_number", null));

            parameters.Add("@_pageStartRow", firstResult);
            return result;
        }

        /// <summary>
        /// 获取SQL语句的排序部分语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>排序部分语句</returns>
        protected string GetOrderByClause(string sql)
        {
            int orderByIndex = sql.LastIndexOf(" ORDER BY ", StringComparison.InvariantCultureIgnoreCase);
            if (orderByIndex == -1)
            {
                return null;
            }

            string result = sql.Substring(orderByIndex).Trim();

            int whereIndex = result.IndexOf(" WHERE ", StringComparison.InvariantCultureIgnoreCase);
            if (whereIndex == -1)
            {
                return result;
            }

            return result.Substring(0, whereIndex).Trim();
        }

        /// <summary>
        /// 获取SQL语句的FROM索引
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>FROM索引</returns>
        protected int GetFromStart(string sql)
        {
            int selectCount = 0;
            string[] words = sql.Split(' ');
            int fromIndex = 0;
            foreach (var word in words)
            {
                if (word.Equals("SELECT", StringComparison.InvariantCultureIgnoreCase))
                {
                    selectCount++;
                }

                if (word.Equals("FROM", StringComparison.InvariantCultureIgnoreCase))
                {
                    selectCount--;
                    if (selectCount == 0)
                    {
                        break;
                    }
                }

                fromIndex += word.Length + 1;
            }

            return fromIndex;
        }

        /// <summary>
        /// 获取SQL语句的SELECT索引
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>SELECT索引</returns>
        protected virtual int GetSelectEnd(string sql)
        {
            if (sql.StartsWith("SELECT DISTINCT", StringComparison.InvariantCultureIgnoreCase))
            {
                return 15;
            }

            if (sql.StartsWith("SELECT", StringComparison.InvariantCultureIgnoreCase))
            {
                return 6;
            }

            throw new ArgumentException("SQL must be a SELECT statement.", "sql");
        }

        /// <summary>
        /// 获取SQL语句里的所有字段名
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>字段名列表</returns>
        protected virtual IList<string> GetColumnNames(string sql)
        {
            int start = GetSelectEnd(sql);
            int stop = GetFromStart(sql);
            string[] columnSql = sql.Substring(start, stop - start).Split(',');
            List<string> result = new List<string>();
            foreach (string c in columnSql)
            {
                int index = c.IndexOf(" AS ", StringComparison.InvariantCultureIgnoreCase);
                if (index > 0)
                {
                    result.Add(c.Substring(index + 4).Trim());
                    continue;
                }

                string[] colParts = c.Split('.');
                result.Add(colParts[colParts.Length - 1].Trim());
            }

            return result;
        }

        /// <inheritdoc />
        public override string SelectLimit(string sql, int limit)
        {
            const string searchFor = "SELECT ";
            if (sql.Contains("TOP ("))
            {
                return sql;
            }

            return sql.Insert(sql.IndexOf(searchFor, StringComparison.OrdinalIgnoreCase) + searchFor.Length,
                string.Format("TOP ({0}) ", limit));
        }

        /// <inheritdoc />
        public override string SetNolock(string sql)
        {
            const string withNolock = " WITH (NOLOCK)";
            const string where = " WHERE ";
            const string orderby = " ORDER BY ";

            if (sql.Contains(withNolock))
            {
                return sql;
            }

            if (sql.Contains(where))
            {
                return sql.Insert(sql.IndexOf(where, StringComparison.OrdinalIgnoreCase), withNolock);
            }

            if (sql.Contains(orderby))
            {
                return sql.Insert(sql.IndexOf(orderby, StringComparison.OrdinalIgnoreCase), withNolock);
            }

            return string.Concat(sql, withNolock);
        }
    }
}