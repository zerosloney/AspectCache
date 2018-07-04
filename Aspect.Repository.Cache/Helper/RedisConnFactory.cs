using System;
using StackExchange.Redis;

namespace Aspect.Repository.Cache
{
    internal static class RedisConnFactory
    {
        private static readonly string ConnectionString;

        static RedisConnFactory()
        {
            ConnectionString = ConfigHelper.GetRedisConnection("RedisConnectionString");
            if (ConnectionString == null)
            {
                throw new ArgumentNullException(nameof(RedisConnFactory) + ":Redis连接配置错误");
            }
        }

        #region Azure Redis Cache Using Method

        private static readonly Lazy<ConnectionMultiplexer> LayzConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(ConnectionString));

        public static ConnectionMultiplexer Instance => LayzConnection.Value;

        public static ConfigurationOptions Options => ConfigurationOptions.Parse(ConnectionString);

        #endregion

       
    }
}
