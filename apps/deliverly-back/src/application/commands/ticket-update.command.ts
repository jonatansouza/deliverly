import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { plainToInstance } from 'class-transformer';
import { Ticket } from 'src/domain/entities/ticket.entity';
import { TicketRepository } from 'src/domain/repositories/ticket.repository';
import { Status } from 'src/domain/types/status.enum';
import { TicketRequestDTO } from '../dtos/ticket-request.dto';
import { TicketDTO } from '../dtos/ticket.dto';

export class TicketUpdateCommand {
  public readonly timestamp: Date;
  public readonly status: Status;

  constructor(public readonly payload: TicketRequestDTO) {
    this.timestamp = new Date();
    this.status = Status.PENDING;
  }
}

@CommandHandler(TicketUpdateCommand)
export class TicketUpdateCommandHandler implements ICommandHandler<TicketUpdateCommand> {
  constructor(private readonly repository: TicketRepository) {}

  async execute(command: TicketUpdateCommand): Promise<TicketDTO> {
    const ticket = {
      origin: command.payload.origin,
      destination: command.payload.destination,
      ticket: command.payload.ticket,
      status: Status.PENDING,
      timestamp: command.timestamp,
    } as Ticket;

    await this.repository.createTicket(ticket);

    return plainToInstance(TicketDTO, ticket);
  }
}
