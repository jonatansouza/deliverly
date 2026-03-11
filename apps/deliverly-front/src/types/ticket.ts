export interface TicketRequest {
  origin: string;
  destination: string;
  ticket: string;
}

export interface Ticket extends TicketRequest {
  status: "PENDING" | "IN_TRANSIT" | "DELIVERED" | "CANCELLED";
  timestamp: string;
}
