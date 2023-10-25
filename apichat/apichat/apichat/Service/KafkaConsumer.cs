using Confluent.Kafka;
using Newtonsoft.Json;

namespace apichat.Service
{
    public class KafkaConsumer: IHostedService, IDisposable
    {
        private readonly ILogger<KafkaConsumer> _logger;
        private IConsumer<string, string> _consumer;
        IConfiguration _configuration;

        public KafkaConsumer(ILogger<KafkaConsumer> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger ?? throw new ArgumentException(nameof(logger));

            Init();
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Kafka Consumer Service has started.");

                    _consumer.Subscribe(new List<string>() { "chat" });
                    await Task.Delay(10);
                    await Consume(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Kafka Consumer Service is stopping.");

            _consumer.Close();

            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _consumer.Dispose();
        }

        private void Init()
        {
            //var pemFileWithKey = "./keystore/secure.pem";

            var config = new ConsumerConfig()
            {
                BootstrapServers = _configuration.GetConnectionString("KafkaUrl"),

            //SslCaLocation = pemFileWithKey,
            //SslCertificateLocation = pemFileWithKey,
            //SslKeyLocation = pemFileWithKey,

            //Debug = "broker,topic,msg",

            GroupId = "foo",
                SecurityProtocol = SecurityProtocol.Plaintext,
                EnableAutoCommit = false,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true
            };

            //_consumer = new ConsumerBuilder<string, string>(config).SetStatisticsHandler((_, kafkaStatistics) => LogKafkaStats(kafkaStatistics)).
            //    SetErrorHandler((_, e) => LogKafkaError(e)).Build();

            _consumer = new ConsumerBuilder<string, string>(config).Build();
        }

        private void LogKafkaStats(string kafkaStatistics)
        {
            var stats = JsonConvert.DeserializeObject<KafkaStatistics>(kafkaStatistics);

            if (stats?.topics != null && stats.topics.Count > 0)
            {
                foreach (var topic in stats.topics)
                {
                    foreach (var partition in topic.Value.Partitions)
                    {

                        var logMessage = $"FxRates:KafkaStats Topic: {topic.Key} Partition: {partition.Key} PartitionConsumerLag: {partition.Value.ConsumerLag}";
                        _logger.LogInformation(logMessage);
                    }
                }
            }
        }

        private void LogKafkaError(Error ex)
        {
            Task.Run(() =>
            {
                var error = $"Kafka Exception: ErrorCode:[{ex.Code}] Reason:[{ex.Reason}] Message:[{ex.ToString()}]";
                _logger.LogError(error);
            });
        }

        private async Task Consume(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(10);
                    var consumeResult = _consumer.Consume(1000);
                    //var consumeResult = _consumer.Consume(cancellationToken);

                    if (consumeResult?.Message == null) continue;

                    if (consumeResult.Topic.Equals("chat"))
                    {
                        await Task.Delay(10);
                        //var json = Encoding.UTF8.GetString(LZ4Codec.Unwrap(Convert.FromBase64String(consumeResult.Message.Value)));
                        _logger.LogInformation($"[{consumeResult.Message.Key}] {consumeResult.Topic} - {consumeResult.Message.Value}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }
    }

    public class KafkaStatistics
    {
        /// <summary>
        ///     Handle instance name.
        /// </summary>
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("topics")]
        public IReadOnlyDictionary<string, TopicData> topics { get; set; }

    }
    public class TopicData
    {
        [JsonProperty("topic")]
        public string Topic { get; set; }

        [JsonProperty("partitions")]
        public IReadOnlyDictionary<string, PartitionData> Partitions { get; set; }
    }
    public class PartitionData
    {
        [JsonProperty("partition")]
        public int Partition { get; set; }

        [JsonProperty("consumer_lag")]
        public int ConsumerLag { get; set; }
    }
}
