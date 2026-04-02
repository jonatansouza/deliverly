namespace DeliverlyCore.Consumers;

public interface IKafkaMessageHandler
{
    string Topic { get; }
    Task HandleAsync(string rawMessage, CancellationToken ct);
}
