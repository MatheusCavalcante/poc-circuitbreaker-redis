using StackExchange.Redis;

namespace poc_circuit_breaker.Utils
{
    public class RedisConnectionHelper
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection;

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        static RedisConnectionHelper()
        {
            RedisConnectionHelper.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect("localhost:6379,ssl=false,password=qwe@123");
            });
        }

        public static void Set(string key, string message)
        {            
            var ttl = TimeSpan.FromMinutes(2);

            Connection.GetDatabase().StringSet(key, message, ttl);
        }
    }
}
