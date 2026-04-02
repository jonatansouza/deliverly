namespace DeliverlyCore.Shared.Messages;

public record TicketCreateMessage(
    string Origin,
    string Destination,
    string Ticket,
    string Status,
    DateTime Timestamp
);
