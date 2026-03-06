import { Inject, Injectable } from '@nestjs/common';
import { plainToInstance } from 'class-transformer';
import * as nano from 'nano';
import { Ticket } from 'src/domain/entities/ticket.entity';
import { TicketRepository } from 'src/domain/repositories/ticket.repository';
import { COUCHDB_CONNECTION } from '../database/database.module';

@Injectable()
export class CouchDbTicketRepository implements TicketRepository {
  protected readonly db: nano.DocumentScope<Ticket>;
  constructor(
    @Inject(COUCHDB_CONNECTION)
    private readonly nano: nano.ServerScope,
  ) {
    this.db = this.nano.db.use<Ticket>('tickets');
  }

  async getTicket(id: string): Promise<Ticket | null> {
    try {
      const doc = await this.db.get(id);
      return plainToInstance(Ticket, doc, {
        excludeExtraneousValues: true,
      });
    } catch (err) {
      return null;
    }
  }

  async createTicket(ticket: Ticket): Promise<void> {
    await this.db.insert({ _id: ticket.ticket, ...ticket });
  }
}
