using Common.Resources.Config;
using Common.Utilities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Database
{
    public static class RedisClient
    {
        private static ILogger _log = new Logger(typeof(RedisClient));

        public static IDatabase Db { get; private set; }
        public static ISubscriber Sub { get; private set; }

        private static ConnectionMultiplexer _conn;

        public static void Connect(DatabaseConfig config)
        {
            var conString = config.Host + ":" + config.Port + ",syncTimeout=60000"; // Build the configuration string to connect to a specific Redis server
            if (!string.IsNullOrWhiteSpace(config.Password))
                conString += ",password=" + config.Password;

            _log.Info($"Connecting to Redis @ {config.Host}:{config.Port}");

            _conn = ConnectionMultiplexer.Connect(conString); // Connect to the Redis server and set up static fields.
            Db = _conn.GetDatabase(config.DbIndex);
            Sub = _conn.GetSubscriber();

            _log.Info($"Connected database to index {config.DbIndex}");
        }

        public static void SetLogger(ILogger logger)
        {
            _log = logger;
        }
    }
}
