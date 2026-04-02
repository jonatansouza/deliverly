using System.Text.Json;

namespace DeliverlyCore.Consumers;

public abstract class KafkaMessageHandler<T> : IKafkaMessageHandler
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public abstract string Topic { get; }

    public Task HandleAsync(string rawMessage, CancellationToken ct)
    {
        var message = JsonSerializer.Deserialize<T>(rawMessage, JsonOptions)!;
        return HandleAsync(message, ct);
    }

    protected abstract Task HandleAsync(T message, CancellationToken ct);
}
