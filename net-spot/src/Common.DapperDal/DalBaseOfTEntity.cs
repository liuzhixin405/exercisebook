using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using DapperDal.Mapper;
using MySql.Data.MySqlClient;

namespace DapperDal
{
    /// <summary>
    /// ʵ�����ݷ��ʲ����
    /// </summary>
    /// <typeparam name="TEntity">ʵ������</typeparam>
    public class DalBase<TEntity> : DalBase<TEntity, int> where TEntity : class
    {
        /// <summary>
        /// Ĭ�ϳ�ʼ�� DAL ��ʵ��
        /// </summary>
        public DalBase() : this("Default")
        {
        }

        /// <summary>
        /// �����ýڵ�����ʼ�� DAL ��ʵ��
        /// </summary>
        /// <param name="connectionName">DB�����ַ������ýڵ���</param>
        /// <exception cref="ArgumentNullException">����Ϊ��</exception>
        /// <exception cref="ConfigurationErrorsException">�Ҳ������ýڵ�</exception>
        public DalBase(string connectionName) : base(connectionName)
        {
        }
    }

    /// <summary>
    /// ʵ�����ݷ��ʲ����
    /// </summary>
    /// <typeparam name="TEntity">ʵ������</typeparam>
    /// <typeparam name="TPrimaryKey">ʵ��ID������������</typeparam>
    public partial class DalBase<TEntity, TPrimaryKey> where TEntity : class
    {
        static DalBase()
        {
            SetDefaultConfiguration();
        }

        /// <summary>
        /// Ĭ�ϳ�ʼ�� DAL ��ʵ��
        /// </summary>
        public DalBase() : this("Default")
        {
        }

        /// <summary>
        /// �����ýڵ�����ʼ�� DAL ��ʵ��
        /// </summary>
        /// <param name="connNameOrConnStr">DB�����ַ������ýڵ���</param>
        /// <exception cref="ArgumentNullException">����Ϊ��</exception>
        /// <exception cref="ConfigurationErrorsException">�Ҳ������ýڵ�</exception>
        public DalBase(string connNameOrConnStr)
        {
            Configuration = DalConfiguration.Default;

            // ��ʼ��������
            SetDefaultOptions();

            ConnectionString = ResolveConnectionString(connNameOrConnStr);
        }

        /// <summary>
        /// ������
        /// </summary>
        public IDalConfiguration Configuration { get; private set; }

        /// <summary>
        /// ������
        /// </summary>
        public DalOptions Options { get; private set; }

        /// <summary>
        /// DB�����ַ���
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// ��DB����
        /// </summary>
        /// <returns>DB����</returns>
        protected virtual IDbConnection OpenConnection()
        {
            return OpenConnection(ConnectionString);
        }

        /// <summary>
        /// ��DB����
        /// </summary>
        /// <param name="connNameOrConnStr">DB �����ַ������ýڵ���</param>
        /// <returns>DB����</returns>
        protected virtual IDbConnection OpenConnection(string connNameOrConnStr)
        {
            var connectionString = ResolveConnectionString(connNameOrConnStr);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            var connection = new SqlConnection(connectionString);
            if (connection == null)
                throw new ConfigurationErrorsException(
                    string.Format("Failed to create a connection using the connection string '{0}'.", connectionString));

            connection.Open();

            return connection;
        }

        /// <summary>
        /// ��ʼ��������
        /// </summary>
        private static void SetDefaultConfiguration()
        {
            if (DalConfiguration.Default != null)
            {
                DalConfiguration.Default.DefaultMapper = typeof(AutoEntityMapper<>);
                DalConfiguration.Default.Nolock = true;
                DalConfiguration.Default.Buffered = true;
            }
        }

        /// <summary>
        /// ��ʼ��������
        /// </summary>
        private void SetDefaultOptions()
        {
            if (Options == null)
            {
                Options = new DalOptions();
            }

            Options.SoftDeletePropsFactory = () => new { IsActive = 0, UpdateTime = DateTime.Now };
            Options.SoftActivePropsFactory = () => new { IsActive = 1, UpdateTime = DateTime.Now };
        }

        /// <summary>
        /// ��ȡ DB ���Ӵ�
        /// </summary>
        /// <param name="connNameOrConnStr">DB �����ַ������ýڵ���</param>
        /// <returns>DB ���Ӵ�</returns>
        private string ResolveConnectionString(string connNameOrConnStr)
        {
            if (string.IsNullOrEmpty(connNameOrConnStr))
            {
                throw new ArgumentNullException("connNameOrConnStr");
            }

            if (connNameOrConnStr.Contains("=") || connNameOrConnStr.Contains(";"))
            {
                return connNameOrConnStr;
            }
            else
            {
                var conStr = ConfigurationManager.ConnectionStrings[connNameOrConnStr];
                if (conStr == null)
                {
                    throw new ConfigurationErrorsException(
                        string.Format("Failed to find connection string named '{0}' in app/web.config.", connNameOrConnStr));
                }

                return conStr.ConnectionString;
            }
        }

    }
}