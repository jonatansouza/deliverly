import { Expose } from 'class-transformer';
import { Status } from '../types/status.enum';

export class Ticket {
  @Expose()
  origin: string;
  @Expose()
  destination: string;
  @Expose()
  ticket: string;
  @Expose()
  status: Status;
  @Expose()
  timestamp: Date;

  constructor() {}
}
