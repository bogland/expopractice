using Confluent.Kafka;
using System.Net;

namespace apichat.Service
{
    public class Kafka : IDisposable
    {
        ProducerBuilder<Null, string> _produce;
        ConsumerBuilder<Null, string> _consume;
        IConsumer<Null, String> _consumer;
        IConfiguration _Configuration;
        public Kafka(IConfiguration Configuration)
        {
            _Configuration = Configuration;
            (_produce, _consume) = Connect();
            SubScribe("chat");
        }

        public (ProducerBuilder<Null,string>, ConsumerBuilder<Null, string>) Connect()
        {
            var connectionStr = _Configuration.GetConnectionString("KafkaUrl");
            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = connectionStr,
                ClientId = Dns.GetHostName(),
            };

            var produce = new ProducerBuilder<Null, string>(config);

            var config2 = new ConsumerConfig
            {
                BootstrapServers = connectionStr,
                GroupId = "foo",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            var consume = new ConsumerBuilder<Null, string>(config2);

            return (produce,consume);
        }
        public async Task<bool> Publish
(string topic, string message)
        {
            var producer = _produce.Build();
            var result = await producer.ProduceAsync
            (topic, new Message<Null, string>
            {
                Value = message
            });

            Console.WriteLine($"Delivery Timestamp:{result.Timestamp.UtcDateTime}");
            return await Task.FromResult(true);
        }

        public void SubScribe(string topic)
        {
            _consumer = _consume.Build();
            _consumer.Subscribe(topic);
        }

        public void UnSubScribe()
        {
            _consumer.Unsubscribe();
        }

        public ConsumeResult<Null, string> Consume(string topic, CancellationToken cancellationToken)
        {
            ConsumeResult<Null, string> result = _consumer.Consume(cancellationToken);
            return result;
        }
        public ConsumeResult<Null, string> Consume(string topic, int time)
        {
            ConsumeResult<Null, string> result = _consumer.Consume(time);
            return result;
        }

        public void Dispose()
        {
            UnSubScribe();
            _consumer.Close();
            _consumer.Dispose();
            Console.WriteLine("나 끝났어");
        }
    }
}
