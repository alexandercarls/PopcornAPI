using StackExchange.Redis;

namespace PopcornApi.Services.Caching
{
    /// <summary>
    /// The caching service
    /// </summary>
    public class CachingService : ICachingService
    {
        /// <summary>
        /// The Redis connection multiplexer
        /// </summary>
        private readonly IConnectionMultiplexer _connection;

        /// <summary>
        /// Redis database
        /// </summary>
        private readonly IDatabase _redisDatabase;

        /// <summary>
        /// Create an instance of <see cref="CachingService"/>
        /// </summary>
        public CachingService(string redisConnectionString)
        {
            _connection = ConnectionMultiplexer.Connect(redisConnectionString);
            _redisDatabase = _connection.GetDatabase();
        }

        /// <summary>
        /// Cache
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void SetCache(string key, string value)
        {
            _redisDatabase.StringSet(key, value);
        }

        /// <summary>
        /// Cache
        /// </summary>
        /// <param name="key">Key</param>
        public string GetCache(string key)
        {
            return _redisDatabase.StringGet(key);
        }
    }
}
