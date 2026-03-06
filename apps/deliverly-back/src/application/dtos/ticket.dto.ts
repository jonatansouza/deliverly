import { Status } from 'src/domain/types/status.enum';
import { TicketRequestDTO } from './ticket-request.dto';

export class TicketDTO extends TicketRequestDTO {
  status: Status;
  timestamp: Date;
}
