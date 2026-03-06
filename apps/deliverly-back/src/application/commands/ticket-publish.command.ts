import { Inject } from '@nestjs/common';
import { CommandHandler, ICommandHandler } from '@nestjs/cqrs';
import { ClientProxy } from '@nestjs/microservices';
import { TicketDTO } from '../dtos/ticket.dto';

export class TicketPublishCommand {
  constructor(public readonly payload: TicketDTO) {}
}

@CommandHandler(TicketPublishCommand)
export class TicketPublishCommandHandler implements ICommandHandler<TicketPublishCommand> {
  constructor(@Inject('TICKET_SERVICE') private readonly bus: ClientProxy) {}

  async execute(command: TicketPublishCommand): Promise<void> {
    this.bus.emit('ticket.create', command.payload);
  }
}
