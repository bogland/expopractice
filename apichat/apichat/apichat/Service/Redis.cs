using Newtonsoft.Json;
using StackExchange.Redis;

namespace apichat.Service
{
    public class Redis
    {
        ConnectionMultiplexer _redis;
        IConfiguration _Configuration;
        public Redis(IConfiguration Configuration)
        {
            Console.WriteLine(DateTime.Now + " 연결 됨");
            _Configuration = Configuration;
            _redis = Connect();
        }
        ~Redis()
        {
            Dissconnect();
        }

        public ConnectionMultiplexer Connect()
        {
            var connectionStr = _Configuration.GetConnectionString("RedisUrl");
            System.Console.WriteLine(connectionStr);
            var redis = ConnectionMultiplexer.Connect(
             new ConfigurationOptions
             {
                 EndPoints = { connectionStr },
             });
            return redis;
        }

        public void Dissconnect()
        {
            _redis.Dispose();
        }

        public void Publish(string channel,string message)
        {
            var pubsub = _redis.GetSubscriber();
            pubsub.PublishAsync(channel, message, CommandFlags.FireAndForget);
        }

        public void Subscribe(string channel)
        {
            var pubsub = _redis.GetSubscriber();
            pubsub.Subscribe(channel, (channel, message) => Console.Write("Message received from test-channel : " + message));
        }

        public void Write(string key = "", object obj = null)
        {
            var db = _redis.GetDatabase();
            db.StringSet($"{key}", JsonConvert.SerializeObject(obj));
        }
        public async Task<T> Read<T>(string key = "")
        {
            var db = _redis.GetDatabase();
            var res = await db.StringGetAsync($"{key}");
            var resDeseriize = JsonConvert.DeserializeObject<T>(res);
            return resDeseriize;
        }
    }
}
