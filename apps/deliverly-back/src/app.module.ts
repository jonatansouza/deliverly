import { Module } from '@nestjs/common';
import { ConfigModule } from '@nestjs/config';
import { TicketModule } from './application/ticket.module';
import { TicketController } from './presentation/controllers/ticket.controller';

@Module({
  imports: [
    ConfigModule.forRoot({
      isGlobal: true,
    }),
    TicketModule,
  ],
  controllers: [TicketController],
})
export class AppModule {}
