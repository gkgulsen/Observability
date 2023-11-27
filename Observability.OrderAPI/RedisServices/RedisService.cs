using StackExchange.Redis;

namespace Observability.OrderAPI.RedisServices
{
    public class RedisService
    {
        private readonly ConnectionMultiplexer connectionMultiplexer;

        public RedisService(IConfiguration configuration)
        {
            var host = configuration.GetSection("Redis")["Host"];
            var port = configuration.GetSection("Redis")["Port"];

            var config = $"{host}:{port}";
            //connectionMultiplexer = ConnectionMultiplexer.Connect(config);
        }

        public ConnectionMultiplexer GetConnectionMultixer => connectionMultiplexer;
        public IDatabase GetDb(int db = 0)
        {
            return connectionMultiplexer.GetDatabase(db);
        }
    }
}
