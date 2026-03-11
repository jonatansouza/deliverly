import { Body, Controller, Get, Param, Post } from '@nestjs/common';
import { TicketDTO } from 'src/application/dtos/ticket.dto';
import { TicketService } from 'src/application/services/ticket.service';
import { TicketRequestDTO } from '../../application/dtos/ticket-request.dto';

@Controller('tickets')
export class TicketController {
  constructor(public readonly service: TicketService) {}

  @Get(':id')
  getTicket(@Param('id') ticketId: string): Promise<TicketDTO> {
    return this.service.read(ticketId);
  }

  @Post()
  createTicket(@Body() payload: TicketRequestDTO): Promise<TicketDTO> {
    return this.service.createAndPublish(payload);
  }
}
