import { IsString } from 'class-validator';

export class TicketRequestDTO {
  @IsString()
  origin: string;

  @IsString()
  destination: string;

  @IsString()
  ticket: string;
}
