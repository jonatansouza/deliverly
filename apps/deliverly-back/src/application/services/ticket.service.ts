import { Injectable } from '@nestjs/common';
import { CommandBus, QueryBus } from '@nestjs/cqrs';
import { plainToInstance } from 'class-transformer';
import { TicketPublishCommand } from '../commands/ticket-publish.command';
import { TicketUpdateCommand } from '../commands/ticket-update.command';
import { TicketRequestDTO } from '../dtos/ticket-request.dto';
import { TicketDTO } from '../dtos/ticket.dto';
import { TicketQuery } from '../queries/ticket.query';

@Injectable()
export class TicketService {
  constructor(
    private readonly commandBus: CommandBus,
    private readonly queryBus: QueryBus,
  ) {}

  async createAndPublish(payload: TicketRequestDTO): Promise<TicketDTO> {
    const read = await this.read(payload.ticket);
    if (!!read) {
      return read;
    }

    const updateCommand = new TicketUpdateCommand(payload);
    const tasks = {
      update: this.commandBus.execute(updateCommand),
      publish: this.commandBus.execute(
        new TicketPublishCommand({
          ...payload,
          status: updateCommand.status,
          timestamp: updateCommand.timestamp,
        }),
      ),
    };

    await Promise.all([tasks.update, tasks.publish]);

    return plainToInstance(TicketDTO, tasks.update);
  }

  async read(ticketId: string): Promise<TicketDTO> {
    const result = this.queryBus.execute(new TicketQuery(ticketId));
    return plainToInstance(TicketDTO, result);
  }
}
