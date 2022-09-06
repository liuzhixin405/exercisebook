using System;

namespace 抽象工厂和工厂方法
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Hello World!");
        }
    }
    public class App
    {
        /// <summary>
        /// 单一工厂,connection的一组实现 ,可能会出现connection是sqlserver,下面的command是mysql,darareader是oracle。
        /// </summary>
        IDbConnectionFactory _connectionFactory;
        IDbCommandFactory _commandFactory; //省略构造实现
        IDbDataReaderFactory _readerFactory;  //省略构造实现
        /// <summary>
        ///高度抽象sqlserver mysql oracle
        /// </summary>
        IDbConnection _conn;
        IDbCommand _command;
        IDbDataReader _reader;

        /// <summary>
        /// 单一工厂,command connection datareader一套实现  ,不会出现串的问题
        /// </summary>
        IDbFactory _dbFactory;
        /// <summary>
        /// 通过运行时来确定具体传入的工厂 可以是 sqldbconnection 也可以是 mysqlconnection  oracleconnection
        /// </summary>
        public App(IDbConnection conn, IDbCommand command, IDbDataReader reader, IDbConnectionFactory connectionFactory, IDbFactory dbFactory)
        {
            _conn = conn;
            _command = command;
            _reader = reader;
            _connectionFactory = connectionFactory;
            _dbFactory = dbFactory;
        }
        public void Init()
        {
            IDbConnection conn = new SqlConnection();
            IDbConnection conn2 = _connectionFactory.Create();
            IDbCommand dBCommand = new SqlCommand();
            IDbDataReader reader = new SqlDataReader();
        }

        public void Connect(string conn)
        {
            _conn.Connect(conn);
        }
        //省略
    }
    #region 抽象接口
    public interface IDbConnection
    {
        void Connect(string connectionString);
    }
    public interface IDbCommand
    {
        IDbCommand CreateCommand();
        int ExtcuteSql();
    }

    public interface IDbDataReader
    {
        string ExecuteReader();
    }
    #endregion

    #region sqlserver
    public class SqlConnection : IDbConnection
    {
        public void Connect(string connectionString)
        {
            throw new NotImplementedException();
        }
    }

    public class SqlCommand : IDbCommand
    {
        public IDbCommand CreateCommand()
        {
            throw new NotImplementedException();
        }

        public int ExtcuteSql()
        {
            throw new NotImplementedException();
        }
    }

    public class SqlDataReader : IDbDataReader
    {
        public string ExecuteReader()
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region mysql
    public class MySqlConnection : IDbConnection
    {
        public void Connect(string connectionString)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
    #region factory 有可能idbconnection 是sqlserver idbcommand是mysql 
    public interface IDbConnectionFactory
    {
        public IDbConnection Create();

    }
    public interface IDbCommandFactory
    {
        //略
    }

    public interface IDbDataReaderFactory
    {
        //略
    }

    public class SqlDbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection Create()
        {
            return new SqlConnection();
        }
    }

    public class MySqlDbConnectionFactory : IDbConnectionFactory
    {
        public IDbConnection Create()
        {
            return new MySqlConnection();
        }
    }

    #endregion

    #region 抽象工厂 针对是一类的抽象   工厂方法是针对单一对象,它可以是抽象工厂的特例 ，加入抽象工厂里只有一个对象

    /// <summary>
    /// 实现了IDbCommand、IDbConnection、IDbDataReader的抽象 工厂，可自动扩展
    /// </summary>
    public interface IDbFactory
    {
        IDbCommand CreateCommand();
        IDbConnection CreateConnection();
        IDbDataReader CreateDataReader();
    }
    /// <summary>
    /// sqlserver工厂
    /// </summary>
    public class SqlDbFactory : IDbFactory
    {
        public IDbCommand CreateCommand()
        {
            return new SqlCommand();
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection();
        }
        public IDbDataReader CreateDataReader()
        {
            return new SqlDataReader();
        }
    }
    #endregion
}
