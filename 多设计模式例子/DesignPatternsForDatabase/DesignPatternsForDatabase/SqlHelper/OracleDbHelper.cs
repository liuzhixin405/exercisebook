using System.Data.SqlClient;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DesignPatternsForDatabase.SqlHelper
{
    public static class OracleDbHelper //后期被database代替
    {

        private const string SystemTableNameRoot = "Table";

        /// <summary>
        /// 实际执行查询的内部方法
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static DataSet DoExecuteDataSet(OracleCommand command, string[] tableNames)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (command.Connection == null) throw new ArgumentNullException("command.connection");
            using (OracleDataAdapter adapter = new OracleDataAdapter(command))
            {
                if (tableNames != null)
                    for (int i = 0; i < tableNames.Length; i++)
                    {
                        string systemTableName = (i == 0) ? SystemTableNameRoot : tableNames[i];
                        adapter.TableMappings.Add(systemTableName, tableNames[i]);
                    }
                DataSet result = new DataSet();
                adapter.Fill(result);
                return result;
            }
        }

        /// <summary>
        /// 返回sql查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="tableNames"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static DataSet ExecuteDataSet(string connectionString, string commandText, string[] tableNames, params SqlParameter[] parameters)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException(nameof(commandText));
            using (OracleConnection conenction = new OracleConnection(connectionString))
            {
                OracleCommand command = new OracleCommand();
                command.Connection = conenction;
                command.CommandType = CommandType.Text;
                command.CommandText = commandText;
                if (parameters != null && parameters.Length > 0)
                {
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }
                }
                return DoExecuteDataSet(command, tableNames);
            }
        }
        /// <summary>
        /// 返回sql查询结果
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string connectionString, string commandText)
        {
            return ExecuteDataSet(connectionString, commandText, null);
        }
    }
}
