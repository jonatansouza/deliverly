import { IQueryHandler, QueryHandler } from '@nestjs/cqrs';
import { plainToInstance } from 'class-transformer';
import { TicketRepository } from 'src/domain/repositories/ticket.repository';
import { TicketDTO } from '../dtos/ticket.dto';

export class TicketQuery {
  constructor(public readonly ticketId: string) {}
}

@QueryHandler(TicketQuery)
export class TicketQueryHandler implements IQueryHandler<TicketQuery> {
  constructor(private readonly repository: TicketRepository) {}

  async execute(query: TicketQuery) {
    const ticket = await this.repository.getTicket(query.ticketId);

    return plainToInstance(TicketDTO, ticket);
  }
}
