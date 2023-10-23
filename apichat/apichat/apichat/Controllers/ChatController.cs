using apichat.Service;
using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace apichat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        Redis _redis;
        Kafka _kafka;
        public ChatController(Redis redis, Kafka kafka)
        {
            _redis = redis;
            _kafka = kafka;
        }

        [HttpGet("Send")]
        public async Task<ActionResult<string>> GetList()
        {
            _redis.Subscribe("chat");
            _redis.Publish("chat","안녕");

            await _kafka.Publish("chat", "안녕2");
            _kafka.SubScribe("chat");
            ConsumeResult<Null, string> consumedData = _kafka.Consume("chat", new CancellationToken());
            Console.WriteLine($"Message consumed: {consumedData.Message.Value}");
            var result = "";
            return result;
        }
    }
}
