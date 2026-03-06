import { Ticket } from '../entities/ticket.entity';

export abstract class TicketRepository {
  abstract getTicket(ticket: string): Promise<Ticket | null>;
  abstract createTicket(ticket: Ticket): Promise<void>;
}
