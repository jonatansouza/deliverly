using Confluent.Kafka;
using DeliverlyCore.Consumers;
using DeliverlyCore.Services;

namespace DeliverlyCore;

public class KafkaConsumerWorker : BackgroundService
{
    private readonly ILogger<KafkaConsumerWorker> _logger;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IReadOnlyDictionary<string, IKafkaMessageHandler> _handlers;

    public KafkaConsumerWorker(
        ILogger<KafkaConsumerWorker> logger,
        IConfigurationService config,
        IEnumerable<IKafkaMessageHandler> handlers)
    {
        _logger = logger;
        _handlers = handlers.ToDictionary(h => h.Topic);

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = config.Get<string>(ConfigurationConstants.KAFKA_BOOTSTRAP_SERVERS),
            GroupId = config.Get<string>(ConfigurationConstants.KAFKA_GROUP_ID),
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);

    private void StartConsumerLoop(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_handlers.Keys);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(stoppingToken);

                if (_handlers.TryGetValue(result.Topic, out var handler))
                    handler.HandleAsync(result.Message.Value, stoppingToken).GetAwaiter().GetResult();
                else
                    _logger.LogWarning("No handler registered for topic: {Topic}", result.Topic);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (ConsumeException e)
            {
                _logger.LogError("Kafka consume error: {Reason}", e.Error.Reason);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unexpected error in Kafka loop");
            }
        }

        _consumer.Close();
    }
}
