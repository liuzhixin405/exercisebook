using DesignPatternsForDatabase.Factory;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.Common;

namespace DesignPatternsForDatabase.Provider
{
    public abstract class Database    //模板类型
    {
        protected string name;
        protected string connectionString;
        protected DbProviderFactory factory; //抽象工厂
        protected const string SystemTableNameRoot = "Table";
        public EventHandler<DbEventArgs> BeforeExecution;
        public EventHandler<DbEventArgs> AfterExecution;
        public Database(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            IConfiguration configuration = GlobalConfigure.GlobalServiceProvider.CreateAsyncScope().ServiceProvider.GetRequiredService<IConfiguration>();
            var providerName = configuration.GetSection("ConnectionStrings").GetSection("LocalSQL").GetSection("ProviderName").ToString();
            connectionString = configuration.GetSection("ConnectionStrings").GetSection("LocalSQL").GetSection("ConnectionString").ToString();
            //factory =DbProviderFactories.GetFactory(providerName);
            factory = DatabaseFactory.Registry;
            this.name = name;
        }
        /// <summary>
        /// 定义参数命名采用的前缀，具体前缀交给子类实现
        /// </summary>
        protected abstract string ParameterPrefix { get; }

        /// <summary>
        /// 实际执行查询内部方法
        /// </summary>
        /// <param name="command"></param>
        /// <param name="tableNames"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private DataSet DoExecuteDataSet(DbCommand command, string[] tableNames)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (command.Connection == null) throw new ArgumentNullException("command.conenction");
            DbEventArgs args = new DbEventArgs();
            args.Command = command;
            if (BeforeExecution != null)
                BeforeExecution(this, args);
            using (DbDataAdapter adapter = factory.CreateDataAdapter())
            {
                adapter.SelectCommand = command;
                if (tableNames != null)
                {
                    for (int i = 0; i < tableNames.Length; i++)
                    {
                        string systemTableName = i == 0 ? SystemTableNameRoot : tableNames[i];
                        adapter.TableMappings.Add(systemTableName, tableNames[i]);
                    }
                }
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet);
                if (AfterExecution != null)
                    AfterExecution(this, args);
                return dataSet;
            }
        }

        /// <summary>
        /// 创建查询命令对象
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected virtual DbCommand CreateCommand(string commandText, params IDataParameter[] parameters)
        {
            DbCommand command = factory.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = CommandType.Text;
            if (parameters != null && parameters.Length > 0)
            {
                foreach (var parameter in parameters)
                {
                    if (!parameter.ParameterName.StartsWith(ParameterPrefix))
                    {
                        parameter.ParameterName += ParameterPrefix;
                    }
                    command.Parameters.Add(parameter);
                }
            }
            return command;
        }

        public virtual DataSet ExecuteDataSet(string commandText, string[] tableNames, params IDataParameter[] parameters)
        {
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException(nameof(commandText));
            using (DbConnection connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                DbCommand dbCommand = CreateCommand(commandText, parameters);
                dbCommand.Connection = connection;
                return DoExecuteDataSet(dbCommand, tableNames);
            }
        }
        public virtual DataSet ExecuteDataSet(string commandText)
        {
            return ExecuteDataSet(commandText, null);
        }
    }
}
