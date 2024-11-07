using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mfc.Core
{
    public class DatabaseProviderFactory
    {
        public static IDatabaseProvider CreateProvider(string databaseType, string connectionString="")
        {
            if(databaseType.Equals("mysql") || databaseType.Equals("sqlserver"))
            {
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ArgumentException("Connection string cannot be null or empty");
                }
            }
            switch (databaseType.ToLower())
            {
                case "mysql":
                    return new MySqlDatabaseProvider(connectionString);
                case "sqlserver":
                    return new SqlServerDatabaseProvider(connectionString);
                case "inmemory":
                    return new InMemoryDatabaseProvider();
                default:
                    throw new NotSupportedException($"Database type {databaseType} not supported");
            }
        }
    }
}
