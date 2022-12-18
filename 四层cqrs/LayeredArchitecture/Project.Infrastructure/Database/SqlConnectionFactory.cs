using Project.Application.Configuration.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Database
{
    public class SqlConnectionFactory : ISqlConnectionFactory,IDisposable
    {
        private readonly string _connectionString;
        private IDbConnection _connection;
        public SqlConnectionFactory(string connectionString)
        {
            _connectionString= connectionString;
        }

        public void Dispose()
        {
            if(_connection != null &&_connection.State== ConnectionState.Open) {
                _connection.Dispose();
            }
        }

        public IDbConnection GetOpenConnection()
        {
            if(this._connection== null||this._connection.State!= ConnectionState.Open) {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }
            return _connection;
        }
    }
}
