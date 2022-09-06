using ServiceStack.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace RedisDemo.Service
{
    public class RedisManager_Back
    {
        private static readonly object Locker = new object();

        private static ConnectionMultiplexer _instance;
        private static readonly ConcurrentDictionary<string, ConnectionMultiplexer> ConnectionCache = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        /// <summary>
        /// redis保存的Key前缀，会自动添加到指定的Key名称前
        /// </summary>
        internal static readonly string RedisSysCustomKey = ApplicationConfig.Configuration["RedisSysCustomKey"];
        internal static readonly int RedisDataBaseIndex =int.Parse( ApplicationConfig.Configuration["RedisDataBaseIndex"]);
        internal static readonly string RedisHostConnection = ApplicationConfig.Configuration["RedisHostConnection"];

        public static ConnectionMultiplexer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Locker)
                    {
                        if(_instance == null || !_instance.IsConnected)
                        {
                            _instance = GetManager();
                        }
                    }
                }
                return _instance;
            }
        }

        public static ConnectionMultiplexer GetConnectionMultiplexer(string connectionString)
        {
            if (!ConnectionCache.ContainsKey(connectionString))
            {
                ConnectionCache[connectionString] = GetManager(connectionString);
            }
            return ConnectionCache[connectionString];
        }

        private static ConnectionMultiplexer GetManager(string connectionString = null)
        {
            connectionString = connectionString ?? RedisHostConnection;
            var connect = ConnectionMultiplexer.Connect(connectionString);
            connect.ConnectionFailed += MuxerConnectionFailed;
            connect.ConnectionRestored += MuxerConnectionRestored;
            connect.ErrorMessage += MuxerErrorMessage;
            connect.ConfigurationChanged += MuxerConfigurationChanged;
            connect.HashSlotMoved += MuxerHashSlotMoved;
            connect.InternalError += MuxerInternalError;
            return connect;
        }

        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            
        }

        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            
        }

        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            
        }

        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
           
        }

        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            
        }

        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            
        }
    }
    
 
}
