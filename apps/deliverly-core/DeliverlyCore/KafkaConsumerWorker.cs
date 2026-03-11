using Confluent.Kafka;
using DeliverlyCore.Services;

namespace DeliverlyCore
{
    public class KafkaConsumerWorker : BackgroundService
    {
        private readonly ILogger<KafkaConsumerWorker> _logger;
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly List<string> Topics = [];

        public KafkaConsumerWorker(ILogger<KafkaConsumerWorker> logger, IConfigurationService config)
        {
            _logger = logger;

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = config.Get<string>(ConfigurationConstants.KAFKA_BOOTSTRAP_SERVERS),
                GroupId = config.Get<string>(ConfigurationConstants.KAFKA_GROUP_ID),
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

            Topics.Add(config.Get<string>(ConfigurationConstants.KAFKA_TOPIC_TICKET_CREATE));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
        
        private void StartConsumerLoop(CancellationToken stoppingToken)
        {
            Topics.ForEach(topic => _consumer.Subscribe(topic));
           
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);

                    _logger.LogInformation("Message received: {Value}", result.Message.Value);
                }
                catch (OperationCanceledException)
                {
                    // Expected when the application is shutting down
                    break;
                }
                catch (ConsumeException e)
                {
                    _logger.LogError($"Kafka consume error: {e.Error.Reason}");
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Unexpected error in Kafka loop");
                }
            }

            _consumer.Close(); // Ensures offsets are committed and the group is notified
        }
    }
}