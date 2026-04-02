using DeliverlyCore.Services;
using DeliverlyCore.Shared.Messages;

namespace DeliverlyCore.Consumers;

public class TicketCreateHandler : KafkaMessageHandler<TicketCreateMessage>
{
    private readonly ILogger<TicketCreateHandler> _logger;

    public TicketCreateHandler(IConfigurationService config, ILogger<TicketCreateHandler> logger)
    {
        Topic = config.Get<string>(ConfigurationConstants.KAFKA_TOPIC_TICKET_CREATE);
        _logger = logger;
    }

    public override string Topic { get; }

    protected override Task HandleAsync(TicketCreateMessage message, CancellationToken ct)
    {
        _logger.LogInformation(
            "Ticket received: {Ticket} | {Origin} -> {Destination} | Status: {Status}",
            message.Ticket, message.Origin, message.Destination, message.Status);

        return Task.CompletedTask;
    }
}
