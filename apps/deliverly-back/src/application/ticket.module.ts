import { Module } from '@nestjs/common';
import { CqrsModule } from '@nestjs/cqrs';
import { TicketPublishCommandHandler } from 'src/application/commands/ticket-publish.command';
import { TicketUpdateCommandHandler } from 'src/application/commands/ticket-update.command';
import { TicketQueryHandler } from 'src/application/queries/ticket.query';
import { TicketService } from 'src/application/services/ticket.service';
import { TicketRepository } from 'src/domain/repositories/ticket.repository';
import { DatabaseModule } from 'src/infra/database/database.module';
import { KafkaModule } from 'src/infra/message-broker/kafka.module';
import { CouchDbTicketRepository } from 'src/infra/repositories';

const queries = [TicketQueryHandler];
const commands = [TicketPublishCommandHandler, TicketUpdateCommandHandler];
const services = [TicketService];
const repositories = [
  {
    provide: TicketRepository,
    useClass: CouchDbTicketRepository,
  },
];

@Module({
  imports: [CqrsModule.forRoot(), DatabaseModule, KafkaModule],
  providers: [...services, ...queries, ...commands, ...repositories],
  exports: [...services],
})
export class TicketModule {}
